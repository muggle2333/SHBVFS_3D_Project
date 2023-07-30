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
    [SerializeField] private TMP_Text timerText;

    private float dyingTimerValue;
    public void InitializeDyingUI()
    {
        GameplayManager.Instance.OnPlayerDying += GameplayManager_OnPlayerDying;
        //GameplayManager.Instance.OnPlayerSelfDying += GameplayManager_OnPlayerSelfDying;
        GameplayManager.Instance.OnLeaveDyingStage += GameplayManager_OnLeaveDyingStage;
    }
    public void Update()
    {
        if (!container.activeSelf) return;
        if(dyingTimerValue>0)
        {
            dyingTimerValue-= Time.deltaTime;
            timerText.text = Mathf.FloorToInt(dyingTimerValue).ToString();
        }
        else
        {
            timerText.text = Mathf.FloorToInt(0).ToString();
        }
    }

    private void GameplayManager_OnPlayerDying(object sender, EventArgs e)
    {
        container.SetActive(true);
        dyingTimerValue = GameplayManager.DYING_TIMER;
        if (GameplayManager.Instance.GetDyingPlayer().Count>0)
        {
            if(GameplayManager.Instance.currentPlayer.HP<=0)
            {
                dyingTitle.text = "</nobr>YOU ARE DYING </nobr> PLAY CARD TO HEAL";
            }
            else
            {
                dyingTitle.text = " ENEMY IS DYING";
            }
        }
        //else if(GameplayManager.Instance.GetDyingPlayer().Count == 2)
        //{
        //    dyingTitle.text = " YOU BOTH ARE DYING";
        //}
    }
    private void GameplayManager_OnPlayerSelfDying(object sender, EventArgs e)
    {
        selfDyingNotice.SetActive(true);
        
    }
    private void GameplayManager_OnLeaveDyingStage(object sender, EventArgs e)
    {
        //selfDyingNotice.SetActive(false);
        container.SetActive(false);
    }

    private void OnDestroy()
    {
        GameplayManager.Instance.OnPlayerDying -= GameplayManager_OnPlayerDying;
        //GameplayManager.Instance.OnPlayerSelfDying -= GameplayManager_OnPlayerSelfDying;
        GameplayManager.Instance.OnLeaveDyingStage -= GameplayManager_OnLeaveDyingStage;
    }
}
