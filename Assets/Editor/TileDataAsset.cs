using UnityEditor;

public class TileDataAsset
{
    [MenuItem("Assets/Create/Tile Data")]
    public static void CreateAsset()
    {
        ScriptableObjectUtility.CreateAsset<TileData>();
    }
}
