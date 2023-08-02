using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class PlayerDataUI : MonoBehaviour
{
    //[SerializeField] private TMP_Text playerNameText;
    [SerializeField] private TMP_Text hpText;
    [SerializeField] private TMP_Text apText;
    [SerializeField] private TMP_Text defenceText;
    [SerializeField] private TMP_Text attackText;
    [SerializeField] private TMP_Text rangeText;
    [SerializeField] private TMP_Text cardARText;
    [SerializeField] private TMP_Text cardADText;
    [SerializeField] private TMP_Text cardDFText;

    [SerializeField] private Image playerImage;
    [SerializeField] private List<Sprite> playerSprites;
    private Player player;
    private void Update()
    {
        if(player != null)
        {
            playerImage.sprite = playerSprites[(int)player.Id];
            hpText.text = player.HP.ToString() + " / " + player.MaxHP.ToString();
            apText.text = player.CurrentActionPoint.ToString() + " / " + player.MaxActionPoint.ToString();
            defenceText.text = player.Defence.ToString();
            attackText.text = player.AttackDamage.ToString();
            rangeText.text = player.Range.ToString();
            if (player.cardAR >= 0)
            {
                cardARText.text = "(" + (player.Range - player.cardAR).ToString() + "+" + player.cardAR.ToString() + ")";
            }
            else
            {
                cardARText.text = "(" + (player.Range - player.cardAR).ToString() + player.cardAR.ToString() + ")";
            }
            if(player.cardAD >= 0)
            {
                cardADText.text = "(" + (player.AttackDamage - player.cardAD).ToString() + "+" + player.cardAD.ToString() + ")";
            }
            else
            {
                cardADText.text = "(" + (player.AttackDamage - player.cardAD).ToString() + player.cardAD.ToString() + ")";
            }
            if(player.cardDF >= 0)
            {
                cardDFText.text = "(" + (player.Defence - player.cardDF).ToString() + "+" + player.cardDF.ToString() + ")";
            }
            else
            {
                cardDFText.text = "(" + (player.Defence - player.cardDF).ToString() + player.cardDF.ToString() + ")";
            }
            

            if (player.cardAR == 0)
            {
                cardARText.gameObject.SetActive(false);
                rangeText.color = Color.white;
            }
            else if(player.cardAR > 0)
            {
                cardARText.gameObject.SetActive(true);
                rangeText.color = new Color32(119,184,116,255);
            }
            else if(player.cardAR < 0)
            {
                cardARText.gameObject.SetActive(true);
                rangeText.color = new Color32(228, 52, 3, 255);
            }

            if(player.cardDF == 0)
            {
                cardDFText.gameObject.SetActive(false);
                defenceText.color = Color.white;
            }
            else if(player.cardDF > 0)
            {
                cardDFText.gameObject.SetActive(true);
                defenceText.color = new Color32(119, 184, 116, 255);
            }
            else if(player.cardDF < 0)
            {
                cardDFText.gameObject.SetActive(true);
                defenceText.color = new Color32(228, 52, 3, 255);
            }

            if(player.cardAD == 0)
            {
                cardADText.gameObject.SetActive(false);
                attackText.color = Color.white;
            }
            else if(player.cardAD > 0)
            {
                cardADText.gameObject.SetActive(true);
                attackText.color = new Color32(119, 184, 116, 255);
            }
            else if(player.cardAD < 0)
            {
                cardADText.gameObject.SetActive(true);
                attackText.color = new Color32(228, 52, 3, 255);
            }
        }
    }
    public void UpdatePlayerData(Player player)
    {
        //playerNameText.text = player.Id.ToString();
        this.player = player;
        hpText.text = player.HP.ToString();
        apText.text = player.CurrentActionPoint.ToString();
        defenceText.text = player.Defence.ToString();
        attackText.text = player.AttackDamage.ToString();
        rangeText.text = player.Range.ToString();
    }

}
