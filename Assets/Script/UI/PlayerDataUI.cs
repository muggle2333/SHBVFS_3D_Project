using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class PlayerDataUI : MonoBehaviour
{
    //[SerializeField] private TMP_Text playerNameText;
    [SerializeField] private TMP_Text hpText;
    [SerializeField] private TMP_Text maxApText;
    [SerializeField] private TMP_Text apText;
    [SerializeField] private TMP_Text defenceText;
    [SerializeField] private TMP_Text attackText;
    [SerializeField] private TMP_Text rangeText;

    private Player player;
    private void Update()
    {
        if(player != null)
        {
            hpText.text = player.HP.ToString();
            apText.text = player.CurrentActionPoint.ToString();
            maxApText.text = "/"+ player.MaxActionPoint.ToString();
            defenceText.text = player.Defence.ToString();
            attackText.text = player.AttackDamage.ToString();
            rangeText.text = player.Range.ToString();
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
