using System.Collections;
using Unity.Services.Core;
using Unity.Services.Authentication;
using System.Collections.Generic;
using Unity.Services.Multiplay;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class TestLobby : MonoBehaviour
{
    public GameObject LobbyPanelObject;
    public GameObject LobbyContainer;
    public TMP_InputField playerName;
    public TMP_InputField idInput;
    private Lobby hostLobby;
    private string randomstring;
    private Lobby joinedLobby;
    private float heartbeatTimer;
    private float lobbyUpdateTimer;
    // Start is called before the first frame update
    private async void Start()
    {
        await UnityServices.InitializeAsync();
        AuthenticationService.Instance.SignedIn += () =>{   
            Debug.Log("Signed");
        };
        await AuthenticationService.Instance.SignInAnonymouslyAsync();
    }
    private void Update() {
        if(Input.GetKeyDown(KeyCode.Q)) CreateLobby();
        if(Input.GetKeyDown(KeyCode.W)) ListLobbies();
        if(Input.GetKeyDown(KeyCode.E)) JoinLobbyByCode(idInput.text);
        if(Input.GetKeyDown(KeyCode.R)) PrintPlayers(hostLobby);
        HandleLobbyUpdates();
        HandleLobbbyHeartbeat();
    }
    async void HandleLobbbyHeartbeat(){
        if(hostLobby != null){
            heartbeatTimer -= Time.deltaTime;
            if(heartbeatTimer >= 0f){
                heartbeatTimer = 15f;
                await LobbyService.Instance.SendHeartbeatPingAsync(hostLobby.Id);

            }
        }
    }
    private async void HandleLobbyUpdates(){
        if(joinedLobby != null){
            lobbyUpdateTimer -= Time.deltaTime;
            if(lobbyUpdateTimer >= 0f){
                lobbyUpdateTimer = 1.1f;
                Lobby lobby = await LobbyService.Instance.GetLobbyAsync(joinedLobby.Id);
                joinedLobby = lobby;

            }
        }
    }

    public async void CreateLobby(){
        try{
            string lobbyName = "simpleLobby";
            int maxPlayers = 4;
            CreateLobbyOptions createLobbyOptions = new CreateLobbyOptions{
                IsPrivate = false,
                Player = GetPlayer(),
                Data = new Dictionary<string, DataObject>{
                    {"Gamemode" , new DataObject(DataObject.VisibilityOptions.Public, "Casual")}
                }                
            };
            Lobby lobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName, maxPlayers, createLobbyOptions);
            hostLobby = lobby;
            joinedLobby = hostLobby;
            PrintPlayers(hostLobby);
            Debug.Log("New lobby " + lobbyName + " " + maxPlayers + " " + lobby.LobbyCode);
        }
        catch(LobbyServiceException e){
            Debug.Log(e);
        }
    }
    private async void ListLobbies(){
        QueryLobbiesOptions queryLobbiesOptions = new QueryLobbiesOptions{
                Count = 25,
                Filters = new List<QueryFilter>(){
                    new QueryFilter(QueryFilter.FieldOptions.AvailableSlots, "0", QueryFilter.OpOptions.GT)
                },
                Order = new List<QueryOrder>{
                    new QueryOrder(false, QueryOrder.FieldOptions.Created)
                }
                
        };
        QueryResponse queryResponse = await Lobbies.Instance.QueryLobbiesAsync(queryLobbiesOptions);
        Debug.Log(queryResponse.Results.Count);
        foreach(Lobby lobby in queryResponse.Results){
            Debug.Log(lobby.LobbyCode);
        }
    }   
    private async void JoinLobbyByCode(string lobbyCode){
        try{
            JoinLobbyByCodeOptions joinLobbyByCodeOptions = new JoinLobbyByCodeOptions{
                Player = GetPlayer()
            };
            Lobby lobby = await LobbyService.Instance.JoinLobbyByCodeAsync(lobbyCode, joinLobbyByCodeOptions);
            joinedLobby = lobby;
            PrintPlayers(joinedLobby);
            Debug.Log("Joined lobby" + lobbyCode);

        }catch(LobbyServiceException e){
            Debug.Log(e);
        }

    }
    private void PrintPlayers(Lobby lobby){
        Debug.Log(lobby.Players.Count);
        foreach(Player player in lobby.Players){
            Debug.Log(player.Data["PLayerName"].Value);
        }
    }
    private Player GetPlayer(){
        return new Player{
            Data = new Dictionary<string, PlayerDataObject>{
            {"PLayerName" , new PlayerDataObject(PlayerDataObject.VisibilityOptions.Public , playerName.text)}} 
        };
    }
    private async void LeaveLobby(){
        try{
            await LobbyService.Instance.RemovePlayerAsync(joinedLobby.Id,AuthenticationService.Instance.PlayerId);
        }catch(LobbyServiceException e){
            Debug.Log(e);
        }
    }
    private async void KickPlayer(string playerId){
        try{
            await LobbyService.Instance.RemovePlayerAsync(joinedLobby.Id,playerId);
        }catch(LobbyServiceException e){
            Debug.Log(e);
        }
    }
    public async void RefreashLobbyList(){
        try{
            QueryLobbiesOptions queryLobbiesOptions = new QueryLobbiesOptions{
                    Count = 25,
                    /*Filters = new List<QueryFilter>(){
                        new QueryFilter(QueryFilter.FieldOptions.AvailableSlots, "0", QueryFilter.OpOptions.GT)
                    },*/
                    Order = new List<QueryOrder>{
                        new QueryOrder(false, QueryOrder.FieldOptions.Created)
                    }
                    
            };
            QueryResponse queryResponse = await Lobbies.Instance.QueryLobbiesAsync(queryLobbiesOptions);
            Debug.Log(queryResponse.Results.Count);
            
            foreach(Transform g in LobbyContainer.GetComponentsInChildren<Transform>()){
                if(g != null && g.name == randomstring) Destroy(g.gameObject);
            }
            randomstring = Random.Range(0f, 100f).ToString();
            foreach(Lobby lobby in queryResponse.Results){
                GameObject lobbyLister = Instantiate(LobbyPanelObject);
                lobbyLister.GetComponent<LobbyListObject>().Setup(lobby.Name, lobby.Players.Count, lobby.MaxPlayers);
                lobbyLister.transform.SetParent(LobbyContainer.transform);
                lobbyLister.name = randomstring;
            }   

        }catch(ServicesInitializationException e){
            Debug.Log(e);
        }
    }
    public async void CreateNewLobby(string lobbyN, int maxPlayerCount){
        try{
            string lobbyName = lobbyN;
            int maxPlayers = maxPlayerCount;
            CreateLobbyOptions createLobbyOptions = new CreateLobbyOptions{
                IsPrivate = false,
                Player = GetPlayer(),
                Data = new Dictionary<string, DataObject>{
                    {"Gamemode" , new DataObject(DataObject.VisibilityOptions.Public, "Casual")}
                }                
            };
            Lobby lobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName, maxPlayers, createLobbyOptions);
            hostLobby = lobby;
            joinedLobby = hostLobby;
            PrintPlayers(hostLobby);
            Debug.Log("New lobby " + lobbyName + " " + maxPlayers + " " + lobby.LobbyCode);
        }
        catch(LobbyServiceException e){
            Debug.Log(e);
        }
    }
}