using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TurnbaseUI : MonoBehaviour
{
    [SerializeField] private TMP_Text stageText;
    [SerializeField] private TMP_Text timerText;
    [SerializeField] private TMP_Text roundText;
    [SerializeField] private Button skipBtn;

    private void Awake()
    {
        skipBtn.onClick.AddListener(() =>
        {

        });
    }

    public void UpdateStageInfo(GameStage stage,float timer, int round)
    {
        stageText.text = stage.ToString();
        timerText.text = Mathf.FloorToInt(timer).ToString();
        roundText.text = round.ToString();
    }
}
