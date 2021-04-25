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

    public void DrawCorridors(HashSet<Vector2Int> corridorPositions, List<Corridor> corridors)
    {
        foreach (var pos in corridorPositions)
        {
            TILEMAP_BASE.SetTile(Utils.Vector2IntToVector3Int(pos), Find_Tilebase_Ground());
        }

        foreach (var corridor in corridors)
        {
            DrawCorridorWalls(corridorPositions, corridor);
        }
    }

    private void DrawCorridorWalls(HashSet<Vector2Int> corridorPositions, Corridor corridor)
    {
        int curPosX = corridor.topLeft.x;
        int curPosY = corridor.topLeft.y + 1;

        int k = 0;
        
        while (true)
        {
            if (k > 10) return;
            
            if (curPosX >= corridor.room1.GetTopLeftPos().x && curPosX <= corridor.room1.GetTopRightPos().x &&
                curPosY >= corridor.room1.GetBotLeftPos().y && curPosY <= corridor.room1.GetTopLeftPos().y) return;
            
            if (curPosX >= corridor.room2.GetTopLeftPos().x && curPosX <= corridor.room2.GetTopRightPos().x &&
                curPosY >= corridor.room2.GetBotLeftPos().y && curPosY <= corridor.room2.GetTopLeftPos().y) return;
            
            Vector2Int curPos = new Vector2Int(curPosX, curPosY);

            if (!corridorPositions.Contains(curPos))
            {
                if (corridorPositions.Contains(new Vector2Int(curPosX, curPosY - 1)))
                {
                    TILEMAP_WALL.SetTile(Utils.Vector2IntToVector3Int(curPos), TILESET_MAP.DEFAULT_WALL_TILEBASE);
                    curPosX++;
                }
                else
                {
                    TILEMAP_WALL.SetTile(Utils.Vector2IntToVector3Int(curPos), TILESET_MAP.LEFT_EDGE_TILEBASE);
                    curPosY--;
                }
            }
            else
            {
                curPosX -= 1;
                curPosY += 1;
                TILEMAP_WALL.SetTile(Utils.Vector2IntToVector3Int(curPos), TILESET_MAP.LEFT_EDGE_TILEBASE);
            }

            k++;
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
