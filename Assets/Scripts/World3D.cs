using System.Collections.Generic;
using UnityEngine;

public class World3D : MonoBehaviour
{
    public TileData Tiles;
    public int Dimensions = 100;

    private static TileType[,,] World;
    private static int Size;
    public static Dictionary<TileType, TileDatum> TileTable = new Dictionary<TileType, TileDatum>();

    private void Awake()
    {
        foreach (TileDatum temp in Tiles.Tiles)
            TileTable.Add(temp.Type, temp);

        Size = Dimensions;

        World = new TileType[Size, Size, Size];

        for (int i = 0; i < Size; i++)
            for (int j = 0; j < Size; j++)
                for (int k = 0; k < Size; k++)
                    World[i, j, k] = (TileType)((i + j + k) % 2 + 1);
    }

    public static TileType GetTileType(Vector3 position, out int depth)
    {
        depth = 0;

        if (position.x < 0 || position.y < 0 || position.z < 0)
            return TileType.Air;

        return World[(int)position.x, (int)position.y, (int)position.z];
    }

    public static bool SetTileType(GameObject go, TileType type)
    {
        int x = (int)go.transform.position.x;
        int y = (int)go.transform.position.y;
        int z = (int)go.transform.position.z;

        if (x < 0 || y < 0 || z < 0 || x >= Size || y >= Size || z >= Size)
            return false;

        World[x, y, z] = type;
        return true;
    }
}
