using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class YI5 : CardFunction
{
    private bool isUnlock = false;
    private void Start()
    {
        player.CurrentActionPoint -= 1;
    }
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
        player.CurrentActionPoint += 3;
        Destroy(this.gameObject);
    }
}
