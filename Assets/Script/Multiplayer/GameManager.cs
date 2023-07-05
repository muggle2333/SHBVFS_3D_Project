using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

public class GameManager : NetworkBehaviour
{
    public static GameManager Instance { get; private set; }

    private Dictionary<ulong, bool> playerReadyDictionary;
    public event EventHandler OnLocalPlayerChanged;
    private enum WholeGameState
    {
        WaitingToStart,
        CountdownToStart,
        GamePlaying,
        GameOver,
    }

    private WholeGameState gameState = WholeGameState.WaitingToStart;
    private bool isLocalPlayerReady = false;
    private void Awake()
    {
        Instance= this;
        DontDestroyOnLoad(gameObject);
        playerReadyDictionary = new Dictionary<ulong, bool>();
    }
    private void GameInput_OnInteractAction()
    {
        if(gameState == WholeGameState.WaitingToStart)
        {
            isLocalPlayerReady= true;
            SetPlayerReadyServerRpc();
            OnLocalPlayerChanged?.Invoke(this,EventArgs.Empty);
        }
    }

    [ServerRpc(RequireOwnership =false)]
    public void SetPlayerReadyServerRpc(ServerRpcParams serverRpcParams=default)
    {
        playerReadyDictionary[serverRpcParams.Receive.SenderClientId] = true;

        bool isAllReady = true;
        foreach(ulong clientId in NetworkManager.Singleton.ConnectedClientsIds)
        {
            if (!playerReadyDictionary.ContainsKey(clientId) || !playerReadyDictionary[clientId])
            {
                isAllReady= false;
                break;
            }
        }
        if(isAllReady)
        {
            gameState = WholeGameState.CountdownToStart;
           
        }
    }


    public bool IsWaitingToStart()
    {
        return gameState == WholeGameState.WaitingToStart;
    }

    
}
