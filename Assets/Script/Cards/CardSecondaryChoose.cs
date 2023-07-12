using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardSecondaryChoose : MonoBehaviour
{


    [SerializeField]
    private GameObject SecondaryChooseUI;
    protected GridObject ChooosedGrid;


    // Update is called once per frame
    void Update()
    {
        
        ChooosedGrid = GetComponent<GridObjectComponent>().gridObject;
        if (GetComponent<CardSelectComponent>().isSelected)
        {
         SecondaryChooseUI.SetActive(true);
        }
        else
        {
         SecondaryChooseUI.SetActive(false);
        }

    }

  
}
