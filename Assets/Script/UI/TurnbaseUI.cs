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
    [SerializeField] private Button endBtn;
    [SerializeField] private Button skipBtn;

    private void Awake()
    {
        skipBtn.onClick.AddListener(() =>
        {
            TurnbasedSystem.Instance.SkipControlStageServerRpc();
            skipBtn.gameObject.SetActive(false);
        });
    }
    private void Update()
    {
        if(TurnbasedSystem.Instance.CurrentGameStage.Value != GameStage.S1)
        {
            skipBtn.gameObject.SetActive(false);
        }
        else
        {
            skipBtn.gameObject.SetActive(true);
        }
    }

    public void UpdateStageInfo(GameStage stage,float timer, int round)
    {
        stageText.text = stage.ToString();
        if (timer <= 0) timer = 0;
        timerText.text = Mathf.FloorToInt(timer).ToString();
        roundText.text = round.ToString();
    }

    [ClientRpc]
    public void ShowSkipBtnClientRpc(bool isShow)
    {
        skipBtn.gameObject.SetActive(isShow);
    }
}
