using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class AddFreeOrbitalStepButton : BaseAddMenuButton, IPointerClickHandler
{
    public override void OnPointerClick(PointerEventData eventData)
    {
        missionTimeline.CreateFreeOrbitalStep(parentAddMenu.parentAddButtonIndexInTimeline);

        base.OnPointerClick(eventData); // Parent GameObject is destroyed in the base call
    }
}
