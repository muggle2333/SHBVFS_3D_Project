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
    private enum GameState
    {
        WaitingToStart,
        CountdownToStart,
        GamePlaying,
        GameOver,
    }

    private GameState gameState = GameState.WaitingToStart;
    private bool isLocalPlayerReady = false;
    private void Awake()
    {
        Instance= this;
        DontDestroyOnLoad(gameObject);
        playerReadyDictionary = new Dictionary<ulong, bool>();
    }
    private void GameInput_OnInteractAction()
    {
        if(gameState == GameState.WaitingToStart)
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
            gameState = GameState.CountdownToStart;
            Loader.LoadNetwork(Loader.Scene.GameplayScene);
        }
    }


    public bool IsWaitingToStart()
    {
        return gameState == GameState.WaitingToStart;
    }
}
