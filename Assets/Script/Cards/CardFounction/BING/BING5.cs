using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BING5 : CardFunction
{
    private void Update()
    {
        if(TurnbasedSystem.Instance.CurrentGameStage.Value == GameStage.S1)
        {
            Invoke("Function", 0.3f);
        }
    }
    void Function()
    {
        player.canCost1APInEnemy = true;
        Destroy(gameObject);
    }
}
