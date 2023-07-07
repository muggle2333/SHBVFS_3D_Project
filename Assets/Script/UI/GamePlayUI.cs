using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GamePlayUI : MonoBehaviour
{
    [SerializeField] private Button playCard;
    [SerializeField] private Button cancel;
    [SerializeField] private Button discardCards;
    [SerializeField] private Button retract;
    [SerializeField] private Button disretract;

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
        retract.onClick.AddListener(() =>
        {
            PlayerManager.Instance.cardSelectManager.Retract(GameplayManager.Instance.currentPlayer);
        });
        disretract.onClick.AddListener(() =>
        {
            PlayerManager.Instance.cardSelectManager.Disretract(GameplayManager.Instance.currentPlayer);
        });



    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
