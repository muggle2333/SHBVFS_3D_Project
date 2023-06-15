using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardDataBase : MonoBehaviour
{
    public static List<Card> cardList = new List<Card>();
    private void Awake()
    {
        cardList.Add(new Card(0, "HP + 1", "HP + 1",Resources.Load<Sprite>("1")));
        cardList.Add(new Card(1, "Defence + 1", "Defence + 1", Resources.Load<Sprite>("1")));
        cardList.Add(new Card(2, "Attack + 1", "Attack + 1", Resources.Load<Sprite>("1")));
        cardList.Add(new Card(3, "Range + 1", "Range + 1", Resources.Load<Sprite>("1")));
    }
}
