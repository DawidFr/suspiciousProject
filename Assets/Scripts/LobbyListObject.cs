using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class LobbyListObject : MonoBehaviour
{
    public TextMeshProUGUI lobbyNameText;
    public TextMeshProUGUI playerCount;


    public void Setup(string lobbyName,int players, int maxPlayers){
        lobbyNameText.text = lobbyName;
        playerCount.text = players + "/" + maxPlayers;
    }
}
