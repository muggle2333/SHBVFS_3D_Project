using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

public class MoveStage : NetworkBehaviour
{
    private Dictionary<Player, List<PlayerInteract>> playerInteractDict = new Dictionary<Player, List<PlayerInteract>>();

    private List<Player> playerList = new List<Player>();

    public void StartStage(Dictionary<Player, List<PlayerInteract>> playerInteractDict)
    {
        if (FindObjectOfType<NetworkManager>()!=null && !NetworkManager.Singleton.IsHost) return;
        this.playerInteractDict = playerInteractDict;
        playerList = new List<Player>();
        StartCoroutine("StartMove");
    }

    IEnumerator StartMove()
    {
        for(int i = 0;i< playerInteractDict.Count;i++)
        {
            playerList.Add(playerInteractDict.ElementAt(i).Key);
        }
        List<Player> priorityList = null;

        priorityList = playerList.OrderByDescending(x => x.Priority).ToList();
        if(priorityList.Count >0 )
        {
            string name = priorityList[0].Id == GameplayManager.Instance.currentPlayer.Id ? "YOU Act first" : "ENEMY Acts first";
            UIManager.Instance.ShowWarningTimerClientRpc(name, 1f);
        }
        else
        {
            UIManager.Instance.ShowWarningTimerClientRpc("Nobody Acts", 1f);
        }

        while(playerInteractDict.Count!=0)
        {
            for(int i = 0;i< priorityList.Count;i++) 
            {
                List<PlayerInteract> playerInteract = null;
                if (playerInteractDict.TryGetValue(priorityList[i], out playerInteract))
                {
                    if(playerInteract.Count==0)
                    {
                        playerInteractDict.Remove(priorityList[i]);
                        break;
                    }
                    //Interact 
                    //Debug.Log(priorityList[i].name + " " + playerInteract[0].PlayerInteractType);

                    yield return new WaitForSeconds(1.5f);


                    if(FindObjectOfType<NetworkManager>())
                    {
                        //针对特定的client执行操作
                        /*PlayerManager.Instance.InteractClientRpc(priorityList[i].Id, playerInteract[0], new ClientRpcParams
                        {
                            Send = new ClientRpcSendParams
                            {
                                TargetClientIds = new ulong[] { (ulong)priorityList[i].Id }
                            }
                        });*/

                        PlayerManager.Instance.InteractClientRpc(priorityList[i].Id, playerInteract[0]);
                    }
                    else
                    {
                        PlayerManager.Instance.Interact(priorityList[i], playerInteract[0]);
                    }

                    playerInteract.RemoveAt(0);
                    playerInteractDict[priorityList[i]] = playerInteract;
                }
            }
        }
        FindObjectOfType<PlayerAcademyBuffcomponent>().UpdatePlayerAcademyBuffServerRpc(PlayerId.BluePlayer);
        FindObjectOfType<PlayerAcademyBuffcomponent>().UpdatePlayerAcademyBuffServerRpc(PlayerId.RedPlayer);
        EndStage();
    }

    public void EndStage()
    {
        GridManager.Instance.RefreshGridVfxClientRpc();
        TurnbasedSystem.Instance.CompleteStage(GameStage.MoveStage);
    }
}
