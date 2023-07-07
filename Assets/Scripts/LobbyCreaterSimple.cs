using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
public class LobbyCreaterSimple : MonoBehaviour
{
    public TMP_InputField lobbyName, maxPlayerCount;
    public TestLobby testLobby;
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))gameObject.SetActive(false);
    }

    public void CreateLobby(){
        int maxPlayers;
        int.TryParse(maxPlayerCount.text.ToString(), out maxPlayers);
        Debug.Log(maxPlayers);
        Debug.Log(maxPlayerCount.text);
        Debug.Log(maxPlayerCount.text.ToString());
        testLobby.CreateNewLobby(lobbyName.text, maxPlayers);
    }
}
