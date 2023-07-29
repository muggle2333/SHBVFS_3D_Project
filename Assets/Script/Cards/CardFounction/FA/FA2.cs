using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class FA2 : CardFunction
{
    public Vector2Int grid;
    void Start()
    {
        grid = SelectMode.Instance.selectedGridDic[cardSetting.cardId][0];
        SelectMode.Instance.selectedGridDic[cardSetting.cardId].RemoveAt(0);
        Function();
    }
    void Function()
    {
        if (NetworkManager.Singleton.IsServer)
        {
            GridManager.Instance.ChangeAcademyServerRpc(grid, AcademyType.FA);
        }
        Destroy(gameObject);
    }
}
