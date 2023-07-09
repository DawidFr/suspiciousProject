using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.UI;

public class LobbyPanelController : MonoBehaviour
{
    public TextMeshProUGUI lobbyNameTM;
    public TestLobby testLobby;
    public GameObject content;
    public GameObject playerPanel;
    public string randomstring;
    private bool isReady;
    public Button startGameButton;

    private void Awake() {
        testLobby.OnLobbyJoin += LobbyOpen;
        testLobby.OnLobbyUpdate += Refreash;
    }
    private void OnEnable() {
        randomstring = Random.Range(0f, 100f).ToString();
        //StartCoroutine(LobbyOpen());
        //LobbyOpen();
    }
    public async void SetReady(){
        isReady = !isReady;
        try{
            UpdatePlayerOptions updatePlayerOptions = new UpdatePlayerOptions();
            updatePlayerOptions.Data = new Dictionary<string, PlayerDataObject>{
                {"IsReady", new PlayerDataObject(PlayerDataObject.VisibilityOptions.Public,value: isReady.ToString())}
            };
            await LobbyService.Instance.UpdatePlayerAsync(testLobby.joinedLobby.Id,testLobby.playerIdtext.text ,updatePlayerOptions);
        }catch(LobbyServiceException e){
            Debug.Log(e);
        }
        Refreash();
    }
    public void LobbyOpen(){
        //Debug.Log(testLobby.joinedLobby.Id);
        //yield return new WaitForSeconds(5f);
        lobbyNameTM.text = testLobby.joinedLobby.Name;
        foreach(Player player in testLobby.joinedLobby.Players){
            GameObject pl = Instantiate(playerPanel);
            pl.GetComponent<PlayerListObject>().Setup(player.Data["PLayerName"].Value , player.Data["IsReady"].Value);
            pl.transform.SetParent(content.transform);
            pl.name = randomstring;
        }
        if(testLobby.IsHost())startGameButton.gameObject.SetActive(true);
    }
    /*void Update(){
        refreashTime -= Time.deltaTime;
        if(refreashTime < 0 && testLobby.joinedLobby != null && canRefreash) Refreash();
    }*/
    public void Refreash(){
        //refreashTime = 3f;
        foreach(Transform g in content.GetComponentsInChildren<Transform>()){
                if(g != null && g.name == randomstring) Destroy(g.gameObject);
        }
        randomstring = Random.Range(0f, 100f).ToString();
        foreach(Player player in testLobby.joinedLobby.Players){
            GameObject pl = Instantiate(playerPanel);
            pl.GetComponent<PlayerListObject>().Setup(player.Data["PLayerName"].Value, player.Data["IsReady"].Value);
            pl.transform.SetParent(content.transform);
            pl.name = randomstring;
        }
    }
}
