using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using System;
using UnityEngine.EventSystems;

public class WaitRoomManager : NetworkBehaviour
{
    public static WaitRoomManager Instance { get; private set; }

    public event EventHandler OnReadyChanged;
    

    private Dictionary<ulong, bool> playerReadyDictionary;

    [SerializeField] private WaitRoomUI waitRoomUI;

    private void Awake()
    {
        Instance= this;
        playerReadyDictionary = new Dictionary<ulong, bool>();
        
    }

    private void Start()
    {
        waitRoomUI = FindObjectOfType<WaitRoomUI>();
        waitRoomUI.SetStartBtn(false);

        if(NetworkManager.Singleton.IsHost)
        {
            //SetPlayerReady();
        }
    }
    public void SetPlayerReady()
    {
        SetPlayerReadyServerRpc();
    }

    [ServerRpc(RequireOwnership = false)]
    private void SetPlayerReadyServerRpc(ServerRpcParams serverRpcParams = default)
    {
        SetPlayerReadyClientRpc(serverRpcParams.Receive.SenderClientId);
        playerReadyDictionary[serverRpcParams.Receive.SenderClientId] = true;

        bool isOthersReady = true;
        foreach (ulong clientId in NetworkManager.Singleton.ConnectedClientsIds)
        {
            if(clientId== 0)
            {
                continue;
            }
            if (!playerReadyDictionary.ContainsKey(clientId) || !playerReadyDictionary[clientId] || NetworkManager.Singleton.ConnectedClientsIds.Count < MultiplayerManager.MAX_PLAYER_AMOUNT)
            {
                isOthersReady = false;
                break;
            }
        }
        waitRoomUI.SetStartBtn(isOthersReady);

    }

    [ClientRpc]
    private void SetPlayerReadyClientRpc(ulong clientId)
    {
        playerReadyDictionary[clientId] = true;

        OnReadyChanged?.Invoke(this,EventArgs.Empty);

    }

    public bool IsPlayerReady(ulong clientId)
    {
        return playerReadyDictionary.ContainsKey(clientId) && playerReadyDictionary[clientId];
    }

    public void StartGameplay()
    {
        Loader.LoadNetwork(Loader.Scene.GameplayScene);
    }
}
