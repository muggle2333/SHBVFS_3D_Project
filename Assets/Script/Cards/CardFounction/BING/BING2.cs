using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BING2 : CardFunction
{
    // Start is called before the first frame update
    void Start()
    {
        Function();
    }
    void Function()
    {
        //Old Bing2
        /*if(player.moveCount == 0)
        {
            Player enemy = GameplayManager.Instance.PlayerIdToPlayer(GameplayManager.Instance.GetEnemy(player.Id));
            if (enemy.Defence==0)
            {
                enemy.HP -= 1;
            }
        }*/
        Player enemy = GameplayManager.Instance.PlayerIdToPlayer(GameplayManager.Instance.GetEnemy(player.Id));
        if(GridManager.Instance.GetGridDistance(player.currentGrid, enemy.currentGrid) <= player.Range)
        {
            enemy.isBleeding = true;
        }
        Destroy(gameObject);
    }
}
