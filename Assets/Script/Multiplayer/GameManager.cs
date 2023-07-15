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

    public event EventHandler OnLocalPlayerReadyChanged;
    public event EventHandler OnWholeGameStateChanged;
    public event EventHandler OnGamePaused;
    public event EventHandler OnGameUnpaused;
    public event EventHandler OnLocalGamePaused;
    public event EventHandler OnLocalGameUnpaused;


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
    private NetworkVariable<bool> isGamePaused = new NetworkVariable<bool>(false);
    private bool isLocalPlayerReady = false;
    private bool isLocalPlayerPaused = false;
    private bool isGameOver = false;

    private Dictionary<ulong, bool> playerReadyDictionary;
    private Dictionary<ulong, bool> playerPausedDictionary;

    [SerializeField] private GameObject managerContainerPath;
    private void Awake()
    {
        Instance= this;
        wholeGameState.Value = WholeGameState.WaitingToStart;
        playerReadyDictionary = new Dictionary<ulong, bool>();
        playerPausedDictionary = new Dictionary<ulong, bool>();
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
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePauseGame();
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
                    UIManager.Instance.HideMessageClientRpc();
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
        if (isGamePaused.Value)
        {
            Time.timeScale = 0f;
            OnGamePaused?.Invoke(this, EventArgs.Empty);
        }
        else
        {
            Time.timeScale = 1f;
            OnGameUnpaused?.Invoke(this, EventArgs.Empty);
        }
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
        if (wholeGameState.Value != WholeGameState.WaitingToStart) return;
        playerReadyDictionary[serverRpcParams.Receive.SenderClientId] = true;

        bool isAllReady = true;
        foreach(ulong clientId in NetworkManager.Singleton.ConnectedClientsIds)
        {
            if (!playerReadyDictionary.ContainsKey(clientId) || !playerReadyDictionary[clientId])
            {
                isAllReady= false;
                break;
            }
            //else
            //{
                //UIManager.Instance.ShowMessageInfoClientRpc("Waiting for others", new ClientRpcParams { Send = new ClientRpcSendParams
                //{
                //    TargetClientIds = new List<ulong> { serverRpcParams.Receive.SenderClientId }
                //} });
            //}
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
    public bool IsPaused()
    {
        return isGamePaused.Value;
    }
    public void TogglePauseGame()
    {
        isLocalPlayerPaused = !isLocalPlayerPaused;
        if (isLocalPlayerPaused)
        {
            PauseGameServerRpc();
            OnLocalGamePaused?.Invoke(this, EventArgs.Empty);
        }
        else
        {
            UnpauseGameServerRpc();
            OnLocalGameUnpaused?.Invoke(this, EventArgs.Empty);
            
        }
    }
    [ServerRpc(RequireOwnership =false)]
    private void PauseGameServerRpc(ServerRpcParams serverRpcParams=default)
    {
        playerPausedDictionary[serverRpcParams.Receive.SenderClientId] = true;
        TestGamePausedState();
    }
    [ServerRpc(RequireOwnership =false)]
    private void UnpauseGameServerRpc(ServerRpcParams serverRpcParams=default)
    {
        playerPausedDictionary[serverRpcParams.Receive.SenderClientId] = false;
        TestGamePausedState();
    }
    private void TestGamePausedState()
    {
        foreach(ulong clientId in NetworkManager.Singleton.ConnectedClientsIds)
        {
            if (playerPausedDictionary.ContainsKey(clientId) && playerPausedDictionary[clientId])
            {
                isGamePaused.Value = true;
                return;
            }
        }
        isGamePaused.Value = false;
    }
    
}
