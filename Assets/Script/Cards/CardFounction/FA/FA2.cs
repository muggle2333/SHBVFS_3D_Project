using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class FA2 : CardFunction
{
    public Vector2Int grid;
    void Start()
    {
        if (player.Id == PlayerId.RedPlayer)
        {
            grid = SelectMode.Instance.redPlayerSelectedGridDic[cardSetting.cardId][0];
            SelectMode.Instance.redPlayerSelectedGridDic[cardSetting.cardId].RemoveAt(0);
        }
        else
        {
            grid = SelectMode.Instance.bluePlayerSelectedGridDic[cardSetting.cardId][0];
            SelectMode.Instance.bluePlayerSelectedGridDic[cardSetting.cardId].RemoveAt(0);
        }
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
