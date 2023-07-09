using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum GameplayUIType
{
    playCard,
    cancel,
    discardCards,
    retract,
    disretract,
}
public class GameplayUI : MonoBehaviour
{
    public Button playCard;
    public Button cancel;
    public Button discardCards;
    public Button playHP;



    // Start is called before the first frame update
    void Awake()
    {
        playCard.onClick.AddListener(() =>
        {
            PlayerManager.Instance.cardSelectManager.PlayCards(GameplayManager.Instance.currentPlayer);
        });
        cancel.onClick.AddListener(() =>
        {
            PlayerManager.Instance.cardSelectManager.CancelCards(GameplayManager.Instance.currentPlayer);
        });
        discardCards.onClick.AddListener(() =>
        {
            PlayerManager.Instance.cardSelectManager.DiscardCards(GameplayManager.Instance.currentPlayer);
        });
        playHP.onClick.AddListener(() =>
        {
            PlayerManager.Instance.cardSelectManager.PlayCards(GameplayManager.Instance.currentPlayer);
        });


    }

}
