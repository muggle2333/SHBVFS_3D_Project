using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Unity.Netcode;

public class MO3 : CardFunction
{
    public GameObject cardPrefeb;
    public GameObject cardContent;
    public bool hasAdd = false;
    public int count;
    void Start()
    {
        cardPrefeb = DrawCardComponent.Instance.cardPrefab;
        cardContent = DrawCardComponent.Instance.CardContent;
        if (NetworkManager.Singleton.IsServer == true)
        {
            DrawCardComponent.Instance.PlayDrawCardAnimationServerRpc(player.Id, 2);
        }
        Function();
    }
    private void Update()
    {
        if(TurnbasedSystem.Instance.CurrentGameStage.Value == GameStage.S4 && hasAdd == false)
        {
            hasAdd = true;
            count++;
            if(count == 3)
            {
                player.canAttack = true;
                Destroy(gameObject);
            }
        }
        if(TurnbasedSystem.Instance.CurrentGameStage.Value == GameStage.S1 && hasAdd == true)
        {
            if(count != 3)
            {
                player.canAttack = false;
            }
            hasAdd = false;
        }
    }
    void Function()
    {
        player.canAttack = false;
        if (player.Id == GameplayManager.Instance.currentPlayer.Id)
        {
            Card card = Instantiate(cardPrefeb, new Vector3(0, 0, 0), Quaternion.identity, cardContent.transform).GetComponent<Card>();
            card.cardSetting = CardDataBase.Instance.AllTopCardListDic[AcademyType.MO][0];
            DrawCardComponent.Instance.AllCardCountPlusServerRpc((int)AcademyType.MO, AcademyType.MO);
            card.UpdateCardData(card.cardSetting);
            CardManager.Instance.playerHandCardDict[player].Add(card);
            card.GetComponent<RectTransform>().localPosition = DrawCardComponent.Instance.GetScreenPosition(GameplayManager.Instance.currentPlayer.gameObject);
            card.transform.localScale = new Vector3(0.05f, 0.05f, 0.05f);
            card.transform.DOScale(1, 0.5f);
            PlayerManager.Instance.cardSelectManager.UpdateCardPos(player);
            CardManager.Instance.AddCardToPlayerHandServerRpc(player.Id, card.cardId);

            card = Instantiate(cardPrefeb, new Vector3(0, 0, 0), Quaternion.identity, cardContent.transform).GetComponent<Card>();
            card.cardSetting = CardDataBase.Instance.AllTopCardListDic[AcademyType.MO][1];
            DrawCardComponent.Instance.AllCardCountPlusServerRpc((int)AcademyType.MO, AcademyType.MO);
            card.UpdateCardData(card.cardSetting);
            CardManager.Instance.playerHandCardDict[player].Add(card);
            card.GetComponent<RectTransform>().localPosition = DrawCardComponent.Instance.GetScreenPosition(GameplayManager.Instance.currentPlayer.gameObject);
            card.transform.localScale = new Vector3(0.05f, 0.05f, 0.05f);
            card.transform.DOScale(1, 0.5f);
            PlayerManager.Instance.cardSelectManager.UpdateCardPos(player);
            CardManager.Instance.AddCardToPlayerHandServerRpc(player.Id, card.cardId);
        }
        

    }
}
