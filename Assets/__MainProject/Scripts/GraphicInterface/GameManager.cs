using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ChessDotNet;
using System.Linq;

public class GameManager : MonoBehaviour
{



    #region public
    public static GameManager Instance;
    public ChessGame ChessGame;
    public void ClearMoves()
    {
        for (int i = 0; i < GameReferences.Rows.Count; i++)
        {
            for (int j = 0; j < GameReferences.Columns.Count; j++)
            {
                GameObject Square = GameObject.Find(GameReferences.Columns.ElementAt(j).Value + GameReferences.Rows.ElementAt(i).Value);
                Square.GetComponent<SpriteRenderer>().color = Color.white;
            }

        }
    }
    #endregion

    #region unity references
    [SerializeField] private Vector3 _boardStartPosition;
    [SerializeField] private Vector2 _squareSize;
    [SerializeField] private Sprite _whiteSquare;
    [SerializeField] private Sprite _blackSquare;
    [SerializeField] private List<PieceData> Pieces;

    #endregion

    #region private

    private List<GameObject> PiecesGameObjects = new List<GameObject>();

    #endregion


    #region Unity callbacks
    private void Awake()
    {
        if (Instance == null) Instance = this;
    }
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


                BoxCollider2D collider = Square.AddComponent<BoxCollider2D>();
                collider.size = new Vector2(squareSprite.size.y, squareSprite.size.x);
                collider.isTrigger = true;
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
        PiecesGameObjects.Add(pieceGameObject);
        SpriteRenderer spriteRendere = pieceGameObject.AddComponent<SpriteRenderer>();
        spriteRendere.sprite = piece.sprite;
        pieceGameObject.transform.position = position - new Vector3(0, 0, 0.5f);
        if (piece.Owner == Owners.Black) { pieceGameObject.transform.eulerAngles = new Vector3(0, 0, 180); }
        pieceGameObject.transform.localScale *= 0.9f;

    }



    private void RemoveAllPieces()
    {
        foreach (var item in PiecesGameObjects)
        {
            Destroy(item);
        }
        PiecesGameObjects.Clear();
    }


    public void RegeneratePieces()
    {
        RemoveAllPieces();
        for (int i = 0; i < GameReferences.Rows.Count; i++)
        {
            for (int j = 0; j < GameReferences.Columns.Count; j++)
            {
                GeneratePiece(GameReferences.Columns.ElementAt(j).Value, GameReferences.Rows.ElementAt(i).Value);
            }
        }
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