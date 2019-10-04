using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleField : MonoBehaviour
{
    public int x = 1;
    public int y = 1;
    public GameObject CellSample;
    public GameObject[,] Map;
    public float cellSideSize = 4.77939f * 1.2885f;

    public Vector3 CalcXYZfromCoords(int x, int y)
    {
        return new Vector3(x * 2.0f * cellSideSize, 0, y * 2.0f * cellSideSize);
    }


    // Start is called before the first frame update
    void Start()
    {
        Map = new GameObject[x, y];
        for (int j = 0; j < y; j++)
            for (int i = 0; i < x; i++)
        {
            Map[i,j] = Instantiate(CellSample, this.transform);
                Map[i, j].transform.localPosition = CalcXYZfromCoords(i, j);
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
