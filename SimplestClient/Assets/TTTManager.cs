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

    public int firstPlayer = 0;
    public int secondPlayer = 0;
    public int startingPlayer;
    public int moveCount;
    int replayActionIndex = 0;

    string playerIcon;
    bool gameOver = false;

    string yourTurnText = "Now is your turn";
    string opponentTurnText = "Opponent is choosing";

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

    public void AssignRole(int connectingID)
    {
        if (connectingID != firstPlayer && connectingID != secondPlayer)
        {
            playerIcon = "Observer";
            return;
        }
        if (startingPlayer == firstPlayer)
            playerIcon = "X";
        else
            playerIcon = "O";
    }

    public void TTTSlotPressed(int slot)
    {
        if (playerIcon != "Observer")
        {
            playSpaces[slot].GetComponentInChildren<Text>().text = playerIcon;
            networkedClient.GetComponent<NetworkedClient>().SendMessageToHost(ClientToServerSignifiers.GameButtonPressed + "," + slot + "," + playerIcon);
            Debug.Log(slot);
        }
    }

    public void UpdateSlot(int slot, string playerIcon)
    {
        playSpaces[slot].GetComponentInChildren<Text>().text = playerIcon;
        playSpaces[slot].interactable = false;
        CheckIfWin(playerIcon);
    }

    public void ReplaySlot(int slot, string playericon)
    {
        playSpaces[slot].GetComponentInChildren<Text>().text = playericon;
    }

    private void CheckIfWin(string currentPlayersIcon)
    {
        playersTurn = (playersTurn == firstPlayer) ? secondPlayer : firstPlayer;
        if (playerIcon != "Observer")
            instructions.text = (instructions.text == yourTurnText) ? opponentTurnText : yourTurnText;
        else
            instructions.text = "You have become a Watcher";
       
        if (playSpaces[0].GetComponentInChildren<Text>().text == currentPlayersIcon &&
            playSpaces[1].GetComponentInChildren<Text>().text == currentPlayersIcon &&
            playSpaces[2].GetComponentInChildren<Text>().text == currentPlayersIcon) // first row
        {
            GameOver(currentPlayersIcon);
        }
        if (playSpaces[3].GetComponentInChildren<Text>().text == currentPlayersIcon &&
            playSpaces[4].GetComponentInChildren<Text>().text == currentPlayersIcon &&
            playSpaces[5].GetComponentInChildren<Text>().text == currentPlayersIcon) // second row
        {
            GameOver(currentPlayersIcon);
        }
        if (playSpaces[6].GetComponentInChildren<Text>().text == currentPlayersIcon &&
            playSpaces[7].GetComponentInChildren<Text>().text == currentPlayersIcon &&
            playSpaces[8].GetComponentInChildren<Text>().text == currentPlayersIcon) // third row
        {
            GameOver(currentPlayersIcon);
        }
        if (playSpaces[0].GetComponentInChildren<Text>().text == currentPlayersIcon &&
            playSpaces[3].GetComponentInChildren<Text>().text == currentPlayersIcon &&
            playSpaces[6].GetComponentInChildren<Text>().text == currentPlayersIcon) // first column
        {
            GameOver(currentPlayersIcon);
        }
        if (playSpaces[1].GetComponentInChildren<Text>().text == currentPlayersIcon &&
            playSpaces[4].GetComponentInChildren<Text>().text == currentPlayersIcon &&
            playSpaces[7].GetComponentInChildren<Text>().text == currentPlayersIcon) // second column
        {
            GameOver(currentPlayersIcon);
        }
        if (playSpaces[2].GetComponentInChildren<Text>().text == currentPlayersIcon &&
            playSpaces[5].GetComponentInChildren<Text>().text == currentPlayersIcon &&
            playSpaces[8].GetComponentInChildren<Text>().text == currentPlayersIcon) // third column
        {
            GameOver(currentPlayersIcon);
        }
        if (playSpaces[0].GetComponentInChildren<Text>().text == currentPlayersIcon &&
            playSpaces[4].GetComponentInChildren<Text>().text == currentPlayersIcon &&
            playSpaces[8].GetComponentInChildren<Text>().text == currentPlayersIcon) // first diagonal
        {
            GameOver(currentPlayersIcon);
        }
        if (playSpaces[2].GetComponentInChildren<Text>().text == currentPlayersIcon &&
            playSpaces[4].GetComponentInChildren<Text>().text == currentPlayersIcon &&
            playSpaces[6].GetComponentInChildren<Text>().text == currentPlayersIcon) // second diagonal
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
            instructions.text = "You have become a Watcher";
            return;
        }
        if (playersTurn == firstPlayer)
            instructions.text = yourTurnText;
        else
            instructions.text = opponentTurnText;
    }

    public void Replay(int slot, string playericon, int isObserver)
    {
        if (isObserver == 1)
        {
            UpdateSlot(slot, playericon);
        }
        else
        {
            StartCoroutine(ReplaySequence(replayActionIndex, slot, playericon));
            replayActionIndex++;
        }
    }

    IEnumerator ReplaySequence(int i, int slot, string playericon)
    {
        yield return new WaitForSeconds(1.0f * i);
        ReplaySlot(slot, playericon);
    }
}

