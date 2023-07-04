using System.Collections;
using System.Collections.Generic;
using System.Net;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode.Transports.UTP;
using System;
using UnityEngine.SceneManagement;

public class MultiplayerManager : NetworkBehaviour
{
    private const int MAX_PLAYER_AMOUNT = 2;
    public static MultiplayerManager Instance { get; private set; }

    public event EventHandler OnTryingToJoinGame;
    public event EventHandler OnFailToJoinGame;
    public event EventHandler OnPlayerIdDataNetworkListChanged;

    private NetworkList<PlayerIdData> playerIdDataNetworkList;
    private string playerName;
    private UnityTransport networkTransport;

    [SerializeField]private List<Color> playerColorList;
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
        DontDestroyOnLoad(gameObject);
        playerName = UnityEngine.Random.Range(0,10).ToString();
        playerIdDataNetworkList = new NetworkList<PlayerIdData>();
        playerIdDataNetworkList.OnListChanged += PlayerIdDataNetworkList_OnListChanged;

    }

    private void PlayerIdDataNetworkList_OnListChanged(NetworkListEvent<PlayerIdData> changeEvent)
    {
        OnPlayerIdDataNetworkListChanged?.Invoke(this, EventArgs.Empty);

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
        Debug.LogError(NetworkManager.Singleton.IsHost);
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
            colorId = GetFirstUnusedColorId(),
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
        PlayerIdData playerTmp = new PlayerIdData();
        playerTmp.playerName = playerName;
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
        Debug.Log("fail");
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
    private bool IsColorAvailable(int colorId)
    {
        foreach (PlayerIdData playerData in playerIdDataNetworkList)
        {
            if (playerData.colorId == colorId)
            {
                return false;
            }
        }
        return true;
    }

    private int GetFirstUnusedColorId()
    {
        for (int i = 0; i < playerColorList.Count; i++)
        {
            if (IsColorAvailable(i))
            {
                return i;
            }
        }
        return -1;
    }





}
