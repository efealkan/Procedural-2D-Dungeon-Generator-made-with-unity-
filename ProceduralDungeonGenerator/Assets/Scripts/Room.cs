using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room
{
    public int id;
    public Vector2Int centrePos;
    public Vector2Int size;
    
    public List<Door> doors = new List<Door>();
    public HashSet<Vector2Int> roomTiles = new HashSet<Vector2Int>();

    public Room(int id, Vector2Int centrePos, Vector2Int size)
    {
        this.id = id;
        this.centrePos = centrePos;
        this.size = size;

        FillRoomTilesSet();
    }

    public void SetDoor(Vector2Int doorCentrePos, int doorLength, bool isDoorHorizontal)
    {
        Door door = new Door(doorCentrePos, doorLength, isDoorHorizontal);
        doors.Add(door);
    }

    private void FillRoomTilesSet()
    {
        Vector2Int size = this.size;
        Vector2Int topLeftPos = this.GetTopLeftPos();
        
        int topLeftPosX = topLeftPos.x;
        int topLeftPosY = topLeftPos.y;

        for (int x = 0; x < size.x; x++)
        {
            for (int y = 0; y < size.y; y++)
            {
                Vector2Int pos = new Vector2Int(topLeftPosX + x, topLeftPosY - y);
                roomTiles.Add(pos);
            }
        }
    }
    
    public Vector2Int GetTopLeftPos() => new Vector2Int(centrePos.x - size.x / 2, centrePos.y + size.y / 2);
    public Vector2Int GetTopRightPos() => new Vector2Int(centrePos.x + size.x / 2, centrePos.y + size.y / 2);
    public Vector2Int GetBotLeftPos() => new Vector2Int(centrePos.x - size.x / 2, centrePos.y - size.y / 2);
    public Vector2Int GetBotRightPos() => new Vector2Int(centrePos.x + size.x / 2, centrePos.y - size.y / 2);
    
    public bool Equals(Room other) => id == other.id;
    
    // centrePos.x - Mathf.CeilToInt((float) size.x / 2), centrePos.y + Mathf.CeilToInt((float) size.y / 2));
}
