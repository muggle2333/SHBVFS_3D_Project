using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tutorial2 : MonoBehaviour
{
    private int landlake = 0;
    private int landforest = 0;
    private int landmountain = 0;
    [SerializeField] private TutorialUI tutorialUI;

    private GridObject selectTmp = null;
    private GridObject selectCurrent = null;
    // Update is called once per frame
    void Update()
    {
        selectTmp = SelectManager.Instance.GetLatestGridObject();
        if(selectCurrent!=selectTmp)
        {
            selectCurrent = selectTmp;
            ClickGrid(selectCurrent);
        }
    }

    public void ClickGrid(GridObject gridObject)
    {
        if (gridObject == null)
        {
            tutorialUI.HideTutorial();
            return;
        }
        switch (gridObject.landType)
        {
            case LandType.Plain:
                tutorialUI.HideTutorial();
                break;
            case LandType.Mountain:
                landmountain++; 
                if(landmountain==1)
                {
                    tutorialUI.ShowMessageText("Climbing mountain costs 2 action points, and player gets 1 more range on the mountain. Also it only takes 1 action point to walk down the mountain.");
                }
                else
                {
                    tutorialUI.HideTutorial();
                }
                break;
            case LandType.Lake:
                landlake++; 
                if(landlake==1)
                {
                    tutorialUI.ShowMessageText("Stepping into water costs 2 action points, but moving in water costs no action points. Also it only takes 1 action point to step out of water.");
                }
                else
                {
                    tutorialUI.HideTutorial();
                }
                break;
            case LandType.Forest:
                landforest++;
                if(landforest==1)
                {
                    tutorialUI.ShowMessageText("Player in the forest get 1 more defense.");
                }
                else
                {
                    tutorialUI.HideTutorial();
                }
                break;
        }
    }
}
