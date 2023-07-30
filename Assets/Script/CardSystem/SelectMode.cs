using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.Netcode;
using UnityEngine;

public class SelectMode : NetworkBehaviour
{
    public static SelectMode Instance;
    public Dictionary<int, List<Vector2Int>> redPlayerSelectedGridDic = new Dictionary<int, List<Vector2Int>>();
    public Dictionary<int, List<Vector2Int>> bluePlayerSelectedGridDic = new Dictionary<int, List<Vector2Int>>();

    private void Awake()
    {
        
        if (Instance != null && Instance != this)
        {
            Destroy(Instance);
        }
        else
        {
            Instance = this;
        }
    }
    private void Start()
    {
        redPlayerSelectedGridDic.Clear();
        bluePlayerSelectedGridDic.Clear();
    }
    [ServerRpc(RequireOwnership = false)]
    public void saveSelectedGridServerRpc(PlayerId playerId,int cardId,int needSelectCount, Vector2Int grid1, Vector2Int grid2)
    {
        saveSelectedGridClientRpc(playerId, cardId, needSelectCount,grid1,grid2);
    }
    [ClientRpc]
    public void saveSelectedGridClientRpc(PlayerId playerId, int cardId, int needSelectCount,Vector2Int grid1, Vector2Int grid2)
    {
        if (playerId == PlayerId.RedPlayer)
        {
            if (!redPlayerSelectedGridDic.ContainsKey(cardId))
            {
                redPlayerSelectedGridDic.Add(cardId, new List<Vector2Int>());
                if(needSelectCount == 1)
                {
                    redPlayerSelectedGridDic[cardId].Add(grid1);
                }
                else if(needSelectCount == 2)
                {
                    redPlayerSelectedGridDic[cardId].Add(grid1);
                    redPlayerSelectedGridDic[cardId].Add(grid2);
                }
            }
            else
            {
                if (needSelectCount == 1)
                {
                    redPlayerSelectedGridDic[cardId].Add(grid1);
                }
                else if (needSelectCount == 2)
                {
                    redPlayerSelectedGridDic[cardId].Add(grid1);
                    redPlayerSelectedGridDic[cardId].Add(grid2);
                }
            }
        }
        else
        {
            if (!bluePlayerSelectedGridDic.ContainsKey(cardId))
            {
                bluePlayerSelectedGridDic.Add(cardId, new List<Vector2Int>());
                if (needSelectCount == 1)
                {
                    bluePlayerSelectedGridDic[cardId].Add(grid1);
                }
                else if (needSelectCount == 2)
                {
                    bluePlayerSelectedGridDic[cardId].Add(grid1);
                    bluePlayerSelectedGridDic[cardId].Add(grid2);
                }
            }
            else
            {
                if (needSelectCount == 1)
                {
                    bluePlayerSelectedGridDic[cardId].Add(grid1);
                }
                else if (needSelectCount == 2)
                {
                    bluePlayerSelectedGridDic[cardId].Add(grid1);
                    bluePlayerSelectedGridDic[cardId].Add(grid2);
                }
            }
        }
    }

}
