using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Shapes;
using TMPro;

public class OrbitalStep : TimelineStep, IInspectable, IPlottable
{
    // Members
    Orbit orbit;
    public Orbit Orbit { get => orbit; set => orbit = value; }
    string stepName;
    bool isFreeOrbit;
    public bool IsFreeOrbit { get { return isFreeOrbit; } }
    public bool IsDeletable { get; set; }
    List<NamedColour> namedColours;
    public NamedColour StepColour { get; set; }
    public NamedColour NotTravelledColour { get { return namedColours[namedColours.Count - 1]; } }
    public double? StartTime
    {
        get
        {
            if (PreceedingStep is TransitionStep)
                return (PreceedingStep as TransitionStep).TransitionTime;
            else
                return null;
        }
    }
    public double? FinalTime
    {
        get
        {
            if (NextStep is TransitionStep)
                return (NextStep as TransitionStep).TransitionTime;
            else
                return null;
        }
    }
    public double? Duration
    {
        get
        {
            return FinalTime - StartTime;
        }
    }
    public Angle? StartTrueAnomaly
    {
        get
        {
            if (PreceedingStep is TransitionStep)
                return orbit.Time2TrueAnomaly((PreceedingStep as TransitionStep).TransitionTime);
            else
                return null;
        }
    }
    public Angle? FinalTrueAnomaly
    {
        get
        {
            if (NextStep is TransitionStep)
                return orbit.Time2TrueAnomaly((NextStep as TransitionStep).TransitionTime);
            else
                return null;
        }
    }

    // Asset References
    [SerializeField]
    ColoursScriptableObject scriptableObject_Colours;

    // Prefabs
    [SerializeField]
    OrbitPlot prefab_OrbitPlot;

    // Child GameObject References
    TextMeshProUGUI textComponent;

    // Scene References
    [SerializeField]
    Image colourBarImage;
    OrbitPlot plot;

    protected override void Awake()
    {
        // In Awake(), the OrbitalStep is set up as a free OrbitalStep. If it is to be a transfer orbit, that change is made later
        // in the SetTransferOrbit(bool) method.
        base.Awake();
        orbit = GravitationalBody.Kerbin.DefaultOrbit;
        isFreeOrbit = true;
        stepName = "Free Orbit";
        textComponent = GetComponentInChildren<TextMeshProUGUI>();
        textComponent.text = stepName;
        namedColours = scriptableObject_Colours.colours;
        StepColour = namedColours[0];
        plot = Instantiate(prefab_OrbitPlot, root3DScene);
        Plot();
    }

    private void OnDestroy()
    {
        inspector.Clear();
        if (plot != null)
            Destroy(plot.gameObject);
    }

    public void SetStepName(string newName)
    {
        stepName = newName;
        textComponent.text = stepName;
    }

    public void SetTransferOrbit(bool isTransferOrbit)
    {
        isFreeOrbit = !isTransferOrbit;
        if (isTransferOrbit)
        {
            SetStepName("Transfer Orbit");
            SetOrbitalStepColour(namedColours[namedColours.Count - 2]);
        }
        else
        {
            SetStepName("Free Orbit");
        }
    }

    public void UpdateTransferOrbit()
    {
        if (!isFreeOrbit)
        {
            double departureTime = earlierTransitionStep.TransitionTime;
            double arrivalTime = laterTransitionStep.TransitionTime;

            Vector3d departurePosition = previousOrbitalStep.Orbit.Time2Point(departureTime);
            Vector3d arrivalPosition = followingOrbitalStep.Orbit.Time2Point(arrivalTime);

            orbit = Orbit.FindTransferOrbit(orbit.GravitationalBody, departurePosition, departureTime, arrivalPosition, arrivalTime);
        }
    }

    public void SetOrbitalStepColour(NamedColour newColour)
    {
        StepColour = newColour;
        colourBarImage.color = newColour.color;
    }

