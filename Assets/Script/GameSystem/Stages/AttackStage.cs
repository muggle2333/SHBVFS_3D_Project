using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;

public class AttackStage : MonoBehaviour
{
    private List<Player> players;
    private int distance;

    public void StartStage()
    {
        StartCoroutine("StartAttack");
        #region Johnny
        //players = GameplayManager.Instance.GetPlayer();
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
        List<Player> playerList = GameplayManager.Instance.GetPlayer();
        //if (TurnbasedSystem.Instance.roundIndex.Value % 2 != 0)
        //{
        //    playerList = playerList.OrderBy(x => x.Priority).ToList();
        //}
        //else
        //{
            playerList = playerList.OrderByDescending(x => x.Priority).ToList();
        //}
        //Attack per Player
        for (int i = 0; i < playerList.Count; i++)
        {
            int minDistance = 10;
            int targetIndex = i;
            //Find target in min distance
            for (int j = 0; j < playerList.Count; j++)
            {
                if (j == i) continue;
                distance = PlayerManager.Instance.CheckDistance(playerList[i], playerList[j].currentGrid);
                //Debug.LogError(playerList[i] + " distance " + distance);
                if (distance < minDistance && playerList[i].Range >= distance)
                {
                    minDistance = distance;
                    targetIndex = j;
                    if (targetIndex != i)
                    {
                        if (FindObjectOfType<NetworkManager>() == null)
                        {
                            playerList[i].AttackTarget = playerList[targetIndex];
                            playerList[i].Attack();
                            SoundManager.Instance.PlaySound(Sound.Attack);
                            //Debug.LogError(playerList[i] + " attack " + playerList[targetIndex]);
                        }
                        else
                        {
                            PlayerManager.Instance.SetAttackClientRpc(playerList[i].Id, playerList[targetIndex].Id);
                        }
                    }
                }

            }
            yield return new WaitForSecondsRealtime(1f);
            //Check dying
            List<Player> dyingPlayers = GameplayManager.Instance.GetDyingPlayer();
            if (dyingPlayers.Count == 0)
            {
                TurnbasedSystem.Instance.isDie.Value = false;
            }
            else
            {
                TurnbasedSystem.Instance.isDie.Value = true;
                GameplayManager.Instance.PlayerDyingStageClientRpc();
                yield return new WaitForSeconds(GameplayManager.DYING_TIMER);

                if (dyingPlayers.Count > 0)
                {
                    GameManager.Instance.SetGameOver();
                }
                else
                {
                    GameplayManager.Instance.LeaveDyingStageClientRpc();
                }
            }
        }
        TurnbasedSystem.Instance.CompleteStage(GameStage.AttackStage);
    }

}
