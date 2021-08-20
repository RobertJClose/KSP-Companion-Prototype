using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public abstract class TransitionStep : TimelineStep, IInspectable, IPlottable
{
    // Members
    float transitionTime;
    public float TransitionTime 
    { 
        get 
        {
            return transitionTime;
        }
        set
        {
            if (earlierTransitionStep == null && laterTransitionStep == null)
                transitionTime = value;
            else if (earlierTransitionStep == null && laterTransitionStep != null)
                transitionTime = Mathf.Clamp(value, float.NegativeInfinity, laterTransitionStep.transitionTime);
            else if (earlierTransitionStep != null && laterTransitionStep == null)
                transitionTime = Mathf.Clamp(value, earlierTransitionStep.transitionTime, float.PositiveInfinity);
            else
                transitionTime = Mathf.Clamp(value, earlierTransitionStep.transitionTime, laterTransitionStep.transitionTime);
        }
    }
    public Vector3? TransitionPoint
    {
        get
        {
            if (PreceedingStep is OrbitalStep)
                return (PreceedingStep as OrbitalStep).Orbit.Time2Point(TransitionTime);
            else if (NextStep is OrbitalStep)
                return (NextStep as OrbitalStep).Orbit.Time2Point(TransitionTime);
            else
                return null;
        }
    }

    // Prefabs
    [SerializeField]
    TransitionStepPlot prefab_TransitionStepPlot;

    // Scene References
    protected TransitionStepPlot plot;

    protected override void Awake()
    {
        base.Awake();
        plot = Instantiate(prefab_TransitionStepPlot, root3DScene);
        plot.SetSprite(GetComponent<Image>().sprite);
        Plot();
    }

    public abstract void OnSelect(BaseEventData eventData);

    public void OnDeselect(BaseEventData eventData)
    {
        inspector.Clear();

        plot.HighlightPlot(false);
    }

    public virtual void Plot()
    {
        if (plot != null)
        {
            if (TransitionPoint.HasValue && TransitionPoint.Value.magnitude != float.PositiveInfinity)
                plot.transform.position = (TransitionPoint ?? Vector3.zero) * Constants.PlotRescaleFactor;
            else
                plot.transform.position = Vector3.zero;
        }       
    }

}