    public void OnSelect(BaseEventData eventData)
    {
        // Set-up of the Inspector header:
        if (IsDeletable)
        {
            if (isFreeOrbit)
                inspector.SetHeader("Free Orbit:", () => missionTimeline.DeleteFreeOrbitalStep(IndexInTimeline));
            else
                inspector.SetHeader("Transfer Orbit:", () => missionTimeline.DeleteTransferOrbit(IndexInTimeline));
        }
        else
        {
            inspector.SetHeader("Free Orbit:");
        }

        // If this is a transfer orbit, set-up of the Inspector pork-chop plot:
        if (!isFreeOrbit)
        {
            InspectorPropertyBlock porkChopPropertyBlock = inspector.AddPorkChopPropertyBlock();
            porkChopPropertyBlock.AddDoubleProperty("Departure Time (s UT)", () => (preceedingStep as TransitionStep).TransitionTime, (double newDepartureTime) => { (preceedingStep as TransitionStep).TransitionTime = newDepartureTime; UpdateTransferOrbit(); });
            porkChopPropertyBlock.AddDoubleProperty("Arrival Time (s UT)", () => (nextStep as TransitionStep).TransitionTime, (double newArrivalTime) => { (nextStep as TransitionStep).TransitionTime = newArrivalTime; UpdateTransferOrbit(); });
        }

        // Set-up of the Inspector property blocks:
        InspectorPropertyBlock blockOne = inspector.AddPropertyBlock();
        blockOne.AddDropdownProperty("Colour", () => StepColour, (NamedColour namedColour) => SetOrbitalStepColour(namedColour), namedColours, NamedColour.TMPDropdownOptionDataConverter, DisplayCondition: () => isFreeOrbit); ;

        InspectorPropertyBlock blockTwo = inspector.AddPropertyBlock();
        blockTwo.AddDoubleProperty("Periapsis Radius (m)",               () => orbit.RPE,                            (double newRPE) => orbit.RPE = newRPE, isFreeOrbit);
        blockTwo.AddDoubleProperty("Eccentricity",                       () => orbit.ECC,                            (double newECC) => orbit.ECC = newECC, isFreeOrbit);
        blockTwo.AddDoubleProperty("Inclination (deg)",                  () => orbit.INC.DegValueMinus180To180Range, (double newINCd) => orbit.INC = (float)newINCd * Mathf.Deg2Rad, isFreeOrbit);
        blockTwo.AddDoubleProperty("Argument Of Periapsis (deg)",        () => orbit.APE.DegValue,                   (double newAPEd) => orbit.APE = (float)newAPEd * Mathf.Deg2Rad, isFreeOrbit);
        blockTwo.AddDoubleProperty("Longitude Of Ascending Node (deg)",  () => orbit.LAN.DegValue,                   (double newLANd) => orbit.LAN = (float)newLANd * Mathf.Deg2Rad, isFreeOrbit);
        blockTwo.AddDoubleProperty("Time Of Periapsis Passage (s UT)",   () => orbit.TPP,                            (double newTPP) => orbit.TPP = newTPP, isFreeOrbit);

        InspectorPropertyBlock blockThree = inspector.AddPropertyBlock();
        blockThree.AddDoubleProperty("Semimajor Axis (m)",       () => orbit.SMA);
        blockThree.AddDoubleProperty("Specific energy (J/kg)",   () => orbit.SpecificEnergy);
        blockThree.AddDoubleProperty("Period (s)",               () => orbit.Period,                                     DisplayCondition: () => orbit.OrbitType == Orbit.ConicSection.Elliptical);
        blockThree.AddDoubleProperty("Apoapsis Radius (m)",      () => orbit.ApoapsisRadius,                             DisplayCondition: () => orbit.OrbitType == Orbit.ConicSection.Elliptical);
        blockThree.AddDoubleProperty("Start True Anomaly (deg)", () => orbit.Time2TrueAnomaly(StartTime ?? 0f).DegValue, DisplayCondition: () => StartTime != null);
        blockThree.AddDoubleProperty("Final True Anomaly (deg)", () => orbit.Time2TrueAnomaly(FinalTime ?? 0f).DegValue, DisplayCondition: () => FinalTime != null);

        plot.HighlightPlot(true);
    }

