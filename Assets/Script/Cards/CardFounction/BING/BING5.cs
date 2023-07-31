using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BING5 : CardFunction
{
    private void Update()
    {
        if(TurnbasedSystem.Instance.CurrentGameStage.Value == GameStage.S1)
        {
            Function();
            Destroy(gameObject);
        }
    }
    void Function()
    {
        player.canCost1APInEnemy = true;
    }
}
