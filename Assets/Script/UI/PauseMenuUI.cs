using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenuUI : MonoBehaviour
{
    [SerializeField] private Button resumeBtn;
    [SerializeField] private Button leaveBtn;
    [SerializeField] private TMP_Text messageText;
    [SerializeField] private GameObject content;
    private void Awake()
    {
        resumeBtn.onClick.AddListener(() =>
        {
            GameManager.Instance.TogglePauseGame();
        });
        leaveBtn.onClick.AddListener(() =>
        {
            Loader.Load(Loader.Scene.MainMenuScene);
        });
    }

    private void Start()
    {
        GameManager.Instance.OnLocalGamePaused += GameMangaer_OnLocalGamePaused;
        GameManager.Instance.OnLocalGameUnpaused += GameMangaer_OnLocalGameUnpaused;
        GameManager.Instance.OnGamePaused += GameManager_OnGamePaused;
        GameManager.Instance.OnGameUnpaused += GameManager_OnGameUnpaused;
    }
    private void GameManager_OnGamePaused(object sender, EventArgs e)
    {
        content.SetActive(true);
        bool isLocalGamePaused = GameManager.Instance.IsLocalPlayerPaused();
        messageText.gameObject.SetActive(!isLocalGamePaused);
        messageText.text = "Waiting for ALL to unpause";
    }
    private void GameManager_OnGameUnpaused(object sender, EventArgs e)
    {;
        content.SetActive(false);
    }
    private void GameMangaer_OnLocalGamePaused(object sender, EventArgs e)
    {
        ShowBtn();
    }

    private void GameMangaer_OnLocalGameUnpaused(object sender, EventArgs e)
    {
        HideBtn();
        
    }

    private void ShowBtn()
    {
        resumeBtn.gameObject.SetActive(true);
        leaveBtn.gameObject.SetActive(true);
        messageText.gameObject.SetActive(false);

    }

    private void HideBtn()
    {
        resumeBtn.gameObject.SetActive(false);
        leaveBtn.gameObject.SetActive(false);
        if (GameManager.Instance.IsPaused())
        {
            messageText.gameObject.SetActive(true);
        }
    }

    private void OnDestroy()
    {
        GameManager.Instance.OnGamePaused -= GameMangaer_OnLocalGamePaused;
        GameManager.Instance.OnGameUnpaused -= GameMangaer_OnLocalGameUnpaused;
    }
}
