using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MO5 : CardSecondaryChoose
{
    

    // Start is called before the first frame update
    void Start()
    {
        MO5 a = new MO5();
        a.ChooosedGrid.SetBuilding(true,false);
        GridVfxManager.Instance.UpdateVfxBuilding(ChooosedGrid, false);
        Destroy(gameObject);
    }

   
}
