using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterSelect : MonoBehaviour
{
    [SerializeField] private int playerIndex;
    [SerializeField] private GameObject readyGameObject;

    private void Awake()
    {

    }

    private void Start()
    {
        MultiplayerManager.Instance.OnPlayerIdDataNetworkListChanged += MultiplayerManager_OnPlayerIdDataNetworkListChanged;
        WaitRoomManager.Instance.OnReadyChanged += WaitRoomManager_OnReadyChanged;
        UpdatePlayer();
    }
    private void WaitRoomManager_OnReadyChanged(object sender, EventArgs e)
    {
        UpdatePlayer();
    }
    private void MultiplayerManager_OnPlayerIdDataNetworkListChanged(object sender, EventArgs e)
    {
        UpdatePlayer();
    }

    private void UpdatePlayer()
    {
        if (MultiplayerManager.Instance.IsPlayerIndexConnect(playerIndex))
        {
            Show();
            PlayerIdData playerIdData = MultiplayerManager.Instance.GetPlayerIdDataFromPlayerIndex(playerIndex);
            readyGameObject.SetActive(WaitRoomManager.Instance.IsPlayerReady(playerIdData.clientId));
        }else
        {
            Hide();
        }
    }

    private void Show()
    {
        gameObject.SetActive(true);
    }

    private void Hide()
    {
        gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        MultiplayerManager.Instance.OnPlayerIdDataNetworkListChanged -= MultiplayerManager_OnPlayerIdDataNetworkListChanged;
        WaitRoomManager.Instance.OnReadyChanged -= WaitRoomManager_OnReadyChanged;
    }
}
