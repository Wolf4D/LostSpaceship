using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShowMeMoney : MonoBehaviour
{
    SidesStats stats;
    Text text;
    // Start is called before the first frame update
    void Start()
    {
        stats = FindObjectOfType<SidesStats>();
        text = GetComponent<Text>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        text.text = stats.MoneyOfSide[1] + "$";

    }
}
