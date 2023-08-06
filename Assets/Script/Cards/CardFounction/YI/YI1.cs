using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class YI1 : CardFunction
{
    void Start()
    {
        Function();
    }
    void Function()
    {
        if(player.moveCount == 0)
        {
            player.HP += 2;
            player.ChangeHP(2);
            if(player.HP > player.MaxHP)
            {
                player.HP = player.MaxHP;
            }
        }
        Destroy(gameObject);
    }
}
