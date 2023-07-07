using System.Collections;
using System.Collections.Generic;
using Unity.Services.Lobbies.Models;
using UnityEngine;



public class LobbyPanelController : MonoBehaviour
{
    public TestLobby testLobby;
    public GameObject content;
    public GameObject playerPanel;
    public string randomstring;
    bool canRefreash;
    float refreashTime;

    private void Awake() {
        testLobby.OnLobbyJoin += LobbyOpen;
        testLobby.OnLobbyUpdate += Refreash;
    }
    private void OnEnable() {
        randomstring = Random.Range(0f, 100f).ToString();
        //StartCoroutine(LobbyOpen());
        //LobbyOpen();
    }
    public void LobbyOpen(){
        //Debug.Log(testLobby.joinedLobby.Id);
        //yield return new WaitForSeconds(5f);
        foreach(Player player in testLobby.joinedLobby.Players){
            GameObject pl = Instantiate(playerPanel);
            pl.GetComponent<PlayerListObject>().Setup(player.Data["PLayerName"].Value);
            pl.transform.SetParent(content.transform);
            pl.name = randomstring;
        }
        canRefreash = true;
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
            pl.GetComponent<PlayerListObject>().Setup(player.Data["PLayerName"].Value);
            pl.transform.SetParent(content.transform);
            pl.name = randomstring;
        }
    }
}
