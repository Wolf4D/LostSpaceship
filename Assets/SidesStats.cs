using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SidesStats : MonoBehaviour
{
    public int[] MoneyOfSide = new int[4];
    public int[] UnitsOfSide = new int[4];
    public Beacon[] Beacons;
    public int[] BeaconsOfSide = new int[4];
    public int BeaconCost = 100;

    public Material[] sideMaterials = new Material[4];


    // Start is called before the first frame update
    void Start()
    {
        Beacons = FindObjectsOfType<Beacon>();   
    }

    public void CheckBeacons()
    {
        BeaconsOfSide[0] = 0;
        BeaconsOfSide[1] = 0;
        BeaconsOfSide[2] = 0;
        BeaconsOfSide[3] = 0;

        foreach (Beacon bc in Beacons)
        { 
            bc.checkCapture();
            BeaconsOfSide[(int)(bc.currentSide)]++;
        }
    }


    public void CheckUnits()
    {
        UnitsOfSide[0] = 0;
        UnitsOfSide[1] = 0;
        UnitsOfSide[2] = 0;
        UnitsOfSide[3] = 0;

        ShipProperties[] ships = FindObjectsOfType<ShipProperties>();

        foreach (ShipProperties shp in ships)
        {
            if (shp.isAlive)
                UnitsOfSide[(int)(shp.side)]++;
        }
    }

    public void TurnAllBeacons(ShipProperties.BattleSides side, bool on)
    {
        foreach (Beacon bc in Beacons)
        {
            if (side == bc.currentSide)
            {
                if (on)
                    bc.LightBeacon();
                else
                    bc.ShutBeacon();
            }
        }
    }

    public bool CanGetSomeForMoney(ShipProperties.BattleSides side, int cost)
    {
        if (MoneyOfSide[(int)(side)] <= cost)
        {
            return true;
        }

        return false;
    }

    public bool GetSomeForMoney(ShipProperties.BattleSides side, int cost)
    {
        //Debug.Log("MoneyOfSide " + side + " " + MoneyOfSide[(int)(side)]);

        if (MoneyOfSide[(int)(side)] >= cost)
        {
            MoneyOfSide[(int)(side)] -= cost;
            return true;
        }

        return false;
    }

    // TODO: А вот это уже плохо!
    public void CollectMoney(ShipProperties.BattleSides side)
    {
        MoneyOfSide[(int)(side)] += BeaconCost * BeaconsOfSide[(int)(side)];
    }

    //public bool IsSideAlive()

    // Update is called once per frame
    void Update()
    {
        
    }
}
