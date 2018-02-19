using System.Collections.Generic;
using UnityEngine;

public class PylonBehaviour : SpecialBehaviour
{
    private SpriteRenderer SR;
    private PylonData PD;
    //private Rigidbody2D RB;

    private HashSet<PylonBehaviour> Attached = new HashSet<PylonBehaviour>();
    private HashSet<LineRenderer> Wires = new HashSet<LineRenderer>();

    private static Sprite Inactive;
    private static Sprite Active;
    private static GameObject Wire;
    private static GameObject Container;
    private void Awake()
    {
        Utility.Load(ref Inactive, "Sprites/repeater");
        Utility.Load(ref Active, "Sprites/repeater_on");
        Utility.Load(ref Wire, "LineRenderer");
        Utility.Find(ref Container, "LineRendererContainer");
    }

    private void Start()
	{
        PD = new PylonData(new Vector3Int((int)transform.position.x, (int)transform.position.y, (int)PlayerController.WorldPos.z), 5);

        GameObject go = new GameObject("PylonSprite");
        go.transform.SetParent(transform, false);

        //RB = go.AddComponent<Rigidbody2D>();
        //RB.bodyType = RigidbodyType2D.Static;

        SR = go.AddComponent<SpriteRenderer>();
        SR.sprite = PD.Powered ? Active : Inactive;
        SR.sortingLayerName = "TransTiles";

        GenerateWires();
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
                //WithPhysics(pb, distance);

                LineRenderer lr = Instantiate(Wire, Container.transform).GetComponent<LineRenderer>();
                Catenary.Generate(lr, transform.position, pb.transform.position, distance * distance / 3, distance / 9);

                Attached.Add(pb);
                pb.Attached.Add(this);
                Wires.Add(lr);
                pb.Wires.Add(lr);
            }
        }
    }

    //private void WithPhysics(PylonBehaviour pb, float distance)
    //{
    //    GameObject lastWire = Instantiate(Wire, transform);
    //    lastWire.GetComponents<HingeJoint2D>()[0].connectedBody = RB;

    //    for (int i = 1; i < distance + 5; i++)
    //    {
    //        GameObject wire = Instantiate(Wire, transform);
    //        wire.GetComponents<HingeJoint2D>()[0].connectedBody = lastWire.GetComponent<Rigidbody2D>();
    //        lastWire.GetComponents<HingeJoint2D>()[1].connectedBody = wire.GetComponent<Rigidbody2D>();
    //        lastWire = wire;
    //    }

    //    GameObject wire2 = Instantiate(Wire, transform);
    //    wire2.GetComponents<HingeJoint2D>()[0].connectedBody = lastWire.GetComponent<Rigidbody2D>();
    //    lastWire.GetComponents<HingeJoint2D>()[1].connectedBody = wire2.GetComponent<Rigidbody2D>();
    //    wire2.GetComponents<HingeJoint2D>()[1].connectedBody = pb.RB;
    //}

    bool oldPowered;
    private void Update()
    {
        if (oldPowered != PD.Powered)
        {
            SR.sprite = PD.Powered ? Active : Inactive;
            oldPowered = PD.Powered;
        }
    }

    private void OnDestroy()
    {
        foreach (LineRenderer lr in Wires)
            if (lr != null)
                Destroy(lr.gameObject);
    }
}

public class PylonData : PersistentData
{
    public bool Powered { get; private set; }

    //public readonly Vector3Int Location;
    public readonly int Radius;

    private List<PylonData> Attached = new List<PylonData>();
    private List<PowerSource> Sources = new List<PowerSource>();

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

                    PersistentData tile = World.GetSpecial(x, y, z);

                    //if (tile != null)
                    //    Debug.Log(x + " " + y + " " + z + " " + tile);

                    PylonData pd = tile as PylonData;
                    if (pd != null && pd != this)
                    {
                        Attached.Add(pd);
                        if (pd.Powered)
                            anyPowered = true;
                    }
                    else
                    {
                        PowerSource ps = tile as PowerSource;
                        if (ps != null)
                        {
                            Sources.Add(ps);
                            if (ps.Powered)
                                anyPowered = true;
                        }
                    }
                }
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

