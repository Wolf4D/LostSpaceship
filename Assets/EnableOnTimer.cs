using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnableOnTimer : MonoBehaviour
{
    public GameObject activateAfter;
    public float timeLeft = 5.0f;
    public bool keyControlled = false;
    public bool destroyThis = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        timeLeft -= Time.deltaTime;
        if ((timeLeft <= 0) || 
            ((keyControlled==true) && (Input.anyKeyDown)))
        {
            activateAfter.SetActive(true);
            if (destroyThis) Destroy(this.gameObject);
            this.enabled = false;

        }
    }
}
