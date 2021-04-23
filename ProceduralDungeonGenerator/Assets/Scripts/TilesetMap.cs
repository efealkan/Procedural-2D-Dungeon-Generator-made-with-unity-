using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Tilemaps;

[Serializable]
public class TilesetMap
{
    public TileBase DEFAULT_GROUND_TILEBASE;
    public TileBase[] ALTERNATIVE_GROUND_TILEBASE;

    public TileBase DEFAULT_WALL_TILEBASE;
    public TileBase[] ALTERNATIVE_WALL_TILEBASE;
    public TileBase LEFT_WALL_TILEBASE;
    public TileBase RIGHT_WALL_TILEBASE;
    
    public TileBase LEFT_TOP_CORNER_TILEBASE;
    public TileBase RIGHT_TOP_CORNER_TILEBASE;
    public TileBase LEFT_BOT_CORNER_TILEBASE;
    public TileBase RIGHT_BOT_CORNER_TILEBASE;

    public TileBase TOP_EDGE_TILEBASE;
    public TileBase BOT_EDGE_TILEBASE;
    public TileBase LEFT_EDGE_TILEBASE;
    public TileBase RIGHT_EDGE_TILEBASE;
}
