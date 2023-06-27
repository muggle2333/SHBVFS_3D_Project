using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using DG.Tweening;

public class CardSelectManager : MonoBehaviour
{
    public float offset;
    public float handX = 0f;
    public float handY = 0f;
    public float cardWidth = 240f;
    public float interval = 300f;
    public CardSelect[] cardsArray;
    public List<CardSelect> cardsList; 
    public void Start()
    {
        cardsArray = GetComponentsInChildren<CardSelect>();
        cardsList = new List<CardSelect>(cardsArray);
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
            if(cardsList[i].isSelected)
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
        if (cardsList.Count > 8) 
            offset = interval / cardsList.Count;
        else 
            offset = cardWidth;
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
}
