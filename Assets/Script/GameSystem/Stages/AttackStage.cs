using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackStage : MonoBehaviour
{
    private List<Player> players;
    private int distance;

    public void StartAttack()
    {
        players = GameplayManager.Instance.GetPlayer();
        Debug.Log(players.Count);
        distance = PlayerManager.Instance.CheckDistance(players[0], players[1].currentGrid);
        if (players[0].Range >= distance && players[1].Range >= distance)
        {
            if (players[0].Priority > players[1].Priority)
            {
                players[0].AttackTarget = players[1];
                players[0].Attack();
                players[1].AttackTarget = players[0];
                players[1].Attack();
            }
            else
            {
                players[1].AttackTarget = players[0];
                players[1].Attack();
                players[0].AttackTarget = players[1];
                players[0].Attack();
            }
        }
        else if(players[0].Range >= distance && players[1].Range < distance)
        {
            players[0].AttackTarget = players[1];
            players[0].Attack();
        }
        else if (players[0].Range < distance && players[1].Range >= distance)
        {
            players[1].AttackTarget = players[0];
            players[1].Attack();
        }
    }
}
