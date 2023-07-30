using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FA3 : CardFunction
{
    public Vector2Int grid;
    // Start is called before the first frame update
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
        if(player.startGrid == player.currentGrid)
        {
            PlayerManager.Instance.MovePlayerNoAP(player, GridManager.Instance.GetGridObject(grid));
        }
        Destroy(gameObject);
    }
}
