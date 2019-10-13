using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnOffAI : MonoBehaviour
{
    public AI aiTT;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void OnEnable()
    {
        aiTT.isBrainActive = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
