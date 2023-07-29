using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class FA4 : CardFunction
{
    // Start is called before the first frame update
    void Start()
    {
        Function();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void Function()
    {
        int randomInt = Random.Range(0, CardManager.Instance.playerHandCardDict[player].Count + 1);

        CardManager.Instance.playerHandCardDict[player][randomInt].gameObject.GetComponent<CardSelectComponent>().CardDiscardAnimation();
        CardManager.Instance.playerHandCardDict[player].RemoveAt(randomInt);
        if (NetworkManager.Singleton.IsServer)
        {
            CardManager.Instance.RemoveCardFromPlayerHandServerRpc(player.Id, randomInt);
            FindObjectOfType<DrawCardComponent>().PlayDrawCardAnimationServerRpc(player.Id, GameplayManager.Instance.discardStage.discardCount[player]);
        }
        FindObjectOfType<CardSelectManager>().UpdateCardPos(player);
        Destroy(gameObject);
    }
}
