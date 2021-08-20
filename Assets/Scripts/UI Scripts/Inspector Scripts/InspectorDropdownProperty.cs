using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class InspectorDropdownProperty : InspectorProperty
{
    TMP_Dropdown dropdownComponent;
    Func<object> ValueGetter;
    Action<object> ValueSetter;
    List<object> optionsList;
    
    protected override void Awake()
    {
        base.Awake();
        dropdownComponent = GetComponentInChildren<TMP_Dropdown>();
        dropdownComponent.onValueChanged.AddListener(ValueChangedListener);
    }

    public void SetDropdownSettings<TOptionType>(Func<TOptionType> ValueGetter, Action<TOptionType> ValueSetter, List<TOptionType> optionsList, Converter<TOptionType, TMP_Dropdown.OptionData> Converter) where TOptionType : class
    {
        this.ValueGetter = ValueGetter;
        this.ValueSetter = new Action<object>((object obj) => ValueSetter(obj as TOptionType));
        this.optionsList = optionsList.ConvertAll<object>((TOptionType item) => item);
        List<TMP_Dropdown.OptionData> convertedList = optionsList.ConvertAll(Converter);
        dropdownComponent.ClearOptions();
        dropdownComponent.AddOptions(convertedList);
    }

    public void ValueChangedListener(int index)
    {
        ValueSetter?.Invoke(optionsList[index]);

        inspector.UpdateInspectorProperties();
    }

    public override void UpdateDisplayedValue()
    {
        if (ValueGetter != null)
        {
            dropdownComponent.SetValueWithoutNotify( optionsList.FindIndex( (object item) => item.Equals(ValueGetter()) ) );
        }

        if (DisplayCondition?.Invoke() == false && gameObject.activeInHierarchy)
            gameObject.SetActive(false);
        else if (DisplayCondition?.Invoke() == true && !gameObject.activeInHierarchy)
            gameObject.SetActive(true);
    }
}
