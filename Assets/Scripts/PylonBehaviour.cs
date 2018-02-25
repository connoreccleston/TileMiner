using System.Collections.Generic;
using UnityEngine;

public class PylonBehaviour : SpecialBehaviour
{
    private SpriteRenderer SR;
    private PylonData PD;

    private HashSet<PylonBehaviour> Attached = new HashSet<PylonBehaviour>();
    private HashSet<LineRenderer> Wires = new HashSet<LineRenderer>();

    //private static Sprite[] Inactive;
    //private static Sprite[] Active;
    private static Sprite[] Sheet;
    private static GameObject Wire;
    private static GameObject Container;
    private void Awake()
    {
        //Utility.LoadAll(ref Inactive, "Sprites/pylon_off");
        //Utility.LoadAll(ref Active, "Sprites/pylon_on");
        Util.LoadAll(ref Sheet, "Sprites/pylon");
        Util.Load(ref Wire, "LineRenderer");
        Util.Find(ref Container, "LineRendererContainer");
        PD = new PylonData(WorldNew.Convert(transform.position), 5);
    }

    private void Start()
	{
        //PD = new PylonData(new Vector3Int((int)transform.position.x, (int)transform.position.y, (int)PlayerController.WorldPos.z), 5);

        GameObject go = new GameObject("PylonSprite");
        go.transform.SetParent(transform, false);

        SR = go.AddComponent<SpriteRenderer>();
        SetSprite();
        SR.sortingLayerName = "TransTiles";

        GenerateWires();
	}

    // Will depend on facing
    private void SetSprite()
    {
        int index = 0;

        if (PD.SourceDir != Vector3Int.zero)
        {
            if (PD.SourceDir == new Vector3Int(0, 1, 0))
                index = 2;
            else if (PD.SourceDir == new Vector3Int(0, -1, 0))
                index = 1;
            else if (PD.SourceDir == new Vector3Int(-1, 0, 0))
                index = 3;
            else if (PD.SourceDir == new Vector3Int(1, 0, 0))
                index = 4;
        }
        else
        {
            if (WorldNew.GetTileType(PD.Location + new Vector3Int(0, 1, 0)) != TileType.Air)
                index = 2;
            else if (WorldNew.GetTileType(PD.Location + new Vector3Int(0, -1, 0)) != TileType.Air)
                index = 1;
            else if (WorldNew.GetTileType(PD.Location + new Vector3Int(-1, 0, 0)) != TileType.Air)
                index = 3;
            else if (WorldNew.GetTileType(PD.Location + new Vector3Int(1, 0, 0)) != TileType.Air)
                index = 4;
        }

        SR.sprite = PD.Powered ? Sheet[5 + index] : Sheet[index];
    }

    private void GenerateWires()
    {
        PylonBehaviour[] allPossible = FindObjectsOfType<PylonBehaviour>();
        foreach (PylonBehaviour pb in allPossible)
        {
            if (pb == this)
                continue;

            float distance = Vector3.Distance(transform.position, pb.transform.position);
            if (!Attached.Contains(pb) && distance <= Mathf.Min(PD.Radius, pb.PD.Radius))
            {
                LineRenderer lr = Instantiate(Wire, Container.transform).GetComponent<LineRenderer>();
                Catenary.Generate(lr, transform.position, pb.transform.position, distance * distance / 3, distance / 9);

                Attached.Add(pb);
                pb.Attached.Add(this);
                Wires.Add(lr);
                pb.Wires.Add(lr);
            }
        }
    }

    bool oldPowered;
    private void Update()
    {
        if (oldPowered != PD.Powered)
        {
            SetSprite();
            oldPowered = PD.Powered;
        }
    }

    private void OnDestroy()
    {
        foreach (LineRenderer lr in Wires)
            if (lr != null)
                lr.transform.GetComponent<WireFX>().FadeOut(true);
    }
}

public class PylonData : PersistentData
{
    public bool Powered { get; private set; }
    public Vector3Int SourceDir { get; private set; }

    //public readonly Vector3Int Location;
    public readonly int Radius;

    private List<PylonData> Attached = new List<PylonData>();
    //private List<PowerSource> Sources = new List<PowerSource>();

    public PylonData(Vector3Int location, int radius) : base(location)
    {
        Powered = false;

        //Location = location;
        Radius = radius;

        //World.SetSpecial(Location, this);

        FindConnected();
    }

    private void FindConnected()
    {
        bool anyPowered = false;

        for (int x = Location.x - Radius; x <= Location.x + Radius; x++)
        {
            for (int y = Location.y - Radius; y <= Location.y + Radius; y++)
            {
                for (int z = Location.z - Radius; z <= Location.z + Radius; z++)
                {
                    if (Vector3.Distance(Location, new Vector3(x, y, z)) > Radius)
                        continue;

                    PersistentData tile = WorldNew.GetSpecial(x, y, z);

                    //if (tile != null)
                    //    Debug.Log(x + " " + y + " " + z + " " + tile);

                    PylonData pd = tile as PylonData;
                    if (pd != null && pd != this)
                    {
                        Attached.Add(pd);
                        if (pd.Powered)
                            anyPowered = true;
                    }
                    //else
                    //{
                    //    PowerSource ps = tile as PowerSource;
                    //    if (ps != null)
                    //    {
                    //        Sources.Add(ps);
                    //        if (ps.Powered)
                    //            anyPowered = true;
                    //    }
                    //}
                }
            }
        }

        // turn into World function
        Vector3Int[] Surrounding = new Vector3Int[]
        {
            new Vector3Int(1, 0, 0), new Vector3Int(0, 1, 0), new Vector3Int(0, 0, 1),
            new Vector3Int(-1, 0, 0), new Vector3Int(0, -1, 0), new Vector3Int(0, 0, -1)
        };
        foreach (var item in Surrounding)
        {
            var data = WorldNew.GetSpecial(Location + item);
            if (data != null && data.GetType() == typeof(PowerSource))
            {
                anyPowered = true;
                SourceDir = item;
                break;
            }
        }

        if (anyPowered)
            PropagatePower();
    }

    private void PropagatePower()
    {
        Powered = true;

        foreach (PylonData pd in Attached)
            if (!pd.Powered)
                pd.PropagatePower();
    }

    //~PylonData()
    //{

    //}

    //private void Remove()
    //{

    //}
}
