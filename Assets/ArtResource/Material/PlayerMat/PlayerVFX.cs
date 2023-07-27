using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerVFX : MonoBehaviour
{

    private float hitVfxFloat;
    private bool isPlayingVfx = false;
    public bool isPlayerHitted;
    private GameObject HurtVfx;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(isPlayerHitted)
        {
            StartCoroutine("PlayHitVfx");
        }
    }

    IEnumerator PlayHitVfx()
    {
        DOTween.To(() => this.hitVfxFloat, x => this.hitVfxFloat = x, 1, 1f);
        yield return new WaitForSeconds(1f);
        DOTween.To(() => this.hitVfxFloat, x => this.hitVfxFloat = x, -1, 1f);
        yield return new WaitForSeconds(1f);
        isPlayingVfx = false;
        yield return null;
    }
}
