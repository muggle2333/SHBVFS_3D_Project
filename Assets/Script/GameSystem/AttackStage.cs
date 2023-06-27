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
        distance = PlayerManager.Instance.CheckDistance(players[0], players[1].currentGrid);
        if (players[0].Range >= distance && players[1].Range >= distance)
        {
            if (players[0].Priority > players[1].Priority)
            {
                //players[0].AttackTarget
            }
            else
            {

            }
        }
        else if(players[0].Range >= distance && players[1].Range < distance)
        {

        }
        else if (players[0].Range < distance && players[1].Range >= distance)
        {

        }
    }
}
