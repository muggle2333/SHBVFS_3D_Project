using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class YI6 : CardFunction
{
    // Start is called before the first frame update
    void Start()
    {
        Function();
    }

    // Update is called once per frame
    void Function()
    {
        for (int i = 0; i < 2; i++)
        {
            GameplayManager.Instance.playerList[i].HP++;
            if(GameplayManager.Instance.playerList[i].HP > GameplayManager.Instance.playerList[i].MaxHP)
            {
                GameplayManager.Instance.playerList[i].HP = GameplayManager.Instance.playerList[i].MaxHP;
            }
        }
        Destroy(gameObject);
    }
}
