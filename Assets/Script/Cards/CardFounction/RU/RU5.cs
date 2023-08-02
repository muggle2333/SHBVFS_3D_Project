using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RU5 : CardFunction
{
    void Start()
    {
        Function();
    }
    void Function()
    {
        Player enemy = GameplayManager.Instance.PlayerIdToPlayer(GameplayManager.Instance.GetEnemy(player.Id));
        player.baseDefense -= 2;
        player.cardDF -= 2;
        enemy.HP -= 2;
        Calculating.Instance.CalculatPlayerBaseData(player);
        Destroy(gameObject);
    }
}
