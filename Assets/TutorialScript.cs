using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialScript : MonoBehaviour
{
    SidesStats stats;
    UnitSpawner spawner;
    public GameObject phase1;
    // Start is called before the first frame update
    void Start()
    {
        stats = FindObjectOfType<SidesStats>();
        spawner = FindObjectOfType<UnitSpawner>();
    }

    // Update is called once per frame
    void Update()
    {
        if (stats.BeaconsOfSide[1] == 1)
        {
            spawner.SpawnUnit(ShipProperties.BattleSides.Asura, spawner.SpawnablePrefabs[11].ShipToBuy, 0, new Vector2(0, 0));
        }
    }
}
