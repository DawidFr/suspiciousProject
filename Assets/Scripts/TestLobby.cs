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
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using System.Threading.Tasks;

public class TestLobby : MonoBehaviour
{
    public const string KEY_START_GAME = "start";
    public GameObject LobbyPanelObject;
    public GameObject LobbyContainer;
    public TMP_InputField playerName;
    public TMP_InputField idInput;
    private Lobby hostLobby;
    private string randomstring;
    public static TestLobby Instance;
    public Lobby joinedLobby;
    private float heartbeatTimer;
    private float lobbyUpdateTimer;
    public string joinedLobbyName;
    public TextMeshProUGUI playerIdtext; 
    public System.Action OnLobbyJoin, OnLobbyUpdate, OnGameStart;
    public float lobbyRefreashTimer;
    // Start is called before the first frame update
    private async void Awake()
    {
        Instance = this;
        await UnityServices.InitializeAsync();
        AuthenticationService.Instance.ClearSessionToken();
            //#if UNITY_EDITOR
            //#endif
     
        await AuthenticationService.Instance.SignInAnonymouslyAsync();
        
        AuthenticationService.Instance.SignedIn += () =>{   
            Debug.Log("Signed" + AuthenticationService.Instance.PlayerId);
        }; 
        //await AuthenticationService.Instance.SignInAnonymouslyAsync();

        playerIdtext.text = AuthenticationService.Instance.PlayerId;
    }
    private void Update() {
        //if(Input.GetKeyDown(KeyCode.Q)) CreateLobby();
        //if(Input.GetKeyDown(KeyCode.W)) ListLobbies();
        //if(Input.GetKeyDown(KeyCode.E)) JoinLobbyByCode(idInput.text);
        //if(Input.GetKeyDown(KeyCode.R)) PrintPlayers(hostLobby);
        HandleLobbyUpdates();
        HandleLobbbyHeartbeat();
        if(joinedLobby != null) joinedLobbyName = joinedLobby.Name;
        
        lobbyRefreashTimer -=Time.deltaTime;
        if(lobbyRefreashTimer < 0){
            if(AuthenticationService.Instance.IsAuthorized)RefreashLobbyList();
            lobbyRefreashTimer = 5f;
        }
    }
    async void HandleLobbbyHeartbeat(){
        if(hostLobby != null){
            heartbeatTimer -= Time.deltaTime;
            if(heartbeatTimer <= 0f){
                heartbeatTimer = 15f;
                await LobbyService.Instance.SendHeartbeatPingAsync(hostLobby.Id);

            }
        }
    }
    private async void HandleLobbyUpdates(){
        if(joinedLobby != null){
            lobbyUpdateTimer -= Time.deltaTime;
            if(lobbyUpdateTimer <= 0f){
                lobbyUpdateTimer = 1.5f;
                Lobby lobby = await LobbyService.Instance.GetLobbyAsync(joinedLobby.Id);
                joinedLobby = lobby;
                Debug.Log("update");
                if(OnLobbyUpdate != null) OnLobbyUpdate.Invoke();
                if(joinedLobby.Data[KEY_START_GAME].Value != "0"){
                    if(!IsHost()){
                        JoinRelay(joinedLobby.Data[KEY_START_GAME].Value);
                        GetComponent<Canvas>().enabled = false;

                    }
                }
            }
        }
    }

    public bool IsHost(){
        
        if(joinedLobby.HostId == AuthenticationService.Instance.PlayerId) return true;
        else return false;

    }

