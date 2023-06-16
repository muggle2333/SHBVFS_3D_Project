using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;
using UnityEngine.UI;

public class CardSelectTest : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private int index;
    public GameObject Info;

    public void OnPointerEnter(PointerEventData eventData)
    {
        transform.DOScale(1.5f,0.25f);
        index = transform.GetSiblingIndex();
        transform.SetAsLastSibling();

        transform.gameObject.GetComponent<Image>().material.SetColor("_EdgeColor", Color.yellow);
        transform.gameObject.GetComponent<Image>().material.SetFloat("_Edge", 0.03f);

        Info.SetActive(true);
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        transform.DOScale(1, 0.25f);
        transform.SetSiblingIndex(index);

        transform.gameObject.GetComponent<Image>().material.SetColor("_EdgeColor", Color.white);
        transform.gameObject.GetComponent<Image>().material.SetFloat("_Edge", 0);

        Info.SetActive(false);
    }


    // Start is called before the first frame update
    void Start()
    {
        transform.gameObject.GetComponent<Image>().material = Instantiate(Resources.Load<Material>("CardEffects/outline"));
    }

    
}
