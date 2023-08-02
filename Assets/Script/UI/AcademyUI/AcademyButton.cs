using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class AcademyButton : MonoBehaviour
{
    public GameObject academyBuffs;
    public bool isOpen;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void AcademuBuffBton()
    {
        if (isOpen == false)
        {
            var seq = DOTween.Sequence();
            seq.Append(academyBuffs.transform.DOLocalMoveX(-146, 0.3f));
            isOpen = true;
        }
        else
        {
            var seq = DOTween.Sequence();
            seq.Append(academyBuffs.transform.DOLocalMoveX(133, 0.3f));
            isOpen = false;
        }
    }
}
