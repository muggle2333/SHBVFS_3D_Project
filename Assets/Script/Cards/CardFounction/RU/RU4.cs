using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RU4 : CardFunction
{
    Player enemy;
    void Start()
    {
        enemy = GameplayManager.Instance.PlayerIdToPlayer(GameplayManager.Instance.GetEnemy(player.Id));
        Function();
    }
    private void Update()
    {
        if(TurnbasedSystem.Instance.CurrentGameStage.Value == GameStage.S4)
        {
            enemy.canAttack = true;
            Destroy(gameObject);
        }
    }
    void Function()
    {
        enemy.canAttack = false;
       
    }
}
