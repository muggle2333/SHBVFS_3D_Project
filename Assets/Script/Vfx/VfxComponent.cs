using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VfxComponent : MonoBehaviour
{
    
    void Start()
    {
        
    }

    private void OnEnable()
    {
        Invoke("DestoryVfx", 1f);
    }

    private void DestroyVfx()
    {
        Pool.Instance.SetObj(gameObject.name, this.gameObject);
    }
}
