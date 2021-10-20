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
    public string StepName { get { return stepName; } set { stepName = value; textComponent.text = stepName; } }

    bool isTransferOrbit;
    public bool IsTransferOrbit { get { return isTransferOrbit; } set { isTransferOrbit = value; if (value) { MayUserEditOrbit = false; StepName = "Transfer Orbit"; } } }

    public bool MayUserEditOrbit { get; set; }
    
    public bool IsDeletable { get; set; }
    
    List<NamedColour> namedColours;
    public List<NamedColour> OrbitalStepNamedColours { get { return namedColours; } }

    NamedColour stepColour;
    public NamedColour StepColour { get { return stepColour; } set { stepColour = value; colourBarImage.color = value.color; } }
    
    public NamedColour NotTravelledColour { get { return namedColours[namedColours.Count - 1]; } }
    
    public double? StartTime
    {
        get
        {
            if (PreviousStep is TransitionStep transitionStep)
                return transitionStep.TransitionTime;
            else
                return null;
        }
    }
    public double? FinalTime
    {
        get
        {
            if (NextStep is TransitionStep transitionStep)
                return transitionStep.TransitionTime;
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
    public Anglef? StartTrueAnomaly
    {
        get
        {
            if (PreviousStep is TransitionStep transitionStep)
                return orbit.Time2TrueAnomaly(transitionStep.TransitionTime);
            else
                return null;
        }
    }
    public Anglef? FinalTrueAnomaly
    {
        get
        {
            if (NextStep is TransitionStep transitionStep)
                return orbit.Time2TrueAnomaly(transitionStep.TransitionTime);
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
        base.Awake();
        
        orbit = GravitationalBody.Kerbin.DefaultOrbit;
        
        MayUserEditOrbit = true;

        IsDeletable = false;
        
        textComponent = GetComponentInChildren<TextMeshProUGUI>();
        StepName = "Orbital Step";
        
        namedColours = scriptableObject_Colours.colours;
        StepColour = namedColours[0];
        
        plot = Instantiate(prefab_OrbitPlot, root3DScene);
    }

    private void OnDestroy()
    {
        if (plot != null)
            Destroy(plot.gameObject);
    }

    public void UpdateTransferOrbit()
    {
        if (isTransferOrbit)
        {
            orbit = Orbit.FindTransferOrbit(previousOrbitalStep.Orbit, previousTransitionStep.TransitionTime, nextOrbitalStep.Orbit, nextTransitionStep.TransitionTime);
        }
    }

    public void OnSelect(BaseEventData eventData)
    {
        inspector.Clear();

        if (IsDeletable)
        {
            inspector.SetHeader(stepName, () => missionTimeline.DeleteTransferOrbit(IndexInTimeline));
        }
        else
        {
            inspector.SetHeader(stepName);
        }

        // If this is a transfer orbit, set-up of the Inspector pork-chop plot:
        if (isTransferOrbit)
        {
            InspectorPropertyBlock porkChopPropertyBlock = inspector.AddPorkChopPropertyBlock();
            porkChopPropertyBlock.AddDoubleProperty("Departure Time (s UT)",    () => previousTransitionStep.TransitionTime,  (double newDepartureTime)   => { previousTransitionStep.TransitionTime = newDepartureTime;    UpdateTransferOrbit(); });
            porkChopPropertyBlock.AddDoubleProperty("Arrival Time (s UT)",      () => nextTransitionStep.TransitionTime,      (double newArrivalTime)     => { nextTransitionStep.TransitionTime = newArrivalTime;          UpdateTransferOrbit(); });
        }

        // Set-up of the Inspector property blocks:
        InspectorPropertyBlock blockOne = inspector.AddPropertyBlock();
        blockOne.AddDropdownProperty("Colour", () => StepColour, (object namedColour) => StepColour = (NamedColour)namedColour, namedColours.ConvertAll((NamedColour namedColour) => (object)namedColour), NamedColour.TMPDropdownOptionDataConverter); ;

        InspectorPropertyBlock blockTwo = inspector.AddPropertyBlock();
        blockTwo.AddDoubleProperty("Periapsis Radius (m)",              ValueGetter: () => { if (orbit != null) return orbit.RPE; else return 0.0; },           ValueSetter: newRPE  => { if (orbit != null) orbit.RPE = newRPE; UpdateAdjacentTransferOrbits(); },                         DisplayCondition: () => orbit != null, isEditable: MayUserEditOrbit);
        blockTwo.AddDoubleProperty("Eccentricity",                      ValueGetter: () => { if (orbit != null) return orbit.ECC; else return 0.0; },           ValueSetter: newECC  => { if (orbit != null) orbit.ECC = newECC; UpdateAdjacentTransferOrbits(); },                         DisplayCondition: () => orbit != null, isEditable: MayUserEditOrbit);
        blockTwo.AddDoubleProperty("Inclination (deg)",                 ValueGetter: () => { if (orbit != null) return orbit.INC.DegValue; else return 0.0; },  ValueSetter: newINCd => { if (orbit != null) orbit.INC = (float)newINCd * Mathf.Deg2Rad; UpdateAdjacentTransferOrbits(); }, DisplayCondition: () => orbit != null, isEditable: MayUserEditOrbit);
        blockTwo.AddDoubleProperty("Argument Of Periapsis (deg)",       ValueGetter: () => { if (orbit != null) return orbit.APE.DegValue; else return 0.0; },  ValueSetter: newAPEd => { if (orbit != null) orbit.APE = (float)newAPEd * Mathf.Deg2Rad; UpdateAdjacentTransferOrbits(); }, DisplayCondition: () => orbit != null, isEditable: MayUserEditOrbit);
        blockTwo.AddDoubleProperty("Longitude Of Ascending Node (deg)", ValueGetter: () => { if (orbit != null) return orbit.LAN.DegValue; else return 0.0; },  ValueSetter: newLANd => { if (orbit != null) orbit.LAN = (float)newLANd * Mathf.Deg2Rad; UpdateAdjacentTransferOrbits(); }, DisplayCondition: () => orbit != null, isEditable: MayUserEditOrbit);
        blockTwo.AddDoubleProperty("Time Of Periapsis Passage (s UT)",  ValueGetter: () => { if (orbit != null) return orbit.TPP; else return 0.0; },           ValueSetter: newTPP  => { if (orbit != null) orbit.TPP = newTPP; UpdateAdjacentTransferOrbits(); },                         DisplayCondition: () => orbit != null, isEditable: MayUserEditOrbit);

        InspectorPropertyBlock blockThree = inspector.AddPropertyBlock();
        blockThree.AddDoubleProperty("Semimajor Axis (m)",          ValueGetter: () => { if (orbit != null) return orbit.SemiMajorAxis; else return 0.0; },                                           DisplayCondition: () => orbit != null);
        blockThree.AddDoubleProperty("Specific energy (J/kg)",      ValueGetter: () => { if (orbit != null) return orbit.SpecificEnergy; else return 0.0; },                                DisplayCondition: () => orbit != null);
        blockThree.AddDoubleProperty("Start True Anomaly (deg)",    ValueGetter: () => { if (orbit != null) return orbit.Time2TrueAnomaly(StartTime.GetValueOrDefault()).DegValue; else return 0.0; },    DisplayCondition: () => StartTime != null && orbit != null);
        blockThree.AddDoubleProperty("Final True Anomaly (deg)",    ValueGetter: () => { if (orbit != null) return orbit.Time2TrueAnomaly(FinalTime.GetValueOrDefault()).DegValue; else return 0.0; },    DisplayCondition: () => FinalTime != null && orbit != null);
        blockThree.AddDoubleProperty("Period (s)",                  ValueGetter: () => { if (orbit != null) return orbit.Period; else return 0.0; },                                        DisplayCondition: () => orbit != null && orbit.OrbitType == Orbit.ConicSection.Elliptical);
        blockThree.AddDoubleProperty("Apoapsis Radius (m)",         ValueGetter: () => { if (orbit != null) return orbit.ApoapsisRadius; else return 0.0; },                                DisplayCondition: () => orbit != null && orbit.OrbitType == Orbit.ConicSection.Elliptical);

        plot.HighlightPlot(true);
    }

    public void OnDeselect(BaseEventData eventData)
    {
        inspector.DisplayMissionOverview();

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
