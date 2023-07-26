using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum GameplayUIType
{
    playCard,
    cancelControl,
    discardCards,
    cancelDiscard,
    playHP,
    cancelDying
}

public class GameplayUI : MonoBehaviour
{
    public Button playCard;
    public Button cancelControl;
    public Button discardCards;
    public Button cancelDiscard;
    public Button playHP;
    public Button cancelDying;



    // Start is called before the first frame update
    void Awake()
    {
        playCard.onClick.AddListener(() =>
        {
            PlayerManager.Instance.cardSelectManager.PlayCards(GameplayManager.Instance.currentPlayer);
        });
        cancelControl.onClick.AddListener(() =>
        {
            PlayerManager.Instance.cardSelectManager.CancelCards(GameplayManager.Instance.currentPlayer);
        });
        discardCards.onClick.AddListener(() =>
        {
            PlayerManager.Instance.cardSelectManager.DiscardCards(GameplayManager.Instance.currentPlayer);
        });
        cancelDiscard.onClick.AddListener(() =>
        {
            PlayerManager.Instance.cardSelectManager.CancelCards(GameplayManager.Instance.currentPlayer);
        });
        playHP.onClick.AddListener(() =>
        {
            PlayerManager.Instance.cardSelectManager.PlayHPCards(GameplayManager.Instance.currentPlayer);
        });
        cancelDying.onClick.AddListener(() =>
        {
            PlayerManager.Instance.cardSelectManager.CancelCards(GameplayManager.Instance.currentPlayer);
        });
    }
}
