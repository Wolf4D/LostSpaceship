﻿using System.Collections;
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

    public Vector3 CalcXYZfromCoords(int x, int y)
    {
        return new Vector3(x * 2.0f * cellSideSize, 0, y * 2.0f * cellSideSize);
    }

    public Vector2 CalcCoordsFromXYZ(Vector3 xyz)
    {
        return new Vector2(Mathf.RoundToInt(xyz.x / (2.0f * cellSideSize)), Mathf.RoundToInt(xyz.z / (2.0f * cellSideSize)));
    }

    // Start is called before the first frame update
    void OnEnable()
    {
        radius += 1;
        Map = new GameObject[radius * 2 - 1, radius * 2 - 1];

        for (int j = 0; j < radius*2-1; j++)
            for (int i = 0; i < radius * 2 - 1; i++)
            {
                bool needAtThis = false;
                switch (radius)
                {
                    case 1: if ((i == 1) && (j == 1)) needAtThis = true; break;
                    case 2: if (r2map[i, j] == 1) needAtThis = true; break;
                    case 3: if (r3map[i, j] == 1) needAtThis = true; break;
                    case 4: if (r4map[i, j] == 1) needAtThis = true; break;
                    case 5: if (r5map[i, j] == 1) needAtThis = true; break;
                }

                if (needAtThis)
                {
                    Map[i, j] = Instantiate(CellSample, this.transform);
                    Map[i, j].transform.localPosition = CalcXYZfromCoords(i-radius+1, j - radius+1);
                    Map[i, j].name = i + ", " + j;
                }
            }
        radius -= 1;

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
