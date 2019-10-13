using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SecondLevelScript : MonoBehaviour
{
    SidesStats stats;
    UnitSpawner spawner;
    TurnSystem turnSystem;
    public int phase = 1;
    public GameObject phase1;
    public GameObject phase2;
    public GameObject phase3;
    public GameObject phase4;
    public GameObject phase5;

    public GameObject playersBase;
    public GameObject disruptorShip;

    public GameObject lose;


    // Start is called before the first frame update
    void Start()
    {
        turnSystem = FindObjectOfType<TurnSystem>();
        stats = FindObjectOfType<SidesStats>();
        spawner = FindObjectOfType<UnitSpawner>();
    }


    // Update is called once per frame
    void Update()
    {
        if (playersBase == null)
        {
            lose.SetActive(true);
            phase = -1;
        }

        if (disruptorShip == null)
        {
            lose.SetActive(true);
            phase = -1;
        }
    }
}
