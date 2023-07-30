using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class AcademyUI : MonoBehaviour
{
    public List<TMP_Text> academies;
    public List<TMP_Text> academiesBuff;
    public Player Player;
    public bool isUnlock;


    private void Start()
    {
        Invoke("GetCurrentPlayer", 3);
    }
    public void GetCurrentPlayer()
    {
        Player = GameplayManager.Instance.currentPlayer;
        isUnlock = true;
    }

    private void Update()
    {
        if (GameManager.Instance.wholeGameState.Value == GameManager.WholeGameState.GameOver) return;
        if (isUnlock)
        {
            if (Player == null) return;
            if (Player.totalAcademyOwnedPoint.Count == 0) return;
            for (int i = 0; i < academies.Count; i++)
            {
                if (Player.totalAcademyOwnedPoint[i] < 0)
                {
                    academies[i].text = "0";
                }
                else if(Player.totalAcademyOwnedPoint[i] > 4)
                {
                    academies[i].text = "4";
                }
                else if(Player.totalAcademyOwnedPoint[i]>=0 && Player.totalAcademyOwnedPoint[i] <= 4)
                {
                    academies[i].text = Player.totalAcademyOwnedPoint[i].ToString();
                }
            }
        }
    }
}
