using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameSystemManager : MonoBehaviour
{
    [Header("Login Menu References")]
    [SerializeField]GameObject submitButton;
    [SerializeField]GameObject userNameInput;
    [SerializeField]GameObject passwordInput;
    [SerializeField]GameObject createToggle;
    [SerializeField]GameObject loginToggle;
    [SerializeField]GameObject loginPanel;
    [Header("Main Menu References")]
    [SerializeField] GameObject joinButton;
    [SerializeField] GameObject queuepanel;
    [SerializeField] GameObject mainMenuPanel;
    [SerializeField] GameObject observerButton;
    [Header("Game UI References")]
    [SerializeField] GameObject mainMenuButton;
    [SerializeField] GameObject gamePanel;
    [SerializeField] GameObject endPanel;

    public ChatBehaviour chatManager;
    [SerializeField]
    GameObject networkedClient;

    private static GameSystemManager instance;
    public static GameSystemManager Instance { get { return instance; } }

    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        submitButton.GetComponent<Button>().onClick.AddListener(SubmitButtonPressed);
        joinButton.GetComponent<Button>().onClick.AddListener(JoinGameButtonPressed);
        observerButton.GetComponent<Button>().onClick.AddListener(WatchAsObserverButtonPressed);
        mainMenuButton.GetComponent<Button>().onClick.AddListener(MainMenuButtonPressed);

        loginPanel.SetActive(false);
        mainMenuPanel.SetActive(false);
        gamePanel.SetActive(false);
        queuepanel.SetActive(false);
        endPanel.SetActive(false);
        ChangeState(GameStates.LoginMenu);
    }

    public void SubmitButtonPressed()
    {
        string n = userNameInput.GetComponent<InputField>().text;
        string p = passwordInput.GetComponent<InputField>().text;
        string msg;

        if(createToggle.GetComponent<Toggle>().isOn)
            msg = ClientToServerSignifiers.CreateAccount + "," + n + "," + p;
        else
            msg = ClientToServerSignifiers.LoginAccount + "," + n + "," + p;

        networkedClient.GetComponent<NetworkedClient>().SendMessageToHost(msg);
    }

    public void JoinGameButtonPressed()
    {
        networkedClient.GetComponent<NetworkedClient>().SendMessageToHost(ClientToServerSignifiers.JoinQueue + "");
        ChangeState(GameStates.WaitingInQueue);
    }

    public void WatchAsObserverButtonPressed()
    {
        networkedClient.GetComponent<NetworkedClient>().SendMessageToHost(ClientToServerSignifiers.JoinAsObserver + "");
        ChangeState(GameStates.WaitingInQueue);
    }

    public void MainMenuButtonPressed()
    {
        networkedClient.GetComponent<NetworkedClient>().SendMessageToHost(ClientToServerSignifiers.LeaveRoom + "");
        ChangeState(GameStates.MainMenu);
    }

    public void ReplayButtonPressed()
    {
        networkedClient.GetComponent<NetworkedClient>().SendMessageToHost(ClientToServerSignifiers.GetReplay + "");
        ChangeState(GameStates.Game);
    }

    public void GameButtonPressed()
    {
        networkedClient.GetComponent<NetworkedClient>().SendMessageToHost(ClientToServerSignifiers.GameButtonPressed + "");
        //ChangeState(GameStates.Game);
    }

    public void ChangeState(int newState)
    {

        switch (newState)
        {
            case GameStates.LoginMenu:
                mainMenuPanel.SetActive(false);
                gamePanel.SetActive(false);
                queuepanel.SetActive(false);
                endPanel.SetActive(false);
                loginPanel.SetActive(true);
                break;
            case GameStates.MainMenu:
                loginPanel.SetActive(false);
                gamePanel.SetActive(false);
                endPanel.SetActive(false);
                mainMenuPanel.SetActive(true);
                break;
            case GameStates.WaitingInQueue:
                mainMenuPanel.SetActive(false);
                queuepanel.SetActive(true);
                break;
            case GameStates.Game:
                mainMenuPanel.SetActive(false);
                queuepanel.SetActive(false);
                endPanel.SetActive(false);
                gamePanel.SetActive(true);
                break;
            case GameStates.End:
                //gamePanel.SetActive(true);
                endPanel.SetActive(true);
                break;
        }
    }
}

static public class GameStates
{
    public const int LoginMenu = 1;
    public const int MainMenu = 2;
    public const int WaitingInQueue = 3;
    public const int Game = 4;
    public const int End = 5;
}
