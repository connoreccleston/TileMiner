using UnityEngine;

public class TileBehaviour : MonoBehaviour
{
    private TileDatum Data;
    private float Damage = 0;
    //private int Depth;
    public int Depth { get; private set; }

    private bool Animating = false;
    private Color NewColor;

    private Vector3 LastPosition;

    private SpriteRenderer SR;
    private BoxCollider2D BC;

    //public static int NumAnimating = 0;
    public const float FlipAllTime = 1.5f;
    public const float FlipOneTime = 1;

    private static TileData TileData;
    private static GameObject LRContainer;
    private void Awake()
    {
        Util.Load(ref TileData, "Tiles");
        Util.Find(ref LRContainer, "LineRendererContainer");
    }

    private void Start()
	{
        LastPosition = transform.position;
        SR = gameObject.AddComponent<SpriteRenderer>();
        SR.sortingLayerName = "Tiles";
        BC = gameObject.AddComponent<BoxCollider2D>();
        BC.size = new Vector2(1, 1);
        ResetTile(false);
    }

    public void ResetTile(bool animate)
    {
        // needs to change based on direction
        Vector3 position = new Vector3(transform.position.x, transform.position.y, PlayerController.WorldPos.z);
        //Debug.Log(position);
        int depth;
        Data = TileData[World.GetTileType(position, out depth)];
        Depth = depth;

        //if (depth != 0)
        //{
        //    Sprite temp = Data.Sprite;
        //    Data = WorldManager.TileTable[TileType.Air];
        //    Data.Sprite = temp;
        //}

        foreach (SpecialBehaviour sb in transform.GetComponents<SpecialBehaviour>())
            Destroy(sb);

        ChangeSprite(Depth, animate);
        name = Data.Name;
        ResetDamage();
    }

    public static void ResetAll()
    {
        //foreach (SpecialBehaviour sb in FindObjectsOfType<SpecialBehaviour>())
        //    Destroy(sb);

        TileBehaviour[] allTiles = FindObjectsOfType<TileBehaviour>();
        float[] delays = new float[allTiles.Length];

        float left = Camera.main.transform.position.x - Camera.main.orthographicSize * Camera.main.aspect - 1;
        float top = Camera.main.transform.position.y + Camera.main.orthographicSize + 1;

        float max = 0;
        for (int i = 0; i < allTiles.Length; i++)
        {
            delays[i] = Mathf.Abs(left - allTiles[i].transform.position.x) / Camera.main.aspect
                      + Mathf.Abs(top - allTiles[i].transform.position.y);
            if (delays[i] > max)
                max = delays[i];
        }

        for (int i = 0; i < allTiles.Length; i++)
            allTiles[i].Invoke("ResetTile", delays[i] / max * FlipAllTime);
    }
    private void ResetTile()
    {
        ResetTile(true);
    }

    private void ChangeSprite(int depth, bool animate)
    {
        int shade = (16 - depth * 2) * (16 - depth * 2) - 1;
        byte value = depth < 16 / 2 ? (byte)shade : (byte)0;
        NewColor = new Color32(value, value, value, 255);

        if (animate)
        {
            Animating = true;
            //NumAnimating++;
        }
        else
        {
            UpdateSprite();
            doUpdate = true;
        }
    }

    bool doUpdate = true;
    private void UpdateSprite()
    {
        if (doUpdate)
        {
            foreach (Transform child in transform)
                Destroy(child.gameObject);

            //Debug.Log("special behaviour " + Data.Behaviour);
            if (!string.IsNullOrEmpty(Data.Behaviour))
            {
                gameObject.AddComponent(System.Type.GetType(Data.Behaviour));
            }
            SR.sprite = Data.Sprite;
            SR.color = NewColor;
            SR.sortingLayerName = Depth > 0 ? "BGTiles" : "Tiles";
            SR.gameObject.layer = Depth > 0 ? 0 : LayerMask.NameToLayer("Tiles");

            doUpdate = false;
        }
    }

    private void Update()
    {
        if (Animating)
        {
            transform.Rotate(Vector3.up * Time.deltaTime * 360 / FlipOneTime);

            if (transform.eulerAngles.y > 90)
            {
                UpdateSprite();
                SR.flipX = true;
            }

            if (transform.eulerAngles.y > 180)
            {
                doUpdate = true;
                Animating = false;
                //NumAnimating--;
                SR.flipX = false;
                transform.eulerAngles = new Vector3(0, 0, 0);
                Destroy(BC);
                BC = gameObject.AddComponent<BoxCollider2D>();
                BC.size = new Vector2(1, 1);
            }
        }

        if (LastPosition != transform.position)
        {
            ResetTile(false);
            LastPosition = transform.position;
        }
    }

    public void Mine(float amount)
    {
        if (Data.Hardness == -1 || Animating || Depth > 1)
            return;

        Damage += amount;
        if (Damage >= Data.Hardness)
            Break();
    }

    public void ResetDamage()
    {
        Damage = 0;
    }

    private void Break()
    {
        Vector3 position = new Vector3(gameObject.transform.position.x, gameObject.transform.position.y, PlayerController.WorldPos.z);
        World.SetTileType(position + new Vector3(0, 0, Depth), TileType.Air);
        ResetTile(true);
    }
}
