using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class VfxManager : MonoBehaviour
{
    public static VfxManager Instance { get; private set; }

    private GameObject vfxPlayerPoint;

    public void Awake()
    {
        Instance= this;
    }

    public void PlayAttackVfx(Transform start, Transform target)
    {
        GameObject vfxAttackStart = Pool.Instance.GetObj("Vfx_Player_Attack_Start");
        vfxAttackStart.transform.position = start.transform.position;
        GameObject vfxAttackTarget = Pool.Instance.GetObj("Vfx_Player_Attack_Target");
        vfxAttackTarget.transform.position = target.transform.position;
    }

    public void SetCurrentOperatePlayer(Player player,bool isShow)
    {
        if(isShow)
        {
            vfxPlayerPoint = Pool.Instance.GetObj("Vfx_Player_Point");
            vfxPlayerPoint.transform.SetParent(player.transform);
        }
        else
        {
            Pool.Instance.SetObj("Vfx_Player_Point", vfxPlayerPoint); 
            
        }
    }

    public void PlayBuildingVfx(Vector3 pos)
    {
        GameObject vfxBuilding = Pool.Instance.GetObj("Vfx_Build");
        vfxBuilding.transform.position = pos;
        Invoke("DestoryBuildingVfx", 1.5f);
    }

    private void DestoryBuildingVfx(GameObject buildingVfx)
    {
        Pool.Instance.SetObj("Vfx_Build", buildingVfx);
    }
}
