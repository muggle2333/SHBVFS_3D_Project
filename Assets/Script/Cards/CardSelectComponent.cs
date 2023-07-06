using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;
using UnityEngine.UI;
using Unity.VisualScripting;

public class CardSelectComponent : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler
{
    private int index;
    public bool Interactable = true;
    public bool IsInOpreationStage;
    public bool isSelected;
    public float targetY;
    public float formerY;
    //public DG.Tweening.Sequence CardDiscardAniamtion;
    //public DG.Tweening.Sequence CardTakeEffectAniamtion;
    //public DG.Tweening.Sequence EnemyCardTakeEffectAniamtion;
    [SerializeField] private float duration;
    public GameObject Info;
    public CardSelectManager cardSelectManager;
    void Start()
    {
        Interactable = true;
        //transform.gameObject.GetComponentInChildren<CardBackGroundComponent>().GetComponent<Image>().material = Instantiate(Resources.Load<Material>("CardEffects/outline"));
        cardSelectManager = PlayerManager.Instance.cardSelectManager;
        isSelected = false;
        duration = 0.25f;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (Interactable == false) return;
        //transform.gameObject.GetComponentInChildren<CardBackGroundComponent>().GetComponent<Image>().material.SetColor("_EdgeColor", Color.yellow);
        //transform.gameObject.GetComponentInChildren<CardBackGroundComponent>().GetComponent<Image>().material.SetFloat("_Edge", 0.03f);

        Info.SetActive(true);
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        //transform.gameObject.GetComponentInChildren<CardBackGroundComponent>().GetComponent<Image>().material.SetColor("_EdgeColor", Color.white);
        //transform.gameObject.GetComponentInChildren<CardBackGroundComponent>().GetComponent<Image>().material.SetFloat("_Edge", 0);

        Info.SetActive(false);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        
    }

    public void OnPointerUp(PointerEventData eventData)
    {

    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (Interactable == false) return;
        if(IsInOpreationStage)
        {
            foreach(var card in cardSelectManager.cardsList)
            {
                if (this == card) continue;
                if(card.isSelected)
                {
                    card.EndSelect();
                    break;
                }
            }
        }
        if (isSelected) EndSelect();
        else OnSelect();
    }

    public void OnSelect()
    {
        //index = transform.GetSiblingIndex();
        //transform.SetAsLastSibling();
        transform.DOLocalMoveY(targetY, duration);
        isSelected = true;
        cardSelectManager.SelectCount++;
        cardSelectManager.SelectButton.SetActive(true);
        cardSelectManager.CancelButton.SetActive(true);
        Debug.Log(cardSelectManager.SelectCount);
    }
    public void EndSelect()
    {
        //transform.SetSiblingIndex(index);
        if(Interactable)
        transform.DOLocalMoveY(formerY, duration);
        isSelected = false;
        cardSelectManager.SelectCount--;
        if (cardSelectManager.SelectCount == 0)
        {
            cardSelectManager.SelectButton.SetActive(false);
            cardSelectManager.CancelButton.SetActive(false);
        }
        Debug.Log(cardSelectManager.SelectCount);
    }

    public void CardPlayAniamtion()
    {
        Interactable = false;
        this.EndSelect();
        this.transform.SetParent(cardSelectManager.canvas.transform);
        var seq = DOTween.Sequence();
        seq.Append(transform.DOLocalMoveY(0, 0.4f));
        seq.Join(transform.DOLocalMoveX(0, 0.4f));
        seq.Join(transform.DOScale(1.5f, 0.4f));
        seq.AppendInterval(0.5f);
        seq.Append(transform.DOLocalMoveX(-800, 0.5f));
        seq.Join(transform.DOScale(0.05f, 0.5f));
        //seq.AppendInterval(1f);
        seq.AppendCallback(() => { this.gameObject.SetActive(false); });
    }
}
