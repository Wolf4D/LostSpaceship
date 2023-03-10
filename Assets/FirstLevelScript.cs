using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstLevelScript : MonoBehaviour
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
        if ((stats.BeaconsOfSide[1] >= 1) && (phase == 1) && (turnSystem.turnCount>=8))
            if (turnSystem.currentSide == ShipProperties.BattleSides.Asura)
            {
                phase1.GetComponent<AI>().isBrainActive = true;
                phase = 2;
                return;
            }

        if ((stats.BeaconsOfSide[2] == 0) && (phase == 2) && (turnSystem.turnCount >= 8))
            //if (turnSystem.currentSide == ShipProperties.BattleSides.Asura)
            {
            phase2.SetActive(true);
                phase = 3;
                return;
            }

        if (playersBase == null)
        {
            lose.SetActive(true);
            phase = -1;
        }

    }
}
