using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AttackStage : MonoBehaviour
{
    private List<Player> players;
    private int distance;

    public void StartStage()
    {
        players = GameplayManager.Instance.GetPlayer();
        StartCoroutine("StartAttack");
        #region Johnny
        //Debug.Log(players.Count);
        //distance = PlayerManager.Instance.CheckDistance(players[0], players[1].currentGrid);
        //if (players[0].Range >= distance && players[1].Range >= distance)
        //{
        //    if (players[0].Priority > players[1].Priority)
        //    {
        //        players[0].AttackTarget = players[1];
        //        players[0].Attack();
        //        players[1].AttackTarget = players[0];
        //        players[1].Attack();
        //    }
        //    else
        //    {
        //        players[1].AttackTarget = players[0];
        //        players[1].Attack();
        //        players[0].AttackTarget = players[1];
        //        players[0].Attack();
        //    }
        //}
        //else if(players[0].Range >= distance && players[1].Range < distance)
        //{
        //    players[0].AttackTarget = players[1];
        //    players[0].Attack();
        //}
        //else if (players[0].Range < distance && players[1].Range >= distance)
        //{
        //    players[1].AttackTarget = players[0];
        //    players[1].Attack();
        //}
        #endregion
    }

    IEnumerator StartAttack()
    {
        List<Player> playerList = players.OrderByDescending(x => x.Priority).ToList();
        for (int i = 0; i < playerList.Count; i++)
        {
            int minDistance = 10;
            int targetIndex = i;
            //Find target in min distance
            for (int j = 0; j < playerList.Count; j++)
            {
                if (j == i) continue;
                distance = PlayerManager.Instance.CheckDistance(playerList[i], playerList[j].currentGrid);
                if (distance < minDistance && playerList[i].Range >= distance)
                {
                    minDistance = distance;
                    targetIndex = j;
                }
            }
            if (targetIndex != i)
            {
                playerList[i].AttackTarget = playerList[targetIndex];
                playerList[i].Attack();
                Debug.Log(playerList[i] + " attack " + playerList[targetIndex]);
                yield return new WaitForSecondsRealtime(1f);
            }
        }
        List<Player> dyingPlayers = new List<Player>();
        List<Player> alivePlayers = new List<Player>();
        for (int i = 0; i < players.Count; i++)
        {
            if (players[i].HP <= 0)
            {
                dyingPlayers.Add(players[i]);
            }
            else
            {
                alivePlayers.Add(players[i]);
            }
        }
        if (dyingPlayers.Count == 0)
        {
            TurnbasedSystem.Instance.isDie.Value = false;
        }
        else
        {
            TurnbasedSystem.Instance.isDie.Value = true;
            PlayerManager.Instance.PlayerDying(dyingPlayers, alivePlayers);
        }
        TurnbasedSystem.Instance.CompleteStage(GameStage.AttackStage);
    }
}
