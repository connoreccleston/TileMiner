using UnityEngine;

public class TileBehaviour : MonoBehaviour
{
    private TileDatum Data;
    private float Damage = 0;

    private bool Animating = false;
    private Color NewColor;

    private SpriteRenderer SR;

    private void Start()
	{
        SR = gameObject.AddComponent<SpriteRenderer>();
        var bc = gameObject.AddComponent<BoxCollider2D>();
        bc.size = new Vector2(1, 1);
        ResetTile(false);
    }

    public void ResetTile(bool animate)
    {
        int depth;
        Data = WorldManager.TileTable[WorldManager.GetTileType(transform.position, out depth)];
        ChangeSprite(depth, animate);
        name = Data.Type.ToString();
        ResetDamage();
    }

    private void ChangeSprite(int depth, bool animate)
    {
        depth = 255 - depth * 128;
        byte value = depth >= 0 ? (byte)depth : (byte)0;
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
        // drop resource
        WorldManager.SetTileType(gameObject, TileType.Air);
        ResetTile(true);
    }
}
