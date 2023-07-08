using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class YI5 : CardFunction
{
    private bool HasTakeEffect=false;
    void Update()
    {
        if(TurnbasedSystem.Instance.CurrentGameStage.Value == GameStage.S4 && HasTakeEffect == false)
        {
            HasTakeEffect = true;
            Function(GameplayManager.Instance.currentPlayer);
        }
    }
    public void Function(Player player)
    {
        player.CurrentActionPoint += 2;
        Destroy(this.gameObject);
    }
}
