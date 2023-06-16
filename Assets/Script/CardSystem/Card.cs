using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
[System.Serializable]

public class Card
{
    public int id;
    public string cardName;
    public string cardDescription;

    public Sprite thisImage;
    public Card()
    {

    }

    public Card(int id, string cardName, string cardDescription, Sprite thisImage)
    {
        this.id = id;
        this.cardName = cardName;
        this.cardDescription = cardDescription;

        this.thisImage = thisImage;
    }
}
