using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class BING3 : CardFunction
{
    public bool hasAttacked = true;
    // Start is called before the first frame update
    void Start()
    {
        Function();
    }

    // Update is called once per frame
    void Update()
    {
        if(TurnbasedSystem.Instance.CurrentGameStage.Value == GameStage.S1 && hasAttacked == false)
        {
            hasAttacked = true;
            if (NetworkManager.Singleton.IsServer)
            {
                player.cardAcademyEffectNum[3] += 2;
                player.ChangeVfx(VfxType.AcademyAdd, 2);
                FindObjectOfType<PlayerAcademyBuffcomponent>().UpdatePlayerAcademyBuffServerRpc(player.Id);
            }
            Destroy(gameObject);
        }
      
    }
    void Function()
    {
        if(player.hasAttcaked == false)
        {
            hasAttacked = false;
        }
    }
}
