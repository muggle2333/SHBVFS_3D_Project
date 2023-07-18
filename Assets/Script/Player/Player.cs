using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public enum PlayerId
{
    RedPlayer,
    BluePlayer,
}


public class Player : Character
{


    public PlayerId Id;
    public NetworkVariable<int> playerIdNetwork;

    public bool canCost1APInEnemy = false;
    public bool canFreeMoveInSelfGrid = false;

    public int CurrentActionPoint; //control阶段也会扣
    public int TrueActionPoint;
    public int ActionPointPerRound;
    public int MaxActionPoint;
    public int MaxCardCount;
    public int freeMoveCount;

    public int cardAR;
    public int cardAD;
    public int cardDF;

    public int baseMaxHP = 3;
    public int baseDefense = 0;
    public int baseAttackDamage = 1;
    public int baseRange = 1;
    public int baseActionPointPerRound = 3;

    public int Priority;
    public NetworkVariable<bool> isDying = new NetworkVariable<bool>(false);

    public List<int> handCards;

    public GridObject targetGrid;
    public GridObject currentGrid;
    public GridObject trueGrid;

    public Dictionary<AcademyType, List<GridObject>> OwnedLandDic = new Dictionary<AcademyType, List<GridObject>>()
/*    {
        { AcademyType.Null,null },
        { AcademyType.YI,null },
        { AcademyType.DAO,null },
        { AcademyType.MO,null },
        { AcademyType.BING,null },
        { AcademyType.RU,null },
    }*/;

    public int[] academyOwnedPoint = new int[6];
    [System.Serializable]
    public class OwnedLandTest
    {
        public AcademyType TestTacademy;
        public List<GridObject> Testlist;
    }

    // [SerializeField]
    // private OwnedLandTest ownedLandTest;

    public void Awake()
    {
        
    }
    void Start()
    {
        MaxHP = 3;
        HP = MaxHP;
        AttackDamage = 1;
        Defence = 0;
        Range = 1;
        ActionPointPerRound = 3;
        CurrentActionPoint = 0;
        TrueActionPoint = CurrentActionPoint;

        List<GridObject> yiLand;
        OwnedLandDic.TryGetValue(AcademyType.YI, out yiLand);
        //Debug.Log(yiLand.Count);
    }
    void Update()
    {

        MaxActionPoint = ActionPointPerRound * 2;
        MaxCardCount = HP;
        if(Defence < 0)
        {
            Defence = 0;
        }
    }

    public void UpdateDataPerTurn()
    {
        CurrentActionPoint = CurrentActionPoint+ ActionPointPerRound>MaxActionPoint? MaxActionPoint: CurrentActionPoint + ActionPointPerRound;
        TrueActionPoint = CurrentActionPoint;

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

    
    public int[] CountAcademyOwnedPoint()
    {
        int[] academyOwnedPoint = new int[6];
        //for (int i=0;i<6;i++)
        //{
        //    List<GridObject> gridList;
        //    OwnedLandDic.TryGetValue((AcademyType)(i+1), out gridList); //academy 第一个是 null
        //    academyOwnedPoint[i] = gridList.Count;
        //}
        var grid = GridManager.Instance.grid;
        for(int x = 0;x< grid.width;x++)
        {
            for(int z = 0;z<grid.length;z++)
            {
                if (grid.gridArray[x, z].owner == this && grid.gridArray[x, z].academy != AcademyType.Null)
                {
                    int academyIndex = (int)grid.gridArray[x, z].academy;
                    academyOwnedPoint[academyIndex-1]++;
                }
            }
        }
        return academyOwnedPoint;
    }


    public void ChangeAcademyOwnPoint(int[] academyPointEffect)
    {
        for(int i=0;i< academyOwnedPoint.Length;i++)
        {
            academyOwnedPoint[i] += academyPointEffect[i];
        }
    }

    public void UpdatePlayerDate(int[] academyPointEffect, PlayerDataEffect playerDataEffect)
    {
        ChangeAcademyOwnPoint(academyPointEffect);
    }

    public bool UseActionPoint(int apCost)
    {
        if (CurrentActionPoint < apCost)
        {
            return false;
        }
        else
        {
            CurrentActionPoint -= apCost;
            return true;
        }
    }

    public bool IsApEnough(int apCost)
    {
        return CurrentActionPoint >= apCost;
    }
    //public void RefreshLinePath()
    //{
    //    LineRenderer lineRenderer= GetComponentInChildren<LineRenderer>();
    //    lineRenderer.transform.rotation = Quaternion.LookRotation(new Vector3(0, -0.5f, 0), lineRenderer.transform.up);
    //    lineRenderer.positionCount= 1;
    //    lineRenderer.SetPosition(0,transform.position+new Vector3(0,0.1f,0));
    //}
    //public void UpdateLinePath(LandType landType)
    //{
    //    LineRenderer lineRenderer = GetComponentInChildren<LineRenderer>();
    //    lineRenderer.positionCount += 1;
    //    Vector3 offset = new Vector3(0,0.1f,0);
    //    if(landType==LandType.Mountain)
    //    {
    //        offset = new Vector3(0, 1.7f, 0);
    //    }
    //    lineRenderer.SetPosition(lineRenderer.positionCount -1, transform.position+offset);
    //}

    
}
