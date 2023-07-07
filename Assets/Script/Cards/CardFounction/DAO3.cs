using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DAO3 : CardFunction
{
    public Dictionary<Player, List<Card>> HandCardContainer = new Dictionary<Player, List<Card>>();
    // Start is called before the first frame update
    void Start()
    {
        Function();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void Function()
    {
        Destroy(gameObject);
    }
}
