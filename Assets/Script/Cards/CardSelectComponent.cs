using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;
using UnityEngine.UI;
using Unity.VisualScripting;
using System.Linq;

public class CardSelectComponent : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    private int index;
    public bool Interactable = true;
    public bool IsInOpreationStage;
    public bool isSelected;
    public bool isBurning;
    public float targetY;
    public float formerY;
    //public DG.Tweening.Sequence CardDiscardAniamtion;
    //public DG.Tweening.Sequence CardTakeEffectAniamtion;
    //public DG.Tweening.Sequence EnemyCardTakeEffectAniamtion;
    public GameObject Info;
    public CardSelectManager cardSelectManager;
    [SerializeField] private Material cardVFX;
    [SerializeField] private float duration;
    [SerializeField] private GameObject outline;
    private float vfxFloat;

    public bool isLocked = false;
    void Start()
    {
        cardVFX.SetTexture("_MainText",gameObject.GetComponent<Card>().cardTexture);
        //Interactable = true;
        //transform.gameObject.GetComponentInChildren<CardBackGroundComponent>().GetComponent<Image>().material = Instantiate(Resources.Load<Material>("CardEffects/Outline"));
        cardSelectManager = PlayerManager.Instance.cardSelectManager;
        isSelected = false;
        duration = 0.25f;
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        SelectManager.Instance.isClickingCard = true;
        UIManager.Instance.ShowGridObjectUI(false, null);
        if (Interactable == false) return;

        if (isSelected) return;
        transform.DOLocalMoveY((targetY - formerY) / 2 + formerY, duration);
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        SelectManager.Instance.isClickingCard = false;
        if (Interactable == false) return;
        if (isSelected) return;
        transform.DOLocalMoveY(formerY, duration);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if(isLocked) return;
        if (eventData.button == PointerEventData.InputButton.Right) return;
        if (Interactable == false) return;
        if (TurnbasedSystem.Instance.CurrentGameStage.Value == GameStage.S1)
        {
            if (isSelected)
            {
                if(gameObject.GetComponent<Card>().selectGridMode != SelectGridMode.Default)
                {
                    if (SelectManager.Instance.selectedDict.Count == gameObject.GetComponent<Card>().needSelectCount)
                    {
                        if (gameObject.GetComponent<Card>().needSelectCount == 1)
                        {
                            SelectMode.Instance.saveSelectedGridServerRpc(GameplayManager.Instance.currentPlayer.Id, gameObject.GetComponent<Card>().cardId, gameObject.GetComponent<Card>().needSelectCount, SelectManager.Instance.selectedDict.ElementAt(0).Key, SelectManager.Instance.selectedDict.ElementAt(0).Key);
                        }
                        else
                        {
                            SelectMode.Instance.saveSelectedGridServerRpc(GameplayManager.Instance.currentPlayer.Id, gameObject.GetComponent<Card>().cardId, gameObject.GetComponent<Card>().needSelectCount, SelectManager.Instance.selectedDict.ElementAt(0).Key, SelectManager.Instance.selectedDict.ElementAt(1).Key);
                        }
                        cardSelectManager.PlayCards(GameplayManager.Instance.currentPlayer);
                        UIManager.Instance.HideWarning();
                    }
                    else
                    {
                        UIManager.Instance.BlinkWarning();
                    }
                }
                else
                {
                    cardSelectManager.PlayCards(GameplayManager.Instance.currentPlayer);
                    UIManager.Instance.HideWarning();
                }

                if(FindObjectOfType<TutorialManager>()!=null)
                {
                    TutorialManager.Instance.CompleteTutorialAction(TutorialAction.ClickPlayCard);
                }
            }
            else OnSelectOperation();
        }
        else if (TurnbasedSystem.Instance.CurrentGameStage.Value == GameStage.DiscardStage)
        {
            if (isSelected) EndSelectDiscard();
            else OnSelectDiscard();
        }
        else if (TurnbasedSystem.Instance.isDie.Value == true && GameplayManager.Instance.currentPlayer.HP <= 0)
        {
            if (this.gameObject.GetComponent<Card>().cardId != 0) return;
            if (!isSelected) OnSelectDying();
        }
        else
        {
            if (isSelected) EndSelectOther();
            else OnSelectOther();
        }
    }

    public void OnSelectOperation()
    {
        Info.SetActive(true);
        index = transform.GetSiblingIndex();
        transform.SetAsLastSibling();
        transform.DOLocalMoveY(targetY, duration);
        isSelected = true;
        cardSelectManager.SelectCount[GameplayManager.Instance.currentPlayer]++;
        foreach (var card in CardManager.Instance.playerHandCardDict[GameplayManager.Instance.currentPlayer])
        {
            if (this.gameObject == card.gameObject) continue;
            if (card.gameObject.GetComponent<CardSelectComponent>().isSelected)
            {
                card.gameObject.GetComponent<CardSelectComponent>().EndSelectOperation();
            }
        }//ensure current selected count
        
        //UIManager.Instance.SetGameplayPlayUI(GameplayUIType.playCard, true);
        //UIManager.Instance.SetGameplayPlayUI(GameplayUIType.cancelControl, true);
        //transform.gameObject.GetComponentInChildren<CardBackGroundComponent>().GetComponent<Image>().material.SetColor("_EdgeColor", Color.yellow);
        //transform.gameObject.GetComponentInChildren<CardBackGroundComponent>().GetComponent<Image>().material.SetFloat("_Edge", 0.03f);
        outline.SetActive(true);
        if (gameObject.GetComponent<Card>().selectGridMode != SelectGridMode.Default)
        {
            SelectManager.Instance.ChangeSelectMode(gameObject.GetComponent<Card>().selectGridMode, gameObject.GetComponent<Card>().needSelectCount);
            if (gameObject.GetComponent<Card>().needSelectCount == 2)
            {
                UIManager.Instance.ShowWarning("You need to select" + 2 + "Grids");
            }
            else if (gameObject.GetComponent<Card>().needSelectCount == 1)
            {
                UIManager.Instance.ShowWarning("You need to select" + 1 + "Grid");
            }
        }
    }

    public void EndSelectOperation()
    {
        SelectManager.Instance.ChangeSelectMode(SelectGridMode.Default,1);
        UIManager.Instance.HideWarning();
        Info.SetActive(false);
        transform.SetSiblingIndex(index);
        transform.DOLocalMoveY(formerY, duration);
        isSelected = false;
        cardSelectManager.SelectCount[GameplayManager.Instance.currentPlayer]--;
        //UIManager.Instance.SetGameplayPlayUI(GameplayUIType.playCard, false);
        //UIManager.Instance.SetGameplayPlayUI(GameplayUIType.cancelControl, false);
       
        //transform.gameObject.GetComponentInChildren<CardBackGroundComponent>().GetComponent<Image>().material.SetColor("_EdgeColor", Color.white);
        //transform.gameObject.GetComponentInChildren<CardBackGroundComponent>().GetComponent<Image>().material.SetFloat("_Edge", 0);
        outline.SetActive(false);
    }

    public void OnSelectDiscard()
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
                card.gameObject.GetComponent<CardSelectComponent>().EndSelectDiscard();
            }
        }//ensure current selected count
        foreach (var card in CardManager.Instance.playerHandCardDict[GameplayManager.Instance.currentPlayer])
        {
            if (this.gameObject == card.gameObject) continue;
            card.gameObject.GetComponent<CardSelectComponent>().Info.SetActive(false);
        }//turn off Info
        if (GameplayManager.Instance.discardStage.discardCount[GameplayManager.Instance.currentPlayer] <= 0)
            return;//no need to discard
        //UIManager.Instance.SetGameplayPlayUI(GameplayUIType.cancelDiscard, true);
        UIManager.Instance.SetGameplayPlayUI(GameplayUIType.discardCards, true);
        cardSelectManager.ToDiscardText.text = cardSelectManager.SelectCount[GameplayManager.Instance.currentPlayer] + " / " + cardSelectManager.maxSelected[GameplayManager.Instance.currentPlayer];
        if (cardSelectManager.SelectCount[GameplayManager.Instance.currentPlayer] == cardSelectManager.maxSelected[GameplayManager.Instance.currentPlayer])
        {
            UIManager.Instance.SetGameplayPlayUIInteractable(GameplayUIType.discardCards, true);
            cardSelectManager.ToDiscardText.color = new Color32(255, 200, 71, 255);//green
        }
        else
        {
            UIManager.Instance.SetGameplayPlayUIInteractable(GameplayUIType.discardCards, false);
            cardSelectManager.ToDiscardText.color = new Color32(65, 16, 0, 255);//red
        }
    }

    public void EndSelectDiscard()
    {
        SelectManager.Instance.ChangeSelectMode(SelectGridMode.None, 1);
        UIManager.Instance.HideWarning();
        Info.SetActive(false);
        transform.SetSiblingIndex(index);
        transform.DOLocalMoveY(formerY, duration);
        isSelected = false;
        cardSelectManager.SelectCount[GameplayManager.Instance.currentPlayer]--;
        cardSelectManager.ToDiscardText.text = cardSelectManager.SelectCount[GameplayManager.Instance.currentPlayer] + " / " + cardSelectManager.maxSelected[GameplayManager.Instance.currentPlayer];
        if (cardSelectManager.SelectCount[GameplayManager.Instance.currentPlayer] < cardSelectManager.maxSelected[GameplayManager.Instance.currentPlayer])
        {
            UIManager.Instance.SetGameplayPlayUIInteractable(GameplayUIType.discardCards, false);
            cardSelectManager.ToDiscardText.color = new Color32(65, 16, 0, 255);//red
        }
        if (cardSelectManager.SelectCount[GameplayManager.Instance.currentPlayer] == 0)
        {
            UIManager.Instance.SetGameplayPlayUI(GameplayUIType.discardCards, false);
            //UIManager.Instance.SetGameplayPlayUI(GameplayUIType.cancelDiscard, false);
        }
        //transform.gameObject.GetComponentInChildren<CardBackGroundComponent>().GetComponent<Image>().material.SetColor("_EdgeColor", Color.white);
        //transform.gameObject.GetComponentInChildren<CardBackGroundComponent>().GetComponent<Image>().material.SetFloat("_Edge", 0);
        outline.SetActive(false);
    }

    public void OnSelectDying()
    {
        Info.SetActive(true);
        index = transform.GetSiblingIndex();
        transform.SetAsLastSibling();
        transform.DOLocalMoveY(targetY, duration);
        isSelected = true;
        cardSelectManager.SelectCount[GameplayManager.Instance.currentPlayer]++;
        foreach (var card in CardManager.Instance.playerHandCardDict[GameplayManager.Instance.currentPlayer])
        {
            if (cardSelectManager.SelectCount[GameplayManager.Instance.currentPlayer] <= (0 - GameplayManager.Instance.currentPlayer.HP) / 2 + 1)
                break;
            if (this.gameObject == card.gameObject) continue;
            if (card.gameObject.GetComponent<CardSelectComponent>().isSelected)
            {
                card.gameObject.GetComponent<CardSelectComponent>().EndSelectDying();
            }
        }//ensure current selected count
        foreach (var card in CardManager.Instance.playerHandCardDict[GameplayManager.Instance.currentPlayer])
        {
            if (this.gameObject == card.gameObject) continue;
            card.gameObject.GetComponent<CardSelectComponent>().Info.SetActive(false);
        }//turn off Info
        UIManager.Instance.SetGameplayPlayUI(GameplayUIType.playHP, true);
        //UIManager.Instance.SetGameplayPlayUI(GameplayUIType.cancelDying, true);
        cardSelectManager.ToPlayHPText.text = cardSelectManager.SelectCount[GameplayManager.Instance.currentPlayer] + " / " + (0 - GameplayManager.Instance.currentPlayer.HP) / 2 + 1;
        if (cardSelectManager.SelectCount[GameplayManager.Instance.currentPlayer] == cardSelectManager.maxSelected[GameplayManager.Instance.currentPlayer])
        {
            UIManager.Instance.SetGameplayPlayUIInteractable(GameplayUIType.playHP, true);
            cardSelectManager.ToPlayHPText.color = new Color32(255, 200, 71, 255);//green
        }
        else
        {
            UIManager.Instance.SetGameplayPlayUIInteractable(GameplayUIType.playHP, false);
            cardSelectManager.ToPlayHPText.color = new Color32(65, 16, 0, 255);//red
        }
    }

    public void EndSelectDying()
    {
        Info.SetActive(false);
        transform.SetSiblingIndex(index);
        transform.DOLocalMoveY(formerY, duration);
        isSelected = false;
        cardSelectManager.SelectCount[GameplayManager.Instance.currentPlayer]--;
        cardSelectManager.ToPlayHPText.text = cardSelectManager.SelectCount[GameplayManager.Instance.currentPlayer] + " / " + ((0 - GameplayManager.Instance.currentPlayer.HP) / 2 + 1);
        if (cardSelectManager.SelectCount[GameplayManager.Instance.currentPlayer] < (0 - GameplayManager.Instance.currentPlayer.HP) / 2 + 1)
        {
            cardSelectManager.ToPlayHPText.color = new Color32(65, 16, 0, 255);//red
            UIManager.Instance.SetGameplayPlayUIInteractable(GameplayUIType.playHP, false);
        }
        if (cardSelectManager.SelectCount[GameplayManager.Instance.currentPlayer] == 0)
        {
            UIManager.Instance.SetGameplayPlayUI(GameplayUIType.playHP, false);
        }
        //transform.gameObject.GetComponentInChildren<CardBackGroundComponent>().GetComponent<Image>().material.SetColor("_EdgeColor", Color.white);
        //transform.gameObject.GetComponentInChildren<CardBackGroundComponent>().GetComponent<Image>().material.SetFloat("_Edge", 0);
        outline.SetActive(false);
    }

    public void OnSelectOther()
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
                card.gameObject.GetComponent<CardSelectComponent>().EndSelectOther();
            }
        }//ensure current selected count
    }

    public void EndSelectOther()
    {
        Info.SetActive(false);
        transform.SetSiblingIndex(index);
        transform.DOLocalMoveY(formerY, duration);
        isSelected = false;
        cardSelectManager.SelectCount[GameplayManager.Instance.currentPlayer]--;
        //transform.gameObject.GetComponentInChildren<CardBackGroundComponent>().GetComponent<Image>().material.SetColor("_EdgeColor", Color.white);
        //transform.gameObject.GetComponentInChildren<CardBackGroundComponent>().GetComponent<Image>().material.SetFloat("_Edge", 0);
        outline.SetActive(false);
    }

    #region Cloud
    //public void OnSelect()
    //{
    //    Info.SetActive(true);
    //    index = transform.GetSiblingIndex();
    //    transform.SetAsLastSibling();
    //    transform.DOLocalMoveY(targetY, duration);
    //    isSelected = true;
    //    cardSelectManager.SelectCount[GameplayManager.Instance.player]++;
    //    foreach (var card in CardManager.Instance.playerHandCardDict[GameplayManager.Instance.player])
    //    {
    //        if (cardSelectManager.SelectCount[GameplayManager.Instance.player] <= cardSelectManager.maxSelected[GameplayManager.Instance.player])
    //            break;
    //        if (this.gameObject == card.gameObject) continue;
    //        if (card.gameObject.GetComponent<CardSelectComponent>().isSelected)
    //        {
    //            card.gameObject.GetComponent<CardSelectComponent>().EndSelect();
    //        }
    //    }//ensure current selected count
    //    if (TurnbasedSystem.Instance.CurrentGameStage.Value == GameStage.S1)
    //    {
    //        S1AdditionalCondition();
    //    }
    //    else if (TurnbasedSystem.Instance.CurrentGameStage.Value == GameStage.DiscardStage && GameplayManager.Instance.discardStage.discardCount[GameplayManager.Instance.player] > 0)
    //    {
    //        DiscardAdditionalCondition();
    //    }
    //    else if(true)
    //    {
    //        DyingAdditionalCondition();
    //    }//activate UI button
    //    //Debug.Log(cardSelectManager.SelectCount[GameplayManager.Instance.player]);
    //}
    //public void EndSelect()
    //{
    //    Info.SetActive(false);
    //    transform.SetSiblingIndex(index);
    //    isSelected = false;
    //    cardSelectManager.SelectCount[GameplayManager.Instance.player]--;
    //    if (cardSelectManager.SelectCount[GameplayManager.Instance.player] < cardSelectManager.maxSelected[GameplayManager.Instance.player])
    //    {
    //        UIManager.Instance.SetGameplayPlayUI(GameplayUIType.playCard, false);
    //        UIManager.Instance.SetGameplayPlayUIInteractable(GameplayUIType.discardCards, false);
    //    }
    //    if(cardSelectManager.SelectCount[GameplayManager.Instance.player] == 0)
    //    {
    //        UIManager.Instance.SetGameplayPlayUI(GameplayUIType.discardCards, false);
    //        UIManager.Instance.SetGameplayPlayUI(GameplayUIType.cancel, false);
    //    }
    //}

    //public void S1AdditionalCondition()
    //{
    //    UIManager.Instance.SetGameplayPlayUI(GameplayUIType.playCard, true);
    //    UIManager.Instance.SetGameplayPlayUI(GameplayUIType.cancel, true);
    //}

    //public void DiscardAdditionalCondition()
    //{
    //    foreach (var card in CardManager.Instance.playerHandCardDict[GameplayManager.Instance.player])
    //    {
    //        if (this.gameObject == card.gameObject) continue;
    //        card.gameObject.GetComponent<CardSelectComponent>().Info.SetActive(false);
    //    }//turn off Info
    //    UIManager.Instance.SetGameplayPlayUI(GameplayUIType.cancel, true);
    //    UIManager.Instance.SetGameplayPlayUI(GameplayUIType.discardCards, true);
    //    UIManager.Instance.SetGameplayPlayUIInteractable(GameplayUIType.discardCards, false);
    //    if (cardSelectManager.SelectCount[GameplayManager.Instance.player] == cardSelectManager.maxSelected[GameplayManager.Instance.player])
    //    {
    //        UIManager.Instance.SetGameplayPlayUIInteractable(GameplayUIType.discardCards, true);
    //    }
    //}

    //public void DyingAdditionalCondition()
    //{
    //    foreach (var card in CardManager.Instance.playerHandCardDict[GameplayManager.Instance.player])
    //    {
    //        if (this.gameObject == card.gameObject) continue;
    //        card.gameObject.GetComponent<CardSelectComponent>().Info.SetActive(false);
    //    }//turn off Info
    //}
    #endregion

    public void CardPlayAniamtion()
    {
        Interactable = false;
        this.EndSelectOperation();
        this.transform.SetParent(cardSelectManager.canvas.transform);
        SoundManager.Instance.PlaySound(Sound.PlayCard);
        var seq = DOTween.Sequence();
        seq.Append(transform.DOLocalMoveY(0, 0.4f));
        seq.Join(transform.DOLocalMoveX(0, 0.4f));
        //seq.Join(transform.DOScale(1.5f, 0.4f));
        seq.AppendInterval(0.5f);
        seq.Append(transform.DOLocalMoveX(-800, 0.5f));
        seq.Join(transform.DOLocalMoveY(500, 0.5f));
        seq.Join(transform.DOScale(0.05f, 0.5f));
        seq.AppendCallback(() => { this.gameObject.SetActive(false); });
    }

    public void CardDiscardAnimation()
    {
        Interactable = false;
        this.EndSelectDiscard();
        this.transform.SetParent(cardSelectManager.canvas.transform);
        //SoundManager.Instance.PlaySound(Sound.DiscardCard);
        var seq = DOTween.Sequence();
        seq.Append(transform.DOLocalMoveY(0, 0.4f));
        seq.AppendInterval(0.5f);
        seq.Join(DOTween.To(() => this.gameObject.GetComponent<CanvasGroup>().alpha, x => this.gameObject.GetComponent<CanvasGroup>().alpha = x, 0, 0.5f));
        seq.AppendCallback(() => { Destroy(this.gameObject); });
    }
    public void HPCardAnimation()
    {
        vfxFloat = 1;
        isBurning = true;
        Interactable = false;
        this.EndSelectDying();
        this.transform.SetParent(cardSelectManager.canvas.transform);
        var seq = DOTween.Sequence();
        seq.Append(transform.DOLocalMoveY(0, 0.4f));
        seq.AppendInterval(0.5f);
        Invoke("AddMaterial", 1);
        SoundManager.Instance.PlaySound(Sound.CardTakeEffect);
        //burn function
        seq.AppendCallback(() => { DOTween.To(() => vfxFloat, x => vfxFloat = x, 0, 1f); });
        seq.AppendInterval(1f);
        seq.AppendCallback(() => { isBurning = false; });
        seq.AppendCallback(() => { Destroy(this.gameObject); });
    }

    private void Update()
    {
        if (isBurning)
        {
            cardVFX.SetFloat("_NoiseStrength", vfxFloat);
        }
    }
    public void CardTakeEffectAnimation()
    {
        vfxFloat = 1;
        isBurning = true;
        Interactable = false;
        this.transform.localPosition = new Vector3(-800, 500, transform.localPosition.z);
        //this.gameObject.SetActive(true);
        var seq = DOTween.Sequence();
        seq.Append(transform.DOLocalMoveX(0, 0.4f));
        seq.Join(transform.DOLocalMoveY(0, 0.4f));
        seq.Join(transform.DOScale(1f, 0.4f));
        seq.AppendCallback(() => { this.Info.SetActive(true); });
        seq.AppendInterval(1f);
        Invoke("AddMaterial", 1);
        SoundManager.Instance.PlaySound(Sound.CardTakeEffect);
        //burn function
        seq.AppendCallback(() => { DOTween.To(() => vfxFloat, x => vfxFloat = x, 0, 1f); });
        seq.AppendInterval(1f);
        //seq.Append(transform.DOLocalMoveX(-200, 0.4f));
        //seq.Join(transform.DOScale(1f, 0.4f));
        //seq.AppendInterval(0.5f);
        seq.AppendCallback(() => { isBurning = false; });
        seq.AppendCallback(() => { Destroy(this.gameObject); });
    }
    public void AddMaterial()
    {
        gameObject.GetComponentInChildren<CardBackGroundComponent>().GetComponent<Image>().material = cardVFX;
    }
    public void EnemyCardTakeEffectAnimation()
    {
        vfxFloat = 1;
        isBurning = true;
        Interactable = false;
        this.transform.localPosition = new Vector3(800, 500, transform.localPosition.z);
        //this.gameObject.SetActive(true);
        var seq = DOTween.Sequence();
        seq.Append(transform.DOLocalMoveX(0, 0.4f));
        seq.Join(transform.DOLocalMoveY(0, 0.4f));
        seq.Join(transform.DOScale(1f, 0.4f));
        seq.AppendCallback(() => { this.Info.SetActive(true); });
        seq.AppendInterval(1f);
        Invoke("AddMaterial", 1);
        SoundManager.Instance.PlaySound(Sound.CardTakeEffect);
        //burn function
        seq.AppendCallback(() => { DOTween.To(() => vfxFloat, x => vfxFloat = x, 0, 1f); });
        seq.AppendInterval(1f);
        //seq.Append(transform.DOLocalMoveX(-200, 0.4f));
        //seq.Join(transform.DOScale(1f, 0.4f));
        //seq.AppendInterval(0.5f);
        seq.AppendCallback(() => { isBurning = false; });
        seq.AppendCallback(() => { Destroy(this.gameObject); });
    }

}
