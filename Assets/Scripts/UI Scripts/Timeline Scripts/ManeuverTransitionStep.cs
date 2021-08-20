using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ManeuverTransitionStep : TransitionStep
{
    public override void OnSelect(BaseEventData eventData)
    {
        inspector.SetHeader("Maneuver:");
        InspectorPropertyBlock blockOne = inspector.AddPropertyBlock();
        blockOne.AddFloatProperty("Maneuver Time (s UT)", () => TransitionTime);
    }

    private void OnDestroy()
    {
        if (plot != null)
            Destroy(plot.gameObject);
    }
}
