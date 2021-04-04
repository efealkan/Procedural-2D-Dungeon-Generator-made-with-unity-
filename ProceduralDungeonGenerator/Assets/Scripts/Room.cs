using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
    public int id;
    public Vector2Int centrePos;
    public Vector2Int size;

    public Room(int id, Vector2Int centrePos, Vector2Int size)
    {
        this.id = id;
        this.centrePos = centrePos;
        this.size = size;
    }

    public bool Equals(Room other) => id == other.id;
}
