using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ThisCard : MonoBehaviour
{
    public List<Card> thisCard = new List<Card>();
    public int thisID;

    public int id;
    public string cardName;
    public string cardDescription;

    public Text nameText;
    public Text descriptionText;

    public Sprite thisSprite;
    public Image thatImage;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //thisCard[0] = CardDataBase.basicCardList[thisID];
        id = thisCard[0].id;
        cardName = thisCard[0].cardName;
        cardDescription = thisCard[0].cardDescription;

        thisSprite = thisCard[0].thisImage;

        nameText.text = "" + cardName;
        descriptionText.text = "" + cardDescription;


        thatImage.sprite = thisSprite;
    }
}
