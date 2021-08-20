using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class BaseAddMenuButton : MonoBehaviour, IPointerClickHandler
{
    [SerializeField]
    protected AddMenu parentAddMenu;
    protected MissionTimeline missionTimeline;

    protected virtual void Awake()
    {
        missionTimeline = GameObject.FindGameObjectWithTag("Mission Timeline").GetComponent<MissionTimeline>();
    }

    public virtual void OnPointerClick(PointerEventData eventData)
    {
        Destroy(parentAddMenu.gameObject);
    }
}
