using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class TurnbaseUI : MonoBehaviour
{
    [SerializeField] private TMP_Text stageText;
    [SerializeField] private TMP_Text timerText;
    [SerializeField] private TMP_Text roundText;
    [SerializeField] private Button skipBtn;
    [SerializeField] private GameObject container;

    private void Awake()
    {
        skipBtn.onClick.AddListener(() =>
        {
            TurnbasedSystem.Instance.SkipControlStageServerRpc();
            skipBtn.gameObject.SetActive(false);
            SelectManager.Instance.CloseSelectionUI();
            if(FindObjectOfType<TutorialManager>()!=null)
            {
                TutorialManager.Instance.CompleteTutorialAction(TutorialAction.ClickSkip);
            }
        });
        //TurnbasedSystem.Instance.CurrentGameStage.OnValueChanged += UpdateTurnbaseUI;
    }
    public void Start()
    {
        container.SetActive(false);
    }
    public void StartTurnbaseUI()
    {
        container.SetActive(true);
        GetComponentInChildren<PlayedCardUI>().InitializePlayedCardUI();
        TurnbasedSystem.Instance.CurrentGameStage.OnValueChanged += UpdateTurnbaseUI;
    }
    private void UpdateTurnbaseUI(GameStage previousValue, GameStage newValue)
    {
        if(previousValue==GameStage.S1) //CONTROL 结束
        {
            skipBtn.gameObject.SetActive(false);
            timerText.gameObject.SetActive(false);
        }else if(newValue==GameStage.S1) // 进入 CONTROL
        { 
            skipBtn.gameObject.SetActive(true);
            timerText.gameObject.SetActive(true);
        }//else if(previousValue==GameStage.DiscardStage)
        //{
        //    timerText.gameObject.SetActive(false);
        //}
    }

    public void UpdateStageInfo(GameStage stage,float timer, int round)
    {
        stageText.text = stage.ToString();
        switch(stage)
        {
            case GameStage.S1:
                stageText.text = "Control Stage";break;
            case GameStage.DiscardStage:
                stageText.text = "Discard Stage";break;
            case GameStage.S2:
                stageText.text = "Calculating"; break;
            case GameStage.MoveStage:
                stageText.text = "Move Stage";break;
            case GameStage.S3:
                stageText.text = "Calculating"; break;
            case GameStage.AttackStage:
                stageText.text = "Attack Stage";break;
            case GameStage.S4:
                stageText.text = "Calculating"; break;
            
        }
        if (timer <= 0) timer = 0;
        timerText.text = Mathf.FloorToInt(timer).ToString();
        roundText.text = round.ToString();
    }

    [ClientRpc]
    public void ShowSkipBtnClientRpc(bool isShow)
    {
        skipBtn.gameObject.SetActive(isShow);
    }
    private void OnDestroy()
    {
        TurnbasedSystem.Instance.CurrentGameStage.OnValueChanged -= UpdateTurnbaseUI;
    }
}
