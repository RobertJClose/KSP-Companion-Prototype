using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewButton : MonoBehaviour
{
    [SerializeField]
    MissionTimeline missionTimeline;

    public void NewTimeline()
    {
        missionTimeline.SetTimelineToDefault();
    }
}
