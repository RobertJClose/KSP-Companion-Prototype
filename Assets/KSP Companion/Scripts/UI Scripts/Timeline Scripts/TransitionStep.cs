using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public abstract class TransitionStep : TimelineStep, IInspectable, IPlottable
{
    // Members
    protected double transitionTime;
    public double TransitionTime 
    { 
        get 
        {
            return transitionTime;
        }
        set
        {
            if (previousTransitionStep == null && nextTransitionStep == null)
                transitionTime = value;
            else if (previousTransitionStep == null && nextTransitionStep != null)
                transitionTime = Mathd.Clamp(value, double.NegativeInfinity, nextTransitionStep.transitionTime);
            else if (previousTransitionStep != null && nextTransitionStep == null)
                transitionTime = Mathd.Clamp(value, previousTransitionStep.transitionTime, double.PositiveInfinity);
            else
                transitionTime = Mathd.Clamp(value, previousTransitionStep.transitionTime, nextTransitionStep.transitionTime);
        }
    }
    public Vector3d? TransitionPoint
    {
        get
        {
            if (previousOrbitalStep != null && previousOrbitalStep.Orbit != null)
                return previousOrbitalStep.Orbit.Time2Point(transitionTime);
            else if (nextOrbitalStep != null && nextOrbitalStep.Orbit != null)
                return nextOrbitalStep.Orbit.Time2Point(transitionTime);
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

    protected void OnDestroy()
    {
        if (plot != null)
            Destroy(plot.gameObject);
    }

    public abstract void OnSelect(BaseEventData eventData);

    public void OnDeselect(BaseEventData eventData)
    {
        inspector.DisplayMissionOverview();

        plot.HighlightPlot(false);
    }

    public virtual void Plot()
    {
        if (plot != null)
        {
            plot.SetPlotPoint((Vector3?)TransitionPoint * Constants.PlotRescaleFactor ?? Vector3.zero);
        }
    }
    
}
