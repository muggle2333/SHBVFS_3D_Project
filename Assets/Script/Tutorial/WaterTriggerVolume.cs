using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterTriggerVolume : MonoBehaviour
{
    public bool isenterred=false;

    private void OnCollisionEnter(Collision collision)
    {
        var tutorialManager = FindObjectOfType<TutorialManager>();
        if (collision.gameObject == GetComponent<PlayerInteractionComponent>())
        {
            isenterred = true;
        }
    }
}
