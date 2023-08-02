using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RU3 : CardFunction
{
    // Start is called before the first frame update
    void Start()
    {
        Function();
    }
    void Function()
    {
        Player enemy = GameplayManager.Instance.PlayerIdToPlayer(GameplayManager.Instance.GetEnemy(player.Id));
        enemy.HP -= enemy.damageThisRound;
        Destroy(gameObject);
    }
}
