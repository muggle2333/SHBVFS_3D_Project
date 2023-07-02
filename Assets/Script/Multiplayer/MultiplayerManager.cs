using System.Collections;
using System.Collections.Generic;
using System.Net;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode.Transports.UTP;
using System;

public class MultiplayerManager : NetworkBehaviour
{
    private const int MAX_PLAYER_AMOUNT = 2;
    public static MultiplayerManager Instance { get; private set; }

    public event EventHandler OnTryingToJoinGame;
    public event EventHandler OnFailToJoinGame;

    private UnityTransport networkTransport;

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
    }
    public void Start()
    {
        networkTransport= FindObjectOfType<UnityTransport>();
    }

    public void StartHost()
    {
        NetworkManager.Singleton.ConnectionApprovalCallback += NetWorkManager_ConnectApprovalCallback;
        NetworkManager.Singleton.StartHost();
    }

    private void NetWorkManager_ConnectApprovalCallback(NetworkManager.ConnectionApprovalRequest connectionApprovalRequest, NetworkManager.ConnectionApprovalResponse connectionApprovalResponse)
    {

        if(NetworkManager.Singleton.ConnectedClientsIds.Count > MAX_PLAYER_AMOUNT)
        {
            connectionApprovalResponse.Approved = false;
            connectionApprovalResponse.Reason = "Game is full";
            return;
        }
        connectionApprovalResponse.Approved = true;
        connectionApprovalResponse.CreatePlayerObject = true;
        //if(GameManager.Instance.IsWaitingToStart())
        //{
        //    connectionApprovalResponse.Approved = true;
        //    connectionApprovalResponse.CreatePlayerObject= true;
        //}
        //else
        //{
        //    connectionApprovalResponse.Approved = false;
        //}
    }

    public void StartClient(string ip)
    {
        networkTransport.SetConnectionData(ip, 7777);
        OnTryingToJoinGame?.Invoke(this, EventArgs.Empty);
        NetworkManager.Singleton.OnClientConnectedCallback += NetworkManager_OnClientDisconnectCallback;
        NetworkManager.Singleton.StartClient();
    }

    private void NetworkManager_OnClientDisconnectCallback(ulong clientId)
    {
        OnFailToJoinGame?.Invoke(this, EventArgs.Empty);
    }


}
