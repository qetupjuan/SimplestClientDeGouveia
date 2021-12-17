using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChatBehaviour : MonoBehaviour
{
    [SerializeField] GameObject networkedClient;
    [SerializeField] InputField chatBox;

    List<string> chatMessages;
    public Text chatLog;

    private void Awake()
    {
        chatMessages = new List<string>();
    }
    private void Start()
    {
        chatLog.text = "";
    }
    public void PremadeMessagePressed(Button pressedButton)
    {
        string msg = pressedButton.GetComponentInChildren<Text>().text;
        networkedClient.GetComponent<NetworkedClient>().SendMessageToHost(ClientToServerSignifiers.ChatMessageSent + "," + msg);
    }

    public void SubmitPressed()
    {
        if (chatBox.text != "")
        {
            string msg = chatBox.text;
            networkedClient.GetComponent<NetworkedClient>().SendMessageToHost(ClientToServerSignifiers.ChatMessageSent + "," + msg);
        }
        chatBox.text = "";
    }
    public void UpdatingChat(string chatMsg)
    {
        string newMsg = chatMsg;
        chatMessages.Add(newMsg);
        if (chatMessages.Count > 10)
        {
            chatMessages.RemoveAt(0);
        }
        chatLog.text = "";
        foreach (string msg in chatMessages)
        {
            chatLog.text += msg + "\n";
        }
    }

    public void ResetChat()
    {
        foreach (string msg in chatMessages)
        {
            chatMessages.Remove(msg);
        }
        chatLog.text = "";
    }
}