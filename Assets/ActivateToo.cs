using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivateToo : MonoBehaviour
{
    public GameObject activateToo;
    public bool doItOnStart = true;
    // Start is called before the first frame update
    void Start()
    {
        if (doItOnStart)
            activateToo.SetActive(true);
    }

    private void OnEnable()
    {
        if (!doItOnStart)
            activateToo.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
