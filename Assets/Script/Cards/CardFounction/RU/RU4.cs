using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RU4 : CardFunction
{
    void Start()
    {
        Function();
    }
    void Function()
    {
        Player enemy = GameplayManager.Instance.PlayerIdToPlayer(GameplayManager.Instance.GetEnemy(player.Id));
        enemy.canAttack = false;
        Destroy(gameObject);
    }
}
