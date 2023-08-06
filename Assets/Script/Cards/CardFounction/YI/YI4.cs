using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class YI4 : CardFunction
{
    // Start is called before the first frame update
    void Start()
    {
        Function();
    }
    public void Function()
    {
        player.HP += player.damageThisRound;
        player.ChangeHP(player.damageThisRound);
        Destroy(gameObject);
    }
}
