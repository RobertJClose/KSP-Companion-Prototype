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
            transform.position = position;
        else
            transform.position = Vector3.zero;
    }
}
