using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RU6 : CardFunction
{
    void Start()
    {
        Function();
    }
    void Function()
    {
        Player enemy = GameplayManager.Instance.PlayerIdToPlayer(GameplayManager.Instance.GetEnemy(player.Id));
        if (enemy.baseDefense > 0)
        {
            enemy.baseDefense -= 1;
            player.baseDefense += 1;
            Calculating.Instance.CalculatPlayerBaseData(enemy);
            Calculating.Instance.CalculatPlayerBaseData(player);
        }
        Destroy(gameObject);
    }
}
