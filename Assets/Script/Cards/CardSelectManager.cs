using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using DG.Tweening;

public class CardSelectManager : MonoBehaviour
{
    public float handX = 0f;
    public float handY = 0f;
    public float interval = 300f;
    private CardSelect[] cardsArray;
    private List<CardSelect> cardsList; 
    void Start()
    {
        cardsArray = GetComponentsInChildren<CardSelect>();
        cardsList = new List<CardSelect>(cardsArray);
        if(HasCard())
        {
            UpdateCardPos();
            handY = cardsList[0].formerY;
        }
    }

    public void SelectCards()
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

    private void UpdateCardPos()
    {
        float offset = interval / cardsList.Count;
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
