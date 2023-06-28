using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAcademyBuffcomponent : MonoBehaviour
{

    public Dictionary<AcademyType, int> AcademyBuff = new Dictionary<AcademyType, int>();
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (this.GetComponent<Player>().academyOwnedPoint[0] == 1)
        {

        }
    }
}
