using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.Services.Lobbies.Models;

public class LobbyListObject : MonoBehaviour
{
    public TextMeshProUGUI lobbyNameText;
    public TextMeshProUGUI playerCount;
    public Button joinButton;
    private string lobbyId;

    private void Start() {
        this.gameObject.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);
    }

    public void Setup(string lobbyName,int players, string lobbyId){

        lobbyNameText.text = lobbyName;
        playerCount.text = players.ToString();
        joinButton.onClick.AddListener(SetActivate);
        this.lobbyId = lobbyId;
        
    }
    void SetActivate(){
        GameObject.FindWithTag("LobbyPanel").GetComponent<Button>().onClick.Invoke();

    }
    public void JoinLobby(){
        TestLobby testLobby = GameObject.FindGameObjectWithTag("Canvas").GetComponent<TestLobby>();
        //testLobby.LeaveLobby();
        testLobby.JoinLobbyById(lobbyId);
    }
    


}
