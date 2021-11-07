using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameSystemFolder : MonoBehaviour
{

    GameObject submitButton, userNameInput, passwordInput, toggle, ConnectionToHost;
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
            else if (go.name == "NetworkClient")
                ConnectionToHost = go;
        }

        submitButton.GetComponent<Button>().onClick.AddListener(SubmitButtonPressed);
        toggle.GetComponent<Toggle>().onValueChanged.AddListener(NewUserTogglePressed);
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

    private void OnDisable()
    {
        submitButton.GetComponent<Button>().onClick.RemoveAllListeners();
        toggle.GetComponent<Toggle>().onValueChanged.RemoveAllListeners();
    }


}