using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Events;

public class InspectorDoubleProperty : InspectorProperty
{
    TMP_InputField inputFieldComponent;
    double defaultValue;
    Func<double> ValueGetter;
    Action<double> ValueSetter;
    public bool IsEditable { get; set; }

    protected override void Awake()
    {
        base.Awake();
        inputFieldComponent = GetComponentInChildren<TMP_InputField>();
        inputFieldComponent.onEndEdit.AddListener(ValueChangedListener);
        IsEditable = true;
    }

    public void SetDefaultValue(float defaultValue)
    {
        this.defaultValue = defaultValue;
    }

    public void SetValueGetter(Func<double> ValueGetter)
    {
        this.ValueGetter = ValueGetter;
    }

    public void SetValueSetter(Action<double> ValueSetter)
    {
        this.ValueSetter = ValueSetter;
    }

    public void SetEditableState(bool isEditable)
    {
        this.IsEditable = isEditable;
    }

    public void ValueChangedListener(string newValue)
    {
        if (string.IsNullOrWhiteSpace(newValue))
        {
            // Store the default value for this float property.
            ValueSetter?.Invoke(defaultValue);
        }
        else
        {
            // Update the stored value for the float with the newValue.
            double value = double.Parse(newValue, System.Globalization.CultureInfo.InvariantCulture);
            ValueSetter?.Invoke(value);
        }

        // Tell the inspector to update the values of all the displayed properties.
        inspector.UpdateInspectorProperties();
    }

    public override void UpdateDisplayedValue()
    {
        inputFieldComponent.text = ValueGetter?.Invoke().ToString();

        if (ValueSetter == null || !IsEditable)
            inputFieldComponent.interactable = false;

        if (DisplayCondition?.Invoke() == false && gameObject.activeInHierarchy)
            gameObject.SetActive(false);
        else if (DisplayCondition?.Invoke() == true && !gameObject.activeInHierarchy)
            gameObject.SetActive(true);
    }
}
