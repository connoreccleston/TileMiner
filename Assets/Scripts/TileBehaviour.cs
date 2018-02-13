using UnityEngine;

public class TileBehaviour : MonoBehaviour
{
    private TileDatum Data;
    private float Damage = 0;

    private bool Animating = false;
    private Color NewColor;

    private SpriteRenderer SR;
    private BoxCollider2D BC;

    private void Start()
	{
        SR = gameObject.AddComponent<SpriteRenderer>();
        BC = gameObject.AddComponent<BoxCollider2D>();
        BC.size = new Vector2(1, 1);
        ResetTile(false);
    }

    public void ResetTile(bool animate)
    {
        int depth;
        Data = World3D.TileTable[World3D.GetTileType(transform.position, out depth)];

        if (depth != 0)
        {
            Sprite temp = Data.Sprite;
            Data = World3D.TileTable[TileType.Air];
            Data.Sprite = temp;
        }

        ChangeSprite(depth, animate);
        name = Data.Type.ToString();
        ResetDamage();
    }

    private void ChangeSprite(int depth, bool animate)
    {
        int shade = (16 - depth * 2) * (16 - depth * 2) - 1;
        byte value = depth < 16 / 2 ? (byte)shade : (byte)0;
        NewColor = new Color32(value, value, value, 255);

        if (animate)
        {
            Animating = true;
        }
        else
        {
            SR.sprite = Data.Sprite;
            SR.color = NewColor;
        }
    }

    public static void ResetAll()
    {
        float width = 4 * Camera.main.orthographicSize * Camera.main.aspect;
        float left = Camera.main.transform.position.x - Camera.main.orthographicSize * Camera.main.aspect - width;
        float center = Camera.main.transform.position.y;
        float angle = Mathf.Atan(1 / Camera.main.aspect) * 180 / Mathf.PI;

        var hits = Physics2D.BoxCastAll(new Vector2(left, center), new Vector2(width, 1), angle, Vector2.right);

        float delay = 0;
        foreach (var hit in hits)
        {
            hit.transform.GetComponent<TileBehaviour>().Invoke("ResetTile", delay);
            delay += .005f; // should move faster for larger views
        }
    }

    private void ResetTile()
    {
        ResetTile(true);
    }

    private void Update()
    {
        if (Animating)
        {
            transform.Rotate(Vector3.up * Time.deltaTime * 360);

            if (transform.eulerAngles.y > 90)
            {
                SR.sprite = Data.Sprite;
                SR.color = NewColor;
                SR.flipX = true;
            }

            if (transform.eulerAngles.y > 180)
            {
                Animating = false;
                SR.flipX = false;
                transform.eulerAngles = new Vector3(0, 0, 0);
                Destroy(BC);
                BC = gameObject.AddComponent<BoxCollider2D>();
                BC.size = new Vector2(1, 1);
            }
        }
    }

    public void Mine(float amount)
    {
        if (Data.Hardness == -1 || Animating)
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
        World3D.SetTileType(gameObject, TileType.Air);
        ResetTile(true);
    }
}
