using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
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
            Hide(true);
        });
        DontDestroyOnLoad(gameObject);
    }
    private void Start()
    {
        MultiplayerManager.Instance.OnFailToJoinGame += MultiplayerManager_OnFailedToJoinGame;
        //MultiplayerManager.Instance.OnWaitingToStart += MultiplayerManager_OnWaitingToStart;
        Hide(false);
    }

    private void MultiplayerManager_OnFailedToJoinGame(object sender, EventArgs e)
    {
        Show();
        notificationText.text = NetworkManager.Singleton.DisconnectReason;

        if(notificationText.text =="")
        {
            notificationText.text = "Failed to connect";
        }

    }
    private void MultiplayerManager_OnWaitingToStart(object sender, EventArgs e)
    {
        Show();
        notificationText.text = "Game Start";

    }

    private void OnDestroy()
    {
        MultiplayerManager.Instance.OnFailToJoinGame -= MultiplayerManager_OnFailedToJoinGame;
    }

    private void Show()
    {
        container.SetActive(true);
    }
    private void Hide(bool isBackScene)
    {
        container.SetActive(false);
        if (!isBackScene) return;
        if (SceneManager.GetActiveScene().name!= Loader.Scene.MainMenuScene.ToString())
        {
            Loader.Load(Loader.Scene.MainMenuScene);
        }
    }
   
}
