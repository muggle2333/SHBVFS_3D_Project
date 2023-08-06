using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class DyingWarningUI : MonoBehaviour
{
    [SerializeField] private GameObject background;

    private Player player;
    private void Start()
    {
        Invoke("InitializePlayer", 2f);
    }
    private void InitializePlayer()
    {
        player = GameplayManager.Instance.playerList[(int)NetworkManager.Singleton.LocalClientId];
    }
    private void Update()
    {
        if (player == null) return; 
        if(player.HP<=1)
        {
            Show();
        }else
        {
            Hide();
        }
    }
    public void Show()
    {
        if (background.activeSelf == true) return;   
        background.SetActive(true);
    }

    public void Hide()
    {
        if (background.activeSelf == false) return;
        background.SetActive(false);
    }
}
