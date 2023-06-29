using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MoveStage : MonoBehaviour
{
    private Dictionary<Player, List<PlayerInteract>> playerInteractDict = new Dictionary<Player, List<PlayerInteract>>();

    private List<Player> playerList = new List<Player>();

    public void StartMoveStage(Dictionary<Player, List<PlayerInteract>> playerInteractDict)
    { 
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
        List<Player> priorityList = playerList.OrderBy(x => x.Priority).ToList();

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

                    yield return new WaitForSeconds(1);
                    PlayerManager.Instance.Interact(priorityList[i], playerInteract[0]);
                    

                    playerInteract.RemoveAt(0);
                    playerInteractDict[priorityList[i]] = playerInteract;
                }
            }
        }
        TurnbasedSystem.Instance.TurnToNextStage();
    }
}
