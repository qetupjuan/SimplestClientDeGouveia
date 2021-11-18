using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class TicTacToeManager : MonoBehaviour
{
    GameObject playerSymbolText, opponentSymbolText, turnIndicatorText, characterSelectionPanel, xButton, oButton;

    NetworkedClient connectionToHost;

    List<TicTacToeSquare> ticTacToeSquares;

    string playerIcon, opponentIcon;

    bool isPlayersTurn = false;
    bool isGameOver = false;
    bool isObserver = false;

    const int three = 3;

    // Start is called before the first frame update
    void Start()
    {
        ticTacToeSquares = new List<TicTacToeSquare>(GetComponentsInChildren<TicTacToeSquare>());

        foreach (TicTacToeSquare square in ticTacToeSquares)
        {
            square.OnSquarePressed += OnTicTacToeSquarePressed;
        }

        foreach (GameObject go in FindObjectsOfType<GameObject>())
        {
            if (go.name == "PlayerSymbolText")
                playerSymbolText = go;
            else if (go.name == "OpponentSymbolText")
                opponentSymbolText = go;
            else if (go.name == "TurnIndicatorText")
                turnIndicatorText = go;
            else if (go.name == "CharacterSelection")
                characterSelectionPanel = go;
            else if (go.name == "X Button")
                xButton = go;
            else if (go.name == "O Button")
                oButton = go;
        }


        turnIndicatorText.SetActive(false);
        if (isObserver)
        {
            characterSelectionPanel.SetActive(false);
            opponentSymbolText.SetActive(false);
            playerSymbolText.GetComponent<Text>().text = "You Are: Observing";
        }
        else
        {
            xButton.GetComponent<Button>().onClick.AddListener(XButtonPressed);
            oButton.GetComponent<Button>().onClick.AddListener(OButtonPressed);
        }

    }


    private void OnTicTacToeSquarePressed(TicTacToeSquare square)
    {
        if (playerIcon == "" || !isPlayersTurn) //player hasn't picked their symbol yet or it isn't their turn, they cant claim a square yet
            return;

        isPlayersTurn = false;
        turnIndicatorText.GetComponent<Text>().text = "It's your opponent's turn";

        square.ClaimSquare(playerIcon);
        if (connectionToHost != null)
            connectionToHost.SendMessageToHost(ClientToServerSignifiers.SelectedTicTacToeSquare + "," + square.ID);

        CheckForLineOfThree(square.row, square.column);
        CheckForTie();
    }

    //checks the row, column and two diagonals to see if theres a winning line of three
    void CheckForLineOfThree(int rowToCheck, int colToCheck)
    {
        int rowCount, colCount, diagonal1Count, diagonal2Count;
        rowCount = colCount = diagonal1Count = diagonal2Count = 0;

        foreach (TicTacToeSquare s in ticTacToeSquares)
        {
            if (s.isSquareTaken == false || s.icon == opponentIcon)
                continue;

            if (s.row == rowToCheck)
                rowCount++;
            if (s.column == colToCheck)
                colCount++;
            if (s.diagonal1)
                diagonal1Count++;
            if (s.diagonal2)
                diagonal2Count++;
        }

        if (rowCount == three || colCount == three || diagonal1Count == three || diagonal2Count == three)
        {
            //win
            connectionToHost.SendMessageToHost(ClientToServerSignifiers.WonTicTacToe + "");
            OnGameOver("You Won!");
        }

        print("rows: " + rowCount + "  col: " + colCount + "  ur-bl: " + diagonal1Count + "  tr-bl: " + diagonal2Count);
    }

    public void OpponentTookTurn(int squareID)
    {
        foreach (TicTacToeSquare s in ticTacToeSquares)
        {
            if (s.ID == squareID)
                s.ClaimSquare(opponentIcon);
        }

        if (!isObserver)
        {
            isPlayersTurn = true;
            turnIndicatorText.GetComponent<Text>().text = "It's your turn";
        }
    }

    public void OnGameOver(string endingMsg)
    {
        isPlayersTurn = false;
        isGameOver = true;
        turnIndicatorText.GetComponent<Text>().text = endingMsg;

        //enable ui for replay

    }

    public void SetNetworkConnection(NetworkedClient networkClient)
    {
        connectionToHost = networkClient;
    }


    void XButtonPressed()
    {
        CharacterSelected("X", "O");
    }
    void OButtonPressed()
    {
        CharacterSelected("O", "X");
    }

    void CharacterSelected(string symbol, string otherSymbol)
    {
        playerIcon = symbol;
        opponentIcon = otherSymbol;

        playerSymbolText.GetComponent<Text>().text = "You Are: " + symbol;
        opponentSymbolText.GetComponent<Text>().text = "Opponent is: " + otherSymbol;

        oButton.GetComponent<Button>().onClick.RemoveAllListeners();
        xButton.GetComponent<Button>().onClick.RemoveAllListeners();

        characterSelectionPanel.SetActive(false);
        turnIndicatorText.SetActive(true);

        //check if the other player made a choice before your icons were set
        foreach (TicTacToeSquare s in ticTacToeSquares)
        {
            if (s.isSquareTaken)
                s.ClaimSquare(opponentIcon);
        }
    }

    public void ChosenAsPlayerOne()
    {
        isPlayersTurn = true;
        turnIndicatorText.GetComponent<Text>().text = "It's your turn";
    }

    //unsubscribe from events and delegates
    private void OnDisable()
    {
        if (ticTacToeSquares != null)
        {
            foreach (TicTacToeSquare square in ticTacToeSquares)
            {
                square.OnSquarePressed -= OnTicTacToeSquarePressed;
            }
        }
    }

    private void CheckForTie()
    {
        int takenTileCount = 0;
        foreach (TicTacToeSquare s in ticTacToeSquares)
        {
            if (s.isSquareTaken)
                takenTileCount++;
        }

        if (takenTileCount >= 9 && isGameOver == false)
        {
            connectionToHost.SendMessageToHost(ClientToServerSignifiers.GameTied + "");
            OnGameOver("No squares left. You tied");
        }
    }

}