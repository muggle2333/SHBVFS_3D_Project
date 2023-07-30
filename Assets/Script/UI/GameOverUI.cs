using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class GameOverUI : MonoBehaviour
{
    [SerializeField] private Button exitBtn;
    [SerializeField] private TMP_Text gameOverText;
    [SerializeField] private GameObject content;
    private void Start()
    {
        GameManager.Instance.OnPlayerDisconnect += GameManager_OnPlayerDisconnect;
        GameManager.Instance.OnGameOver += GameManager_OnGameOver;
    }
    private void Awake()
    {
        exitBtn.onClick.AddListener(() =>
        {
            Loader.Load(Loader.Scene.MainMenuScene);
        });
    }
    private void GameManager_OnPlayerDisconnect(object sender, System.EventArgs e)
    {
        content.SetActive(true);
        //gameOverText.text = "YOUR COMPETITOR LEFT";
        gameOverText.text = "YOU WIN";
    }

    private void GameManager_OnGameOver(object sender, System.EventArgs e)
    {
        content.SetActive(true);
        int winnerId = GameplayManager.Instance.GetWinner();
        if (winnerId == 2)
        {
            gameOverText.text = "YOU BOTH WIN";
            SoundManager.Instance.PlayBgm(Bgm.WinBGM);
        }
        else
        {
            if (winnerId == (int)NetworkManager.Singleton.LocalClientId)
            {
                gameOverText.text = "YOU WIN";
                SoundManager.Instance.PlayBgm(Bgm.WinBGM);
            }
            else
            {
                gameOverText.text = "YOU LOSE";
                SoundManager.Instance.PlayBgm(Bgm.LoseBGM);
            }
        }
    }
}
