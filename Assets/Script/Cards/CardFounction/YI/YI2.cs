using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class YI2 : CardFunction
{
    // Start is called before the first frame update
    void Start()
    {
        Function();
    }
    public void Function()
    {
        player.HP += player.descoverLandCount/2;
        if(player.HP > player.MaxHP)
        {
            player.HP = player.MaxHP;
        }
        Destroy(gameObject);
    }
}
