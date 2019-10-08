using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraTrick : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void OnEnable()
    {
        Camera.main.orthographic = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
