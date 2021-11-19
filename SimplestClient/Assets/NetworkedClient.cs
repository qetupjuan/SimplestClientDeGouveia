using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class NetworkedClient : MonoBehaviour
{

    int connectionID;
    int maxConnections = 1000;
    int reliableChannelID;
    int unreliableChannelID;
    int hostID;
    int socketPort = 5491;
    byte error;
    bool isConnected = false;
    int ourClientID;
    public Text chatText = null;

    GameObject gameSystemManager, ticTacToeManager;


    // Start is called before the first frame update
    void Start()
    {
        GameObject[] allObjects = UnityEngine.Object.FindObjectsOfType<GameObject>();

        foreach (GameObject go in allObjects)
        {
            if (go.GetComponent<GameSystemManager>() != null)
                gameSystemManager = go;
            if (go.GetComponent<TicTacToeManager>() != null)
                ticTacToeManager = go;
        }

        Connect();
    }

    void Update()
    {
        UpdateNetworkConnection();
    }

    private void UpdateNetworkConnection()
    {
        if (isConnected)
        {
            int recHostID;
            int recConnectionID;
            int recChannelID;
            byte[] recBuffer = new byte[1024];
            int bufferSize = 1024;
            int dataSize;
            NetworkEventType recNetworkEvent = NetworkTransport.Receive(out recHostID, out recConnectionID, out recChannelID, recBuffer, bufferSize, out dataSize, out error);

            switch (recNetworkEvent)
            {
                case NetworkEventType.ConnectEvent:
                    Debug.Log("connected.  " + recConnectionID);
                    ourClientID = recConnectionID;
                    break;
                case NetworkEventType.DataEvent:
                    string msg = Encoding.Unicode.GetString(recBuffer, 0, dataSize);
                    ProcessRecievedMsg(msg, recConnectionID);
                    //Debug.Log("got msg = " + msg);
                    break;
                case NetworkEventType.DisconnectEvent:
                    isConnected = false;
                    Debug.Log("disconnected.  " + recConnectionID);
                    break;
            }
        }
    }

    private void Connect()
    {

        if (!isConnected)
        {
            Debug.Log("Attempting to create connection");

            NetworkTransport.Init();

            ConnectionConfig config = new ConnectionConfig();
            reliableChannelID = config.AddChannel(QosType.Reliable);
            unreliableChannelID = config.AddChannel(QosType.Unreliable);
            HostTopology topology = new HostTopology(config, maxConnections);
            hostID = NetworkTransport.AddHost(topology, 0);
            Debug.Log("Socket open.  Host ID = " + hostID);

            connectionID = NetworkTransport.Connect(hostID, "192.168.2.23", socketPort, 0, out error); // server is local on network

            if (error == 0)
            {
                isConnected = true;

                Debug.Log("Connected, id = " + connectionID);

            }
        }
    }

    public void Disconnect()
    {
        NetworkTransport.Disconnect(hostID, connectionID, out error);
    }

    public void SendMessageToHost(string msg)
    {
        byte[] buffer = Encoding.Unicode.GetBytes(msg);
        NetworkTransport.Send(hostID, connectionID, reliableChannelID, buffer, msg.Length * sizeof(char), out error);
    }

    private void ProcessRecievedMsg(string msg, int id)
    {
        Debug.Log("msg received = " + msg + ".  connection id = " + id);

        string[] csv = msg.Split(',');

        int signifier = int.Parse(csv[0]);

        if (signifier == ServerToClientSignifiers.AccountCreated || signifier == ServerToClientSignifiers.LoginComplete)
        {
            gameSystemManager.GetComponent<GameSystemManager>().ChangeState(GameStates.MainMenu);
        }
        else if (signifier == ServerToClientSignifiers.GameStart)
        {
            gameSystemManager.GetComponent<GameSystemManager>().ChangeState(GameStates.TicTacToe);
        }
        else if (signifier == ServerToClientSignifiers.ChosenAsPlayerOne)
        {
            ticTacToeManager.GetComponent<TicTacToeManager>().ChosenAsPlayerOne();
        }
        else if (signifier == ServerToClientSignifiers.OpponentChoseASquare)
        {
            ticTacToeManager.GetComponent<TicTacToeManager>().OpponentTookTurn(int.Parse(csv[1]));
        }
        else if (signifier == ServerToClientSignifiers.OpponentLeftRoomEarly)
        {
            ticTacToeManager.GetComponent<TicTacToeManager>().OnGameOver(":)");
        }
        else if (signifier == ServerToClientSignifiers.OpponentWonTicTacToe)
        {
            ticTacToeManager.GetComponent<TicTacToeManager>().OnGameOver(":(");
        }
        else if (signifier == ServerToClientSignifiers.GameTied)
        {
            ticTacToeManager.GetComponent<TicTacToeManager>().OnGameOver(":/");
        }
    }

    public bool IsConnected()
    {
        return isConnected;
    }
}


public static class ClientToServerSignifiers
{
    public const int CreateAccount = 1;
    public const int Login = 2;
    public const int JoinGameRoomQueue = 3;
    public const int SelectedTicTacToeSquare = 4;
    public const int WonTicTacToe = 5;
    public const int GameTied = 6;
    public const int LeavingGameRoom = 7;
    public const int ChatLogMessage = 8;
}

public static class ServerToClientSignifiers
{
    public const int LoginComplete = 1;
    public const int LoginFailed = 2;
    public const int AccountCreated = 3;
    public const int AccountCreationFailed = 4;
    public const int GameStart = 5;
    public const int ChosenAsPlayerOne = 6;
    public const int OpponentChoseASquare = 7;
    public const int OpponentLeftRoomEarly = 8;
    public const int OpponentWonTicTacToe = 9;
    public const int GameTied = 10;
    public const int ChatLogMessage = 8;
}