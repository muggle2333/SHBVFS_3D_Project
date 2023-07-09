using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : NetworkBehaviour
{
    public static GameManager Instance { get; private set; }

    private Dictionary<ulong, bool> playerReadyDictionary;
    public event EventHandler OnLocalPlayerReadyChanged;
    public event EventHandler OnWholeGameStateChanged;


    public enum WholeGameState
    {
        WaitingToStart,
        CountdownToStart,
        GamePlaying,
        GameOver,
    }

    //[SerializeField] private Transform playerPrefab;
    public NetworkVariable<WholeGameState> wholeGameState = new NetworkVariable<WholeGameState>(WholeGameState.WaitingToStart);
    private NetworkVariable<float> countdownToStartTimer = new NetworkVariable<float>(3.5f);
    private bool isLocalPlayerReady = false;
    private bool isPlayerPauesed;
    private bool isGameOver = false;
    private NetworkVariable<bool> isGamePaused = new NetworkVariable<bool> (false);

    [SerializeField] private GameObject managerContainerPath;
    private void Awake()
    {
        Instance= this;
        wholeGameState.Value = WholeGameState.WaitingToStart;
        playerReadyDictionary = new Dictionary<ulong, bool>();
    }
    //public override void OnNetworkSpawn()
    private void Start()
    {
        wholeGameState.OnValueChanged += WholeGameState_OnValueChanged;
        isGamePaused.OnValueChanged += IsGamePaused_OnValueChanged;
        if (FindObjectOfType<NetworkManager>() == null) { return; }
        if (NetworkManager.Singleton.IsServer)
        {
            NetworkManager.Singleton.OnClientDisconnectCallback += NetworkManager_OnClientDisconnectCallback;
            NetworkManager.Singleton.SceneManager.OnLoadEventCompleted += SceneManager_OnLoadEventCompleted;
        }
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            GameInput_OnInteractAction();
        }
        if(!IsServer)
        {
            return;
        }

        switch(wholeGameState.Value)
        {
            case WholeGameState.WaitingToStart:
                break;
            case WholeGameState.CountdownToStart:
                countdownToStartTimer.Value -=Time.deltaTime;
                UIManager.Instance.ShowMessageTimerClientRpc(countdownToStartTimer.Value);
                if(countdownToStartTimer.Value<0f)
                {
                    UIManager.Instance.HideMessageTimerClientRpc();
                    wholeGameState.Value = WholeGameState.GamePlaying;
                    TurnbasedSystem.Instance.StartTurnbaseSystem();
                }
                break;
            case WholeGameState.GamePlaying:
                if(isGameOver)
                {
                    wholeGameState.Value = WholeGameState.GameOver;
                }
                break;
            case WholeGameState.GameOver:
                break;
        }
    }


    private void WholeGameState_OnValueChanged(WholeGameState previousValue, WholeGameState newValue)
    {
        OnWholeGameStateChanged?.Invoke(this, EventArgs.Empty);
    }

    private void IsGamePaused_OnValueChanged(bool previousValue, bool newValue)
    {
        throw new NotImplementedException();
    }

    private void NetworkManager_OnClientDisconnectCallback(ulong obj)
    {
        //throw new NotImplementedException();
    }

    private void SceneManager_OnLoadEventCompleted(string sceneName, LoadSceneMode loadSceneMode, List<ulong> clientsCompleted, List<ulong> clientsTimedOut)
    {
        Debug.Log(sceneName);
         foreach(ulong clientId in NetworkManager.Singleton.ConnectedClientsIds)
        {
            string playerPath = clientId == 0 ? "PlayerPrefab_Red" : "PlayerPrefab_Blue";
            Transform playerTransform = Instantiate(Resources.Load<GameObject>(playerPath).transform);
            playerTransform.GetComponent<Player>().Id=(PlayerId)clientId;
            playerTransform.GetComponent<NetworkObject>().SpawnAsPlayerObject(clientId,true);
        }
        GameObject managerContainer = Instantiate(Resources.Load<GameObject>(managerContainerPath.name), Vector3.zero, Quaternion.identity);
        managerContainer.GetComponent<NetworkObject>().Spawn();
        //GameplayManager.Instance.InitializePlayer();
    }


    private void GameInput_OnInteractAction()
    {
        if(wholeGameState.Value == WholeGameState.WaitingToStart)
        {
            isLocalPlayerReady= true;
            OnLocalPlayerReadyChanged?.Invoke(this,EventArgs.Empty);
            SetPlayerReadyServerRpc();
        }
    }

    [ServerRpc(RequireOwnership =false)]
    private void SetPlayerReadyServerRpc(ServerRpcParams serverRpcParams=default)
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
            else
            {
                UIManager.Instance.ShowMessageInfoClientRpc("Waiting for others", new ClientRpcParams { Send = new ClientRpcSendParams
                {
                    TargetClientIds = new List<ulong> { serverRpcParams.Receive.SenderClientId }
                } });
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
