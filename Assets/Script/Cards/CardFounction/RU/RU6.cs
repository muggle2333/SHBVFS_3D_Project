using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RU6 : CardFunction
{
    int defence;
    Player enemy;
    void Start()
    {
        enemy = GameplayManager.Instance.PlayerIdToPlayer(GameplayManager.Instance.GetEnemy(player.Id));
        defence = enemy.Defence;
        Function();
    }
    private void Update()
    {
        if(TurnbasedSystem.Instance.CurrentGameStage.Value == GameStage.S4)
        {
            enemy.baseDefense += defence;
            enemy.cardDF += defence;
            player.baseDefense -= defence;
            player.cardDF -= defence;
            Calculating.Instance.CalculatPlayerBaseData(enemy);
            Calculating.Instance.CalculatPlayerBaseData(player);
            Destroy(gameObject);
        }
    }
    void Function()
    {
        if (enemy.baseDefense > 0)
        {
            enemy.baseDefense -= defence;
            enemy.cardDF -= defence;
            player.baseDefense += defence;
            player.cardDF += defence;
            Calculating.Instance.CalculatPlayerBaseData(enemy);
            Calculating.Instance.CalculatPlayerBaseData(player);
        }
    }
}
