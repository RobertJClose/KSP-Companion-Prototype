using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveSlotButton : MonoBehaviour
{
    [SerializeField]
    private MissionTimeline missionTimeline;

    [SerializeField]
    private int slotNumber;

    public void Save()
    {
        missionTimeline.SaveMissionTimeline(slotNumber);
    }
}
