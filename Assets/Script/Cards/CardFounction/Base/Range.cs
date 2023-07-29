using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Range : CardFunction
{
    // Start is called before the first frame update
    void Start()
    {
        Function();
    }

    // Update is called once per frame
    void Function()
    {
        player.baseRange += 1;
        Calculating.Instance.CalculatPlayerBaseData(player);
        Destroy(gameObject);
    }
}
