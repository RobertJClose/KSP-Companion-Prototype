using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public abstract class InspectablePlot : MonoBehaviour
{
    public abstract void HighlightPlot(bool isHighlighted);
}
