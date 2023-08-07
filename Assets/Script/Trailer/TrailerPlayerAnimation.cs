using Cinemachine;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TrailerPlayerAnimation : MonoBehaviour
{
    [SerializeField]
    private Animator m_Animator;
    void Start()
    {
        //PalyAttackLoop();
    }

    
    void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            m_Animator.SetTrigger("Idle");
            
            //Debug.Log(1111);
        }
        //m_Animator.SetTrigger("Attack");
        //Debug.Log(111);
    }

    private void PalyAttackLoop()
    {
        var seq = DOTween.Sequence();
        seq.SetLoops(5);
        seq.AppendInterval(3f);
        seq.OnStepComplete(() => { m_Animator.SetTrigger("Attack"); });
    }
}
