using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MO2 : CardFunction
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
        player.freeMoveCount += player.moveCount;
        player.trueFreeMoveCount += player.moveCount;
        Destroy(gameObject);
    }
}
