using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public abstract class TimelineStep : MonoBehaviour
{
    // Members
    public int IndexInTimeline { get { return missionTimeline.GetMissionTimeline().IndexOf(this); } }
    [SerializeField] protected TimelineStep preceedingStep;
    public TimelineStep PreviousStep { get { return preceedingStep; } }
    [SerializeField] protected TimelineStep nextStep;
    public TimelineStep NextStep { get { return nextStep; } }
    [SerializeField] protected TransitionStep earlierTransitionStep;
    public TransitionStep PreviousTransitionStep { get { return earlierTransitionStep; } }
    [SerializeField] protected TransitionStep laterTransitionStep;
    public TransitionStep NextTransitionStep { get { return laterTransitionStep; } }
    [SerializeField] protected OrbitalStep previousOrbitalStep;
    public OrbitalStep PreviousOrbitalStep { get { return previousOrbitalStep; } }
    [SerializeField] protected OrbitalStep followingOrbitalStep;
    public OrbitalStep NextOrbitalStep { get { return followingOrbitalStep; } }

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
        preceedingStep = missionTimeline.GetMissionTimeline().FindLast(step => step.IndexInTimeline < IndexInTimeline);
        earlierTransitionStep = missionTimeline.GetMissionTimeline().FindLast(step => step is TransitionStep && step.IndexInTimeline < IndexInTimeline) as TransitionStep;
        previousOrbitalStep = missionTimeline.GetMissionTimeline().FindLast(step => step is OrbitalStep && step.IndexInTimeline < IndexInTimeline) as OrbitalStep;

        nextStep = missionTimeline.GetMissionTimeline().Find(step => step.IndexInTimeline > IndexInTimeline);
        laterTransitionStep = missionTimeline.GetMissionTimeline().Find(step => step is TransitionStep && step.IndexInTimeline > IndexInTimeline) as TransitionStep;
        followingOrbitalStep = missionTimeline.GetMissionTimeline().Find(step => step is OrbitalStep && step.IndexInTimeline > IndexInTimeline) as OrbitalStep;
    }
}
