using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public class DrawComponents : MonoBehaviour
{
    public Tilemap Tilemap_Base;
    public Tilemap Tilemap_Wall;
    public Tilemap Tilemap_Doorway;

    public TileBase[] Tilebase_Ground;
    
    public TileBase tilebase;
    public TileBase tilebase2;

    public Transform debugRays;
    public Material debugRayMaterial;

    private List<Corridor> corridors;
    private List<GameObject> debugCorridorRays = new List<GameObject>();

    private void Update()
    {
        CorridorRaysHandler();
    }
    
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
                Tilemap_Base.SetTile(pos, tilebase);
            }
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
                    Tilemap_Base.SetTile(Utils.Vector2IntToVector3Int(pos), tilebase2);

                    for (int j = 1; j <= corridor.corridorWidth/2; j++)
                    {
                        Vector2Int posUp = new Vector2Int(entry.Key[0].x + i, entry.Key[0].y + j);
                        Vector2Int posDown = new Vector2Int(entry.Key[0].x + i, entry.Key[0].y - j);
                        
                        Tilemap_Base.SetTile(Utils.Vector2IntToVector3Int(posUp), tilebase2);
                        Tilemap_Base.SetTile(Utils.Vector2IntToVector3Int(posDown), tilebase2);
                    }
                }
            }
            else
            {
                int distance = Math.Abs(entry.Key[0].y - entry.Key[1].y);
                for (int i = 0; i <= distance; i++)
                {
                    Vector2Int pos = new Vector2Int(entry.Key[0].x, entry.Key[0].y + i);
                    Tilemap_Base.SetTile(Utils.Vector2IntToVector3Int(pos), tilebase2);
                    
                    for (int j = 1; j <= corridor.corridorWidth/2; j++)
                    {
                        Vector2Int posRight = new Vector2Int(entry.Key[0].x + j, entry.Key[0].y + i);
                        Vector2Int posLeft = new Vector2Int(entry.Key[0].x - j, entry.Key[0].y + i);
                        
                        Tilemap_Base.SetTile(Utils.Vector2IntToVector3Int(posRight), tilebase2);
                        Tilemap_Base.SetTile(Utils.Vector2IntToVector3Int(posLeft), tilebase2);
                    }
                }
            }
        }
    }
    
    public void SetupCorridorRays(List<Corridor> corridors)
    {
        this.corridors = corridors;
    }

    private bool corridorRaysToggled = false;
    private void CorridorRaysHandler()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            if (corridorRaysToggled)
            {
                corridorRaysToggled = false;
                DestroyCorridorRays();
            }
            else
            {
                corridorRaysToggled = true;
                DrawCorridorRays();
            }
        }
    }
    
    private void DrawCorridorRays()
    {
        foreach (var corridor in corridors)
        {
            DrawLine(Utils.Vector2IntToVector3(corridor.room1.centrePos), Utils.Vector2IntToVector3(corridor.room2.centrePos), Color.green);
        }
    }

    private void DestroyCorridorRays()
    {
        foreach (var ray in debugCorridorRays)
        {
            Destroy(ray);
        }
        
        debugCorridorRays = new List<GameObject>();
    }
    
    /// <summary>
    /// Draw representative lines of corridors in MST.
    /// </summary>
    private void DrawLine(Vector3 start, Vector3 end, Color color)
    {
        GameObject myLine = new GameObject();

        myLine.name = "DebugCorridorRay";
        myLine.transform.parent = debugRays;
        myLine.transform.position = start;
        myLine.AddComponent<LineRenderer>();
        
        debugCorridorRays.Add(myLine);
        
        LineRenderer lr = myLine.GetComponent<LineRenderer>();

        lr.material = debugRayMaterial;
        
        lr.startColor = color;
        lr.endColor = color;
        lr.startWidth = 0.5f;
        lr.sortingOrder = 100;
        lr.SetPosition(0, start);
        lr.SetPosition(1, end);
    }
}
