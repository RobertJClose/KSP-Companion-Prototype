using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public abstract class InspectorProperty : MonoBehaviour
{
    protected Inspector inspector;
    protected InspectorPropertyBlock parentPropertyBlock;
    TextMeshProUGUI propertyText;
    protected Func<bool> DisplayCondition; // If the method invoked returns true, then this property should be displayed. If false, then don't display this property.

    protected virtual void Awake()
    {
        inspector = GameObject.FindGameObjectWithTag("Inspector").GetComponent<Inspector>();
        parentPropertyBlock = transform.parent.GetComponent<InspectorPropertyBlock>();
        propertyText = GetComponentInChildren<TextMeshProUGUI>();
    }

    public void SetPropertyText(string text)
    {
        propertyText.text = text;
    }

    public void SetDisplayCondition(Func<bool> DisplayCondition)
    {
        this.DisplayCondition = DisplayCondition;
    }

    public abstract void UpdateDisplayedValue();
}
