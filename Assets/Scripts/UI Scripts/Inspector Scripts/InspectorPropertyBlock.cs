using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InspectorPropertyBlock : MonoBehaviour
{
    // Prefabs
    [SerializeField]
    InspectorFloatProperty prefab_FloatProperty;
    [SerializeField]
    InspectorDropdownProperty prefab_DropdownProperty;

    // Members
    GridLayoutGroup gridLayoutGroup;
    Action UpdateDisplays;

    private void Awake()
    {
        // The PropertyBlock gameobjects have a grid layout group that is meant to contain Property prefabs of various types. The Property Block
        // is to help group related/similar properties together. 
        // The grid size of the grid layout group must be set to match the size of each property UI element, which can be found by a 
        // reference to the Property prefab used as a base for the different types of properties. 
        // The number of columns of the grid layout group is then set so that as many properties as possible will fit into the parent transform
        // of the property block, given the amount of padding and spacing along the x direction
        gridLayoutGroup = GetComponent<GridLayoutGroup>();
        gridLayoutGroup.cellSize = prefab_FloatProperty.GetComponent<RectTransform>().rect.size;
        gridLayoutGroup.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        float availableSpace = (transform.parent as RectTransform).rect.width;
        float xPadding = gridLayoutGroup.padding.left + gridLayoutGroup.padding.right;
        float xSpacing = gridLayoutGroup.spacing.x;
        float cellWidth = gridLayoutGroup.cellSize.x;
        gridLayoutGroup.constraintCount = Mathf.FloorToInt((availableSpace - xPadding + xSpacing) / (cellWidth + xSpacing)); // Do some inequality maths for why this formula works
    }

    public void AddFloatProperty(string text, Func<float> ValueGetter)
    {
        AddFloatProperty(text, ValueGetter, null, null, true);
    }

    public void AddFloatProperty(string text, Func<float> ValueGetter, Func<bool> DisplayCondition)
    {
        AddFloatProperty(text, ValueGetter, null, DisplayCondition, true);
    }

    public void AddFloatProperty(string text, Func<float> ValueGetter, Action<float> ValueSetter)
    {
        AddFloatProperty(text, ValueGetter, ValueSetter, null, true);
    }

    public void AddFloatProperty(string text, Func<float> ValueGetter, Action<float> ValueSetter, Func<bool> DisplayCondition)
    {
        AddFloatProperty(text, ValueGetter, ValueSetter, DisplayCondition, true);
    }

    public void AddFloatProperty(string text, Func<float> ValueGetter, bool isEditable)
    {
        AddFloatProperty(text, ValueGetter, null, null, isEditable);
    }

    public void AddFloatProperty(string text, Func<float> ValueGetter, Func<bool> DisplayCondition, bool isEditable)
    {
        AddFloatProperty(text, ValueGetter, null, DisplayCondition, isEditable);
    }

    public void AddFloatProperty(string text, Func<float> ValueGetter, Action<float> ValueSetter, bool isEditable)
    {
        AddFloatProperty(text, ValueGetter, ValueSetter, null, isEditable);
    }

    public void AddFloatProperty(string text, Func<float> ValueGetter, Action<float> ValueSetter, Func<bool> DisplayCondition, bool isEditable)
    {
        InspectorFloatProperty newFloat = Instantiate(prefab_FloatProperty, transform);

        newFloat.SetPropertyText(text);
        newFloat.SetValueGetter(ValueGetter);
        newFloat.SetValueSetter(ValueSetter);
        newFloat.SetDisplayCondition(DisplayCondition);
        newFloat.SetEditableState(isEditable);
        UpdateDisplays += newFloat.UpdateDisplayedValue;
        newFloat.UpdateDisplayedValue();
    }

    public void AddDropdownProperty<TOptionType>(string text, Func<TOptionType> ValueGetter, Action<TOptionType> ValueSetter, List<TOptionType> optionsList, Converter<TOptionType, TMP_Dropdown.OptionData> Converter) where TOptionType : class
    {
        AddDropdownProperty(text, ValueGetter, ValueSetter, optionsList, Converter, null);
    }

    public void AddDropdownProperty<TOptionType>(string text, Func<TOptionType> ValueGetter, Action<TOptionType> ValueSetter, List<TOptionType> optionsList, Converter<TOptionType, TMP_Dropdown.OptionData> Converter, Func<bool> DisplayCondition) where TOptionType : class
    {
        InspectorDropdownProperty newDropdown = Instantiate(prefab_DropdownProperty, transform);

        newDropdown.SetPropertyText(text);
        newDropdown.SetDropdownSettings(ValueGetter, ValueSetter, optionsList, Converter);
        newDropdown.SetDisplayCondition(DisplayCondition);
        UpdateDisplays += newDropdown.UpdateDisplayedValue;
        newDropdown.UpdateDisplayedValue();
    }


    public void UpdateBlockProperties()
    {
        UpdateDisplays();
    }
}
