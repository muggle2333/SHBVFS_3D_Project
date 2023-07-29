using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MO5 : CardFunction
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
        GridManager.Instance.SetBuilding(grid, false);
    }
}
