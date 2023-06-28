using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using DG.Tweening;
using UnityEngine.EventSystems;

public class CardSelectManager : MonoBehaviour, IPointerClickHandler
{
    public bool IsRetracted;
    public float offset;
    public float handX = 0f;
    public float handY = 0f;
    public float cardWidth = 240f;
    public float interval = 300f;
    [SerializeField] private float upperY;
    [SerializeField] private float lowerY;
    [SerializeField] private float duration;
    public CardSelectComponent[] cardsArray;
    public List<CardSelectComponent> cardsList; 
    public void Start()
    {
        IsRetracted = false;
        offset = cardWidth;
        cardsArray = GetComponentsInChildren<CardSelectComponent>();
        cardsList = new List<CardSelectComponent>(cardsArray);
        if(HasCard())
        {
            UpdateCardPos();
            handY = cardsList[0].formerY;
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
    public void SelectCards()
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

    public void OnPointerClick(PointerEventData eventData)
    {
        if(IsRetracted) transform.DOLocalMoveY(upperY, duration);
        else transform.DOLocalMoveY(lowerY, duration);
    }
}
