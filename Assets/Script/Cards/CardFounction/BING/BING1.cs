using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BING1 : CardFunction
{
    // Start is called before the first frame update
    void Start()
    {
        Function();
    }

    // Update is called once per frame
    void Function()
    {
        if(GameplayManager.Instance.playerList[0].Defence< GameplayManager.Instance.playerList[1].AttackDamage)
        {
            GameplayManager.Instance.playerList[0].HP -= GameplayManager.Instance.playerList[1].AttackDamage - GameplayManager.Instance.playerList[0].Defence;
        }
        if (GameplayManager.Instance.playerList[1].Defence < GameplayManager.Instance.playerList[0].AttackDamage)
        {
            GameplayManager.Instance.playerList[1].HP -= GameplayManager.Instance.playerList[0].AttackDamage - GameplayManager.Instance.playerList[1].Defence;
        }
        Destroy(gameObject);
    }
}
