using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransitionStepPlot : InspectablePlot
{
    Transform mainCameraTransform;
    
    SpriteRenderer spriteRenderer;
    SpriteRenderer childSpriteRenderer;

    private void Awake()
    {
        mainCameraTransform = Camera.main.transform;

        spriteRenderer = GetComponent<SpriteRenderer>();
        childSpriteRenderer = transform.GetChild(0).GetComponent<SpriteRenderer>();

        childSpriteRenderer.gameObject.SetActive(false);
    }

    private void Update()
    {
        transform.LookAt(mainCameraTransform, Vector3.forward);
    }

    public override void HighlightPlot(bool isHighlighted)
    {
        childSpriteRenderer.gameObject.SetActive(isHighlighted);
    }

    public void SetSprite(Sprite sprite)
    {
        spriteRenderer.sprite = sprite;
        childSpriteRenderer.sprite = sprite;
    }

    public void SetPlotPoint(Vector3 position)
    {
        if (position.magnitude < Constants.MaximumPlotDistance)
        {
            // Unity's render engine uses a left-handed coordinate system, whereas KSP uses a right-handed system.
            // To account for this difference the y-coordinate is flipped before plotting.
            transform.position = new Vector3(position.x, -position.y, position.z);
        }
        else
        {
            transform.position = Vector3.zero;
        }
    }
}
