using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.UI;
using UnityEngine.EventSystems;

public class EventSystemController : MonoBehaviour
{
    // This singleton exists to sidestep a bug in the InputField/MultiplayerEventSystem interaction that allows an InputField
    // to become the selected object for a MES with a different PlayerRoot. It happens because the InputField makes itself
    // selected in LateUpdate(), and this avoids the PlayerRoot checking performed by the Input System Input Module during Update().
    // The fix is to set EventSystem.current to be the MES with the PlayerRoot of the InputField that is causing the issue, so that 
    // by the time of LateUpdate() the value of EventSystem.current is the correct MES. 

    [SerializeField]
    private MultiplayerEventSystem inspectorEventSystem;
    [SerializeField]
    private MultiplayerEventSystem timelineEventSystem;

    private void Start()
    {
        EventSystem.current = inspectorEventSystem;
    }
}
