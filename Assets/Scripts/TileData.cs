using System.Collections.Generic;
using UnityEngine;

public enum TileType : byte
{
    Air,
    Dirt,
    Stone,
    Pylon,
    Generator,
    Void
}

public class TileData : ScriptableObject
{
    [SerializeField] private bool Initialized = false;
    [SerializeField] private int OldSize;
    [SerializeField] private int EnumSize;

    [SerializeField] private List<TileDatum> Tiles = new List<TileDatum>();

    public TileDatum this[TileType type]
    {
        get
        {
            Init();
            return Tiles[(int)type];
        }
    }

    private void Awake() { Init(); }
    private void OnEnable() { Init(); }

    public void Init()
    {
        if (Initialized) return;

        for (byte i = 0; i < byte.MaxValue; i++)
        {
            if (System.Enum.IsDefined(typeof(TileType), i))
            {
                if (i < Tiles.Count)
                {
                    if ((TileType)i != Tiles[i].Type)
                    {
                        Debug.Log("at index " + i + " " + (TileType)i + " != " + Tiles[i].Type);
                        Tiles[i] = new TileDatum((TileType)i);
                    }
                }
                else
                {
                    Tiles.Add(new TileDatum((TileType)i));
                }
            }
            else
            {
                if (i < Tiles.Count)
                    Tiles.RemoveRange(i, Tiles.Count - i);
                break;
            }
        }

        OldSize = Tiles.Count;
        EnumSize = System.Enum.GetValues(typeof(TileType)).Length;

        Tiles.TrimExcess();

        Initialized = true;
    }

    private void OnValidate()
    {        
        bool enumChanged = EnumSize != System.Enum.GetValues(typeof(TileType)).Length;
        if (Tiles.Count != OldSize || enumChanged)
        {
            if (Tiles.Count < OldSize)
                Debug.LogWarning("End of \"" + name + "\" was truncated.");

            if (enumChanged)
                Debug.LogWarning("\"" + name + "\" was updated because enum was updated.");

            Initialized = false;
            Init();
        }
    }
}

[System.Serializable]
public class TileDatum
{
    [SerializeField, HideInInspector] private string displayName;
    public string Name { get { return displayName; } }

    [SerializeField, HideInInspector] private TileType type;
    public TileType Type { get { return type; } }

    [SerializeField] private Sprite sprite;
    public Sprite Sprite { get { return sprite; } }

    [SerializeField] private float hardness;
    public float Hardness { get { return hardness; } }

    [SerializeField, MonoScript] private string behaviour;
    public string Behaviour { get { return behaviour; } }

    [SerializeField] private bool transparent;
    public bool Transparent { get { return transparent; } }

    [SerializeField] private bool nonColliding;
    public bool NonColliding { get { return nonColliding; } }

    public TileDatum(TileType type)
    {
        this.type = type;
        displayName = type.ToString();
    }
}
