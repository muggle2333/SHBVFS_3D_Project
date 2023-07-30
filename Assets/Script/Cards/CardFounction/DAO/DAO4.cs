using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;

public class DAO4 : CardFunction
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

    // Update is called once per frame
    void Update()
    {
        
    }
    void Function()
    {
        if (NetworkManager.Singleton.IsServer)
        {
            GridManager.Instance.ChangeAcademyServerRpc(grid, AcademyType.DAO);
        }
        Destroy(gameObject);
    }
}