    public void OnDeselect(BaseEventData eventData)
    {
        inspector.Clear();

        plot.HighlightPlot(false);
    }

    public void Plot()
    {
        if (plot != null)
        {
            // First get a list of Vector3 points along the orbital trajectory.
            List<Vector3d> orbitPoints = orbit.OrbitalPoints(-orbit.MaxTrueAnomaly, orbit.MaxTrueAnomaly, out List<Angle> trueAnomalies);

            // Now each one of these points must be turned into a PolylinePoint and added to the Polyline plot.
            PolylinePoint nextPoint;
            Angle nextPointTrueAnomaly;
            int pointIndex = 0;
            List<PolylinePoint> polylinePoints = new List<PolylinePoint>(orbitPoints.Count);
            foreach (Vector3d point in orbitPoints)
            {
                nextPointTrueAnomaly = trueAnomalies[pointIndex];

                // If a point is at an infinite/NaN radius, don't add it to the plot. And remove its true anomaly value from the 'trueAnomalies' list.
                // This maintains equivalent indexing between the 'polylinePoints' list and the 'trueAnomalies' list.
                if (point.magnitude == double.PositiveInfinity || point.magnitude == double.NaN || point.magnitude == double.NegativeInfinity)
                {
                    trueAnomalies.Remove(nextPointTrueAnomaly);
                    continue;
                }
                else
                    nextPoint.point = (Vector3)point;

                // Colour the point according to whether it is actually travelled on during this orbital step.
                if (DetermineIfPointIsTravelledOn(nextPointTrueAnomaly))
                    nextPoint.color = StepColour.color;
                else
                    nextPoint.color = NotTravelledColour.color;

                nextPoint.thickness = Constants.OrbitPlotThickness;

                polylinePoints.Add(nextPoint);
                pointIndex++;
            }

            plot.SetPlotPoints(polylinePoints, (Vector3)orbit.PeriapsisPoint, (Vector3?)orbit.ApoapsisPoint, (Vector3?)orbit.AscendingNode, (Vector3?)orbit.DescendingNode);
            if (orbit.OrbitType == Orbit.ConicSection.Elliptical)
                plot.SetClosedPlot(true);
            else
                plot.SetClosedPlot(false);
        }
    }

    private bool DetermineIfPointIsTravelledOn(Angle pointTrueAnomaly)
    {
        if (orbit.OrbitType == Orbit.ConicSection.Elliptical)
        {
            // Elliptical orbit case.
            if ((StartTrueAnomaly == null) ||
                (FinalTrueAnomaly == null) ||
                (Duration > orbit.Period) ||
                (pointTrueAnomaly.IsBetween(StartTrueAnomaly, FinalTrueAnomaly)))
            {
                // Point is travelled on.
                return true;
            }
            else
            {
                // Point isn't travelled on.
                return false;
            }
        }
        else
        {
            // Open orbit case.
            if ((StartTrueAnomaly == null && FinalTrueAnomaly == null) ||
                (StartTrueAnomaly == null && pointTrueAnomaly.IsBetween(-orbit.MaxTrueAnomaly, FinalTrueAnomaly)) ||
                (pointTrueAnomaly.IsBetween(StartTrueAnomaly, orbit.MaxTrueAnomaly) && FinalTrueAnomaly == null) ||
                (pointTrueAnomaly.IsBetween(StartTrueAnomaly, FinalTrueAnomaly)))
            {
                // Point is travelled on.
                return true;
            }
            else
            {
                // Point isn't travelled on.
                return false;
            }
        }
    }
}
