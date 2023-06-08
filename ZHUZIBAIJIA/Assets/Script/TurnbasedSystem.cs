using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnbasedSystem : MonoBehaviour
{
    public int RoundNumber = 1;
    public int round;
    public bool IsPlayerDead=true;
    public bool IsControlOvered = true;


    // Start is called before the first frame update
    void Start()
    {
       
    }

    // Update is called once per frame
    void Update()
    {
        while (!IsPlayerDead)
        {
            while(!IsControlOvered)
            {

            }
            RoundNumber++;
            
        }
    }
}
