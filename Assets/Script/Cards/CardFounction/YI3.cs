using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class YI3 : CardFunction
{
    // Start is called before the first frame update
    void Start()
    {
        Function();
    }
    public void Function()
    {
        var enemy = GameplayManager.Instance.PlayerIdToPlayer(GameplayManager.Instance.GetEnemy(player.Id));
        enemy.HP -= player.occuplyCount/2;
    }
}
