using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialScript : MonoBehaviour
{
    SidesStats stats;
    UnitSpawner spawner;
    TurnSystem turnSystem;
    public GameObject phase1;
    public GameObject phase2;
    public GameObject phase3;

    public int phase = 1;
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
        if ((stats.BeaconsOfSide[1] == 1) && (phase==1))
            if (turnSystem.currentSide==ShipProperties.BattleSides.Asura)
        {
            phase1.SetActive(true);
            phase++;
            return;
        }
        if ((stats.UnitsOfSide[2] == 0) && (stats.UnitsOfSide[1] > 1) && (phase == 2))
            if (turnSystem.currentSide == ShipProperties.BattleSides.Asura)
            {
                phase2.SetActive(true);
            phase++;
            return;
        }

        if ((stats.UnitsOfSide[2] <= 2) && (turnSystem.turnCount>7) && (phase == 3))
           if (turnSystem.currentSide == ShipProperties.BattleSides.Asura)
            {
            phase3.SetActive(true);
            phase++;
            return;
        }
    }
}
