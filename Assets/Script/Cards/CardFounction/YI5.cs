using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class YI5 : CardFunction
{
    void Update()
    {
        if(TurnbasedSystem.Instance.CurrentGameStage.Value == GameStage.S1)
        {
            Function();
        }
    }
    public void Function()
    {
        player.CurrentActionPoint += 2;
        Destroy(this.gameObject);
    }
}
