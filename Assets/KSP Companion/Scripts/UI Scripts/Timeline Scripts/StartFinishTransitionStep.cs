﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class StartFinishTransitionStep : TransitionStep
{
    private bool isStartStep;
    public bool IsStartStep { get { return isStartStep; } set { isStartStep = value; } }
    public bool IsFinishStep { get { return !isStartStep; } set { isStartStep = !value; } }

    public override void OnSelect(BaseEventData eventData)
    {
        inspector.Clear();

        if (IndexInTimeline == 0)
        {
            // Start step.
            inspector.SetHeader("Mission Start Time:");
            InspectorPropertyBlock blockOne = inspector.AddPropertyBlock();
            blockOne.AddDoubleProperty("Start Time (s UT)", () => TransitionTime, (double newTime) => TransitionTime = newTime);
        }
        else if (IndexInTimeline == missionTimeline.StepCount - 1)
        {
            // Finish step.
            inspector.SetHeader("Mission Finish Time:");
            InspectorPropertyBlock blockOne = inspector.AddPropertyBlock();
            blockOne.AddDoubleProperty("Finish Time (s UT)", () => TransitionTime, (double newTime) => TransitionTime = newTime);
        }

        plot.HighlightPlot(true);
    }
}
