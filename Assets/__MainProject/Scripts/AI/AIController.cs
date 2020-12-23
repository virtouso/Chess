using ChessDotNet;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;

public class AIController : MonoBehaviour
{

    public static AIController Instance;
    public void MakeAiMove()
    {
        MoveScorePair movePair = SelectBestAiMove(GameManager.Instance.ChessGame, 0, 1, 0, Player.Black);
        GameManager.Instance.ChessGame.MakeMove(movePair.Move, true);

        GameManager.Instance.RegeneratePieces();
    }

    private void Awake()
    {
        Instance = this;
    }



    #region private
    private int finalStateScore = 200;


    Dictionary<string, int> _scores = new Dictionary<string, int>()
    {
        {"Win",1000 },
        {"King",0 },
        {"Queen",700 },
        {"Bishop",300 },
        {"Knight",200 },
        {"Rook",300 },
        {"Pawn",50 }
    };

    #endregion




    #region Utility

    public MoveScorePair SelectBestAiMove(ChessGame chessGame, int depth, int alpha, int beta, Player player)
    {
        // UnityEngine.Debug.Log(depth);
        if (chessGame.IsDraw())
        {
            return new MoveScorePair(0);
        }
        else if (chessGame.IsWinner(Player.Black))
        {
            return new MoveScorePair(-1 * _scores["Win"] - depth);
        }
        else if (chessGame.IsWinner(Player.White))
        {
            return new MoveScorePair(depth - _scores["Win"]);
        }
        depth++;
        MoveScorePair levelBestMove = CalculateMoveScorePairForPlayer(player);

        var PossibleMoves = chessGame.GetValidMoves(player);




        int bestScore = 0;
        Move bestMove = PossibleMoves[0];
       // if (depth >= 100 && player == Player.Black)
       if(chessGame.PiecesOnBoard.Count>7)
        {
            foreach (var item in PossibleMoves)
            {
                if (!chessGame.IsValidMove(item)) continue;
                ChessGame cloneBoard = new ChessGame(chessGame.GetGameCreationData());
                cloneBoard.MakeMove(item, true);
                int moveScore = CalculateBoardScore(cloneBoard);
                if (moveScore <= bestScore)
                {
                    bestMove = item;
                    bestScore = moveScore;
                }

            }
            return new MoveScorePair(bestMove, bestScore);

        }//todo test



        int minValue = int.MaxValue;
        int maxValue = int.MinValue;
        foreach (var item in PossibleMoves)
        {
            ChessGame cloneBoard = new ChessGame(chessGame.GetGameCreationData());

            cloneBoard.MakeMove(item, true);
            //MoveScorePair newMove = null;

            //Thread t = new Thread(() =>
            //{
            //    newMove = SelectBestAiMove(cloneBoard, depth, alpha, beta, (player ^ Player.White));
            //});
            //t.Start();
            //t.Join();
            MoveScorePair newMove = SelectBestAiMove(cloneBoard, depth, alpha, beta, (player ^ Player.White));

            newMove.Score = CalculateBoardScore(cloneBoard);
            if (player == Player.Black)
            {
                if (newMove.Score <= levelBestMove.Score)
                {
                    levelBestMove.Move = newMove.Move;
                    levelBestMove.Score = newMove.Score;
                }

                minValue = Mathf.Min(minValue, newMove.Score);
                beta = Mathf.Min(beta, newMove.Score);
                if (beta <= alpha) break;
            }
            else
            {
                if (newMove.Score >= levelBestMove.Score)
                {
                    levelBestMove.Move = newMove.Move;
                    levelBestMove.Score = newMove.Score;
                }

                maxValue = Mathf.Max(maxValue, newMove.Score);
                alpha = Mathf.Max(alpha, newMove.Score);
                if (beta <= alpha) break;


            }


        }


        return levelBestMove;
    }


    private MoveScorePair CalculateMoveScorePairForPlayer(Player player)
    {
        if (player == Player.White) { return new MoveScorePair(int.MinValue); }
        else { return new MoveScorePair(int.MaxValue); }

    }

    private int CalculateBoardScore(ChessGame chessGame)
    {

        int score = 0;

        for (int i = 0; i < GameReferences.Rows.Count; i++)
        {
            for (int j = 0; j < GameReferences.Columns.Count; j++)
            {
                Piece piece = chessGame.GetPieceAt(new Position(GameReferences.Columns.ElementAt(j).Value + GameReferences.Rows.ElementAt(i).Value));
                if (piece == null) continue;
                if (piece.Owner == Player.Black)
                    score -= _scores[piece.GetType().Name];
                else
                    score += _scores[piece.GetType().Name];

            }

        }

        return score;
    }


    #endregion

}



public class MoveScorePair
{
    public Move Move;
    public int Score;

    public MoveScorePair(int score)
    {
        Score = score;
    }


    public MoveScorePair(Move move, int score)
    {
        Score = score;
        Move = move;
    }


}

