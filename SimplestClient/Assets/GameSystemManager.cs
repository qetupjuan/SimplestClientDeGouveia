using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameSystemManager : MonoBehaviour
{

    GameObject submitButton, userNameInput, passwordInput, toggle, ConnectionToHost, gameRoomButton, observerButton, titleText, ticTacToeWindow, logInWindow, roomNumInput, leaveRoomButton;
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
            else if (go.name == "Panel Title")
                titleText = go;
            else if (go.name == "TicTacToe Window")
                ticTacToeWindow = go;
            else if (go.name == "Log In Window")
                logInWindow = go;
            else if (go.name == "Observer Button")
                observerButton = go;
            else if (go.name == "RoomNumInputField")
                roomNumInput = go;
            else if (go.name == "LeaveRoomButton")
                leaveRoomButton = go;
        }

        submitButton.GetComponent<Button>().onClick.AddListener(SubmitButtonPressed);
        gameRoomButton.GetComponent<Button>().onClick.AddListener(GameRoomButtonPressed);
        toggle.GetComponent<Toggle>().onValueChanged.AddListener(NewUserTogglePressed);

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
        if (isNewUser)
            titleText.GetComponent<Text>().text = "Create Account";
        else
            titleText.GetComponent<Text>().text = "Login";
    }

    private void GameRoomButtonPressed()
    {
        ConnectionToHost.GetComponent<NetworkedClient>().SendMessageToHost(ClientToServerSignifiers.JoinGameRoomQueue + "");
        ChangeState(GameStates.WaitingInQueueForOtherPlayer);
    }

    public void ChangeState(int state)
    {
        userNameInput.SetActive(false);
        passwordInput.SetActive(false);
        submitButton.SetActive(false);
        toggle.SetActive(false);
        gameRoomButton.SetActive(false);
        titleText.SetActive(false);
        observerButton.SetActive(false);
        roomNumInput.SetActive(false);
        leaveRoomButton.SetActive(false);

        ticTacToeWindow.SetActive(false);
        logInWindow.SetActive(false);

        if (state == GameStates.LoginMenu)
        {
            logInWindow.SetActive(true);
            submitButton.SetActive(true);
            userNameInput.SetActive(true);
            passwordInput.SetActive(true);
            toggle.SetActive(true);
            titleText.SetActive(true);
        }
        else if (state == GameStates.MainMenu)
        {
            logInWindow.SetActive(true);
            observerButton.SetActive(true);
            gameRoomButton.SetActive(true);
            roomNumInput.SetActive(true);
        }
        else if (state == GameStates.WaitingInQueueForOtherPlayer)
        {
            logInWindow.SetActive(true);
            leaveRoomButton.SetActive(true);
        }
        else if (state == GameStates.TicTacToe)
        {
            ticTacToeWindow.SetActive(true);
            ticTacToeWindow.GetComponent<TicTacToeManager>().SetNetworkConnection(ConnectionToHost.GetComponent<NetworkedClient>());
            leaveRoomButton.SetActive(true);
        }


    }

    private void OnDisable()
    {
        if (submitButton != null)
            submitButton.GetComponent<Button>().onClick.RemoveAllListeners();
        if (gameRoomButton != null)
            gameRoomButton.GetComponent<Button>().onClick.RemoveAllListeners();
        if (toggle != null)
            toggle.GetComponent<Toggle>().onValueChanged.RemoveAllListeners();

    }


}


static public class GameStates
{
    public const int LoginMenu = 1;
    public const int MainMenu = 2;
    public const int WaitingInQueueForOtherPlayer = 3;
    public const int TicTacToe = 4;
}