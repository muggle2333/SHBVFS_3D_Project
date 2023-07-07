using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using DG.Tweening;
using UnityEngine.EventSystems;

public class CardSelectManager : MonoBehaviour
{
    public Dictionary<Player, int> SelectCount = new Dictionary<Player, int>();
    public bool IsRetracted;
    public float RetractOffset;
    public float offset;
    public float handX = 0f;
    public float handY = 0f;
    public float cardWidth = 240f;
    public float interval = 300f;
    //public GameObject UpBotton;
    //public GameObject DownBotton;
    //public GameObject SelectButton;
    //public GameObject CancelButton;
    public Canvas canvas;

    [SerializeField] private float upperY;
    [SerializeField] private float lowerY;
    [SerializeField] private float duration;

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
        SelectCount[GameplayManager.Instance.currentPlayer] = 0;
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
                CardManager.Instance.playerHandCardDict[player][i].gameObject.GetComponent<CardSelectComponent>().EndSelect();
                Destroy(CardManager.Instance.playerHandCardDict[player][i].gameObject.GetComponent<CardSelectComponent>().gameObject);
                CardManager.Instance.playerHandCardDict[player].RemoveAt(i);
                i--;
            }
        }
        UpdateCardPos(player);
    }
    public void PlayCards(Player player)
    {
        for (int i = 0; i < CardManager.Instance.playerHandCardDict[player].Count; i++)
        {
            if (CardManager.Instance.playerHandCardDict[player][i].gameObject.GetComponent<CardSelectComponent>().isSelected)
            {
                //Debug.Log("Card " + cardsList[i].name + " is played.");
                
                if(CardManager.Instance.playerHandCardDict[player][i].effectStage == EffectStage.Every)
                {
                    CardManager.Instance.ImmediateCardTakeEffect(player);
                }
                else if (CardManager.Instance.playerHandCardDict[player][i].effectStage == EffectStage.S1)
                {
                    CardManager.Instance.S1CardTakeEffect(player);
                }
                else
                {
                    CardManager.Instance.playedCardDict[player].Add(CardManager.Instance.playerHandCardDict[player][i]);
                }

                CardManager.Instance.playerHandCardDict[player][i].gameObject.GetComponent<CardSelectComponent>().CardPlayAniamtion();
                CardManager.Instance.playerHandCardDict[player].RemoveAt(i);
                i--;
            }
        }
        UpdateCardPos(player);
    }

    public void CancelCards(Player player)
    {
        for (int i = 0; i < CardManager.Instance.playerHandCardDict[player].Count; i++)
        {
            if (CardManager.Instance.playerHandCardDict[player][i].gameObject.GetComponent<CardSelectComponent>().isSelected)
            {
                CardManager.Instance.playerHandCardDict[player][i].gameObject.GetComponent<CardSelectComponent>().EndSelect();
                i--;
            }
        }
    }

    public void UpdateCardPos(Player player)
    {
        Debug.Log(3333);
        //offset = interval / cardsList.Count;
        int count = CardManager.Instance.playerHandCardDict[player].Count;
        this.GetComponent<RectTransform>().sizeDelta = new Vector2(cardWidth * count + 100, 100);
        Vector2 startPos = new Vector2(handX - count / 2.0f * offset + offset * 0.5f, handY);
        for (int i = 0; i < count; i++)
        {
            CardManager.Instance.playerHandCardDict[player][i].GetComponent<RectTransform>().DOAnchorPos(startPos, 0.4f);
            startPos.x += offset;
        }
    }

    public void Retract(Player player)
    {
        CancelCards(player);
        handY -= RetractOffset;
        IsRetracted = true;
        UpdateCardPos(player);
        GameplayManager.Instance.gameplayUI.disretract.gameObject.SetActive(true);
        GameplayManager.Instance.gameplayUI.retract.gameObject.SetActive(false);
    }

    public void Disretract(Player player)
    {
        handY += RetractOffset;
        IsRetracted = false;
        UpdateCardPos(player);
        GameplayManager.Instance.gameplayUI.disretract.gameObject.SetActive(false);
        GameplayManager.Instance.gameplayUI.retract.gameObject.SetActive(true);
    }

}