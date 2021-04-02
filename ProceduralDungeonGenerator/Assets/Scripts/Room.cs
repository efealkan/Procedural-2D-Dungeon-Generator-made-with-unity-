using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
    public int roomId;
    public Vector2Int roomCentrePos;
    public Vector2Int size;

    public Room(int roomId, Vector2Int roomCentrePos, Vector2Int size)
    {
        this.roomId = roomId;
        this.roomCentrePos = roomCentrePos;
        this.size = size;
    }

    public bool Equals(Room other) => roomId == other.roomId;
}
