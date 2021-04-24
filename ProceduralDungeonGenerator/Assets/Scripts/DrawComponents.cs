using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public class DrawComponents : MonoBehaviour
{
    [Header("Tilemap")]
    public Tilemap TILEMAP_BASE;
    public Tilemap TILEMAP_WALL;

    [Header("Tileset")]
    public TilesetMap TILESET_MAP;

    [Header("Components")]
    public DungeonGeneratorData GeneratorData;
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
        Vector2Int size = room.size;
        Vector2Int topLeftPos = room.GetTopLeftPos();
        
        int topLeftPosX = topLeftPos.x;
        int topLeftPosY = topLeftPos.y;

        for (int x = 0; x < size.x; x++)
        {
            for (int y = 0; y < size.y; y++)
            {
                Vector3Int pos = new Vector3Int(topLeftPosX + x, topLeftPosY - y, 0);
                TILEMAP_BASE.SetTile(pos, Find_Tilebase_Ground());
            }
        }

        DrawWallsOfRooms(room);
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
                    TILEMAP_BASE.SetTile(Utils.Vector2IntToVector3Int(pos), Find_Tilebase_Ground());

                    for (int j = 1; j <= corridor.corridorWidth/2; j++)
                    {
                        Vector2Int posUp = new Vector2Int(entry.Key[0].x + i, entry.Key[0].y + j);
                        Vector2Int posDown = new Vector2Int(entry.Key[0].x + i, entry.Key[0].y - j);
                        
                        //Draw Ground
                        TILEMAP_BASE.SetTile(Utils.Vector2IntToVector3Int(posUp), Find_Tilebase_Ground());
                        TILEMAP_BASE.SetTile(Utils.Vector2IntToVector3Int(posDown), Find_Tilebase_Ground());
                        
                        //Draw Walls
                        TILEMAP_WALL.SetTile(Utils.Vector2IntToVector3Int(Utils.AddNumberToV2Int(posUp, 0, 1)),
                            TILESET_MAP.DEFAULT_WALL_TILEBASE);
                        TILEMAP_WALL.SetTile(Utils.Vector2IntToVector3Int(Utils.AddNumberToV2Int(posUp, 0, 2)),
                            TILESET_MAP.TOP_EDGE_TILEBASE);
                        TILEMAP_WALL.SetTile(Utils.Vector2IntToVector3Int(posDown), TILESET_MAP.BOT_EDGE_TILEBASE);
                    }
                }
            }
            else
            {
                int distance = Math.Abs(entry.Key[0].y - entry.Key[1].y);
                for (int i = 0; i <= distance; i++)
                {
                    Vector2Int pos = new Vector2Int(entry.Key[0].x, entry.Key[0].y + i);
                    TILEMAP_BASE.SetTile(Utils.Vector2IntToVector3Int(pos), Find_Tilebase_Ground());
                    
                    for (int j = 1; j <= corridor.corridorWidth/2; j++)
                    {
                        Vector2Int posRight = new Vector2Int(entry.Key[0].x + j, entry.Key[0].y + i);
                        Vector2Int posLeft = new Vector2Int(entry.Key[0].x - j, entry.Key[0].y + i);
                        
                        TILEMAP_BASE.SetTile(Utils.Vector2IntToVector3Int(posRight), Find_Tilebase_Ground());
                        TILEMAP_BASE.SetTile(Utils.Vector2IntToVector3Int(posLeft), Find_Tilebase_Ground());
                    }
                }
            }
        }
    }

    private void DrawWallsOfRooms(Room room)
    {
        Vector2Int topLeftPos = room.GetTopLeftPos();
        Vector2Int topRightPos = room.GetTopRightPos();
        Vector2Int botLeftPos = room.GetBotLeftPos();
        Vector2Int botRightPos = room.GetBotRightPos();
        
        //Top Walls
        TILEMAP_WALL.SetTile(Utils.Vector2IntToVector3Int(Utils.AddNumberToV2Int(topLeftPos, 0, 1)), 
            TILESET_MAP.LEFT_WALL_TILEBASE);
        TILEMAP_WALL.SetTile(Utils.Vector2IntToVector3Int(Utils.AddNumberToV2Int(topRightPos, 0, 1)),
            TILESET_MAP.RIGHT_WALL_TILEBASE);
        
        TILEMAP_WALL.SetTile(Utils.Vector2IntToVector3Int(Utils.AddNumberToV2Int(topLeftPos, 0, 2)), 
            TILESET_MAP.LEFT_TOP_CORNER_TILEBASE);
        TILEMAP_WALL.SetTile(Utils.Vector2IntToVector3Int(Utils.AddNumberToV2Int(topRightPos, 0, 2)),
            TILESET_MAP.RIGHT_TOP_CORNER_TILEBASE);

        for (int i = 1; i < topRightPos.x - topLeftPos.x; i++)
        {
            TILEMAP_WALL.SetTile(Utils.Vector2IntToVector3Int(Utils.AddNumberToV2Int(topLeftPos, i, 1)), 
                TILESET_MAP.DEFAULT_WALL_TILEBASE);
            TILEMAP_WALL.SetTile(Utils.Vector2IntToVector3Int(Utils.AddNumberToV2Int(topLeftPos, i, 2)), 
                TILESET_MAP.TOP_EDGE_TILEBASE);
        }
        
        //Bottom Walls
        TILEMAP_WALL.SetTile(Utils.Vector2IntToVector3Int(botLeftPos), 
            TILESET_MAP.LEFT_BOT_CORNER_TILEBASE);
        TILEMAP_WALL.SetTile(Utils.Vector2IntToVector3Int(botRightPos),
            TILESET_MAP.RIGHT_BOT_CORNER_TILEBASE);
        
        for (int i = 1; i < botRightPos.x - botLeftPos.x; i++)
        {
            TILEMAP_WALL.SetTile(Utils.Vector2IntToVector3Int(Utils.AddNumberToV2Int(botLeftPos, i, 0)), 
                TILESET_MAP.BOT_EDGE_TILEBASE);
        }
        
        //Left Walls
        for (int i = 1; i <= topLeftPos.y - botLeftPos.y; i++)
        {
            TILEMAP_WALL.SetTile(Utils.Vector2IntToVector3Int(Utils.AddNumberToV2Int(botLeftPos, 0, i)), 
                TILESET_MAP.LEFT_EDGE_TILEBASE);
        }
        
        //Right Walls
        for (int i = 1; i <= topRightPos.y - botRightPos.y; i++)
        {
            TILEMAP_WALL.SetTile(Utils.Vector2IntToVector3Int(Utils.AddNumberToV2Int(botRightPos, 0, i)), 
                TILESET_MAP.RIGHT_EDGE_TILEBASE);
        }
        
        
        //Remove walls from door location
        foreach (var door in room.doors)
        {
            int length = door.length / 2;

            if (door.isHorizontal)
            {
                if (door.centrePos.y > room.centrePos.y)  //Door at top
                {
                    for (int i = 0; i <= length; i++)
                    {
                        TILEMAP_WALL.SetTile(Utils.Vector2IntToVector3Int(Utils.AddNumberToV2Int(door.centrePos, i,1)), null);
                        TILEMAP_WALL.SetTile(Utils.Vector2IntToVector3Int(Utils.AddNumberToV2Int(door.centrePos, -i,1)), null);
                        TILEMAP_WALL.SetTile(Utils.Vector2IntToVector3Int(Utils.AddNumberToV2Int(door.centrePos, i,2)), null);
                        TILEMAP_WALL.SetTile(Utils.Vector2IntToVector3Int(Utils.AddNumberToV2Int(door.centrePos, -i,2)), null);
                    }   
                }
                else //Door at bottom
                {
                    for (int i = 0; i <= length; i++)
                    {
                        TILEMAP_WALL.SetTile(Utils.Vector2IntToVector3Int(Utils.AddNumberToV2Int(door.centrePos, i,0)), null);
                        TILEMAP_WALL.SetTile(Utils.Vector2IntToVector3Int(Utils.AddNumberToV2Int(door.centrePos, -i,0)), null);
                    }   
                }
            }
            else //Door at either left or right
            {
                for (int i = 0; i <= length; i++)
                {
                    TILEMAP_WALL.SetTile(Utils.Vector2IntToVector3Int(Utils.AddNumberToV2Int(door.centrePos, 0,i)), null);
                    TILEMAP_WALL.SetTile(Utils.Vector2IntToVector3Int(Utils.AddNumberToV2Int(door.centrePos, 0,-i)), null);
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
    
    private TileBase Find_Tilebase_Ground()
    {
        if (Utils.RandomInt(0, 10) < GeneratorData.oldnessLevel && TILESET_MAP.ALTERNATIVE_GROUND_TILEBASE.Length > 0)
            return TILESET_MAP.ALTERNATIVE_GROUND_TILEBASE[Utils.RandomInt(0, TILESET_MAP.ALTERNATIVE_GROUND_TILEBASE.Length)];
        return TILESET_MAP.DEFAULT_GROUND_TILEBASE;
    }
}
