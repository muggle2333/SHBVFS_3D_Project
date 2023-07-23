using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using System;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class WaitRoomManager : NetworkBehaviour
{
    public static WaitRoomManager Instance { get; private set; }

    public event EventHandler OnReadyChanged;
    

    private Dictionary<ulong, bool> playerReadyDictionary;
    private NetworkVariable<bool> isTutorial = new NetworkVariable<bool>(false);

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

        isTutorial.OnValueChanged += WaitRoomManager_UpdateTutorial;
        if(NetworkManager.Singleton.IsHost)
        {
            //SetPlayerReady();
        }
    }

    private void WaitRoomManager_UpdateTutorial(bool previousValue, bool newValue)
    {
        if(!NetworkManager.Singleton.IsHost)
        {
            waitRoomUI.SetToggle(newValue);
        }
        
    }

    public void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            Loader.Load(Loader.Scene.MainMenuScene);
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
        if(!isTutorial.Value)
        {
            Loader.LoadNetwork(Loader.Scene.GameplayScene);
        }else
        {
            Loader.LoadNetwork(Loader.Scene.TutorialScene);
        }
    }

    public void SetToggleTutorial(bool isToggle)
    {
        isTutorial.Value = isToggle;
    }
}
