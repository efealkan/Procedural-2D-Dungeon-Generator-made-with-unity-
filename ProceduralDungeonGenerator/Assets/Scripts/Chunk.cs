using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chunk : MonoBehaviour
{
    public int id;
    public Vector2Int size;
    public Vector2Int topLeftPos;
    public Vector2Int topRightPos;
    public Vector2Int bottomLeftPos;

    public Room room;

    public Chunk(int id, Vector2Int size, Vector2Int topLeftPos, Vector2Int topRightPos, Vector2Int bottomLeftPos)
    {
        this.id = id;
        this.size = size;
        this.topLeftPos = topLeftPos;
        this.topRightPos = topRightPos;
        this.bottomLeftPos = bottomLeftPos;
    }

    public void SetRoom(Room room) => this.room = room;
    public bool IsChunkEmpty() => room == null;
}
