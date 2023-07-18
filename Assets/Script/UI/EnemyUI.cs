using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EnemyUI : MonoBehaviour
{
    [SerializeField] private TMP_Text playerName;
    [SerializeField] private TMP_Text hpText;
    [SerializeField] private TMP_Text defenceText;
    [SerializeField] private TMP_Text attackText;
    [SerializeField] private TMP_Text rangeText;
    [SerializeField] private GameObject container;
    [SerializeField] private List<Color> colorList;


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
        defenceText.text = enemy.Defence.ToString();
        attackText.text = enemy.AttackDamage.ToString();
        rangeText.text = enemy.Range.ToString();

    }
}
