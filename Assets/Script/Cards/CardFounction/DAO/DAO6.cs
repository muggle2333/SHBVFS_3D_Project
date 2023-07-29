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
        grid1 = SelectMode.Instance.selectedGridDic[cardSetting.cardId][0];
        SelectMode.Instance.selectedGridDic[cardSetting.cardId].RemoveAt(0);
        grid2 = SelectMode.Instance.selectedGridDic[cardSetting.cardId][0];
        SelectMode.Instance.selectedGridDic[cardSetting.cardId].RemoveAt(0);
        Function();
    }
    void Function()
    {
        if (NetworkManager.Singleton.IsServer)
        {
            GridManager.Instance.SwitchAcademyServerRpc(grid1, grid2);
        }
        Destroy(gameObject);
    }
}
