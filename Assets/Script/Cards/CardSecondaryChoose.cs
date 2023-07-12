using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardSecondaryChoose : MonoBehaviour
{


    [SerializeField]
    private GameObject SecondaryChooseUI;
    protected GridObject ChooosedGrid;
    public bool isUsed;

    // Update is called once per frame
    private void Start()
    {
        SecondaryChooseUI = GameObject.FindObjectOfType<CardSecondaryChooseUI>().gameObject;
    }
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
