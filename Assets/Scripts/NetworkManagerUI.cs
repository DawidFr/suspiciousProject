using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class NetworkManagerUI : NetworkBehaviour
{
    [SerializeField] private Button serverBtn, hostBtn, clientBtn;
    [SerializeField] private TMP_InputField nameInputField;     

    private void Awake() {

        serverBtn.onClick.AddListener(() => {
            NetworkManager.Singleton.StartServer();
        });
        hostBtn.onClick.AddListener(() => {
            NetworkManager.Singleton.StartHost();
            //StreamChatBehaviour.Instance.CreateOrGetChannel("1234");
        });
        clientBtn.onClick.AddListener(() => {
            NetworkManager.Singleton.StartClient();
        });
        //NetworkManager.Singleton.OnClientConnectedCallback += OnPlayerConnectedToServer;
    }
}
