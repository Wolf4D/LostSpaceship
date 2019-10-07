using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Beacon : MonoBehaviour
{
    public ShipProperties.BattleSides currentSide = ShipProperties.BattleSides.Neutral;
    float searchRadius = 5.0f;
    public BattleField CurrentBattleField;
    public GameObject SpawnZone;
    public GameObject SpawnZonePrefab;

    Vector2 coords;

    SidesStats stats;
    // Start is called before the first frame update
    void Start()
    {
        stats = FindObjectOfType<SidesStats>();
        CurrentBattleField = FindObjectOfType<BattleField>();

    }

    public void LightBeacon()
    {
        SpawnZone = Instantiate(SpawnZonePrefab);
        SpawnZone.GetComponent<DrawZone>().radius = 2;
        SpawnZone.transform.position = this.transform.position;
        SpawnZone.SetActive(true);
    }

    public void ShutBeacon()
    {
        Destroy(SpawnZone);
    }

   public void checkCapture()
    {
        /*
        ShipProperties[] ships = FindObjectOfType<ShipProperties>();
        ShipProperties[] shipsNear = new ShipProperties;

        foreach (ShipProperties shp in ships)
        {
            if (Vector3.Distance(this.transform.localPosition, shp.transform.localPosition) < searchRadius)
                shipsNear
        }
        */
        coords = CurrentBattleField.CalcCoordsFromXYZ(transform.localPosition);

        GameObject ship = CurrentBattleField.GetObjectAtCoords(coords);
        if (ship == null) return;
        ShipProperties shp = ship.GetComponent<ShipProperties>();
        if (shp == null) return;
        currentSide = shp.side;
        this.GetComponentInChildren<Renderer>().material = stats.sideMaterials[(int)(currentSide)];

        // bool[] sidesShips = new bool[4];

        /*
        var layerMask = 1 << LayerMask.NameToLayer("Spaceship");

        RaycastHit[] hit = Physics.SphereCastAll(this.transform.position, searchRadius, transform.forward, layerMask);

        foreach (RaycastHit rh in hit)
        {
            if (rh.transform.parent.gameObject.GetComponent<ShipProperties>())
            {
                Debug.Log(rh.transform.parent.gameObject);
                sidesShips[(int)(rh.transform.parent.gameObject.GetComponent<ShipProperties>().side)]=true;
            }
        }
        */
        /*
                if ((sidesShips[1] == true) && (sidesShips[2] == false) && (sidesShips[3] == false))
                    this.GetComponentInChildren<Renderer>().material = stats.sideMaterials[1];
                    // Debug.Log("Earth!");

                if ((sidesShips[1] == false) && (sidesShips[2] == true) && (sidesShips[3] == false))
                    this.GetComponentInChildren<Renderer>().material = stats.sideMaterials[2];
                //Debug.Log("As!");

                if ((sidesShips[1] == false) && (sidesShips[2] == false) && (sidesShips[3] == true))
                    this.GetComponentInChildren<Renderer>().material = stats.sideMaterials[3];
                    */
        //Debug.Log("He!");       
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
