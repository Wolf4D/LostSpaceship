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

        if (Input.GetMouseButton(0))
            TrySelect();
    }

    bool TrySelect()
    {
        if ((CurrentBattleField==null) || (cellUnderMouse == null))
            return false;

        Vector2 coords = CurrentBattleField.CalcCoordsFromXYZ(cellUnderMouse.transform.localPosition);
        GameObject toSelect = CurrentBattleField.GetObjectAtCoords(coords);

        //Debug.Log(toSelect);
        if (toSelect != SelectedObject)
        {
            ProceedSelection(toSelect);
            return true;
        }

        return false;
    }

    void ProceedSelection(GameObject newObj)
    {
        MouseSelectionBorder.SetActive(false);
        SelectedObject = newObj;

        if (SelectedObject != null)
        {
            MouseSelectionBorder.SetActive(true);
            MouseSelectionBorder.transform.position = SelectedObject.transform.position;
        }
        


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
