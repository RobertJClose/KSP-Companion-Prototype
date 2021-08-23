using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissionTimeline : MonoBehaviour
{
    // Members
    [SerializeField]
    List<TimelineStep> missionTimeline = new();
    public int StepCount { get { return missionTimeline.Count; } }

    // Prefab references:
    [SerializeField]
    OrbitalStep prefab_OrbitalStep;
    [SerializeField]
    StartFinishTransitionStep prefab_StartStep;
    [SerializeField]
    StartFinishTransitionStep prefab_FinishStep;
    [SerializeField]
    ManeuverTransitionStep prefab_ManeuverStep;
    [SerializeField]
    AddButton prefab_AddButton;

    public List<TimelineStep> GetMissionTimeline() { return missionTimeline; }

    public void CreateFreeOrbitalStep(int addButtonIndex)
    {
        // This operation turns a single AddButton in the timeline into: AddButton - Free OrbitalStep - AddButton.
        
        // Create a new free OrbitalStep and add it in after the AddButton that led to this call.
        OrbitalStep newOrbitalStep = Instantiate(prefab_OrbitalStep, transform);
        newOrbitalStep.transform.SetSiblingIndex(addButtonIndex + 1);
        newOrbitalStep.IsDeletable = true;
        missionTimeline.Insert(addButtonIndex + 1, newOrbitalStep);

        // Create a new AddButton and add it in after.
        AddButton newAddButton = Instantiate(prefab_AddButton, transform);
        newAddButton.transform.SetSiblingIndex(addButtonIndex + 2);
        missionTimeline.Insert(addButtonIndex + 2, newAddButton);

        UpdateAllSurroundingSteps();
        PlotSteps();
    }

    public void CreateTransferOrbit(int addButtonIndex)
    {
        // This operation turns an AddButton into: ManeuverStep - Transfer OrbitalStep - ManeuverStep.

        // Destroy the AddButton that led to this call.
        Destroy(transform.GetChild(addButtonIndex).gameObject);
        missionTimeline.RemoveAt(addButtonIndex);

        // Create a new ManeuverStep and add it in where the AddButton used to be.
        ManeuverTransitionStep firstManeuver = Instantiate(prefab_ManeuverStep, transform);
        firstManeuver.transform.SetSiblingIndex(addButtonIndex);
        firstManeuver.TransitionTime = double.NegativeInfinity;
        missionTimeline.Insert(addButtonIndex, firstManeuver);

        // Create a new OrbitalStep and set it up as a transfer orbit. Add it after the first ManeuverStep
        OrbitalStep newTransferOrbit = Instantiate(prefab_OrbitalStep, transform);
        newTransferOrbit.SetTransferOrbit(true);
        newTransferOrbit.transform.SetSiblingIndex(addButtonIndex + 1);
        newTransferOrbit.IsDeletable = true;
        missionTimeline.Insert(addButtonIndex + 1, newTransferOrbit);

        // Create the second new ManeuverStep and add it after the transfer OrbitalStep
        ManeuverTransitionStep secondManeuver = Instantiate(prefab_ManeuverStep, transform);
        secondManeuver.transform.SetSiblingIndex(addButtonIndex + 2);
        secondManeuver.TransitionTime = double.PositiveInfinity;
        missionTimeline.Insert(addButtonIndex + 2, secondManeuver);

        UpdateAllSurroundingSteps();
        ValidateTransitionTimes();
        UpdateAllTransferOrbits();
        PlotSteps();
    }

    public void DeleteFreeOrbitalStep(int orbitalStepIndex)
    {
        // This operation deletes the specified OrbitalStep. If it is surrounded by a transfer orbit then the transfer orbit 
        // and its attached ManeuverTransitionSteps need to be destroyed as well. The end result of this operation should always be just
        // a single 'Add Button'.
        // If:
        // O    - The specified OrbitalStep
        // MTM  - A transfer OrbitalStep and its ManeuverTransitionSteps
        // A    - An AddButton
        // Then this function performs:
        // Case 1: AOMTM    -> A
        // Case 2: MTMOA    -> A
        // Case 3: MTMOMTM  -> A
        // Case 4: AOA      -> A

        OrbitalStep orbitalStepToBeDestroyed = (missionTimeline[orbitalStepIndex] as OrbitalStep);
        TimelineStep nextStep = orbitalStepToBeDestroyed.NextStep;
        TimelineStep preceedingStep = orbitalStepToBeDestroyed.PreceedingStep;

        if (preceedingStep is AddButton && nextStep is ManeuverTransitionStep)
        {
            // Case 1:
            Destroy(orbitalStepToBeDestroyed.gameObject);
            Destroy(nextStep.NextStep.NextStep.gameObject);
            Destroy(nextStep.NextStep.gameObject);
            Destroy(nextStep.gameObject);
            missionTimeline.RemoveRange(orbitalStepIndex, 4);
        }
        else if (preceedingStep is ManeuverTransitionStep && nextStep is AddButton)
        {
            // Case 2:
            Destroy(orbitalStepToBeDestroyed.gameObject);
            Destroy(preceedingStep.PreceedingStep.PreceedingStep.gameObject);
            Destroy(preceedingStep.PreceedingStep.gameObject);
            Destroy(preceedingStep.gameObject);
            missionTimeline.RemoveRange(orbitalStepIndex - 3, 4);
        }
        else if (preceedingStep is ManeuverTransitionStep && nextStep is ManeuverTransitionStep)
        {
            // Case 3:
            AddButton newAddButton = Instantiate(prefab_AddButton, transform);
            newAddButton.transform.SetSiblingIndex(orbitalStepIndex + 4);
            missionTimeline.Insert(orbitalStepIndex + 4, newAddButton);

            Destroy(orbitalStepToBeDestroyed.gameObject);
            Destroy(preceedingStep.PreceedingStep.PreceedingStep.gameObject);
            Destroy(preceedingStep.PreceedingStep.gameObject);
            Destroy(preceedingStep.gameObject);
            Destroy(nextStep.NextStep.NextStep.gameObject);
            Destroy(nextStep.NextStep.gameObject);
            Destroy(nextStep.gameObject);
            missionTimeline.RemoveRange(orbitalStepIndex - 3, 7);
        }
        else if (preceedingStep is AddButton && nextStep is AddButton)
        {
            // Case 4:
            Destroy(orbitalStepToBeDestroyed.gameObject);
            Destroy(nextStep.gameObject);
            missionTimeline.RemoveRange(orbitalStepIndex, 2);
        }

        UpdateAllSurroundingSteps();
        PlotSteps();
    }

    public void DeleteTransferOrbit(int orbitalStepIndex)
    {
        // This operation turns a 'ManeuverStep - Transfer OrbitalStep - ManeuverStep' into an AddButton.

        // Destroy the ManeuverStep that comes after the transfer OrbitalStep whose index matches the input parameter.
        Destroy(transform.GetChild(orbitalStepIndex + 1).gameObject);
        missionTimeline.RemoveAt(orbitalStepIndex + 1);

        // Destroy the transfer OrbitalStep.
        Destroy(transform.GetChild(orbitalStepIndex).gameObject);
        missionTimeline.RemoveAt(orbitalStepIndex);

        // Destroy the ManeuverStep that came before.
        Destroy(transform.GetChild(orbitalStepIndex - 1).gameObject);
        missionTimeline.RemoveAt(orbitalStepIndex - 1);

        // Create an AddButton in place of the destroyed TimelineSteps and add it to the missionTimeline list.
        AddButton newAddButton = Instantiate(prefab_AddButton, transform);
        newAddButton.transform.SetSiblingIndex(orbitalStepIndex - 1);
        missionTimeline.Insert(orbitalStepIndex - 1, newAddButton);

        UpdateAllSurroundingSteps();
        PlotSteps();
    }

    public void UpdateAllSurroundingSteps()
    {
        foreach (TimelineStep step in missionTimeline)
        {
            step.UpdateSurroundingSteps();
        }
    }

    public void ValidateTransitionTimes()
    {
        // This method guarantees that there will be proper ordering of the TransitionTimes for each TransitionStep
        // along the missionTimeline. So a TransitionStep cannot end up with a TransitionTime that is earlier(later)
        // than another TransitionStep that is before(after) it in the timeline.

        for (int forwardIndex = 0; forwardIndex < missionTimeline.Count; forwardIndex++)
        {
            // Step forwards and clamp each TransitionTime from below based on the step that is before each TransitionStep.
            // Ignore the first step in the list as the first TransitionStep has no earlier TransitionStep.
            TimelineStep step = missionTimeline[forwardIndex];
            if (step is TransitionStep && forwardIndex != 0)
                (step as TransitionStep).TransitionTime = Mathd.Clamp((step as TransitionStep).TransitionTime, (step as TransitionStep).EarlierTransitionStep.TransitionTime, float.PositiveInfinity);
        }

        for (int backwardIndex = missionTimeline.Count - 1; backwardIndex > -1; backwardIndex--)
        {
            // Step backwards and clamp each TransitionTime based on the step that comes after each TransitionStep.
            // Ignore the final step in the list as the final TransitionStep has no later TransitionStep.
            TimelineStep step = missionTimeline[backwardIndex];
            if (step is TransitionStep && backwardIndex != missionTimeline.Count - 1)
                (step as TransitionStep).TransitionTime = Mathd.Clamp((step as TransitionStep).TransitionTime, float.NegativeInfinity, (step as TransitionStep).LaterTransitionStep.TransitionTime);
        }
    }

    public void UpdateAllTransferOrbits()
    {
        foreach (TimelineStep step in missionTimeline)
        {
            if (step is OrbitalStep && !(step as OrbitalStep).IsFreeOrbit)
            {
                (step as OrbitalStep).UpdateTransferOrbit();
            }
        }
    }

    public void PlotSteps()
    {
        foreach (var step in missionTimeline)
        {
            if (step is IPlottable)
                (step as IPlottable).Plot();
        }
    }

    private void InitialiseTimeline()
    {
        // Add the mission start step
        StartFinishTransitionStep startStep = Instantiate(prefab_StartStep, transform);
        startStep.TransitionTime = 0.0;
        missionTimeline.Add(startStep);

        // Add the initial free orbital step
        OrbitalStep initialOrbit = Instantiate(prefab_OrbitalStep, transform);
        initialOrbit.IsDeletable = false;
        initialOrbit.Orbit = GravitationalBody.Kerbin.DefaultOrbit;
        initialOrbit.SetStepName("Initial Orbit");
        missionTimeline.Add(initialOrbit);

        // Add an 'add button' inbetween the initial and final orbital steps
        AddButton addButton = Instantiate(prefab_AddButton, transform);
        missionTimeline.Add(addButton);

        // Add the final free orbital step
        OrbitalStep finalOrbit = Instantiate(prefab_OrbitalStep, transform);
        finalOrbit.IsDeletable = false;
        finalOrbit.Orbit = GravitationalBody.Kerbin.DefaultOrbit;
        finalOrbit.SetStepName("Final Orbit");
        finalOrbit.Orbit.RPE = 1_250_000f;
        finalOrbit.Orbit.ECC = 1.5f;
        missionTimeline.Add(finalOrbit);

        // Add the mission finish step
        StartFinishTransitionStep finishStep = Instantiate(prefab_FinishStep, transform);
        finishStep.TransitionTime = 2.0 * 60.0 * 60.0; // 2 hours after the mission start.
        missionTimeline.Add(finishStep);
    }

    private void Start()
    {
        InitialiseTimeline();
        UpdateAllSurroundingSteps();
        PlotSteps();
    }

}
