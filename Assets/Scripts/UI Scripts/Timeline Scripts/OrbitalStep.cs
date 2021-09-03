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
            if (PreviousStep is TransitionStep)
                return (PreviousStep as TransitionStep).TransitionTime;
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
            if (PreviousStep is TransitionStep)
                return orbit.Time2TrueAnomaly((PreviousStep as TransitionStep).TransitionTime);
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
            orbit = Orbit.FindTransferOrbit(previousOrbitalStep.Orbit, previousTransitionStep.TransitionTime, nextOrbitalStep.Orbit, nextTransitionStep.TransitionTime);
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
            porkChopPropertyBlock.AddDoubleProperty("Departure Time (s UT)",    () => (previousStep as TransitionStep).TransitionTime,  (double newDepartureTime)   => { (previousStep as TransitionStep).TransitionTime = newDepartureTime;    UpdateTransferOrbit(); });
            porkChopPropertyBlock.AddDoubleProperty("Arrival Time (s UT)",      () => (nextStep as TransitionStep).TransitionTime,      (double newArrivalTime)     => { (nextStep as TransitionStep).TransitionTime = newArrivalTime;          UpdateTransferOrbit(); });
        }

        // Set-up of the Inspector property blocks:
        InspectorPropertyBlock blockOne = inspector.AddPropertyBlock();
        blockOne.AddDropdownProperty("Colour", () => StepColour, (NamedColour namedColour) => SetOrbitalStepColour(namedColour), namedColours, NamedColour.TMPDropdownOptionDataConverter, DisplayCondition: () => isFreeOrbit); ;

        InspectorPropertyBlock blockTwo = inspector.AddPropertyBlock();
        blockTwo.AddDoubleProperty("Periapsis Radius (m)",              ValueGetter: () => { if (orbit != null) return orbit.RPE; else return 0.0; },           ValueSetter: newRPE  => { if (orbit != null) orbit.RPE = newRPE; UpdateAdjacentTransferOrbits(); },                         DisplayCondition: () => orbit != null, isEditable: IsFreeOrbit);
        blockTwo.AddDoubleProperty("Eccentricity",                      ValueGetter: () => { if (orbit != null) return orbit.ECC; else return 0.0; },           ValueSetter: newECC  => { if (orbit != null) orbit.ECC = newECC; UpdateAdjacentTransferOrbits(); },                         DisplayCondition: () => orbit != null, isEditable: IsFreeOrbit);
        blockTwo.AddDoubleProperty("Inclination (deg)",                 ValueGetter: () => { if (orbit != null) return orbit.INC.DegValue; else return 0.0; },  ValueSetter: newINCd => { if (orbit != null) orbit.INC = (float)newINCd * Mathf.Deg2Rad; UpdateAdjacentTransferOrbits(); }, DisplayCondition: () => orbit != null, isEditable: IsFreeOrbit);
        blockTwo.AddDoubleProperty("Argument Of Periapsis (deg)",       ValueGetter: () => { if (orbit != null) return orbit.APE.DegValue; else return 0.0; },  ValueSetter: newAPEd => { if (orbit != null) orbit.APE = (float)newAPEd * Mathf.Deg2Rad; UpdateAdjacentTransferOrbits(); }, DisplayCondition: () => orbit != null, isEditable: IsFreeOrbit);
        blockTwo.AddDoubleProperty("Longitude Of Ascending Node (deg)", ValueGetter: () => { if (orbit != null) return orbit.LAN.DegValue; else return 0.0; },  ValueSetter: newLANd => { if (orbit != null) orbit.LAN = (float)newLANd * Mathf.Deg2Rad; UpdateAdjacentTransferOrbits(); }, DisplayCondition: () => orbit != null, isEditable: IsFreeOrbit);
        blockTwo.AddDoubleProperty("Time Of Periapsis Passage (s UT)",  ValueGetter: () => { if (orbit != null) return orbit.TPP; else return 0.0; },           ValueSetter: newTPP  => { if (orbit != null) orbit.TPP = newTPP; UpdateAdjacentTransferOrbits(); },                         DisplayCondition: () => orbit != null, isEditable: IsFreeOrbit);

        InspectorPropertyBlock blockThree = inspector.AddPropertyBlock();
        blockThree.AddDoubleProperty("Semimajor Axis (m)",          ValueGetter: () => { if (orbit != null) return orbit.SMA; else return 0.0; },                                           DisplayCondition: () => orbit != null);
        blockThree.AddDoubleProperty("Specific energy (J/kg)",      ValueGetter: () => { if (orbit != null) return orbit.SpecificEnergy; else return 0.0; },                                DisplayCondition: () => orbit != null);
        blockThree.AddDoubleProperty("Start True Anomaly (deg)",    ValueGetter: () => { if (orbit != null) return orbit.Time2TrueAnomaly(StartTime ?? 0f).DegValue; else return 0.0; },    DisplayCondition: () => StartTime != null && orbit != null);
        blockThree.AddDoubleProperty("Final True Anomaly (deg)",    ValueGetter: () => { if (orbit != null) return orbit.Time2TrueAnomaly(FinalTime ?? 0f).DegValue; else return 0.0; },    DisplayCondition: () => FinalTime != null && orbit != null);
        blockThree.AddDoubleProperty("Period (s)",                  ValueGetter: () => { if (orbit != null) return orbit.Period; else return 0.0; },                                        DisplayCondition: () => orbit != null && orbit.OrbitType == Orbit.ConicSection.Elliptical);
        blockThree.AddDoubleProperty("Apoapsis Radius (m)",         ValueGetter: () => { if (orbit != null) return orbit.ApoapsisRadius; else return 0.0; },                                DisplayCondition: () => orbit != null && orbit.OrbitType == Orbit.ConicSection.Elliptical);

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
            if (orbit != null)
                plot.PlotTrajectory(orbit, StepColour.color, StartTime, FinalTime);
            else
                plot.PlotTrajectory(GravitationalBody.Kerbin.ZeroOrbit, Color.black);
        }
    }

    private void UpdateAdjacentTransferOrbits()
    {
        PreviousOrbitalStep?.UpdateTransferOrbit();
        NextOrbitalStep?.UpdateTransferOrbit();
    }
}
