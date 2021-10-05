using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ExitButton : MonoBehaviour
{
    [SerializeField]
    MissionTimeline missionTimeline;

    public void ExitMissionPlanner()
    {
        missionTimeline.SaveMissionTimeline(saveSlot: -1);              // Save the timeline to the temporary cache.
        SceneManager.LoadScene("TitleScreen", LoadSceneMode.Single);    // Load the title screen.
    }
}
