using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ChessDotNet;
using System.Linq;

public class GameManager : MonoBehaviour
{



    #region public

    #endregion

    #region unity references
    [SerializeField] private Vector3 _boardStartPosition;
    [SerializeField] private Vector2 _squareSize;
    [SerializeField] private Sprite _whiteSquare;
    [SerializeField] private Sprite _blackSquare;
    [SerializeField] private List<PieceData> Pieces;

    #endregion

    #region private
    ChessGame ChessGame;




    #endregion


    #region Unity callbacks
    void Start()
    {
        this.ChessGame = new ChessGame();
        GenerateChessBoard();

    }
    #endregion



    #region utility
    private void GenerateChessBoard()
    {

        for (int i = 0; i < GameReferences.Rows.Count; i++)
        {
            for (int j = 0; j < GameReferences.Columns.Count; j++)
            {


                GameObject Square = new GameObject(GameReferences.Columns.ElementAt(j).Value + GameReferences.Rows.ElementAt(i).Value);
                var squareSprite = Square.AddComponent<SpriteRenderer>();
                //Square.transform.localScale = new Vector3(1 / 5.12f, 1 , 1); ;
                if ((i + j) % 2 == 0)
                {
                    squareSprite.sprite = _whiteSquare;
                }
                else
                {
                    squareSprite.sprite = _blackSquare;
                }

                Square.transform.position = /*_boardStartPosition += */new Vector3(j * squareSprite.size.y, i * squareSprite.size.x, 0);
                GeneratePiece(GameReferences.Columns.ElementAt(j).Value, GameReferences.Rows.ElementAt(i).Value);
            }

        }
    }
    private void GeneratePiece(string column, string row)
    {
        Piece piece = this.ChessGame.GetPieceAt(new Position(column + row));
        if (piece == null) return;
        GameObject square = GameObject.Find(column + row);


        PieceData wantedPiece = Pieces.Where(x => x.Name == piece.GetType().Name).First(x => x.Owner.ToString() == piece.Owner.ToString());
        GeneratePieceObject(square.transform.position, wantedPiece);

    }


    private void GeneratePieceObject(Vector3 position, PieceData piece)
    {
        GameObject pieceGameObject = new GameObject(piece.Name);
        SpriteRenderer spriteRendere = pieceGameObject.AddComponent<SpriteRenderer>();
        spriteRendere.sprite = piece.sprite;
        pieceGameObject.transform.position = position - new Vector3(0, 0, 0.5f);
        if (piece.Owner == Owners.Black) { pieceGameObject.transform.eulerAngles = new Vector3(0, 0, 180); }
        pieceGameObject.transform.localScale *= 0.9f;

    }

    #endregion



}

[System.Serializable]
struct PieceData
{
    public string Name;
    public Sprite sprite;
    public Owners Owner;
}
enum Owners { White, Black }