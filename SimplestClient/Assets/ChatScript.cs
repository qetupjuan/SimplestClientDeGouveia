using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChatBehaviour : MonoBehaviour
{
    [SerializeField]
    List<Text> textLines;

    public GameObject inputField, sendButton, connectionToClient;

    List<Button> prefabMessages;

    private void Start()
    {
        sendButton.GetComponent<Button>().onClick.AddListener(OnSendButtonPressed);
    }

    public void AddChatMessage(string msg, bool fromPlayer)
    {
        //start at the top and copy the text box below itself
        for (int i = textLines.Count - 1; i > 0; i--)
        {
            textLines[i].text = textLines[i - 1].text;
            textLines[i].alignment = textLines[i - 1].alignment;
        }
        textLines[0].text = msg;

        if (fromPlayer)
        {
            textLines[0].alignment = TextAnchor.MiddleRight;
        }
        else
        {
            textLines[0].alignment = TextAnchor.MiddleLeft;
        }
    }

    void SendChatMessageToServer(string msg)
    {
        connectionToClient.GetComponent<NetworkedClient>().SendMessageToHost(ClientToServerSignifiers.ChatLogMessage + "," + msg);
    }
    public void OnPrefabMessagePressed(string msg)
    {
        AddChatMessage(msg, true);
        SendChatMessageToServer(msg);
    }
    void OnSendButtonPressed()
    {
        InputField input = inputField.GetComponent<InputField>();
        string msg = input.textComponent.text;
        if (msg == "")
            return;

        input.text = "";

        AddChatMessage(msg, true);
        SendChatMessageToServer(msg);
    }

    void ClearAllMessages()
    {
        foreach (Text t in textLines)
        {
            t.text = "";
        }
    }

    private void OnDisable()
    {
        if (textLines != null)
        {
            ClearAllMessages();
        }
    }
}
