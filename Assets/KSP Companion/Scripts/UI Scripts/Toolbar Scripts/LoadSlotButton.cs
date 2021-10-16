using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadSlotButton : MonoBehaviour
{
    [SerializeField]
    private MissionTimeline missionTimeline;

    [SerializeField]
    private int slotNumber;

    public void Load()
    {
        missionTimeline.LoadMissionTimeline(slotNumber);
    }
}
