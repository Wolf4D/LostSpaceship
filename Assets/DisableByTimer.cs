using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisableByTimer : MonoBehaviour
{
    public GameObject activateAfter;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if ((Input.anyKeyDown))
        {
            if (activateAfter!=null)
                activateAfter.SetActive(true);

            Destroy(this.gameObject);

        }
    }
}
