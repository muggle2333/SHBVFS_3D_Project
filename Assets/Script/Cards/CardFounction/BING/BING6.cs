using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BING6 : CardFunction
{
    public int playedCardCount;
    public int costCount;
    private int distance;
    void Start()
    {
        playedCardCount = player.playedCardCount;
        costCount = 2;
        Function();
    }
    void Function()
    {
        Player enemy = GameplayManager.Instance.PlayerIdToPlayer(GameplayManager.Instance.GetEnemy(player.Id));
        distance = PlayerManager.Instance.CheckDistance(player, enemy.currentGrid);
        if(player.Range >= distance)
        {
            while(playedCardCount>= costCount)
            {
                player.AttackTarget = enemy;
                player.Attack();
                playedCardCount -= costCount;
                costCount++;
            }
        }
        player.hasAttcaked = false;
        Destroy(gameObject);
    }
}
