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
        grid = SelectMode.Instance.selectedGridDic[cardSetting.cardId][0];
        SelectMode.Instance.selectedGridDic[cardSetting.cardId].RemoveAt(0);
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
