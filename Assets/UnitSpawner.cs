using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitSpawner : MonoBehaviour
{
    public SidesStats Stats;
    public UnitCard[] SpawnablePrefabs = new UnitCard[12];
    public BattleField CurrentBattleField;
    public GameObject spawnEffect;
    private TurnSystem turnSystem;



    // Start is called before the first frame update
    void Start()
    {
        Stats = GetComponent<SidesStats>();
        CurrentBattleField = FindObjectOfType<BattleField>();
        turnSystem = FindObjectOfType<TurnSystem>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public bool SpawnUnit(ShipProperties.BattleSides side, GameObject ship, int cost, Vector2 coords)
    {
        if (Stats.GetSomeForMoney(side, cost))
        {
            Debug.Log("Got money");
            GameObject obj = Instantiate(ship, CurrentBattleField.transform);
            obj.transform.localPosition = CurrentBattleField.CalcXYZfromCoords((int)(coords.x), (int)(coords.y));
            GameObject spawnEff = Instantiate(spawnEffect, CurrentBattleField.transform);
            spawnEff.transform.localPosition = CurrentBattleField.CalcXYZfromCoords((int)(coords.x), (int)(coords.y));
            Destroy(spawnEff, 3);
            obj.GetComponent<ShipProperties>().hasMoved = true;
            obj.GetComponent<ShipProperties>().side = side;
            obj.GetComponentInChildren<MeshRenderer>().material = Stats.sideMaterials[(int)(side)];
            CurrentBattleField.ClampObjectToGrid(obj);
            turnSystem.OneActionMade();
            return true;
        }
        return false;

    }
}
