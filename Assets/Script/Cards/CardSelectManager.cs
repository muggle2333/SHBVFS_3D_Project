using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using DG.Tweening;
using UnityEngine.EventSystems;

public class CardSelectManager : MonoBehaviour
{
    public int SelectCount;
    public bool IsRetracted;
    public float RetractOffset;
    public float offset;
    public float handX = 0f;
    public float handY = 0f;
    public float cardWidth = 240f;
    public float interval = 300f;
    public GameObject UpBotton;
    public GameObject DownBotton;
    public GameObject SelectButton;
    public GameObject CancelButton;
    public Canvas canvas;

    [SerializeField] private float upperY;
    [SerializeField] private float lowerY;
    [SerializeField] private float duration;

    public CardSelectComponent[] cardsArray;
    public List<CardSelectComponent> cardsList;
    private void Awake()
    {
        SelectCount = 0;
        IsRetracted = false;
        offset = cardWidth;
        handY = -200f;
    }
    public void Start()
    {
        cardsArray = GetComponentsInChildren<CardSelectComponent>();
        cardsList = new List<CardSelectComponent>(cardsArray);
        if(HasCard())
        {
            //handY = cardsList[0].formerY;
            UpdateCardPos();
        }
    }

    public void DiscardCards()
    {
        for (int i = 0; i < cardsList.Count; i++)
        {
            if (cardsList[i].isSelected)
            {
                Debug.Log("Card " + cardsList[i].name + " is played.");
                cardsList[i].EndSelect();
                Destroy(cardsList[i].gameObject);
                cardsList.RemoveAt(i);
                i--;
            }
        }
        UpdateCardPos();
    }
    public void PlayCards(Player player)
    {
        for (int i = 0; i < cardsList.Count; i++)
        {
            if (cardsList[i].isSelected)
            {
                Debug.Log("Card " + cardsList[i].name + " is played.");
                //cardsList[i].EndSelect();

                CardManager.Instance.AddPlayedCard(cardsList[i].gameObject.GetComponent<Card>(),player);
                for(int k = 0; k < CardManager.Instance.playerHandCardDict[player].Count; k++)
                {
                    if (CardManager.Instance.playerHandCardDict[player][k] == cardsList[i])
                    {
                        CardManager.Instance.playerHandCardDict[player].RemoveAt(k);
                    }
                }
                
                if(cardsList[i].gameObject.GetComponent<Card>().effectStage == EffectStage.Every)
                {
                    CardManager.Instance.ImmediateCardTakeEffect(player);
                }
                else if (cardsList[i].gameObject.GetComponent<Card>().effectStage == EffectStage.S1)
                {
                    CardManager.Instance.S1CardTakeEffect(player);
                }
                else
                {
                    CardManager.Instance.AddPlayedCard( cardsList[i].gameObject.GetComponent<Card>(),player);
                }
                
                cardsList[i].CardPlayAniamtion();
                //cardsList[i].Interactable = false;
                //cardsList[i].EndSelect();
                cardsList.RemoveAt(i);
                i--;
            }
        }
        UpdateCardPos();
    }

    public void CancelCards()
    {
        for (int i = 0; i < cardsList.Count; i++)
        {
            if (cardsList[i].isSelected)
            {
                Debug.Log("Card " + cardsList[i].name + " is canceled.");
                cardsList[i].EndSelect();
            }
        }
    }

    public void UpdateCardPos()
    {
        //offset = interval / cardsList.Count;
        this.GetComponent<RectTransform>().sizeDelta = new Vector2(cardWidth * cardsList.Count + 50, 100);
        Vector2 startPos = new Vector2(handX - cardsList.Count / 2.0f * offset + offset * 0.5f, handY);
        for (int i = 0; i < cardsList.Count; i++)
        {
            cardsList[i].GetComponent<RectTransform>().DOAnchorPos(startPos, 0.4f);
            startPos.x += offset;
        }
    }

    public bool HasCard()
    {
        return cardsList.Count > 0;
    }

    public void Retract()
    {
        CancelCards();
        handY -= RetractOffset;
        IsRetracted = true;
        UpdateCardPos();
        UpBotton.SetActive(true);
        DownBotton.SetActive(false);
    }

    public void Disretract()
    {
        handY += RetractOffset;
        IsRetracted = false;
        UpdateCardPos();
        DownBotton.SetActive(true);
        UpBotton.SetActive(false);
    }

    //public IEnumerator playCardAnimation(CardSelectComponent card)
    //{
    //    card.transform.DOLocalMove(new Vector3(0, 450, 0), 0.4f);
    //    yield return new WaitForSeconds(1f);
    //    Destroy(card.gameObject);
    //}


}