using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class NotificationUI : MonoBehaviour
{
    [SerializeField] private GameObject container;
    [SerializeField] private TMP_Text notificationText;
    [SerializeField] private Button backBtn;
    private void Awake()
    {
        backBtn.onClick.AddListener(() =>
        {
            Hide();
        });
    }
    private void Start()
    {
        MultiplayerManager.Instance.OnFailToJoinGame += MultiplayerManager_OnFailedToJoinGame;
        Hide();
    }

    private void MultiplayerManager_OnFailedToJoinGame(object sender, EventArgs e)
    {
        Show();
        Debug.Log(NetworkManager.Singleton.DisconnectReason);
        notificationText.text = NetworkManager.Singleton.DisconnectReason;
    }

    private void OnDestroy()
    {
        MultiplayerManager.Instance.OnFailToJoinGame -= MultiplayerManager_OnFailedToJoinGame;
    }

    private void Show()
    {
        container.SetActive(true);
    }
    private void Hide()
    {
        container.SetActive(false);
    }
   
}
