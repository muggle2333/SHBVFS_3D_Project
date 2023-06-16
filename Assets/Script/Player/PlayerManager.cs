using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager Instance;

    [SerializeField] private Vector2 playerStartPoint;
    private GameObject player;
    public void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(Instance);
        }
        else
        {
            Instance = this;
        }
        
    }
    private void Start()
    {
        InitializePlayer();
    }
    private void InitializePlayer()
    {
        player = FindObjectOfType<PlayerInteractionComponent>().gameObject;
        player.transform.position = GridManager.Instance.grid.GetWorldPositionCenter((int)playerStartPoint.x,(int)playerStartPoint.y);
    }

    public void MovePlayer(Vector2 dirPos)
    {
        player.GetComponent<PlayerInteractionComponent>().Move(dirPos);
    }
}
