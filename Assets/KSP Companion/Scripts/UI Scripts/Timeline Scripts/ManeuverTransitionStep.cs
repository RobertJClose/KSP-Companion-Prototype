using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ManeuverTransitionStep : TransitionStep
{
    public Vector3d? ManeuverVelocityChange
    {
        get
        {
            if (CheckAdjacentOrbits())
                return nextOrbitalStep.Orbit.Time2Velocity(transitionTime) - previousOrbitalStep.Orbit.Time2Velocity(transitionTime);
            else
                return null;
        }
    }

    public double? DeltaV
    {
        get
        {
            if (CheckAdjacentOrbits())
                return ManeuverVelocityChange.Value.magnitude;
            else
                return null;
        }
    }    

    public Angled? ManeuverAzimuth
    {
        get
        {
            if (CheckAdjacentOrbits())
                return (float)Mathd.Atan2(ManeuverVelocityChange.Value.y, ManeuverVelocityChange.Value.x);
            else
                return null;
        }
    }

    public Angled? ManeuverElevation
    {
        get
        {
            if (CheckAdjacentOrbits())
                return (float)Mathd.Atan2(ManeuverVelocityChange.Value.z, Mathd.Sqrt(ManeuverVelocityChange.Value.x * ManeuverVelocityChange.Value.x + ManeuverVelocityChange.Value.y * ManeuverVelocityChange.Value.y));
            else
                return null;
        }
    }

    public override void OnSelect(BaseEventData eventData)
    {
        inspector.Clear();

        inspector.SetHeader("Maneuver:");

        InspectorPropertyBlock blockOne = inspector.AddPropertyBlock();
        blockOne.AddDoubleProperty("Maneuver Time (s UT)", () => TransitionTime);

        InspectorPropertyBlock blockTwo = inspector.AddPropertyBlock();
        blockTwo.AddDoubleProperty("\u0394V (m/s)",             ValueGetter: () => { if (DeltaV.HasValue) return DeltaV.Value; else return 0.0; },                                                  DisplayCondition: () => DeltaV.HasValue);
        blockTwo.AddDoubleProperty("Maneuver azimuth (Deg)",    ValueGetter: () => { if (ManeuverAzimuth.HasValue) return ManeuverAzimuth.Value.DegValue; else return 0.0; },                       DisplayCondition: () => ManeuverAzimuth.HasValue);
        blockTwo.AddDoubleProperty("Maneuver elevation (Deg)",  ValueGetter: () => { if (ManeuverElevation.HasValue) return ManeuverElevation.Value.DegValueMinus180To180Range; else return 0.0; }, DisplayCondition: () => ManeuverElevation.HasValue);

        plot.HighlightPlot(true);
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
