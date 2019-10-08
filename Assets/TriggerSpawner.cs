using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerSpawner : MonoBehaviour
{
    public GameObject[] objectsToSpawn;
    public Vector2[] coords;
    public ShipProperties.BattleSides mySide;
    UnitSpawner spawner;

    
    // Start is called before the first frame update
    void Start()
    {
        spawner = FindObjectOfType<UnitSpawner>();

    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < objectsToSpawn.Length; i++)
        {
            spawner.SpawnUnit(mySide, objectsToSpawn[i], 0, coords[i], false);
        }
        this.enabled = false;
    }
}
