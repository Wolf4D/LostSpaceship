using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawZone : MonoBehaviour
{
    public GameObject CellSample;
    public int radius = 3;
    public float cellSideSize = 4.77939f * 1.2885f;
    public GameObject[,] Map;

    private int[,] r2map = new int[3, 3] {  {0, 1, 0 },
                                            {1, 1, 1 },
                                            {0, 1, 0 } };


    private int[,] r3map = new int[5, 5] {  {0, 0, 1, 0, 0 },
                                            {0, 1, 1, 1, 0 },
                                            {1, 1, 1, 1, 1 },
                                            {0, 1, 1, 1, 0 },
                                            {0, 0, 1, 0, 0 }};

    private int[,] r4map = new int[7, 7] {  {0, 0, 0, 1, 0, 0, 0 },
                                            {0, 0, 1, 1, 1, 0, 0 },
                                            {0, 1, 1, 1, 1, 1, 0 },
                                            {1, 1, 1, 1, 1, 1, 1 },
                                            {0, 1, 1, 1, 1, 1, 0 },
                                            {0, 0, 1, 1, 1, 0, 0 },
                                            {0, 0, 0, 1, 0, 0, 0 }};

    private int[,] r5map = new int[9, 9] {  { 0, 0, 0, 0, 1, 0, 0, 0, 0 },
                                            { 0, 0, 0, 1, 1, 1, 0, 0, 0 },
                                            { 0, 0, 1, 1, 1, 1, 1, 0, 0 },
                                            { 0, 1, 1, 1, 1, 1, 1, 1, 0 },
                                            { 1, 1, 1, 1, 1, 1, 1, 1, 1 },
                                            { 0, 1, 1, 1, 1, 1, 1, 1, 0 },
                                            { 0, 0, 1, 1, 1, 1, 1, 0, 0 },
                                            { 0, 0, 0, 1, 1, 1, 0, 0, 0 },
                                            { 0, 0, 0, 0, 1, 0, 0, 0, 0 }};

   public bool IsInZone(int x, int y)
    {
        //Debug.Log(x + " " + y + "   " + radius);
        if ((x >= radius*2+1) || (y >= radius * 2+1)) return false;
        if ((x < 0) || (y < 0)) return false;
        switch (radius)
        {
           
            case 1: if (r2map[x, y] == 1) return true; break;
            case 2: if (r3map[x, y] == 1) return true; break;
            case 3: if (r4map[x, y] == 1) return true; break;
            case 4: if (r5map[x, y] == 1) return true; break;
            default: if ((x == 1) && (y == 1)) return true; break;
        }

        return false;

    }

    public Vector3 CalcXYZfromCoords(int x, int y)
    {
        return new Vector3((x-radius) * 2.0f * cellSideSize, 0, (y-radius) * 2.0f * cellSideSize);
    }

    public Vector2 CalcCoordsFromXYZ(Vector3 xyz)
    {
        return new Vector2(Mathf.RoundToInt(xyz.x / (2.0f * cellSideSize))+radius, Mathf.RoundToInt(xyz.z / (2.0f * cellSideSize))+radius);
    }

    // Start is called before the first frame update
    void OnEnable()
    {
        Map = new GameObject[radius * 2 + 1, radius * 2 + 1];

        for (int j = 0; j < radius * 2+1; j++)
            for (int i = 0; i < radius * 2 + 1; i++)
            {
                bool needAtThis = IsInZone(i, j);
               

                if (needAtThis)
                {
                    Map[i, j] = Instantiate(CellSample, this.transform);
                    Map[i, j].transform.localPosition = CalcXYZfromCoords(i, j);
                    Map[i, j].name = i + ", " + j;
                }
            }

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
