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

    public WallTile[] WALL_TILES;
}

[Serializable]
public class WallTile
{
    public bool hasTileUp;
    public bool hasTileDown;
    public bool hasTileLeft;
    public bool hasTileRight;
    public bool hasTileUpLeft;
    public bool hasTileUpRight;
    public bool hasTileDownLeft;
    public bool hasTileDownRight;

    public TileBase tileBase;

    [HideInInspector] public string code;
}
