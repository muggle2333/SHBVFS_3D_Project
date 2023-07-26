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
        Invoke("GetCurrentPlayer", 10);
    }
    public void GetCurrentPlayer()
    {
        Player = GameplayManager.Instance.currentPlayer;
        isUnlock = true;
    }

    private void Update()
    {
        if (isUnlock)
        {
            if (Player.totalAcademyOwnedPoint.Count == 0) return;
            for (int i = 0; i < academies.Count; i++)
            {
                academies[i].text = Player.totalAcademyOwnedPoint[i].ToString();
            }
        }
    }
}
