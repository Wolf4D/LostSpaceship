using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipHandling : MonoBehaviour
{
    public GameObject NewShipHandler;
    // Start is called before the first frame update
    void Start()
    {
        NewShipHandler = Instantiate(NewShipHandler);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
