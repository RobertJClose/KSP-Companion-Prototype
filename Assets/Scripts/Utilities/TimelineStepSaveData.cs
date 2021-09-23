public class TimelineStepSaveData
{
    public TimelineStepType stepType;

    public bool isStartStep;

    public double transitionTime;

    public bool isOrbitalStepOrbitNull;
    public int orbitGravBodyID;
    public double orbitRPE;
    public double orbitECC;
    public float orbitINC;
    public float orbitAPE;
    public float orbitLAN;
    public double orbitTPP;
    public string stepName;
    public bool mayUserEditOrbit;
    public bool isTransferOrbit;
    public bool isDeletable;
    public NamedColour stepColour;

    public static TimelineStepSaveData AddButtonSaveData()
    {
        TimelineStepSaveData step = new TimelineStepSaveData();
        step.stepType = TimelineStepType.AddButton;
        return step;
    }

    public static TimelineStepSaveData StartFinishStepSaveData()
    {
        TimelineStepSaveData step = new TimelineStepSaveData();
        step.stepType = TimelineStepType.StartFinishTransitionStep;
        return step;
    }

    public static TimelineStepSaveData ManeuverStepSaveData()
    {
        TimelineStepSaveData step = new TimelineStepSaveData();
        step.stepType = TimelineStepType.ManeuverTransitionStep;
        return step;
    }

    public static TimelineStepSaveData OrbitalStepSaveData()
    {
        TimelineStepSaveData step = new TimelineStepSaveData();
        step.stepType = TimelineStepType.OrbitalStep;
        return step;
    }

    public static string DefaultTimelineJsonRepresentation
    {
        get
        {
            return "asdf";
        }
    }

    public static string JsonRepresentationDelimiter
    {
        get
        {
            return "\n\n";
        }
    }
}

public enum TimelineStepType
{
    AddButton,
    StartFinishTransitionStep,
    ManeuverTransitionStep,
    OrbitalStep
}
