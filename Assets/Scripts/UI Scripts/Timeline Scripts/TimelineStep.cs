using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public abstract class TimelineStep : MonoBehaviour
{
    // Members
    public int IndexInTimeline { get { return missionTimeline.GetMissionTimeline().IndexOf(this); } }
    
    [SerializeField] protected TimelineStep previousStep;
    public TimelineStep PreviousStep { get { return previousStep; } }
    
    [SerializeField] protected TimelineStep nextStep;
    public TimelineStep NextStep { get { return nextStep; } }

    [SerializeField] protected TransitionStep previousTransitionStep;
    public TransitionStep PreviousTransitionStep { get { return previousTransitionStep; } }
    
    [SerializeField] protected TransitionStep nextTransitionStep;
    public TransitionStep NextTransitionStep { get { return nextTransitionStep; } }
    
    [SerializeField] protected OrbitalStep previousOrbitalStep;
    public OrbitalStep PreviousOrbitalStep { get { return previousOrbitalStep; } }
    
    [SerializeField] protected OrbitalStep nextOrbitalStep;
    public OrbitalStep NextOrbitalStep { get { return nextOrbitalStep; } }

    // Scene References
    protected Inspector inspector;
    protected Transform root3DScene;
    protected MissionTimeline missionTimeline;

    protected virtual void Awake()
    {
        missionTimeline = GameObject.FindGameObjectWithTag("Mission Timeline").GetComponent<MissionTimeline>();
        inspector = GameObject.FindGameObjectWithTag("Inspector").GetComponent<Inspector>();
        root3DScene = GameObject.FindGameObjectWithTag("3D Scene Root").transform;
    }

    public virtual void UpdateSurroundingSteps()
    {
        previousStep = missionTimeline.GetMissionTimeline().FindLast(step => step.IndexInTimeline < IndexInTimeline);
        previousTransitionStep = missionTimeline.GetMissionTimeline().FindLast(step => step is TransitionStep && step.IndexInTimeline < IndexInTimeline) as TransitionStep;
        previousOrbitalStep = missionTimeline.GetMissionTimeline().FindLast(step => step is OrbitalStep && step.IndexInTimeline < IndexInTimeline) as OrbitalStep;

        nextStep = missionTimeline.GetMissionTimeline().Find(step => step.IndexInTimeline > IndexInTimeline);
        nextTransitionStep = missionTimeline.GetMissionTimeline().Find(step => step is TransitionStep && step.IndexInTimeline > IndexInTimeline) as TransitionStep;
        nextOrbitalStep = missionTimeline.GetMissionTimeline().Find(step => step is OrbitalStep && step.IndexInTimeline > IndexInTimeline) as OrbitalStep;
    }
}
