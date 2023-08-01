using System.Collections;
using System.Collections.Generic;
using System.Net;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode.Transports.UTP;
using System;
using UnityEngine.SceneManagement;


[Serializable]
public struct PlayerData
{
    public ulong clientId;
    public string playerName;
}
public class MultiplayerManager : NetworkBehaviour
{
    public const int MAX_PLAYER_AMOUNT = 2;
    public static MultiplayerManager Instance { get; private set; }

    public event EventHandler OnTryingToJoinGame;
    public event EventHandler OnFailToJoinGame;
    public event EventHandler OnPlayerIdDataNetworkListChanged;
    public event EventHandler OnWaitingToStart;

    private NetworkList<PlayerIdData> playerIdDataNetworkList;
    private string playerName;
    private UnityTransport networkTransport;

    [SerializeField] private List<PlayerData> playerList = new List<PlayerData>();//For serializefield
    private void Awake()
    {
        if(Instance!=null&&Instance!=this)
        {
            Destroy(Instance);
        }
        else
        {
            Instance= this;
        }
        Time.timeScale = 1.0f;
        DontDestroyOnLoad(gameObject);
        playerName = UnityEngine.Random.Range(0,10).ToString();
        playerIdDataNetworkList = new NetworkList<PlayerIdData>();
        playerIdDataNetworkList.OnListChanged += PlayerIdDataNetworkList_OnListChanged;

    }

    private void PlayerIdDataNetworkList_OnListChanged(NetworkListEvent<PlayerIdData> changeEvent)
    {
        OnPlayerIdDataNetworkListChanged?.Invoke(this, EventArgs.Empty);
        playerList.Clear();
        foreach(var player in playerIdDataNetworkList) 
        {
            playerList.Add(new PlayerData { clientId = player.clientId, playerName = player.playerName.ToString() });
        }
    }

    public void Start()
    {
        networkTransport= FindObjectOfType<UnityTransport>();
    }

    public void StartHost()
    {
        NetworkManager.Singleton.ConnectionApprovalCallback += NetWorkManager_Host_ConnectedApprovalCallback;
        NetworkManager.Singleton.OnClientConnectedCallback += NetWorkManager_Host_OnClientConnectedCallback;
        NetworkManager.Singleton.OnClientDisconnectCallback += NetWorkManager_Host_OnClientDisconnectCallback;
        NetworkManager.Singleton.StartHost();
        Loader.LoadNetwork(Loader.Scene.WaitRoomScene);
    }



    private void NetWorkManager_Host_ConnectedApprovalCallback(NetworkManager.ConnectionApprovalRequest connectionApprovalRequest, NetworkManager.ConnectionApprovalResponse connectionApprovalResponse)
    {
        //if(NetworkManager.Singleton.IsHost==false && SceneManager.GetActiveScene().name != Loader.Scene.WaitRoomScene.ToString()) 
        if(SceneManager.GetActiveScene().name != Loader.Scene.WaitRoomScene.ToString()) 
        {
            connectionApprovalResponse.Approved = false;
            connectionApprovalResponse.Reason = "Game has already started";
            return;
        }
        if(NetworkManager.Singleton.ConnectedClientsIds.Count > MAX_PLAYER_AMOUNT)
        {
            connectionApprovalResponse.Approved = false;
            connectionApprovalResponse.Reason = "Game is full";
            return;
        }
        connectionApprovalResponse.Approved = true;
        //connectionApprovalResponse.CreatePlayerObject = true;
        
    }

    private void NetWorkManager_Host_OnClientConnectedCallback(ulong clientId)
    {
        playerIdDataNetworkList.Add(new PlayerIdData
        {
            clientId = clientId,
        });
        SetPlayerNameServerRpc(GetPlayerNameFromInput());

    }
    private void NetWorkManager_Host_OnClientDisconnectCallback(ulong clientId)
    {
        foreach(var playerIdData in playerIdDataNetworkList)
        {
            if(playerIdData.clientId==clientId)
            {
                playerIdDataNetworkList.Remove(playerIdData);
            }
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void SetPlayerNameServerRpc(string playerName,ServerRpcParams serverRpcParams = default)
    {
        int playerIdDataIndex = GetPlayerIdDataIndex(serverRpcParams.Receive.SenderClientId);
        PlayerIdData playerTmp = playerIdDataNetworkList[playerIdDataIndex];
        //playerTmp.playerName = playerName;
        if(playerIdDataIndex==0)
        {
            playerTmp.playerName = "Red Player";
        }
        else
        {
            playerTmp.playerName = "Blue Player";
        }
        playerIdDataNetworkList[playerIdDataIndex] = playerTmp;
    }
    public void StartClient(string ip)
    {
        networkTransport.SetConnectionData(ip, 7777);
        OnTryingToJoinGame?.Invoke(this, EventArgs.Empty);
        NetworkManager.Singleton.OnClientDisconnectCallback += NetworkManager_Client_OnClientDisconnectCallback;
        NetworkManager.Singleton.OnClientConnectedCallback += NetworkManager_Client_OnClientConnectedCallback;
        NetworkManager.Singleton.StartClient();

    }

    private void NetworkManager_Client_OnClientDisconnectCallback(ulong clientId)
    {
        OnFailToJoinGame?.Invoke(this, EventArgs.Empty);
    }

    private void NetworkManager_Client_OnClientConnectedCallback(ulong clientId)
    {
        SetPlayerNameServerRpc("blueplayer");
    }

    private int GetPlayerIdDataIndex(ulong clientId)
    {
        for(int i=0;i<playerIdDataNetworkList.Count;i++)
        {
            if (playerIdDataNetworkList[i].clientId == clientId) 
            return i;
        }
        return -1;
    }

    private string GetPlayerNameFromInput()
    {
        MainMenuUI mainMenu = FindObjectOfType<MainMenuUI>();
        //return mainMenu.GetPlayerName();
        return "0";
    }

    public bool IsPlayerIndexConnect(int playerIndex)
    {
        return playerIndex < playerIdDataNetworkList.Count;
    }

    public PlayerIdData GetPlayerIdDataFromPlayerIndex(int playerIndex)
    {
        return playerIdDataNetworkList[playerIndex];
    }

    public void StartGamePlay()
    {
        OnWaitingToStart?.Invoke(this, EventArgs.Empty);
        Loader.LoadNetwork(Loader.Scene.GameplayScene);
    }

    public void StartTutorial()
    {
        NetworkManager.Singleton.StartHost();
        Loader.LoadNetwork(Loader.Scene.TutorialScene);
    }

}
