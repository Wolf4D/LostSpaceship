using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class MouseControl : MonoBehaviour
{

    public BattleField[] BattleFields;
    public BattleField CurrentBattleField = null;
    public GameObject MouseTrackBorder;
    public GameObject MouseSelectionBorder;
    public GameObject cellUnderMouse;
    public GameObject SelectedObject;
    public GameObject WalkZoneDemonstrator;
    public GameObject WalkZoneDemonstratorPrefab;
    public GameObject FireZoneDemonstrator;
    public GameObject FireZoneDemonstratorPrefab;


    public InformationPanel infoPanel;

    public MouseModes currentMouseMode = MouseModes.SelectShip;
    public enum MouseModes
    {
        SelectShip = 0,
        ShipSelected = 1
    };

    // private Camera cameraUI;

    // Start is called before the first frame update
    void Start()
    {
        BattleFields = FindObjectsOfType<BattleField>();
        //cameraUI = GetComponent<Camera>();
        MouseTrackBorder = Instantiate(MouseTrackBorder);
        MouseSelectionBorder = Instantiate(MouseSelectionBorder);
        MouseSelectionBorder.SetActive(false);
 
    }

    // Update is called once per frame
    void Update()
    {
        ProbeBattleFieldsForHover();

        if (Input.GetMouseButtonDown(0))
        {
            if (currentMouseMode == MouseModes.SelectShip)
                TrySelect();
            else
                if (!TryCommand())
                    TrySelect();
        }
    }

    bool TryCommand()
    {
        Debug.Log("TC");

        if ((CurrentBattleField == null) || (cellUnderMouse == null) || (SelectedObject==null))
            return false;

        // Узнаем, можем ли мы ещё действовать? Нет - сразу не команда
        ShipProperties ship = SelectedObject.GetComponent<ShipProperties>();
        if (ship.hasMoved) return false;

        // Есть ли враг?
        Vector2 coords = CurrentBattleField.CalcCoordsFromXYZ(cellUnderMouse.transform.localPosition);
        GameObject target = CurrentBattleField.GetObjectAtCoords(coords);

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
                //MouseSelectionBorder.transform.position = cellUnderMouse.transform.position;
                //return false;
            }
            else
            { 
                Debug.Log("No Move!");
                return false;
            }

            return true;
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

}
