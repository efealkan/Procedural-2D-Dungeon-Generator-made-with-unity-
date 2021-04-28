using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door
{
    public Vector2Int centrePos;
    public int length;
    public bool isHorizontal;

    public Door(Vector2Int centrePos, int length, bool isHorizontal)
    {
        this.centrePos = centrePos;
        this.length = length;
        this.isHorizontal = isHorizontal;
    }
}
