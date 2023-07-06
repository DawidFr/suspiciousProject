using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
public class LobbyCreaterSimple : MonoBehaviour
{
    public TextMeshProUGUI lobbyName, maxPlayerCount;
    public TestLobby testLobby;
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))gameObject.SetActive(false);
    }

    public void CreateLobby(){
        int maxPlayers = System.Convert.ToInt32(maxPlayerCount);
        testLobby.CreateNewLobby(lobbyName.text, maxPlayers);
    }
}
