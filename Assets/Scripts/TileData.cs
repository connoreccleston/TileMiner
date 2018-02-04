using UnityEngine;

public class TileData : ScriptableObject
{
    public TileDatum[] Tiles;
}

[System.Serializable]
public struct TileDatum
{
    public TileType Type;
    public Sprite Sprite;
    public float Hardness;
}

public enum TileType : byte
{
    Air,
    Dirt,
    Stone
}
