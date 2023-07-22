using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using DG.Tweening;
using UnityEngine.EventSystems;
using Unity.Netcode;
using System;
using TMPro;

public class CardSelectManager : MonoBehaviour
{
    public Dictionary<Player, int> SelectCount = new Dictionary<Player, int>();
    public Dictionary<Player, int> maxSelected = new Dictionary<Player, int>();
    public TMP_Text ToDiscardText;
    public bool IsRetracted;
    public float RetractOffset;
    public float offset;
    public float handX = 0f;
    public float handY = 0f;
    public float cardWidth;
    public float interval = 300f;
    //public GameObject UpBotton;
    //public GameObject DownBotton;
    //public GameObject SelectButton;
    //public GameObject CancelButton;
    public Canvas canvas;

    [SerializeField] private float upperY;
    [SerializeField] private float lowerY;
    [SerializeField] private float duration;

    public event EventHandler OnDiscardCard;

    //public CardSelectComponent[] cardsArray;
    //public List<CardSelectComponent> cardsList;
    private void Awake()
    {
        IsRetracted = false;
        offset = cardWidth;
        handY = -200f;
    }
    public void Start()
    {
        if (FindObjectOfType<NetworkManager>()) return;
        InitializeCardSelectManager();
    }


    public void InitializeCardSelectManager()
    {
        SelectCount[GameplayManager.Instance.currentPlayer] = 0;
        maxSelected[GameplayManager.Instance.currentPlayer] = 1;
        //cardsArray = GetComponentsInChildren<CardSelectComponent>();
        //cardsList = new List<CardSelectComponent>(cardsArray);
        UpdateCardPos(GameplayManager.Instance.currentPlayer);
    }
    public void DiscardCards(Player player)
    {
        for (int i = 0; i < CardManager.Instance.playerHandCardDict[player].Count; i++)
        {
            if (CardManager.Instance.playerHandCardDict[player][i].gameObject.GetComponent<CardSelectComponent>().isSelected)
            {
                //Debug.Log("Card " + cardsList[i].name + " is played.");
                CardManager.Instance.playerHandCardDict[player][i].gameObject.GetComponent<CardSelectComponent>().CardDiscardAnimation();
                CardManager.Instance.playerHandCardDict[player].RemoveAt(i);
                CardManager.Instance.RemoveCardFromPlayerHandServerRpc(player.Id, i);
                i--;
            }
        }
        UpdateCardPos(player);
        GameplayManager.Instance.discardStage.discardCount[player] = 0;
        maxSelected[player] = 1;
        OnDiscardCard?.Invoke(this,EventArgs.Empty);
        UIManager.Instance.SetGameplayPlayUI(GameplayUIType.discardCards, false);
    }
    public void PlayCards(Player player)
    {
        for (int i = 0; i < CardManager.Instance.playerHandCardDict[player].Count; i++)
        {
            if (CardManager.Instance.playerHandCardDict[player][i].gameObject.GetComponent<CardSelectComponent>().isSelected)
            {
                //Debug.Log("Card " + cardsList[i].name + " is played.");
                if (CardManager.Instance.playerHandCardDict[player][i].effectStage == EffectStage.Every)
                {
                    CardManager.Instance.ImmediateCardTakeEffectServerRpc(player.Id, CardManager.Instance.playerHandCardDict[player][i].cardId);
                    CardManager.Instance.playerHandCardDict[player][i].gameObject.GetComponent<CardSelectComponent>().CardDiscardAnimation();
                }
                else if (CardManager.Instance.playerHandCardDict[player][i].effectStage == EffectStage.S1)
                {
                    CardManager.Instance.S1CardTakeEffectServerRpc(player.Id, CardManager.Instance.playerHandCardDict[player][i].cardId);
                    CardManager.Instance.playerHandCardDict[player][i].gameObject.GetComponent<CardSelectComponent>().CardDiscardAnimation();
                }
                else if(CardManager.Instance.playerHandCardDict[player][i].effectStage == EffectStage.S2 || CardManager.Instance.playerHandCardDict[player][i].effectStage == EffectStage.S3 || CardManager.Instance.playerHandCardDict[player][i].effectStage == EffectStage.S4)
                {
                    //CardManager.Instance.playedCardDict[player].Add(CardManager.Instance.playerHandCardDict[player][i]);
                    CardManager.Instance.AddPlayedCardServerRpc(player.Id, CardManager.Instance.playerHandCardDict[player][i].cardId);
                    CardManager.Instance.playerHandCardDict[player][i].gameObject.GetComponent<CardSelectComponent>().CardPlayAniamtion();
                }

                CancelCards(player);
                CardManager.Instance.playerHandCardDict[player].RemoveAt(i);
                CardManager.Instance.RemoveCardFromPlayerHandServerRpc(player.Id, i);//?
                i--;
            }
        }
        UpdateCardPos(player);
        UIManager.Instance.SetGameplayPlayUI(GameplayUIType.playCard, false);
    }

