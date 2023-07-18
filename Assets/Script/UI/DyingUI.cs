using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DyingUI : MonoBehaviour
{
    [SerializeField] private GameObject container;
    [SerializeField] private GameObject selfDyingNotice;
    [SerializeField] private TMP_Text dyingTitle;

    public void InitializeDyingUI()
    {
        GameplayManager.Instance.OnPlayerDying += GameplayManager_OnPlayerDying;
        GameplayManager.Instance.OnPlayerSelfDying += GameplayManager_OnPlayerSelfDying;
        GameplayManager.Instance.OnLeaveDyingStage += GameplayManager_OnLeaveDyingStage;
    }

    private void GameplayManager_OnPlayerDying(object sender, EventArgs e)
    {
        container.SetActive(true);
        if(GameplayManager.Instance.GetDyingPlayer().Count==1)
        {
            if(GameplayManager.Instance.currentPlayer.HP<=0)
            {
                dyingTitle.text = " YOU ARE DYING";
            }
        }
        else if(GameplayManager.Instance.GetDyingPlayer().Count == 2)
        {
            dyingTitle.text = " YOU BOTH ARE DYING";
        }
    }
    private void GameplayManager_OnPlayerSelfDying(object sender, EventArgs e)
    {
        selfDyingNotice.SetActive(true);
    }
    private void GameplayManager_OnLeaveDyingStage(object sender, EventArgs e)
    {
        selfDyingNotice.SetActive(false);
        container.SetActive(false);
    }

    private void OnDestroy()
    {
        GameplayManager.Instance.OnPlayerDying -= GameplayManager_OnPlayerDying;
        GameplayManager.Instance.OnPlayerSelfDying -= GameplayManager_OnPlayerSelfDying;
        GameplayManager.Instance.OnLeaveDyingStage -= GameplayManager_OnLeaveDyingStage;
    }
}
