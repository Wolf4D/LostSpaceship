using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivateOnNearby : MonoBehaviour
{
    public GameObject objectToTarget;
    public GameObject objectToEnable;
    public float enableDistance = 50;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        
        float dist = Vector3.Distance(objectToTarget.transform.position, this.transform.position);
        //Debug.Log(" " + dist);
        if (enableDistance >= dist)
        {
            objectToEnable.SetActive(true);
            this.enabled = false;
        }
    }
}
