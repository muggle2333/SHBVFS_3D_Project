using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class FA4 : CardFunction
{
    void Start()
    {
        Function();
    }
    void Function()
    {
        Player enemy = GameplayManager.Instance.PlayerIdToPlayer(GameplayManager.Instance.GetEnemy(player.Id));
        int randomInt = Random.Range(0, CardManager.Instance.playerHandCardDict[enemy].Count);
        if (CardManager.Instance.playerHandCardDict[enemy].Count > 0)
        {
            
            if (enemy.Id == GameplayManager.Instance.currentPlayer.Id)
            {
                CardManager.Instance.playerHandCardDict[enemy][randomInt].gameObject.GetComponent<CardSelectComponent>().CardDiscardAnimation_FA4();
                CardManager.Instance.playerHandCardDict[enemy].RemoveAt(randomInt);
                
                FindObjectOfType<CardSelectManager>().UpdateCardPos(enemy);
            }
            CardManager.Instance.RemoveCardFromPlayerHandServerRpc(enemy.Id, randomInt);
            FindObjectOfType<DrawCardComponent>().PlayDrawCardAnimationServerRpc(enemy.Id, -1);
        }
        Destroy(gameObject);
    }
}
