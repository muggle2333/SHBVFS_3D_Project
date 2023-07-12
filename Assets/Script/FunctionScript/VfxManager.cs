using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VfxManager : MonoBehaviour
{
    public static VfxManager Instance { get; private set; }

    public void Awake()
    {
        Instance= this;
    }

    public void PlayAttackVfx(Transform start, Transform target)
    {

    }
}
