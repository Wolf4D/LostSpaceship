using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class MouseControl : MonoBehaviour
{

    public BattleField[] BattleFields;
    public BattleField CurrentBattleField = null;
    public TurnSystem turnSystem = null;
    public GameObject MouseTrackBorder;
    public GameObject MouseSelectionBorder;
    public GameObject cellUnderMouse;
    public GameObject SelectedObject;
    public GameObject WalkZoneDemonstrator;
    public GameObject WalkZoneDemonstratorPrefab;
    public GameObject FireZoneDemonstrator;
    public GameObject FireZoneDemonstratorPrefab;
    public SidesStats Stats;
    public UnitSpawner Spawner;
    //public GameObject UnitToPlace;
    public GameObject UnitToSpawnAtMouse;
    public int UnitToSpawnCost = -1;


    public InformationPanel infoPanel;

    public MouseModes currentMouseMode = MouseModes.SelectShip;
    public enum MouseModes
    {
        SelectShip = 0,
        ShipSelected = 1,
        ShipPlacing = 2
    };

    // private Camera cameraUI;

    // Start is called before the first frame update
    void Start()
    {
        BattleFields = FindObjectsOfType<BattleField>();
        turnSystem = FindObjectOfType<TurnSystem>();
        Spawner = FindObjectOfType<UnitSpawner>();
        Stats = FindObjectOfType<SidesStats>();

        //cameraUI = GetComponent<Camera>();
        MouseTrackBorder = Instantiate(MouseTrackBorder);
        MouseSelectionBorder = Instantiate(MouseSelectionBorder);
        MouseSelectionBorder.SetActive(false);

 
    }

    // Update is called once per frame
    void Update()
    {
        ProbeBattleFieldsForHover();

        if (turnSystem.currentSide != ShipProperties.BattleSides.Earth) return;

        if (Input.GetMouseButtonDown(0))
        {
            turnSystem.wasAnyPlayersMove = true;
            switch (currentMouseMode)
            {
                case (MouseModes.SelectShip): { TrySelect(); } ; break;
                case (MouseModes.ShipSelected):
                    { 
                        if (!TryCommand())
                            TrySelect();
                    }; break;
                case (MouseModes.ShipPlacing): { TryPlace(); }; break;

            }
        }

        if (Input.GetMouseButtonDown(1))
        { 
            currentMouseMode = MouseModes.SelectShip;
            DropSelections();
        }
    }

    bool TryCommand()
    {
        //Debug.Log("TC");

        if ((CurrentBattleField == null) || (cellUnderMouse == null) || (SelectedObject==null))
            return false;

        // Узнаем, можем ли мы ещё действовать? Нет - сразу не команда
        ShipProperties ship = SelectedObject.GetComponent<ShipProperties>();
        if (ship.hasMoved) return false;
        if (ship.side != ShipProperties.BattleSides.Earth) return false;

        // Есть ли враг?
        Vector2 coords = CurrentBattleField.CalcCoordsFromXYZ(cellUnderMouse.transform.localPosition);
        GameObject target = CurrentBattleField.GetObjectAtCoords(coords);

        // Есть враг?
        if ((target != null) && (target!= SelectedObject))
        {
            if (ship.attackCooldownLeft > 0) return false;

            ShipProperties targetShip = target.GetComponent<ShipProperties>();
            if (ship.side == targetShip.side) return false;
            if (!targetShip.isAlive) return false;

            // Проверим, может ли корабль выстрелить?
            Vector2 coordsInMoveZone = FireZoneDemonstrator.GetComponent<DrawZone>().
                                        CalcCoordsFromXYZ(FireZoneDemonstrator.transform.InverseTransformPoint(cellUnderMouse.transform.position));
            if (CanFireThere(coordsInMoveZone))
            {
               // CurrentBattleField.MoveObject(
               //     CurrentBattleField.CalcCoordsFromXYZ(SelectedObject.transform.localPosition),
                //    coords);

                ship.Attack(targetShip);
                Debug.Log("Fire!");

                if (WalkZoneDemonstrator != null)
                    WalkZoneDemonstrator.SetActive(false);
                if (FireZoneDemonstrator != null)
                    FireZoneDemonstrator.SetActive(false);

                //ProceedSelection(SelectedObject, coords);
                ProceedSelection(null, new Vector2(-1, -1));
                turnSystem.OneActionMade();
                //MouseSelectionBorder.transform.position = cellUnderMouse.transform.position;
                return true;
            }
            else
            {
                Debug.Log("No Fire!");
                return false;
            }


        }

        // Нет врага? Попробуем двигаться
        if (target == null)
        {
            // Проверим, может ли корабль двигаться?
            Vector2 coordsInMoveZone = WalkZoneDemonstrator.GetComponent<DrawZone>().
                                        CalcCoordsFromXYZ(WalkZoneDemonstrator.transform.InverseTransformPoint(cellUnderMouse.transform.position));
            if (CanMoveThere(coordsInMoveZone))
            {
                CurrentBattleField.MoveObject(
                    CurrentBattleField.CalcCoordsFromXYZ(SelectedObject.transform.localPosition),
                    coords);

                ship.Move(cellUnderMouse.transform.position);
                //Debug.Log("Move!");
                if (WalkZoneDemonstrator!=null)
                    WalkZoneDemonstrator.SetActive(false);
                if (FireZoneDemonstrator != null)
                    FireZoneDemonstrator.SetActive(false);

                ProceedSelection(SelectedObject, coords);
                turnSystem.OneActionMade();

                return true;
                //MouseSelectionBorder.transform.position = cellUnderMouse.transform.position;
                //return false;
            }
            else
            { 
                Debug.Log("No Move!");
                return false;
            }

            //return true;
        }

        return false;
    }

    bool CanMoveThere(Vector2 coords)
    {
        if (WalkZoneDemonstrator.GetComponent<DrawZone>().IsInZone(
            (int)(coords.x), (int)(coords.y)))
            return true;

        return false;
    }

    bool CanFireThere(Vector2 coords)
    {
        if (FireZoneDemonstrator.GetComponent<DrawZone>().IsInZone(
            (int)(coords.x), (int)(coords.y)))
            return true;

        return false;
    }

    bool TryPlace()
    {
        if (UnitToSpawnAtMouse == null) return false;
        if ((CurrentBattleField == null) || (cellUnderMouse == null))
        {
            return false;
        }

        Vector2 coords = CurrentBattleField.CalcCoordsFromXYZ(cellUnderMouse.transform.localPosition);
        GameObject toSelect = CurrentBattleField.GetObjectAtCoords(coords);

        //Debug.Log("TP " + toSelect);
        if (toSelect == null)
        {
            //Debug.Log("SPN " + UnitToSpawnAtMouse);

            bool isOnBeacon = false;
            foreach (Beacon bc in Stats.Beacons)
            {
                if (bc.currentSide == ShipProperties.BattleSides.Earth)
                {
                    if (bc.SpawnZone == null) return false;
                    DrawZone dz = bc.SpawnZone.GetComponent<DrawZone>();
                    
                    Vector2 beacCoord = dz.CalcCoordsFromXYZ(dz.transform.InverseTransformPoint(
                        cellUnderMouse.transform.position));

                    if (dz.IsInZone((int)beacCoord.x, (int)beacCoord.y))
                        isOnBeacon = true;
                }
            }

            if (!isOnBeacon)
                return false;

            Spawner.SpawnUnit(ShipProperties.BattleSides.Earth, UnitToSpawnAtMouse, UnitToSpawnCost, coords);

            currentMouseMode = MouseModes.SelectShip;
            Stats.TurnAllBeacons(ShipProperties.BattleSides.Earth, false);

            // ProceedPlace(coords);
            return true;
        }

        //currentMouseMode = MouseModes.SelectShip;
        return false;
    }

    bool TrySelect()
    {
        if ((CurrentBattleField == null) || (cellUnderMouse == null))
        {
            return false;
        }

        Vector2 coords = CurrentBattleField.CalcCoordsFromXYZ(cellUnderMouse.transform.localPosition);
        GameObject toSelect = CurrentBattleField.GetObjectAtCoords(coords);

        //Debug.Log(toSelect);
        if (toSelect != SelectedObject)
        {
            ProceedSelection(toSelect, coords);
            return true;
        }
 
        currentMouseMode = MouseModes.SelectShip;
        return false;
    }

    void ProceedSelection(GameObject newObj, Vector2 coords)
    {
        MouseSelectionBorder.SetActive(false);
        infoPanel.gameObject.SetActive(false);
        if (WalkZoneDemonstrator != null)
            Destroy(WalkZoneDemonstrator);

        if (FireZoneDemonstrator != null)
            Destroy(FireZoneDemonstrator);

        SelectedObject = newObj;

        if (SelectedObject != null)
        {
            MouseSelectionBorder.SetActive(true);
            GameObject cellSel = CurrentBattleField.GetCellAtCoords(coords);
            MouseSelectionBorder.transform.position = cellSel.transform.position;
            infoPanel.gameObject.SetActive(true);
            infoPanel.currentUnit = SelectedObject.GetComponent<ShipProperties>();

            ShipProperties ship = SelectedObject.GetComponent<ShipProperties>();

            // Покажем, как можно ходить
            WalkZoneDemonstrator = Instantiate(WalkZoneDemonstratorPrefab);
            WalkZoneDemonstrator.GetComponent<DrawZone>().radius = SelectedObject.GetComponent<ShipProperties>().speed;
            WalkZoneDemonstrator.transform.position = cellSel.transform.position;
            WalkZoneDemonstrator.SetActive(true);

            // Покажем, как можно стрелять
            FireZoneDemonstrator = Instantiate(FireZoneDemonstratorPrefab);
            FireZoneDemonstrator.GetComponent<DrawZone>().radius = SelectedObject.GetComponent<ShipProperties>().range;
            FireZoneDemonstrator.transform.position = cellSel.transform.position;
            FireZoneDemonstrator.SetActive(true);

            if (ship.range==0)
                FireZoneDemonstrator.SetActive(false);

            if (ship.hasMoved)
            { 
                WalkZoneDemonstrator.SetActive(false);
                FireZoneDemonstrator.SetActive(false);

            }

            currentMouseMode = MouseModes.ShipSelected;
            return;
        }

        currentMouseMode = MouseModes.SelectShip;


    }

    void ProbeBattleFieldsForHover()
    {
        MouseTrackBorder.SetActive(false);
        cellUnderMouse = GetCellUnderMouse();
        if (cellUnderMouse!=null)
        { 
        MouseTrackBorder.transform.position = cellUnderMouse.transform.position;
        MouseTrackBorder.SetActive(true);
        // Пока так
        CurrentBattleField = cellUnderMouse.transform.parent.GetComponent<BattleField>();
        }
    }

    GameObject GetCellUnderMouse()
    {

        // Проверяем, не попали ли в UI?

        Vector3 mouseOptPos = Input.mousePosition;
        mouseOptPos.y += 30;
        Ray ray = Camera.main.ScreenPointToRay(mouseOptPos);
        RaycastHit hit;
        var layerMask = 1 << LayerMask.NameToLayer("BattleGrid");

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask, QueryTriggerInteraction.UseGlobal))
            {
            //Debug.Log(hit.collider.gameObject.name);
            Debug.DrawLine(ray.origin, hit.point);

            for (int i = 0; i < BattleFields.Length; i++)
            {
                foreach(GameObject obj in BattleFields[i].Map)
                if (obj == hit.collider.gameObject)
                        return obj;
            }
        }

        return null;
        //  CreatureSelected.Invoke(selected);

    }

    void ProceedPlace(Vector2 coords)
    {
       // if (Stats.GetSomeForMoney())
       // {
          //  UnitSpawner
        //}


    }

    public void DropSelections()
    {
        MouseSelectionBorder.SetActive(false);
        infoPanel.gameObject.SetActive(false);
        if (WalkZoneDemonstrator != null)
            Destroy(WalkZoneDemonstrator);

        if (FireZoneDemonstrator != null)
            Destroy(FireZoneDemonstrator);

        Stats.TurnAllBeacons(ShipProperties.BattleSides.Earth, false);


        SelectedObject = null;
    }

    public void SpawnUnit(UnitCard unit)
    {
        Debug.Log("Got spawn");

        DropSelections();

        Stats.TurnAllBeacons(ShipProperties.BattleSides.Earth, true);

        UnitToSpawnAtMouse = unit.ShipToBuy;
        UnitToSpawnCost = unit.Cost;
        currentMouseMode = MouseModes.ShipPlacing;
    }

    // Это плохооооо!
    /*
    public void SpawnFighter(int side) 
    {
        if (Stats.GetSomeForMoney()
        //Debug.Log("AA "  + " " + side);

    }

 
    */


}
