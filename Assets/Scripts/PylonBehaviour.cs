using System.Collections.Generic;
using UnityEngine;

public class PylonBehaviour : MonoBehaviour
{
    private GameObject GO;
    private SpriteRenderer SR;
    private PylonData PD;

    private static Sprite Inactive;
    private static Sprite Active;
    private void Awake()
    {
        if (Inactive == null)
            Inactive = Resources.Load<Sprite>("Sprites/repeater");
        if (Active == null)
            Active = Resources.Load<Sprite>("Sprites/repeater_on"); 
    }

	private void Start()
	{
        PD = new PylonData(new Vector3Int((int)transform.position.x, (int)transform.position.y, (int)PlayerController.Position.z), 5);

        GO = new GameObject("PylonSprite");
        GO.transform.SetParent(transform, false);
        SR = GO.AddComponent<SpriteRenderer>();
        SR.sprite = PD.Powered ? Active : Inactive;
        SR.sortingOrder = 1;
	}

    bool oldPowered;
    private void Update()
    {
        if (oldPowered != PD.Powered)
        {
            SR.sprite = PD.Powered ? Active : Inactive;
            oldPowered = PD.Powered;
        }
    }
}

public class PylonData : TileMetadata
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
                    TileMetadata tile = World.GetSpecial(x, y, z);

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

