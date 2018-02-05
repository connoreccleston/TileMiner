using System.Collections.Generic;
using UnityEngine;

public class WorldManager : MonoBehaviour
{
    public TileData Tiles;
    public int Dimensions = 100;

    private static TileType[,,] World;
    private static int Size;
    private static int Slice = 0;
    private static Direction Dir = Direction.North;
    public static Dictionary<TileType, TileDatum> TileTable = new Dictionary<TileType, TileDatum>();

    private void Awake()
	{
        foreach (TileDatum temp in Tiles.Tiles)
            TileTable.Add(temp.Type, temp);

        Size = Dimensions;

        Camera.main.transform.position = new Vector3(Size / 2, Size / 2, -10);

        World = new TileType[Size, Size, 2];

        for (int i = 0; i < Size; i++)
            for (int j = 0; j < Size; j++)
                for (int k = 0; k < 2; k++)//
                    World[i, j, k] = (TileType)((i + j + k) % 2 + 1);
	}
	
	public static TileType GetTileType(Vector3 Position, out int Depth)
    {
        Depth = 0;

        int x = -1;
        int y = (int)Position.y;
        int z = -1;
        switch (Dir)
        {
            case Direction.North: // World[x, y, Slice]                
                x = (int)Position.x;
                z = Slice;
                break;
            case Direction.East: // World[-Slice, y, x]
                x = -Slice;
                z = (int)Position.x;
                break;
            case Direction.South: // World[-x, y, -Slice]
                x = (int)-Position.x;
                z = -Slice;
                break;
            case Direction.West: // World[Slice, y, -x]
                x = Slice;
                z = (int)-Position.x;
                break;
        }

        if (x < 0 || y < 0 || z < 0 || x >= Size || y >= Size || z >= Size)
            return TileType.Air;

        TileType type =  World[x, y, z];

        while (type == TileType.Air)
        {
            switch (Dir)
            {
                case Direction.North:
                    z++;
                    break;
                case Direction.East:
                    x--;
                    break;
                case Direction.South:
                    z--;
                    break;
                case Direction.West:
                    x++;
                    break;
            }

            if (x < 0 || y < 0 || z < 0 || x >= Size || y >= Size || z >= Size)
                return TileType.Air;

            type = World[x, y, z];
            Depth++;
        }

        return type;
    }

    public static bool SetTileType(GameObject GO, TileType Type)
    {
        int x = (int)GO.transform.position.x;
        int y = (int)GO.transform.position.y;

        if (x < 0 || y < 0 || x >= Size || y >= Size)
            return false;

        switch (Dir)
        {
            case Direction.North:
                World[x, y, Slice] = Type;
                break;
            case Direction.East:
                World[-Slice, y, x] = Type;
                break;
            case Direction.South:
                World[-x, y, -Slice] = Type;
                break;
            case Direction.West:
                World[Slice, y, -x] = Type;
                break;
        }

        return true;
    }

    private enum Direction
    {
        North,
        East,
        South,
        West
    }
}
