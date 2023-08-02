using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterTriggerVolume : MonoBehaviour
{

    private void OnTriggerStay(Collider collision)
    {

        Debug.Log("enterred");
        var tutorialManager = FindObjectOfType<TutorialManager>();
        if (collision.gameObject.GetComponent<PlayerInteractionComponent>()!=null)
        {
            Debug.Log("enterred");
            tutorialManager.waterTrigger = true;
        }
    }
}