    /*public async void CreateLobby(){
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
    }*/
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
    public async void JoinLobbyByCode(string lobbyCode){
        try{
            JoinLobbyByCodeOptions joinLobbyByCodeOptions = new JoinLobbyByCodeOptions{
                Player = GetPlayer()
            };
            //if(joinedLobby != null)LeaveLobby();
            Lobby lobby = await LobbyService.Instance.JoinLobbyByCodeAsync(lobbyCode, joinLobbyByCodeOptions);
            joinedLobby = lobby;
            //PrintPlayers(joinedLobby);
            
            Debug.Log("Joined lobby" + lobbyCode);
            if(OnLobbyJoin != null)OnLobbyJoin.Invoke();


        }catch(LobbyServiceException e){
            Debug.Log(e);
        }

    }
    public async void JoinLobbyById(string lobbyId){
        try{
            JoinLobbyByIdOptions joinLobbyByIdOptions = new JoinLobbyByIdOptions{
                Player = GetPlayer()
            };
            //if(joinedLobby != null)LeaveLobby();
            Lobby lobby = await LobbyService.Instance.JoinLobbyByIdAsync(lobbyId, joinLobbyByIdOptions);
            joinedLobby = lobby;
            //PrintPlayers(joinedLobby);
            
            Debug.Log("Joined lobby" + lobbyId);
            if(OnLobbyJoin != null)OnLobbyJoin.Invoke();
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
    public Player GetPlayer(){
        return new Player{
            Data = new Dictionary<string, PlayerDataObject>{
            {"PLayerName" , new PlayerDataObject(PlayerDataObject.VisibilityOptions.Public , playerName.text)},
            {"IsReady", new PlayerDataObject(PlayerDataObject.VisibilityOptions.Public , "false")}
            }
        };
    }
    public async void LeaveLobby(){
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
                //Debug.Log(lobby.LobbyCode + " " + lobby.Id + " " + lobby.Name);
                lobbyLister.GetComponent<LobbyListObject>().Setup(lobby.Name, lobby.AvailableSlots, lobby.Id);
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
                    {"Gamemode" , new DataObject(DataObject.VisibilityOptions.Public, "Casual")},
                    {KEY_START_GAME, new DataObject(DataObject.VisibilityOptions.Member, "0")}
                }                
            };
            Lobby lobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName, maxPlayers, createLobbyOptions);
            hostLobby = lobby;
            joinedLobby = hostLobby;
            PrintPlayers(hostLobby);
            Debug.Log("New lobby " + lobbyName + " " + maxPlayers + " " + lobby.LobbyCode);
            if(OnLobbyJoin != null)OnLobbyJoin.Invoke();
            StartGame();
            GetComponent<Canvas>().enabled = false;

        }
        catch(LobbyServiceException e){
            Debug.Log(e);
        }
    }

    //************************
    //Relay testing start here
    //************************

    public async Task<string> createRelay(){
        try{
            Allocation allocation = await RelayService.Instance.CreateAllocationAsync(12);
            string joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);
            RelayServerData relayServerData = new RelayServerData(allocation, "dtls");
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);
            return joinCode;
        }
        catch(RelayServiceException e){
            Debug.Log(e);
        }
        return null;

    }
    public async void JoinRelay(string joinCode){
        try{
            JoinAllocation joinedAllocation = await RelayService.Instance.JoinAllocationAsync(joinCode);
            RelayServerData relayServerData = new RelayServerData(joinedAllocation, "dtls");
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);
            NetworkManager.Singleton.StartClient();
        }
        catch(RelayServiceException e){
            Debug.Log(e);
        }
    }
    public async void StartGame(){
        //dodaj coś co sprawdza czy jezt hostem
        //Nie zapomnij o tym!!
        //Jezeli to czytasz znaczy, że zapomniałem
        try{
            string relayCode = await createRelay();
            Lobby lobby = await Lobbies.Instance.UpdateLobbyAsync(joinedLobby.Id, new UpdateLobbyOptions{
                Data = new Dictionary<string, DataObject>{
                    {KEY_START_GAME, new DataObject(DataObject.VisibilityOptions.Member, relayCode)}
                }
            });
            joinedLobby = lobby;
            NetworkManager.Singleton.StartHost();
            //SceneLoader.LoadNetwork(SceneLoader.Scene.Game);
             
        }
        catch(RelayServiceException e){
            Debug.Log(e);
        }

    }









}
