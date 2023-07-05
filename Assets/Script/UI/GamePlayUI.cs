using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GamePlayUI : MonoBehaviour
{
    [SerializeField] private Button playCard;
    [SerializeField] private Button cancel;


    // Start is called before the first frame update
    void Awake()
    {
        playCard.onClick.AddListener(() =>
        {
            PlayerManager.Instance.cardSelectManager.PlayCards(GameplayManager.Instance.currentPlayer);
        });
        cancel.onClick.AddListener(() =>
        {
            PlayerManager.Instance.cardSelectManager.CancelCards();
        });
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
