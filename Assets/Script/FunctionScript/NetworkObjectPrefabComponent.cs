using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class NetworkObjectPrefabComponent : MonoBehaviour
{
    [SerializeField]private List<GameObject> prebabs= new List<GameObject>();
    private void Awake()
    {
        foreach(GameObject obj in prebabs)
        {
            GetComponent<NetworkManager>().AddNetworkPrefab(obj);
        }
       
    }
}
