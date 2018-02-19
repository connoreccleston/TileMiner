using System.Collections.Generic;
using UnityEngine;

public class WireFX : MonoBehaviour
{
    public float frequency = 0.4f;
    private float offset;

    private LineRenderer LR;

    private Color StartColorSolid;
    private Color StartColorTrans;
    private Color EndColorSolid;
    private Color EndColorTrans;

    private int Length;
    private Vector3[] BasePositions;
    private float xLen;

    private float Timer = -1;
    private bool CountDown;
    private bool Animating = false;

    private bool DoDestroy = false;

    private void Start()
	{
        frequency += Random.Range(-0.1f, 0.1f);
        offset = Random.value * 2 * Mathf.PI;

        LR = transform.GetComponent<LineRenderer>();

        StartColorSolid = LR.startColor;
        StartColorTrans = new Color(LR.startColor.r, LR.startColor.g, LR.startColor.b, 0);
        EndColorSolid = LR.endColor;
        EndColorTrans = new Color(LR.endColor.r, LR.endColor.g, LR.endColor.b, 0);

        Length = LR.positionCount;
        BasePositions = new Vector3[Length];
        LR.GetPositions(BasePositions);
        xLen = Mathf.Abs(BasePositions[0].x - BasePositions[Length - 1].x);

        FadeIn();
    }

    public void FadeIn()
    {
        if (Animating) return;

        LR.startColor = StartColorTrans;
        LR.endColor = EndColorTrans;

        Timer = 0;
        CountDown = false;
    }

    public void FadeOut(bool destroyAfter = false)
    {
        if (Animating) return;

        LR.startColor = StartColorSolid;
        LR.endColor = EndColorSolid;

        Timer = 1;
        CountDown = true;

        DoDestroy = destroyAfter;
    }
	
	private void Update()
	{
        if (Timer >= 0 && Timer <= 1)
        {
            Animating = true;

            LR.startColor = Color.Lerp(StartColorTrans, StartColorSolid, Timer);
            LR.endColor = Color.Lerp(EndColorTrans, EndColorSolid, Timer);

            if (CountDown)
                Timer -= Time.deltaTime / (TileBehaviour.FlipOneTime / 4);
            else
                Timer += Time.deltaTime / (TileBehaviour.FlipOneTime / 2);
        }
        else
        {
            Animating = false;

            if (DoDestroy)
                Destroy(gameObject);
        }

        // Make line sway by having each point move left and right based on index, middle being fastest and edges not at all.
        for (int i = 1; i < Length - 1; i++)
        {
            //float magnitude = -1f * (2f / (Length - 1f)) * Mathf.Abs(i - (Length - 1f) / 2f) + 1f;
            float magnitude = -1f * (2f / (Length - 1f)) * (2f / (Length - 1f)) * (i - (Length - 1f) / 2f) * (i - (Length - 1f) / 2f) + 1f;
            float periodic = Mathf.Sin(frequency * Time.time - offset);
            LR.SetPosition(i, BasePositions[i] + new Vector3(magnitude * periodic * xLen / Length, 0));
        }
    }
}
