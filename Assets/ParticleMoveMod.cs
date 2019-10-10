using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleMoveMod : MonoBehaviour
{
    public float speed = 0.1f;
    public float time = 3.0f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (time>0)
        { 
        time -= Time.deltaTime;
        transform.Translate(Vector3.back * speed * Time.deltaTime);
        }
    }
}
