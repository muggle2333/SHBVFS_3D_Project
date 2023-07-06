using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class YI5 : CardFounction
{
    private bool HasTakeEffect=false;
    void Update()
    {
        if(TurnbasedSystem.Instance.CurrentGameStage == GameStage.S4 && HasTakeEffect == false)
        {
            HasTakeEffect = true;
            Founction(GameplayManager.Instance.currentPlayer);
        }
    }
    public void Founction(Player player)
    {
        player.CurrentActionPoint += 2;
        Destroy(this.gameObject);
    }
}
