using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DiscardUI : MonoBehaviour
{
    [SerializeField] private GameObject container;
    [SerializeField] private TMP_Text discardText;
    [SerializeField] private TMP_Text timerText;

    private DiscardStage discardStage;
    private CardSelectManager cardSelectManager;

    public void InitializeDiscardUI()
    {
        discardStage = FindObjectOfType<DiscardStage>();
        cardSelectManager = FindObjectOfType<CardSelectManager>();
        discardStage.OnStartDiscardStage += DiscardStage_OnStartDiscardStage;
        discardStage.OnStartSelfDiscardStage += DiscardStage_OnStartSelfDiscardStage;
        discardStage.OnCompleteDiscardStage += DiscardStage_OnCompleteDiscardStage;
        cardSelectManager.OnDiscardCard += DiscardStage_OnSelectDiscard;
    }

    public void Update()
    {
        if(discardStage != null)
        {
            timerText.text = Mathf.FloorToInt(discardStage.timerValue.Value).ToString();
        }
    }
    private void DiscardStage_OnStartDiscardStage(object sender, EventArgs e)
    {
        container.SetActive(true);
        discardText.text = "Competitor is discarding";
    }
    
    private void DiscardStage_OnStartSelfDiscardStage(object sender, EventArgs e)
    {
        timerText.gameObject.SetActive(true);
        discardText.text = "Discard your card to fit your HP";
    }

    private void DiscardStage_OnCompleteDiscardStage(object sender, EventArgs e)
    {
        container.SetActive(false);
        timerText.gameObject.SetActive(false);
    }

    private void DiscardStage_OnSelectDiscard(object sender, EventArgs e)
    {
        timerText.gameObject.SetActive(false);
        discardText.text = "Competitor is discarding";
    }
}
