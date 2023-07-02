using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;
using Unity.Netcode;

public class LobbyUI : MonoBehaviour
{
    [SerializeField] private Button createGameBtn;
    [SerializeField] private Button joinGameBtn;
    [SerializeField] private Button closeBtn;
    [SerializeField] private TMP_Text messageText;
    [SerializeField] private GameObject connectingUI;
    [SerializeField] private GameObject disconnectUI;

    private void Awake()
    {
        createGameBtn.onClick.AddListener(() =>
        {
            MultiplayerManager.Instance.StartHost();
            //Loader.LoadNetwork(Loader.Scene.CharacterSelectedScene);
        });
        joinGameBtn.onClick.AddListener(() =>
        {
            //MultiplayerManager.Instance.StartClient();
        });
        closeBtn.onClick.AddListener(() =>
        {
            disconnectUI.SetActive(false);
        });
    }

    private void Start()
    {
        MultiplayerManager.Instance.OnTryingToJoinGame += MultiplayerManager_OnTryingToJoinGame;
        MultiplayerManager.Instance.OnFailToJoinGame += MultiplayerManager_OnFailToJoinGame;
        CloseAlltheMessageUI();
    }

    private void MultiplayerManager_OnTryingToJoinGame(object sender, EventArgs e)
    {
        ShowConnectUI();
    }
    
    private void MultiplayerManager_OnFailToJoinGame(object sender, EventArgs e)
    {
        ShowDisConnectUI();
        messageText.text = NetworkManager.Singleton.DisconnectReason;

        if(messageText.text=="")
        {
            messageText.text = "Failed to connect";
        }
    }

    private void ShowConnectUI()
    {
        disconnectUI.SetActive(false);
        connectingUI.SetActive(true);
    }

    private void ShowDisConnectUI()
    {
        connectingUI.SetActive(false);
        disconnectUI.SetActive(true);
    }

    private void CloseAlltheMessageUI()
    {
        connectingUI.SetActive(false);
        disconnectUI.SetActive(false);
    }

    private void OnDestroy()
    {
        MultiplayerManager.Instance.OnTryingToJoinGame -= MultiplayerManager_OnTryingToJoinGame;
        MultiplayerManager.Instance.OnFailToJoinGame -= MultiplayerManager_OnFailToJoinGame;
    }
}
