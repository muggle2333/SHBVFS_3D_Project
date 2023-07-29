using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;

public class TextColorControl : MonoBehaviour
{
    
    // Update is called once per frame
    void Update()
    {
        //ColorBlink(new Color32(234, 201, 121, 255), new Color32(231, 42, 0, 255), 0.4f);
    }

    public void ColorBlink(Color32 formerColor, Color32 targetColor, float duration)
    {
        var seq = DOTween.Sequence();
        seq.Append(DOTween.To(() => transform.GetComponent<Text>().color, x => transform.GetComponent<Text>().color = x, targetColor, duration / 2));
        seq.Append(DOTween.To(() => transform.GetComponent<Text>().color, x => transform.GetComponent<Text>().color = x, formerColor, duration / 2));
    }
}
