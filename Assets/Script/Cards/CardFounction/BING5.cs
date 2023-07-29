using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BING5 : CardFunction
{
    // Start is called before the first frame update
    void Start()
    {
        Function();
    }

    void Function()
    {
        player.canCost1APInEnemy = true;
    }
}
