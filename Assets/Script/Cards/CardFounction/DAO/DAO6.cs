using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class DAO6 : CardFunction
{
    public Vector2Int grid1;
    public Vector2Int grid2;
    void Start()
    {
        if (player.Id == PlayerId.RedPlayer)
        {
            grid1 = SelectMode.Instance.redPlayerSelectedGridDic[cardSetting.cardId][0];
            grid2 = SelectMode.Instance.redPlayerSelectedGridDic[cardSetting.cardId][1];
            SelectMode.Instance.redPlayerSelectedGridDic[cardSetting.cardId].RemoveAt(0);
            SelectMode.Instance.redPlayerSelectedGridDic[cardSetting.cardId].RemoveAt(0);
        }
        else
        {
            grid1 = SelectMode.Instance.bluePlayerSelectedGridDic[cardSetting.cardId][0];
            grid2 = SelectMode.Instance.bluePlayerSelectedGridDic[cardSetting.cardId][1];
            SelectMode.Instance.bluePlayerSelectedGridDic[cardSetting.cardId].RemoveAt(0);
            SelectMode.Instance.bluePlayerSelectedGridDic[cardSetting.cardId].RemoveAt(0);
        }
        Function();
    }
    void Function()
    {
        if (NetworkManager.Singleton.IsServer)
        {
            Debug.LogError(grid1);
            Debug.LogError(grid2);

            GridManager.Instance.SwitchAcademyServerRpc(grid1, grid2);
        }
        Destroy(gameObject);
    }
}
