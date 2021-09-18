using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MissionPlannerButton : MonoBehaviour
{
    public void LoadMissionPlanner()
    {
        SceneManager.LoadScene("MissionPlanner", LoadSceneMode.Single);
    }
}
