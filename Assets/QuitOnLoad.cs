﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuitOnLoad : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void OnEnable()
    {
        Application.Quit();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
