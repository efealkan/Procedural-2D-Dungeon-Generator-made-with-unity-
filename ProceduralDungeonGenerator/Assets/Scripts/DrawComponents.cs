using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public class DrawComponents : MonoBehaviour
{
    public Tilemap tilemap;
    public TileBase tilebase;
    public TileBase tilebase2;
    
    public void DrawRoom(Room room)
    {
        Vector2Int centre = room.centrePos;
        Vector2Int size = room.size;

        int topLeftPosX = centre.x - size.x / 2;
        int topLeftPosY = centre.y + size.y / 2;

        for (int x = 0; x < size.x; x++)
        {
            for (int y = 0; y < size.y; y++)
            {
                Vector3Int pos = new Vector3Int(topLeftPosX + x, topLeftPosY - y, 0);
                tilemap.SetTile(pos, tilebase);
            }
        }
    }

    public void DrawCorridorRays(List<Corridor> corridors)
    {
        foreach (var corridor in corridors)
        {
            DrawLine(Utils.Vector2IntToVector3(corridor.room1.centrePos), Utils.Vector2IntToVector3(corridor.room2.centrePos), Color.blue);
        }
    }

    public void DrawCorridor(Corridor corridor)
    {
        foreach (var entry in corridor.corridorVertexMap)
        {
            if (entry.Value == DirectionOfTheCorridor.horizontal)
            {
                int distance = Math.Abs(entry.Key[0].x - entry.Key[1].x);
                for (int i = 0; i <= distance; i++)
                {
                    Vector2Int pos = new Vector2Int(entry.Key[0].x + i, entry.Key[0].y);
                    tilemap.SetTile(Utils.Vector2IntToVector3Int(pos), tilebase2);
                }
            }
            else
            {
                int distance = Math.Abs(entry.Key[0].y - entry.Key[1].y);
                for (int i = 0; i <= distance; i++)
                {
                    Vector2Int pos = new Vector2Int(entry.Key[0].x, entry.Key[0].y + i);
                    tilemap.SetTile(Utils.Vector2IntToVector3Int(pos), tilebase2);
                }
            }
        }
    }
    
    private void DrawLine(Vector3 start, Vector3 end, Color color)
    {
        GameObject myLine = new GameObject();
        myLine.transform.position = start;
        myLine.AddComponent<LineRenderer>();
        LineRenderer lr = myLine.GetComponent<LineRenderer>();
        lr.SetColors(color, color);
        lr.SetWidth(0.5f, 0.5f);
        lr.sortingOrder = 10;
        lr.SetPosition(0, start);
        lr.SetPosition(1, end);
    }
}
