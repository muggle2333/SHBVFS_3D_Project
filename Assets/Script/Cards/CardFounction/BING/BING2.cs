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

    // Update is called once per frame
    void Update()
    {
        
    }
    void Function()
    {

        if(player.moveCount == 0)
        {
            Player enemy = GameplayManager.Instance.PlayerIdToPlayer(GameplayManager.Instance.GetEnemy(player.Id));
            if (enemy.Defence==0)
            {
                enemy.HP -= 1;
            }
        }
        Destroy(gameObject);
    }
}
