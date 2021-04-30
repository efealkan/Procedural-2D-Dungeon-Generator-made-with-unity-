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

    private void SetupTileWallCodes()
    {
        foreach (var tile in TILESET_MAP.WALL_TILES)
        {
            string code = "";
            
            code += tile.hasTileUp ? "1" : "0";
            code += tile.hasTileDown ? "1" : "0";
            code += tile.hasTileLeft ? "1" : "0";
            code += tile.hasTileRight ? "1" : "0";
            code += tile.hasTileUpLeft ? "1" : "0";
            code += tile.hasTileUpRight ? "1" : "0";
            code += tile.hasTileDownLeft ? "1" : "0";
            code += tile.hasTileDownRight ? "1" : "0";

            tile.code = code;
            if (tile.name == "") tile.name = tile.tileBase.name;
        }
    }

    public void Draw(HashSet<Vector2Int> groundTilePositions, List<Room> rooms)
    {
        SetupTileWallCodes();
        DrawGroundTiles(groundTilePositions);
        DrawWallTiles(groundTilePositions);
    }

    public void ClearAllTiles()
    {
        TILEMAP_BASE.ClearAllTiles();
        TILEMAP_WALL.ClearAllTiles();
        DestroyCorridorRays();
        corridorRaysToggled = false;
    }
    
    private void DrawGroundTiles(HashSet<Vector2Int> groundTilePositions)
    {
        foreach (var pos in groundTilePositions)
        {
            TILEMAP_BASE.SetTile(Utils.Vector2IntToVector3Int(pos), Find_Tilebase_Ground());
        }
    }
    
    private void DrawWallTiles(HashSet<Vector2Int> groundTilePositions)
    {
        foreach (var pos in groundTilePositions)
        {
            string code = FindSuitableWallType(groundTilePositions, pos);
            TileBase wall = FindWallTileBase(code);

            if (wall == null) continue;
            
            TILEMAP_WALL.SetTile(Utils.Vector2IntToVector3Int(pos), wall);
        }
    }

    private string FindSuitableWallType(HashSet<Vector2Int> groundTilePositions, Vector2Int pos)
    {
        Vector2Int up = Utils.AddNumberToV2Int(pos, 0, 1);
        Vector2Int down = Utils.AddNumberToV2Int(pos, 0, -1);
        Vector2Int left = Utils.AddNumberToV2Int(pos, -1, 0);
        Vector2Int right = Utils.AddNumberToV2Int(pos, 1, 0);
        Vector2Int upLeft = Utils.AddNumberToV2Int(pos, -1, 1);
        Vector2Int upRight = Utils.AddNumberToV2Int(pos, 1, 1);
        Vector2Int downLeft = Utils.AddNumberToV2Int(pos, -1, -1);
        Vector2Int downRight = Utils.AddNumberToV2Int(pos, 1, -1);

        string code = "";

        code += groundTilePositions.Contains(up) ? "1" : "0";
        code += groundTilePositions.Contains(down) ? "1" : "0";
        code += groundTilePositions.Contains(left) ? "1" : "0";
        code += groundTilePositions.Contains(right) ? "1" : "0";
        code += groundTilePositions.Contains(upLeft) ? "1" : "0";
        code += groundTilePositions.Contains(upRight) ? "1" : "0";
        code += groundTilePositions.Contains(downLeft) ? "1" : "0";
        code += groundTilePositions.Contains(downRight) ? "1" : "0";
        return code;
    }

    private TileBase FindWallTileBase(string code)
    {
        foreach (var tile in TILESET_MAP.WALL_TILES)
        {
            if (tile.code == code) return tile.tileBase;
        }

        return null;
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
