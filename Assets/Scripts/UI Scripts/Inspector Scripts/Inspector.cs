using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Inspector : MonoBehaviour
{
    // Members
    readonly List<InspectorPropertyBlock> propertyBlocks = new List<InspectorPropertyBlock>();

    // Prefabs
    [SerializeField]
    InspectorPropertyBlock prefab_InspectorPropertyBlock;

    // Scene references
    [SerializeField]
    InspectorHeader header;
    [SerializeField]
    Transform porkChopArea;
    [SerializeField]
    Transform propertiesArea;
    [SerializeField]
    MissionTimeline missionTimeline;

    public void SetHeader(string headerText)
    {
        SetHeader(headerText, null);
    }

    public void SetHeader(string headerText, UnityAction deleteButtonListener)
    {
        header.SetText(headerText);
        header.SetDeleteButtonListener(deleteButtonListener);
    }

    public InspectorPropertyBlock AddPorkChopPropertyBlock()
    {
        InspectorPropertyBlock newPropertyBlock = Instantiate(prefab_InspectorPropertyBlock, porkChopArea);
        propertyBlocks.Add(newPropertyBlock);
        return newPropertyBlock;
    }

    public InspectorPropertyBlock AddPropertyBlock()
    {
        InspectorPropertyBlock newPropertyBlock = Instantiate(prefab_InspectorPropertyBlock, propertiesArea);
        propertyBlocks.Add(newPropertyBlock);
        return newPropertyBlock;
    }

    public void UpdateInspectorProperties()
    {
        foreach (InspectorPropertyBlock item in propertyBlocks)
        {
            item.UpdateBlockProperties();
        }

        missionTimeline.PlotSteps();
    }

    public void Clear()
    {
        header.ResetHeader();
        foreach (InspectorPropertyBlock item in propertyBlocks)
        {
            Destroy(item.gameObject);
        }
        propertyBlocks.Clear();
    }

    public void DisplayMissionOverview()
    {
        Clear();

        SetHeader("Mission Overview");

        InspectorPropertyBlock overviewBlock = AddPropertyBlock();
        overviewBlock.AddDoubleProperty("Start Time (s UT)", () => missionTimeline.StartStep.TransitionTime);
        overviewBlock.AddDoubleProperty("Finish Time (s UT)", () => missionTimeline.FinishStep.TransitionTime);
        overviewBlock.AddDoubleProperty("Duration (s)", () => missionTimeline.FinishStep.TransitionTime - missionTimeline.StartStep.TransitionTime);
        overviewBlock.AddDoubleProperty("Total \u0394V (m/s)", () => missionTimeline.TryGetTotalDeltaV().GetValueOrDefault(), DisplayCondition: () => missionTimeline.IsMissionConnected() == true);
    }

}
