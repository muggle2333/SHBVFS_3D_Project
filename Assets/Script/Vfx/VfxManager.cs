using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using static UnityEditor.PlayerSettings;
public enum VfxType
{
    HpAdd,
    HpDeduce,
    Defence,
    Damage,
    Range,
    AP,
    AcademyAdd,
    AcademyDeduce,
}
public class VfxManager : MonoBehaviour
{
    public static VfxManager Instance { get; private set; }

    private GameObject vfxPlayerPoint;
    [SerializeField] public List<GameObject> vfxBuffList;

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

    public void PlayBuildingVfx(Vector3 pos,bool isCreate)
    {
        StartCoroutine(PlayBuildVfx(pos,isCreate));
    }
    IEnumerator PlayBuildVfx(Vector3 pos, bool isCreate)
    {
        var name = "";
        if(isCreate)
        {
            name = "Vfx_Building_Create";
        }else
        {
            name = "Vfx_Building_Destroy";
        }
        GameObject vfxBuilding = Pool.Instance.GetObj(name);
        vfxBuilding.transform.position = pos;
        yield return new WaitForSeconds(1.5f);
        if(vfxBuffList!=null)
        {
            Pool.Instance.SetObj(name, vfxBuilding);
        }
    }
    public void PlayVfx(Vector3 pos, VfxType vfxType)
    {
        StartCoroutine(PlaySpecificVfx(pos, vfxType));
    }
    IEnumerator PlaySpecificVfx(Vector3 pos, VfxType vfxType)
    {
        var name = vfxBuffList[(int)vfxType].name;
        GameObject vfxTmp = Pool.Instance.GetObj(name);
        vfxTmp.transform.position = pos;
        yield return new WaitForSeconds(1.5f);
        if (vfxTmp != null)
        {
            Pool.Instance.SetObj(name, vfxTmp);
        }
        
    }
}
