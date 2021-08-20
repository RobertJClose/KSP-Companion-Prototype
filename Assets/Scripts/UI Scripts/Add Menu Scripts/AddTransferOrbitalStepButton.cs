using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class AddTransferOrbitalStepButton : BaseAddMenuButton, IPointerClickHandler
{
    public override void OnPointerClick(PointerEventData eventData)
    {
        missionTimeline.CreateTransferOrbit(parentAddMenu.parentAddButtonIndexInTimeline);

        base.OnPointerClick(eventData); // Parent GameObject is destroyed in the base call
    }
}
