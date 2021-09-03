﻿using System.Collections;
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
        blockTwo.AddDoubleProperty("Periapsis Radius (m)",              () => orbit.RPE,                            newRPE => { orbit.RPE = newRPE;                             UpdateAdjacentTransferOrbits(); }, isEditable: IsFreeOrbit);
        blockTwo.AddDoubleProperty("Eccentricity",                      () => orbit.ECC,                            newECC => { orbit.ECC = newECC;                             UpdateAdjacentTransferOrbits(); }, isEditable: IsFreeOrbit);
        blockTwo.AddDoubleProperty("Inclination (deg)",                 () => orbit.INC.DegValueMinus180To180Range, newINCd => { orbit.INC = (float)newINCd * Mathf.Deg2Rad;    UpdateAdjacentTransferOrbits(); }, isEditable: IsFreeOrbit);
        blockTwo.AddDoubleProperty("Argument Of Periapsis (deg)",       () => orbit.APE.DegValue,                   newAPEd => { orbit.APE = (float)newAPEd * Mathf.Deg2Rad;    UpdateAdjacentTransferOrbits(); }, isEditable: IsFreeOrbit);
        blockTwo.AddDoubleProperty("Longitude Of Ascending Node (deg)", () => orbit.LAN.DegValue,                   newLANd => { orbit.LAN = (float)newLANd * Mathf.Deg2Rad;    UpdateAdjacentTransferOrbits(); }, isEditable: IsFreeOrbit);
        blockTwo.AddDoubleProperty("Time Of Periapsis Passage (s UT)",  () => orbit.TPP,                            newTPP => { orbit.TPP = newTPP;                             UpdateAdjacentTransferOrbits(); }, isEditable: IsFreeOrbit);

        InspectorPropertyBlock blockThree = inspector.AddPropertyBlock();
        blockThree.AddDoubleProperty("Semimajor Axis (m)",          () => orbit.SMA);
        blockThree.AddDoubleProperty("Specific energy (J/kg)",      () => orbit.SpecificEnergy);
        blockThree.AddDoubleProperty("Start True Anomaly (deg)",    () => orbit.Time2TrueAnomaly(StartTime ?? 0f).DegValue, DisplayCondition: () => StartTime != null);
        blockThree.AddDoubleProperty("Final True Anomaly (deg)",    () => orbit.Time2TrueAnomaly(FinalTime ?? 0f).DegValue, DisplayCondition: () => FinalTime != null);
        blockThree.AddDoubleProperty("Period (s)",                  () => orbit.Period,                                     DisplayCondition: () => orbit.OrbitType == Orbit.ConicSection.Elliptical);
        blockThree.AddDoubleProperty("Apoapsis Radius (m)",         () => orbit.ApoapsisRadius,                             DisplayCondition: () => orbit.OrbitType == Orbit.ConicSection.Elliptical);
        
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
