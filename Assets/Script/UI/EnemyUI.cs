using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EnemyUI : MonoBehaviour
{
    [SerializeField] private TMP_Text playerName;
    [SerializeField] private TMP_Text hpText;
    [SerializeField] private TMP_Text maxHpText;
    [SerializeField] private TMP_Text defenceText;
    [SerializeField] private TMP_Text attackText;
    [SerializeField] private TMP_Text rangeText;
    [SerializeField] private GameObject container;
    [SerializeField] private List<Color> colorList;
    [SerializeField] private TMP_Text cardARText;
    [SerializeField] private TMP_Text cardADText;
    [SerializeField] private TMP_Text cardDFText;


    private Player enemy;
    public void SetEnemyUI(GridObject gridObject)
    {
        if (enemy == null)
        {
            InitializeEnemy();
        }
        if(enemy.currentGrid.x == gridObject.x && enemy.currentGrid.z == gridObject.z)
        {
            ShowEnemyData();
        }
        else
        {
            container.SetActive(false);
        }
    }
    public void InitializeEnemy()
    {
        var playerList = GameplayManager.Instance.GetPlayer();
        foreach(var player in playerList)
        {
            if(player.Id != GameplayManager.Instance.currentPlayer.Id)
            {
                enemy = player;
            }
        }
    }
    public void ShowEnemyData()
    {
        container.SetActive(true);
        if(enemy == null)
        {
            InitializeEnemy();
        }
        playerName.text = enemy.Id == PlayerId.RedPlayer ? "RED" : "BLUE";
        playerName.color = colorList[(int)enemy.Id];

        hpText.text = enemy.HP.ToString();
        maxHpText.text = "/" + enemy.MaxHP.ToString();
        defenceText.text = enemy.Defence.ToString();
        attackText.text = enemy.AttackDamage.ToString();
        rangeText.text = enemy.Range.ToString();

        cardARText.text = "(" + (enemy.Range - enemy.cardAR).ToString() + "+" + enemy.cardAR.ToString() + ")";
        cardADText.text = "(" + (enemy.AttackDamage - enemy.cardAD).ToString() + "+" + enemy.cardAD.ToString() + ")";
        cardDFText.text = "(" + (enemy.Defence - enemy.cardDF).ToString() + "+" + enemy.cardDF.ToString() + ")";

        if (enemy.cardAR == 0)
        {
            cardARText.gameObject.SetActive(false);
            rangeText.color = Color.white;
        }
        else
        {
            cardARText.gameObject.SetActive(true);
            rangeText.color = Color.green;
        }

        if (enemy.cardDF == 0)
        {
            cardDFText.gameObject.SetActive(false);
            defenceText.color = Color.white;
        }
        else
        {
            cardDFText.gameObject.SetActive(true);
            defenceText.color = Color.green;
        }

        if (enemy.cardAD == 0)
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
