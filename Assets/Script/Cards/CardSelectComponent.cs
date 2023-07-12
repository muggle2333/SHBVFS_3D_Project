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

        if (isSelected) return;
        transform.DOLocalMoveY((targetY - formerY) / 2 + formerY, duration);
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        if (Interactable == false) return;
        //transform.gameObject.GetComponentInChildren<CardBackGroundComponent>().GetComponent<Image>().material.SetColor("_EdgeColor", Color.white);
        //transform.gameObject.GetComponentInChildren<CardBackGroundComponent>().GetComponent<Image>().material.SetFloat("_Edge", 0);

        if (isSelected) return;
        transform.DOLocalMoveY(formerY, duration);
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
        if(true)
        {

        }
        else if (TurnbasedSystem.Instance.CurrentGameStage.Value == GameStage.S1)
        {
            if (isSelected) EndSelectOperation();
            else OnSelectOperation();
        }
        else if (TurnbasedSystem.Instance.CurrentGameStage.Value == GameStage.DiscardStage)
        {
            if (isSelected) EndSelectDiscard();
            else OnSelectDiscard();
        }
        else if (TurnbasedSystem.Instance.isDie.Value == true)
        {
            if(this.gameObject.GetComponent<Card>().cardId == 0)
            if (isSelected) EndSelectDying();
            else OnSelectDying();
        }
        else
        {
            if (isSelected) EndSelectOther();
            else OnSelectOther();
        }

        if (isSelected) EndSelect();
        else OnSelect();
    }

    public void OnSelectOperation()
    {

    }

    public void EndSelectOperation()
    {

    }

    public void OnSelectDiscard()
    {

    }

    public void EndSelectDiscard()
    {

    }

    public void OnSelectDying()
    {
        
    }

    public void EndSelectDying()
    {

    }

    public void OnSelectOther()
    {
        Info.SetActive(true);
        index = transform.GetSiblingIndex();
        transform.SetAsLastSibling();
        transform.DOLocalMoveY(targetY, duration);
        isSelected = true;
    }

    public void EndSelectOther()
    {
        Info.SetActive(false);
        transform.SetSiblingIndex(index);
        transform.DOLocalMoveY(formerY, duration);
        isSelected = false;
    }

    public void OnSelect()
    {
        Info.SetActive(true);
        index = transform.GetSiblingIndex();
        transform.SetAsLastSibling();
        transform.DOLocalMoveY(targetY, duration);
        isSelected = true;
        cardSelectManager.SelectCount[GameplayManager.Instance.currentPlayer]++;
        foreach (var card in CardManager.Instance.playerHandCardDict[GameplayManager.Instance.currentPlayer])
        {
            if (cardSelectManager.SelectCount[GameplayManager.Instance.currentPlayer] <= cardSelectManager.maxSelected[GameplayManager.Instance.currentPlayer])
                break;
            if (this.gameObject == card.gameObject) continue;
            if (card.gameObject.GetComponent<CardSelectComponent>().isSelected)
            {
                card.gameObject.GetComponent<CardSelectComponent>().EndSelect();
            }
        }//ensure current selected count
        if (TurnbasedSystem.Instance.CurrentGameStage.Value == GameStage.S1)
        {
            S1AdditionalCondition();
        }
        else if (TurnbasedSystem.Instance.CurrentGameStage.Value == GameStage.DiscardStage && GameplayManager.Instance.discardStage.discardCount[GameplayManager.Instance.currentPlayer] > 0)
        {
            DiscardAdditionalCondition();
        }
        else if(true)
        {
            DyingAdditionalCondition();
        }//activate UI button
        //Debug.Log(cardSelectManager.SelectCount[GameplayManager.Instance.currentPlayer]);
    }
    public void EndSelect()
    {
        Info.SetActive(false);
        transform.SetSiblingIndex(index);
        if (Interactable) transform.DOLocalMoveY(formerY, duration);
        isSelected = false;
        cardSelectManager.SelectCount[GameplayManager.Instance.currentPlayer]--;
        if (cardSelectManager.SelectCount[GameplayManager.Instance.currentPlayer] < cardSelectManager.maxSelected[GameplayManager.Instance.currentPlayer])
        {
            UIManager.Instance.SetGameplayPlayUI(GameplayUIType.playCard, false);
            UIManager.Instance.SetGameplayPlayUIInteractable(GameplayUIType.discardCards, false);
        }
        if(cardSelectManager.SelectCount[GameplayManager.Instance.currentPlayer] == 0)
        {
            UIManager.Instance.SetGameplayPlayUI(GameplayUIType.discardCards, false);
            UIManager.Instance.SetGameplayPlayUI(GameplayUIType.cancel, false);
        }
    }

    public void S1AdditionalCondition()
    {
        UIManager.Instance.SetGameplayPlayUI(GameplayUIType.playCard, true);
        UIManager.Instance.SetGameplayPlayUI(GameplayUIType.cancel, true);
    }

    public void DiscardAdditionalCondition()
    {
        foreach (var card in CardManager.Instance.playerHandCardDict[GameplayManager.Instance.currentPlayer])
        {
            if (this.gameObject == card.gameObject) continue;
            card.gameObject.GetComponent<CardSelectComponent>().Info.SetActive(false);
        }//turn off Info
        UIManager.Instance.SetGameplayPlayUI(GameplayUIType.cancel, true);
        UIManager.Instance.SetGameplayPlayUI(GameplayUIType.discardCards, true);
        UIManager.Instance.SetGameplayPlayUIInteractable(GameplayUIType.discardCards, false);
        if (cardSelectManager.SelectCount[GameplayManager.Instance.currentPlayer] == cardSelectManager.maxSelected[GameplayManager.Instance.currentPlayer])
        {
            UIManager.Instance.SetGameplayPlayUIInteractable(GameplayUIType.discardCards, true);
        }
    }

    public void DyingAdditionalCondition()
    {
        foreach (var card in CardManager.Instance.playerHandCardDict[GameplayManager.Instance.currentPlayer])
        {
            if (this.gameObject == card.gameObject) continue;
            card.gameObject.GetComponent<CardSelectComponent>().Info.SetActive(false);
        }//turn off Info
    }

    public void CardPlayAniamtion()
    {
        Interactable = false;
        this.EndSelect();
        this.transform.SetParent(cardSelectManager.canvas.transform);
        var seq = DOTween.Sequence();
        seq.Append(transform.DOLocalMoveY(0, 0.4f));
        seq.Join(transform.DOLocalMoveX(0, 0.4f));
        //seq.Join(transform.DOScale(1.5f, 0.4f));
        seq.AppendInterval(0.5f);
        seq.Append(transform.DOLocalMoveX(-800, 0.5f));
        seq.Join(transform.DOScale(0.05f, 0.5f));
        seq.AppendCallback(() => { this.gameObject.SetActive(false); });
    }

    public void CardDiscardAnimation()
    {
        Interactable = false;
        this.EndSelect();
        this.transform.SetParent(cardSelectManager.canvas.transform);
        var seq = DOTween.Sequence();
        seq.Append(transform.DOLocalMoveY(0, 0.4f));
        seq.AppendInterval(0.5f);
        seq.AppendCallback(() => { Destroy(this.gameObject); });
    }

    public void CardTakeEffectAnimation()
    {
        this.gameObject.SetActive(true);
        var seq = DOTween.Sequence();
        seq.Append(transform.DOLocalMoveX(0, 0.4f));
        seq.Join(transform.DOScale(1.5f, 0.4f));
        seq.AppendInterval(0.5f);
        seq.Append(transform.DOLocalMoveX(-200, 0.4f));
        seq.Join(transform.DOScale(1f, 0.4f));
        seq.AppendInterval(0.5f);
        seq.AppendCallback(() => { Destroy(this.gameObject); });
    }

    public void EnemyCardTakeEffectAnimation()
    {
        this.transform.localPosition = new Vector3(800, transform.localPosition.y, transform.localPosition.z);
        this.gameObject.SetActive(true);
        var seq = DOTween.Sequence();
        seq.Append(transform.DOLocalMoveX(0, 0.4f));
        seq.Join(transform.DOScale(1.5f, 0.4f));
        seq.AppendInterval(0.5f);
        seq.Append(transform.DOLocalMoveX(-200, 0.4f));
        seq.Join(transform.DOScale(1f, 0.4f));
        seq.AppendInterval(0.5f);
        seq.AppendCallback(() => { Destroy(this.gameObject); });
    }

}
