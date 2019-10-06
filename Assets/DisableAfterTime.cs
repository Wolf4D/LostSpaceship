using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisableAfterTime : MonoBehaviour
{
    public float timing = 2.0f;
    private float remainTime = 0;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        remainTime -= Time.deltaTime;
        if (remainTime <= 0)
            gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        remainTime = timing;
    }
}
