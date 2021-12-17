using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public class NetworkedClient : MonoBehaviour
{
    public int connectionID;
    int maxConnections = 1000;
    int reliableChannelID;
    int unreliableChannelID;
    int hostID;
    int socketPort = 5491;
    byte error;
    bool isConnected = false;
    int ourClientID;
    public ChatBehaviour chatManager;
    public TTTManager TTTmanager;
    public GameSystemManager gameSystemManager;

    // Start is called before the first frame update
    void Start()
    {
        Connect();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.S))
            Connect();

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
                    break;
                case NetworkEventType.DisconnectEvent:
                    isConnected = false;
                    GameSystemManager.Instance.ChangeState(GameStates.LoginMenu);
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
            config.DisconnectTimeout = 3000;
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
        Debug.Log("msg recieved = " + msg + ".  connection id = " + id);

        string[] csv = msg.Split(',');

        int signifier = int.Parse(csv[0]);

        switch (signifier)
        {
            case ServertoClientSignifiers.LoginComplete:
                GameSystemManager.Instance.ChangeState(GameStates.MainMenu);
                Debug.Log("Account Login Complete");
                break;
            case ServertoClientSignifiers.LoginFailed:
                Debug.Log("Account Login Failed");
                break;
            case ServertoClientSignifiers.AccountCreationComplete:
                GameSystemManager.Instance.ChangeState(GameStates.MainMenu);
                Debug.Log("Account Creation Complete");
                break;
            case ServertoClientSignifiers.AccountCreationFailed:
                Debug.Log("Account Creation Failed");
                break;
            case ServertoClientSignifiers.OpponentPlay:
                //UpdateSlot(int.Parse(csv[1]), csv[2]);
                TTTmanager.UpdateSlot(int.Parse(csv[1]), csv[2]);
                //Debug.Log(csv[1] + " " + csv[2]);
                break;
            case ServertoClientSignifiers.GameStart:
                gameSystemManager.ChangeState(GameStates.Game);
                TTTmanager.firstPlayer = int.Parse(csv[1]);
                TTTmanager.secondPlayer = int.Parse(csv[2]);
                TTTmanager.startingPlayer = int.Parse(csv[3]);
                //Debug.Log("Starting player: " + TTTManager.Instance.startingPlayer);
                TTTmanager.playersTurn = TTTManager.Instance.startingPlayer;
                TTTmanager.AssignRole(int.Parse(csv[4]));
                TTTmanager.ResetBoard();
                break;
            case ServertoClientSignifiers.SendChatMessage:
                //chatManager.UpdatingChat(csv[1], csv[2]);
                chatManager.UpdatingChat(csv[1]);
                break;
            case ServertoClientSignifiers.BackToMainMenu:
                GameSystemManager.Instance.ChangeState(GameStates.MainMenu);
                break;
            case ServertoClientSignifiers.SendReplay:
                TTTManager.Instance.Replay(int.Parse(csv[1]), csv[2], int.Parse(csv[3]));
                break;
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
    public const int LoginAccount = 2;
    public const int JoinQueue = 3;
    public const int GameButtonPressed = 4;
    public const int ChatMessageSent = 5;
    public const int JoinAsObserver = 6;
    public const int LeaveRoom = 7;
    public const int GetReplay = 8;
    public const int PlayerMove = 9;
}

public static class ServertoClientSignifiers
{
    public const int LoginComplete = 1;
    public const int LoginFailed = 2;
    public const int AccountCreationComplete = 3;
    public const int AccountCreationFailed = 4;
    public const int OpponentPlay = 5;
    public const int GameStart = 6;
    public const int SendChatMessage = 7;
    public const int BackToMainMenu = 8;
    public const int SendReplay = 9;
    public const int PlayerMove = 10;
}
