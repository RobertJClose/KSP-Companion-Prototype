using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class StartFinishTransitionStep : TransitionStep
{
    public override void OnSelect(BaseEventData eventData)
    {
        if (IndexInTimeline == 0)
        {
            // Start step.
            inspector.SetHeader("Mission Start Time:");
            InspectorPropertyBlock blockOne = inspector.AddPropertyBlock();
            blockOne.AddDoubleProperty("Transition Time (s UT)", () => TransitionTime, (double newTime) => TransitionTime = newTime);
        }
        else if (IndexInTimeline == missionTimeline.StepCount - 1)
        {
            // Finish step.
            inspector.SetHeader("Mission End Time:");
            InspectorPropertyBlock blockOne = inspector.AddPropertyBlock();
            blockOne.AddDoubleProperty("Transition Time (s UT)", () => TransitionTime, (double newTime) => TransitionTime = newTime);
        }

        plot.HighlightPlot(true);
    }
}
