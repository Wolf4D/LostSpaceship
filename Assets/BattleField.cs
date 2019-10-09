using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleField : MonoBehaviour
{
    public int x = 1;
    public int y = 1;
    public GameObject CellSample;
    public GameObject[,] Map;
    public GameObject[,] Objects;
    //public GameObject[] ObjectsToSnap; // объекты, которые должны быть подтянуты на карту
    public float cellSideSize = 4.77939f * 1.2885f;

    public Vector3 CalcXYZfromCoords(int x, int y)
    {
        return new Vector3(x * 2.0f * cellSideSize, 0, y * 2.0f * cellSideSize);
    }

    public Vector2 CalcCoordsFromXYZ(Vector3 xyz)
    {
        return new Vector2(Mathf.RoundToInt(xyz.x / (2.0f * cellSideSize)), Mathf.RoundToInt(xyz.z / (2.0f * cellSideSize)));
    }

    public void MoveObject(Vector2 from, Vector2 to)
    {
        if (((int)(to.x) >= x) || ((int)(to.y) >= y) || ((int)(to.x) <0) || ((int)(to.y) < 0))
            return;

        if (((int)(from.x) >= x) || ((int)(from.y) >= y) || ((int)(from.x) < 0) || ((int)(from.y) < 0))
            return;

        GameObject obj = Objects[(int)(from.x), (int)(from.y)];
        Objects[(int)(to.x), (int)(to.y)] = obj;
        Objects[(int)(from.x), (int)(from.y)] = null;
    }

    public void ClampObjectToGrid(GameObject obj)
    {
        Vector2 coord = CalcCoordsFromXYZ(obj.transform.localPosition);
        if (obj.GetComponent<ShipProperties>())
        {
            (obj.GetComponent<ShipProperties>()).battleField = this;
            obj.transform.localPosition = CalcXYZfromCoords((int)(coord.x), (int)(coord.y));
            Objects[(int)(coord.x), (int)(coord.y)] = obj;
        }
    }


    // Start is called before the first frame update
    void Start()
    {
        Objects = new GameObject[x, y];
        for (int i = 0; i < transform.childCount; i++)
            ClampObjectToGrid(transform.GetChild(i).gameObject);

        Map = new GameObject[x, y];
        for (int j = 0; j < y; j++)
            for (int i = 0; i < x; i++)
        {
            Map[i,j] = Instantiate(CellSample, this.transform);
                Map[i, j].transform.localPosition = CalcXYZfromCoords(i, j);
                Map[i, j].name = i + ", " + j;
        }
    }

    public GameObject GetObjectAtCoords(Vector2 coord)
    {
        if ((((int)(coord.x) < x) && ((int)(coord.y) < y)) && (((int)(coord.x) >= 0) && ((int)(coord.y) >= 0)))
            return Objects[(int)(coord.x), (int)(coord.y)];
        else
            return null;
    }

    public GameObject GetCellAtCoords(Vector2 coord)
    {
        return Map[(int)(coord.x), (int)(coord.y)];
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
