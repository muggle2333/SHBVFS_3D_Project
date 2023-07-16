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
        gameOverText.text = "YOUR OPONENT LEFT";
    }

    private void GameManager_OnGameOver(object sender, System.EventArgs e)
    {
        content.SetActive(true);
        int winnerId = GameplayManager.Instance.GetWinner();
        if (winnerId == 2)
        {
            gameOverText.text = "YOU BOTH WIN";
        }
        else
        {
            if (winnerId == (int)NetworkManager.Singleton.LocalClientId)
            {
                gameOverText.text = "YOU WIN";
            }
            else
            {
                gameOverText.text = "YOU LOOSE";
            }
        }
    }
}
