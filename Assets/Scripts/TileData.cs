using UnityEngine;

public class TileData : ScriptableObject
{
    public TileDatum[] Tiles;

    public TileDatum Find(TileType type)
    {
        for (int i = 0; i < Tiles.Length; i++)
            if (Tiles[i].Type == type)
                return Tiles[i];

        return new TileDatum();
    }
}

[System.Serializable]
public struct TileDatum
{
    public TileType Type;
    //public TileProperties Properties;
    public Sprite Sprite;
    public float Hardness;
    [MonoScript] public string Behaviour;
    public bool Transparent;
    public bool NonColliding;
}

public enum TileType : byte
{
    Air,
    Dirt,
    Stone,
    Pylon
}

//[System.Flags]
//public enum TileProperties
//{
//    Transparent = 1 << 0,
//    NonColliding = 1 << 1
//}
