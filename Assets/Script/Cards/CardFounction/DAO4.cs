using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DAO4 : CardSecondaryChoose
{
    // Start is called before the first frame update
    void Start()
    {
        DAO4 b = new DAO4();
        b.ChooosedGrid.academy = AcademyType.DAO;
        if(b.ChooosedGrid.academy == AcademyType.DAO)
        {
            Destroy(this.gameObject);
        }
    }

    
}
