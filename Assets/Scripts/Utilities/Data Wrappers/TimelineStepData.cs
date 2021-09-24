public class TimelineStepData
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

    public static TimelineStepData AddButtonSaveData()
    {
        TimelineStepData step = new TimelineStepData();
        step.stepType = TimelineStepType.AddButton;
        return step;
    }

    public static TimelineStepData StartFinishStepSaveData()
    {
        TimelineStepData step = new TimelineStepData();
        step.stepType = TimelineStepType.StartFinishTransitionStep;
        return step;
    }

    public static TimelineStepData ManeuverStepSaveData()
    {
        TimelineStepData step = new TimelineStepData();
        step.stepType = TimelineStepType.ManeuverTransitionStep;
        return step;
    }

    public static TimelineStepData OrbitalStepSaveData()
    {
        TimelineStepData step = new TimelineStepData();
        step.stepType = TimelineStepType.OrbitalStep;
        return step;
    }

    public static string StepDelimiter
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
