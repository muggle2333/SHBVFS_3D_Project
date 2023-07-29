using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BING1 : CardFunction
{
    // Start is called before the first frame update
    void Start()
    {
        Function();
    }

    // Update is called once per frame
    void Function()
    {
        for(int i = 0; i < 2; i++)
        {
            if(GameplayManager.Instance.playerList[i].Defence == 0)
            {
                GameplayManager.Instance.playerList[i].HP --;
            }
        }
        Destroy(gameObject);
    }
}
