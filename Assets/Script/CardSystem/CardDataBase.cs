using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardDataBase : MonoBehaviour
{
    public static List<Card> cardList = new List<Card>();
    private void Awake()
    {
        cardList.Add(new Card(0, "HP + 1", "HP + 1",Resources.Load<Sprite>("0")));
        cardList.Add(new Card(1, "Range + 1", "Range + 1", Resources.Load<Sprite>("1")));
        cardList.Add(new Card(2, "Defence + 1", "Defence + 1", Resources.Load<Sprite>("2")));
        cardList.Add(new Card(3, "Damage + 1", "Damage + 1", Resources.Load<Sprite>("3")));
        
    }
}
