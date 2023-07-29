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
        //ColorBlink(new Color32(231, 42, 0, 255), 0.4f);
    }

    public void ColorBlink(Color32 targetcolor, float duration)
    {
        Color32 formercolor = transform.GetComponent<Text>().color;
        var seq = DOTween.Sequence();
        seq.Append(DOTween.To(() => transform.GetComponent<Text>().color, x => transform.GetComponent<Text>().color = x, targetcolor, duration / 2));
        seq.Append(DOTween.To(() => transform.GetComponent<Text>().color, x => transform.GetComponent<Text>().color = x, formercolor, duration / 2));
    }
}
