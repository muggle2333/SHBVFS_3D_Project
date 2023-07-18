using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class PlayerDataUI : MonoBehaviour
{
    //[SerializeField] private TMP_Text playerNameText;
    [SerializeField] private TMP_Text hpText;
    [SerializeField] private TMP_Text maxHpText;
    [SerializeField] private TMP_Text maxApText;
    [SerializeField] private TMP_Text apText;
    [SerializeField] private TMP_Text defenceText;
    [SerializeField] private TMP_Text attackText;
    [SerializeField] private TMP_Text rangeText;
    [SerializeField] private TMP_Text cardARText;
    [SerializeField] private TMP_Text cardADText;
    [SerializeField] private TMP_Text cardDFText;

    private Player player;
    private void Update()
    {
        if(player != null)
        {
            hpText.text = player.HP.ToString();
            maxHpText.text = "/"+ player.MaxHP.ToString();
            apText.text = player.CurrentActionPoint.ToString();
            maxApText.text = "/"+ player.MaxActionPoint.ToString();

            defenceText.text = player.Defence.ToString();
            attackText.text = player.AttackDamage.ToString();
            rangeText.text = player.Range.ToString();

            cardARText.text = "(" + (player.Range - player.cardAR).ToString() + "+" + player.cardAR.ToString()+")";
            cardADText.text = "(" + (player.AttackDamage - player.cardAD).ToString() + "+" + player.cardAD.ToString() + ")";
            cardDFText.text = "(" + (player.Defence - player.cardDF).ToString() + "+" + player.cardDF.ToString() + ")";

            if (player.cardAR == 0)
            {
                cardARText.gameObject.SetActive(false);
                rangeText.color = Color.white;
            }
            else
            {
                cardARText.gameObject.SetActive(true);
                rangeText.color = Color.green;
            }

            if(player.cardDF == 0)
            {
                cardDFText.gameObject.SetActive(false);
                defenceText.color = Color.white;
            }
            else
            {
                cardDFText.gameObject.SetActive(true);
                defenceText.color = Color.green;
            }

            if(player.cardAD == 0)
            {
                cardADText.gameObject.SetActive(false);
                attackText.color = Color.white;
            }
            else
            {
                cardADText.gameObject.SetActive(true);
                attackText.color = Color.green;
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
