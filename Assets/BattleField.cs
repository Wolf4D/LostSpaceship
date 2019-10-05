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
        GameObject obj = Objects[(int)(from.x), (int)(from.y)];
        Objects[(int)(to.x), (int)(to.y)] = obj;
        Objects[(int)(from.x), (int)(from.y)] = null;
    }


    // Start is called before the first frame update
    void Start()
    {
        Objects = new GameObject[x, y];
        for (int i = 0; i < transform.childCount; i++)
        {
            GameObject obj = transform.GetChild(i).gameObject;
            Vector2 coord = CalcCoordsFromXYZ(obj.transform.localPosition);
            //Debug.Log("" + obj.name + ":" + coord.x + "," + coord.y);
            if (obj.GetComponent<ShipProperties>())
                (obj.GetComponent<ShipProperties>()).battleField = this;

            Objects[(int)(coord.x), (int)(coord.y)] = obj;
        }

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
        return Objects[(int)(coord.x), (int)(coord.y)];
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
