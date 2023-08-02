using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class AcademyUI : MonoBehaviour
{
    public List<TMP_Text> redAcademies;
    public List<TMP_Text> blueAcademies;
    public List<TMP_Text> academiesBuff;
    public List<Player> players;
    public bool isUnlock;


    private void Start()
    {
        Invoke("GetPlayers", 3);
    }
    public void GetPlayers()
    {
        players = GameplayManager.Instance.GetPlayer();
        isUnlock = true;
    }

    private void Update()
    {
        if (GameManager.Instance.wholeGameState.Value == GameManager.WholeGameState.GameOver) return;
        if (isUnlock)
        {
            if (players[0] == null) return;
            if (players[0].totalAcademyOwnedPoint.Count == 0) return;
            for (int i = 0; i < 6; i++)
            {
                if (players[0].totalAcademyOwnedPoint[i] < 0)
                {
                    redAcademies[i].text = "0";
                }
                else
                {
                    redAcademies[i].text = players[0].totalAcademyOwnedPoint[i].ToString();
                }
            }

            if(players[1] == null) return;
            if(players[1].totalAcademyOwnedPoint.Count == 0) return;
            for(int i = 0; i < 6; i++)
            {
                if (players[1].totalAcademyOwnedPoint[i] < 0)
                {
                    blueAcademies[i].text = "0";
                }
                else
                {
                    blueAcademies[i].text = players[1].totalAcademyOwnedPoint [i].ToString();
                }
            }
        }
    }
}
