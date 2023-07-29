using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MAXHP : CardFunction
{
    // Start is called before the first frame update
    void Start()
    {
        Function();
    }

    // Update is called once per frame
    void Function()
    {
        if(TurnbasedSystem.Instance.isDie.Value == false)
        {
            player.baseMaxHP += 1;
            Calculating.Instance.CalculatPlayerBaseData(player);
        }
        else
        {
            player.HP += 2;
        }
        Destroy(gameObject);
    }
}
