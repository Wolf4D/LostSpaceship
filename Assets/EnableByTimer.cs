using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnableByTimer : MonoBehaviour
{
    public GameObject[] toEnable;
    public float[] timeToHold;
    public int currentObject = 0;
    public float timeLeft = 5.0f;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        timeLeft -= Time.deltaTime;
        if ((Input.anyKeyDown) || (timeLeft<=0))
        {
          if (toEnable[currentObject]!=null)
            toEnable[currentObject].SetActive(false);
            currentObject++;
            toEnable[currentObject].SetActive(true);
            timeLeft = timeToHold[currentObject];

        }
    }

    

}
