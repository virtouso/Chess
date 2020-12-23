using ChessDotNet;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public static PlayerController Instance;
    private System.Action PlayerAction;

    private void Awake()
    {
        Instance = this;
        PlayerAction = SelectPiece;

    }
    void Update()
    {
        GetPlayerInteraction();
    }






    private void GetPlayerInteraction()
    {
        if (GameManager.Instance.ChessGame.WhoseTurn == ChessDotNet.Player.Black) return;

        if (!Input.GetMouseButtonDown(0)) return;


        if (PlayerAction != null) PlayerAction.Invoke();


    }


    private string selectedMove;
    private void SelectPiece()
    {
        GetConditionForMouseClick(out bool hitSquare, out string squareName);

        if (hitSquare)
        {
            selectedMove = squareName;
            ReadOnlyCollection<Move> validMovesList = GameManager.Instance.ChessGame.GetValidMoves(new ChessDotNet.Position(selectedMove));
            if (validMovesList == null) return;
            if (validMovesList.Count <= 0) return;
            ColorizeValidMoves(validMovesList);
            PlayerAction = MovePiece;
        }


    }



    private void MovePiece()
    {
        GetConditionForMouseClick(out bool hitSquare, out string squareName);
        if (!hitSquare)
        {
            GameManager.Instance.ClearMoves();
            PlayerAction = SelectPiece;
            return;
        }
        if (!GameManager.Instance.ChessGame.IsValidMove(new Move(new Position(selectedMove), new Position(squareName), Player.White)))
        {
            GameManager.Instance.ClearMoves();
            PlayerAction = SelectPiece;
            return;
        }

        GameManager.Instance.ChessGame.MakeMove(new Move(new Position(selectedMove), new Position(squareName), Player.White), true);
        GameManager.Instance.ClearMoves();
        GameManager.Instance.RegeneratePieces();
        StartCoroutine(StartAI());
    }


    IEnumerator StartAI()
    {
        yield return new  WaitForSeconds(1);
        AIController.Instance.MakeAiMove();

    }





    private void GetConditionForMouseClick(out bool hitSquare, out string name)
    {
        hitSquare = false;
        name = "";
        RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
        if (hit.collider != null)
        {
            Debug.Log("Target name: " + hit.collider.name);
            hitSquare = true;
            name = hit.collider.name;
        }
    }



    private void ColorizeValidMoves(ReadOnlyCollection<Move> validMovesList)
    {
        foreach (var item in validMovesList)
        {
            string moveName = item.NewPosition.ToString();
            GameObject square = GameObject.Find(moveName);
            square.GetComponent<SpriteRenderer>().color = Color.black;
        }
    }







}
