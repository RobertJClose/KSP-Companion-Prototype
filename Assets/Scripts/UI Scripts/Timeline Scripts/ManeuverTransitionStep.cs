using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ManeuverTransitionStep : TransitionStep
{
    public override void OnSelect(BaseEventData eventData)
    {
        inspector.SetHeader("Maneuver:");
        InspectorPropertyBlock blockOne = inspector.AddPropertyBlock();
        blockOne.AddDoubleProperty("Maneuver Time (s UT)", () => TransitionTime);
    }

    private void OnDestroy()
    {
        if (plot != null)
            Destroy(plot.gameObject);
    }

    private bool CheckAdjacentOrbits()
    {
        // If an adjacent transfer OrbitalStep cannot find a valid transfer orbit, then this method will return false. Otherwise it will return true.
        if (previousOrbitalStep != null && previousOrbitalStep.Orbit != null && nextOrbitalStep != null && nextOrbitalStep.Orbit != null)
            return true;
        else
            return false;
    }

    public override void Plot()
    {
        if (plot != null)
        {
            if (CheckAdjacentOrbits())
                plot.SetPlotPoint((Vector3?)TransitionPoint * Constants.PlotRescaleFactor ?? Vector3.zero);
            else
                plot.SetPlotPoint(Vector3.zero);
        }
    }
}
