using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayedCardUI : MonoBehaviour
{
    [SerializeField] private TMP_Text SelfPlayedCard;
    [SerializeField] private TMP_Text EnemyPlayedCard;
    private bool canStart=false;
    private void Start()
    {
        Invoke("UnLock", 3);
    }
    private void Update()
    {
        if (canStart)
        {
            if (GameplayManager.Instance.currentPlayer.Id == PlayerId.RedPlayer)
            {
                SelfPlayedCard.text = CardManager.Instance.redPlayerPlayedCards.Count.ToString();
                if(TurnbasedSystem.Instance.CurrentGameStage.Value == GameStage.S1)
                {
                    EnemyPlayedCard.text = "--";
                }
                else
                {
                    EnemyPlayedCard.text = CardManager.Instance.bluePlayerPlayedCards.Count.ToString();
                }
            }
            else
            {
                SelfPlayedCard.text = CardManager.Instance.bluePlayerPlayedCards.Count.ToString();
                if (TurnbasedSystem.Instance.CurrentGameStage.Value == GameStage.S1)
                {
                    EnemyPlayedCard.text = "--";
                }
                else
                {
                    EnemyPlayedCard.text = CardManager.Instance.redPlayerPlayedCards.Count.ToString();
                }
            }
        }
    }
    private void UnLock()
    {
        canStart = true;
    }
}
