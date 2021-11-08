using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameSystemFolder : MonoBehaviour
{

    GameObject submitButton, userNameInput, passwordInput, toggle, ConnectionToHost, gameRoomButton, ticTacToeButton;
    bool isNewUser = false;

    // Start is called before the first frame update
    void Start()
    {

        GameObject[] allObjects = FindObjectsOfType<GameObject>();

        foreach (GameObject go in allObjects)
        {
            if (go.name == "username input")
                userNameInput = go;
            else if (go.name == "password input")
                passwordInput = go;
            else if (go.name == "logIn button")
                submitButton = go;
            else if (go.name == "toggle")
                toggle = go;
            else if (go.name == "GameRoomButton")
                gameRoomButton = go;
            else if (go.name == "NetworkClient")
                ConnectionToHost = go;
            else if (go.name == "TicTacToeSquare")
                ticTacToeButton = go;
        }

        submitButton.GetComponent<Button>().onClick.AddListener(SubmitButtonPressed);
        gameRoomButton.GetComponent<Button>().onClick.AddListener(GameRoomButtonPressed);
        toggle.GetComponent<Toggle>().onValueChanged.AddListener(NewUserTogglePressed);
        ticTacToeButton.GetComponent<Button>().onClick.AddListener(TicTacToeButtonPressed);
        ChangeState(GameStates.LoginMenu);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SubmitButtonPressed()
    {
        string u = userNameInput.GetComponent<InputField>().text;
        string p = passwordInput.GetComponent<InputField>().text;

        string msg;
        if (isNewUser)
            msg = ClientToServerSignifiers.CreateAccount + "," + u + "," + p;
        else
            msg = ClientToServerSignifiers.Login + "," + u + "," + p;

        ConnectionToHost.GetComponent<NetworkedClient>().SendMessageToHost(msg);

    }

    public void NewUserTogglePressed(bool newValue)
    {
        isNewUser = newValue;
    }

    private void GameRoomButtonPressed()
    {
        ConnectionToHost.GetComponent<NetworkedClient>().SendMessageToHost(ClientToServerSignifiers.JoinGameRoomQueue + "");
        ChangeState(GameStates.WaitingInQueueForOtherPlayer);
    }

    private void TicTacToeButtonPressed()
    {

    }

    public void ChangeState(int state)
    {
        if (state == GameStates.LoginMenu)
        {
            submitButton.SetActive(true);
            userNameInput.SetActive(true);
            passwordInput.SetActive(true);
            toggle.SetActive(true);
            gameRoomButton.SetActive(false);
            ticTacToeButton.SetActive(false);
        }
        else if (state == GameStates.MainMenu)
        {
            submitButton.SetActive(false);
            userNameInput.SetActive(false);
            passwordInput.SetActive(false);
            toggle.SetActive(false);
            gameRoomButton.SetActive(true);
        }
        else if (state == GameStates.WaitingInQueueForOtherPlayer)
        {
            gameRoomButton.SetActive(false);
        }
        else if (state == GameStates.TicTacToe)
        {
            ticTacToeButton.SetActive(true);
        }


    }

    private void OnDisable()
    {
        submitButton.GetComponent<Button>().onClick.RemoveAllListeners();
        gameRoomButton.GetComponent<Button>().onClick.RemoveAllListeners();
        toggle.GetComponent<Toggle>().onValueChanged.RemoveAllListeners();
        ticTacToeButton.GetComponent<Button>().onClick.RemoveAllListeners();
    }


}


static public class GameStates
{
    public const int LoginMenu = 1;
    public const int MainMenu = 2;
    public const int WaitingInQueueForOtherPlayer = 3;
    public const int TicTacToe = 4;
}