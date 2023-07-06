using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : NetworkBehaviour
{
    public static GameManager Instance { get; private set; }

    private Dictionary<ulong, bool> playerReadyDictionary;
    public event EventHandler OnLocalPlayerChanged;
    public event EventHandler OnStateChanged;
    public event EventHandler OnLocalGamePaused;
    public event EventHandler OnLocalGameUnpaused;
    public event EventHandler OnMultiplayerGamePaused;
    public event EventHandler OnMultiplayerGameUnpaused;
    private enum WholeGameState
    {
        WaitingToStart,
        CountdownToStart,
        GamePlaying,
        GameOver,
    }

    [SerializeField] private Transform playerPrefab;
    private NetworkVariable<WholeGameState> wholeGameState = new NetworkVariable<WholeGameState>(WholeGameState.WaitingToStart);
    private bool isLocalPlayerReady = false;
    private bool isPlayerPauesed;
    private NetworkVariable<bool> isGamePaused = new NetworkVariable<bool> (false);
    private void Awake()
    {
        Instance= this;
        DontDestroyOnLoad(gameObject);
        playerReadyDictionary = new Dictionary<ulong, bool>();
        if (NetworkManager.Singleton.IsServer)
        {
            GetComponent<NetworkObject>().Spawn();
        }
        
    }
    public override void OnNetworkSpawn()
    {
        wholeGameState.OnValueChanged += WholeGameState_OnValueChanged;
        isGamePaused.OnValueChanged += IsGamePaused_OnValueChanged;
        if (NetworkManager.Singleton.IsServer)
        {
            Debug.Log(NetworkManager.Singleton.IsServer);
            NetworkManager.Singleton.OnClientDisconnectCallback += NetworkManager_OnClientDisconnectCallback;
            NetworkManager.Singleton.SceneManager.OnLoadEventCompleted += SceneManager_OnLoadEventCompleted;
        }
    }


    private void WholeGameState_OnValueChanged(WholeGameState previousValue, WholeGameState newValue)
    {
        throw new NotImplementedException();
    }

    private void IsGamePaused_OnValueChanged(bool previousValue, bool newValue)
    {
        throw new NotImplementedException();
    }

    private void NetworkManager_OnClientDisconnectCallback(ulong obj)
    {
        throw new NotImplementedException();
    }

    private void SceneManager_OnLoadEventCompleted(string sceneName, LoadSceneMode loadSceneMode, List<ulong> clientsCompleted, List<ulong> clientsTimedOut)
    {
        Debug.Log(sceneName);
        foreach(ulong clientId in NetworkManager.Singleton.ConnectedClientsIds)
        {

            Transform playerTransform = Instantiate(playerPrefab);
            playerTransform.GetComponent<NetworkObject>().SpawnAsPlayerObject(clientId,true);
        }
    }

    private void GameInput_OnInteractAction()
    {
        if(wholeGameState.Value == WholeGameState.WaitingToStart)
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
            wholeGameState.Value = WholeGameState.CountdownToStart;
           
        }
    }


    public bool IsWaitingToStart()
    {
        return wholeGameState.Value == WholeGameState.WaitingToStart;
    }

    
}