    public void CancelCards(Player player)
    {
        for (int i = 0; i < CardManager.Instance.playerHandCardDict[player].Count; i++)
        {
            if (CardManager.Instance.playerHandCardDict[player][i].gameObject.GetComponent<CardSelectComponent>().isSelected)
            {
                CardManager.Instance.playerHandCardDict[player][i].gameObject.GetComponent<CardSelectComponent>().EndSelectOther();
                i--;
            }
        }
        UIManager.Instance.SetGameplayPlayUI(GameplayUIType.playCard, false);
        UIManager.Instance.SetGameplayPlayUI(GameplayUIType.cancelControl, false);
        UIManager.Instance.SetGameplayPlayUI(GameplayUIType.discardCards, false);
        UIManager.Instance.SetGameplayPlayUI(GameplayUIType.cancelDiscard, false);
        UIManager.Instance.SetGameplayPlayUI(GameplayUIType.playHP, false);
        UIManager.Instance.SetGameplayPlayUI(GameplayUIType.cancelDying, false);
    }

    public void UpdateCardPos(Player player)
    {
        //Debug.Log(3333);
        //offset = interval / cardsList.Count;
        int count = CardManager.Instance.playerHandCardDict[player].Count;
        this.GetComponent<RectTransform>().sizeDelta = new Vector2(cardWidth * count + 100, 100);
        Vector2 startPos = new Vector2(handX - count / 2.0f * offset + offset * 0.5f, handY);
        for (int i = 0; i < count; i++)
        {
            CardManager.Instance.playerHandCardDict[player][i].GetComponent<RectTransform>().DOAnchorPos(startPos, 0.5f);
            //CardManager.Instance.playerHandCardDict[player][i].gameObject.GetComponent<CardSelectComponent>().EndSelectOther();
            if (CardManager.Instance.playerHandCardDict[player][i].gameObject.GetComponent<CardSelectComponent>().isSelected)
            {
                CardManager.Instance.playerHandCardDict[player][i].gameObject.GetComponent<CardSelectComponent>().EndSelectOther();
            }
            startPos.x += offset;
        }
    }

    
    //public void Retract(Player player)
    //{
    //    CancelCards(player);
    //    handY -= RetractOffset;
    //    IsRetracted = true;
    //    UpdateCardPos(player);
    //    //GameplayManager.Instance.gameplayUI.disretract.gameObject.SetActive(true);
    //    //GameplayManager.Instance.gameplayUI.retract.gameObject.SetActive(false);
    //    UIManager.Instance.SetGameplayPlayUI(GameplayUIType.disretract, true);
    //    UIManager.Instance.SetGameplayPlayUI(GameplayUIType.retract, false);
    //}

    //public void Disretract(Player player)
    //{
    //    handY += RetractOffset;
    //    IsRetracted = false;
    //    UpdateCardPos(player);
    //    //GameplayManager.Instance.gameplayUI.disretract.gameObject.SetActive(false);
    //    //GameplayManager.Instance.gameplayUI.retract.gameObject.SetActive(true);
    //    UIManager.Instance.SetGameplayPlayUI(GameplayUIType.disretract, false);
    //    UIManager.Instance.SetGameplayPlayUI(GameplayUIType.retract, true);
    //}

}