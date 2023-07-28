using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RU1 : CardFunction
{
    // Start is called before the first frame update
    void Start()
    {
        Function();
    }
    void Function()
    {
        if (player.baseAttackDamage >= 1)
        {
            player.baseAttackDamage--;
            player.baseDefense++;
            Calculating.Instance.CalculatPlayerBaseData(player);
        }
        Destroy(gameObject);
    }
}
