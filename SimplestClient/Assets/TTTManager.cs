using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TTTManager : MonoBehaviour
{
    [SerializeField]
    GameObject networkedClient;
    public List<Button> playSpaces;
    public Text instructions;

    public int player1ID = 0;
    public int player2ID = 0;
    public int startingPlayer;
    public int moveCount;
    int replayActionIndex = 0;

    string playerIcon;
    bool gameOver = false;

    string yourTurnText = "It's your turn.";
    string opponentTurnText = "It's your opponent's turn.";

    public int playersTurn;

    private static TTTManager instance;
    public static TTTManager Instance { get { return instance; } }

    private void Awake()
    {
        instance = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        GameObject[] allObjects = FindObjectsOfType<GameObject>();

        foreach (GameObject go in allObjects)
        {
            if (go.name == "NetworkedClient")
                networkedClient = go;
        }
        for (int i = 0; i < transform.GetChild(0).childCount; i++)
        {
            playSpaces.Add(transform.GetChild(0).GetChild(i).GetComponent<Button>());
        }

        ResetBoard();
    }

    public void SlotPressed(int slot)
    {
        if (playersTurn == player1ID)
        {
            if (playerIcon != "Observer")
            {
                //playSpaces[slot].GetComponentInChildren<Text>().text = playerIcon;
                networkedClient.GetComponent<NetworkedClient>().SendMessageToHost(ClientToServerSignifiers.GameButtonPressed + "," + slot + "," + playerIcon);
                //Debug.Log(slot);
            }
        }
    }

    public void UpdateSlot(int slot, string playericon)
    {
        playSpaces[slot].GetComponentInChildren<Text>().text = playericon;

        playSpaces[slot].interactable = false;
        EndTurn(playericon);

    }

    public void ReplaySlot(int slot, string playericon)
    {
        playSpaces[slot].GetComponentInChildren<Text>().text = playericon;
    }

    public void SetupGame(int connectingID)
    {
        if (connectingID != player1ID && connectingID != player2ID)
        {
            playerIcon = "Observer";
            return;
        }
        if (startingPlayer == player1ID)
            playerIcon = "X";
        else
            playerIcon = "O";
    }
    private void EndTurn(string currentPlayersIcon)
    {
        playersTurn = (playersTurn == player1ID) ? player2ID : player1ID;
        if (playerIcon != "Observer")
            instructions.text = (instructions.text == yourTurnText) ? opponentTurnText : yourTurnText;
        else
            instructions.text = "Enjoy the Show!";

        //Check Row 1
        if (playSpaces[0].GetComponentInChildren<Text>().text == currentPlayersIcon &&
            playSpaces[1].GetComponentInChildren<Text>().text == currentPlayersIcon &&
            playSpaces[2].GetComponentInChildren<Text>().text == currentPlayersIcon)
        {
            GameOver(currentPlayersIcon);
        }
        //Check Row 2
        if (playSpaces[3].GetComponentInChildren<Text>().text == currentPlayersIcon &&
            playSpaces[4].GetComponentInChildren<Text>().text == currentPlayersIcon &&
            playSpaces[5].GetComponentInChildren<Text>().text == currentPlayersIcon)
        {
            GameOver(currentPlayersIcon);
        }
        //Check Row 3
        if (playSpaces[6].GetComponentInChildren<Text>().text == currentPlayersIcon &&
            playSpaces[7].GetComponentInChildren<Text>().text == currentPlayersIcon &&
            playSpaces[8].GetComponentInChildren<Text>().text == currentPlayersIcon)
        {
            GameOver(currentPlayersIcon);
        }
        //Check Col 1
        if (playSpaces[0].GetComponentInChildren<Text>().text == currentPlayersIcon &&
            playSpaces[3].GetComponentInChildren<Text>().text == currentPlayersIcon &&
            playSpaces[6].GetComponentInChildren<Text>().text == currentPlayersIcon)
        {
            GameOver(currentPlayersIcon);
        }
        //Check Col 2
        if (playSpaces[1].GetComponentInChildren<Text>().text == currentPlayersIcon &&
            playSpaces[4].GetComponentInChildren<Text>().text == currentPlayersIcon &&
            playSpaces[7].GetComponentInChildren<Text>().text == currentPlayersIcon)
        {
            GameOver(currentPlayersIcon);
        }
        //Check Col 3
        if (playSpaces[2].GetComponentInChildren<Text>().text == currentPlayersIcon &&
            playSpaces[5].GetComponentInChildren<Text>().text == currentPlayersIcon &&
            playSpaces[8].GetComponentInChildren<Text>().text == currentPlayersIcon)
        {
            GameOver(currentPlayersIcon);
        }
        //Check Left to Right Diagonal
        if (playSpaces[0].GetComponentInChildren<Text>().text == currentPlayersIcon &&
            playSpaces[4].GetComponentInChildren<Text>().text == currentPlayersIcon &&
            playSpaces[8].GetComponentInChildren<Text>().text == currentPlayersIcon)
        {
            GameOver(currentPlayersIcon);
        }
        // Check Right to Left Diagonal
        if (playSpaces[2].GetComponentInChildren<Text>().text == currentPlayersIcon &&
            playSpaces[4].GetComponentInChildren<Text>().text == currentPlayersIcon &&
            playSpaces[6].GetComponentInChildren<Text>().text == currentPlayersIcon)
        {
            GameOver(currentPlayersIcon);
        }
        moveCount++;

        if (moveCount > 8)
            GameOver("draw");


    }

    private void GameOver(string temp)
    {
        gameOver = true;
        if (temp != "draw")
        {
            string wintext = "'" + temp + "'" + " Wins";
            instructions.text = wintext;
        }
        else
            instructions.text = "Game ends in a draw";

        foreach (Button but in playSpaces)
        {
            but.interactable = false;
        }

        GameSystemManager.Instance.ChangeState(GameStates.End);
    }

    public void ResetBoard()
    {
        moveCount = 0;
        replayActionIndex = 0;
        GameSystemManager.Instance.chatManager.ResetChat();

        foreach (Button but in playSpaces)
        {
            but.GetComponentInChildren<Text>().text = "";
            but.interactable = true;
        }

        if (playerIcon == "Observer")
        {
            instructions.text = "Enjoy the Show!";
            return;
        }
        if (playersTurn == player1ID)
            instructions.text = yourTurnText;
        else
            instructions.text = opponentTurnText;
    }

    public void PrepBoardforReplay()
    {
        foreach (Button but in playSpaces)
        {
            but.GetComponentInChildren<Text>().text = "";
            but.interactable = false;
        }
    }

    public void Replay(int slot, string playericon, int isObserver)
    {
        if (isObserver == 1)
        {
            UpdateSlot(slot, playericon);
        }
        else
        {
            StartCoroutine(ShowReplayAction(replayActionIndex, slot, playericon));
            replayActionIndex++;
        }
    }

    IEnumerator ShowReplayAction(int i, int slot, string playericon)
    {
        yield return new WaitForSeconds(1.0f * i);
        ReplaySlot(slot, playericon);
    }

}

