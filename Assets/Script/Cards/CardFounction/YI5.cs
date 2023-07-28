using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class YI5 : CardFunction
{
    private bool isUnlock = false;
    void Update()
    {
        if(TurnbasedSystem.Instance.CurrentGameStage.Value == GameStage.S1 && isUnlock == false)
        {
            isUnlock = true;
            Function();
        }
    }
    public void Function()
    {
        player.CurrentActionPoint += 2;
        Destroy(this.gameObject);
    }
}
