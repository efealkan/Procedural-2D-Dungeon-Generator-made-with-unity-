using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Basemap : MonoBehaviour
{
    public int numberOfRows;
    public int numberOfCols;
    public Vector2Int size;

    public Basemap(int numberOfRows, int numberOfCols, Vector2Int size)
    {
        this.numberOfRows = numberOfRows;
        this.numberOfCols = numberOfCols;
        this.size = size;
    }
}
