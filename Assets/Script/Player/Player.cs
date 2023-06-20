using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PlayerId
{
    RedPlayer,
    BluePlayer,
    None,
    Both,
}

public class Player : Character
{
    public PlayerId Id;

    public int CurrentActionPoint;
    public int ActionPointPerRound;
    public int MaxActionPoint;
    public int MaxCardCount;

    public GridObject currentGrid;

    public Dictionary<AcademyType, List<GridObject>> OwnedLandDic = new Dictionary<AcademyType, List<GridObject>>()
/*    {
        { AcademyType.Null,null },
        { AcademyType.YI,null },
        { AcademyType.DAO,null },
        { AcademyType.MO,null },
        { AcademyType.BING,null },
        { AcademyType.RU,null },
    }*/;

    [System.Serializable]
    public class OwnedLandTest
    {
        public AcademyType TestTacademy;
        public List<GridObject> Testlist;
    }

   // [SerializeField]
   // private OwnedLandTest ownedLandTest;
    void Start()
    {
        MaxHP = 3;
        HP = MaxHP;
        AttackDamage = 1;
        Defence = 0;
        Range = 1;
        ActionPointPerRound = 3;

        List<GridObject> yiLand;
        OwnedLandDic.TryGetValue(AcademyType.YI, out yiLand);
        //Debug.Log(yiLand.Count);
    }
    void Update()
    {

        MaxActionPoint = ActionPointPerRound * 2;
        MaxCardCount = HP;
        if (AttackTarget != null)
        {
            Attack();
        }
        if (HP > MaxHP)
        {
            HP = MaxHP;
        }

    }

    public bool OccupyGrid(GridObject gridObject)
    {
        List<GridObject> gridList;
        if(OwnedLandDic.TryGetValue(gridObject.academy,out gridList))
        {
            gridList.Add(gridObject);
            OwnedLandDic[gridObject.academy] = gridList;
        }
        else
        {
            List<GridObject> gridListNew = new List<GridObject>();
            gridListNew.Add(gridObject);
            OwnedLandDic.Add(gridObject.academy, gridListNew);
        }

        return true;
    }
}
