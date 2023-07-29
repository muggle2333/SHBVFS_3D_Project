using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Defense : CardFunction
{
    void Start()
    {
        Function();
    }

    // Update is called once per frame
    void Function()
    {
        player.baseDefense += 1;
        Calculating.Instance.CalculatPlayerBaseData(player);
        Destroy(gameObject);
    }
}
