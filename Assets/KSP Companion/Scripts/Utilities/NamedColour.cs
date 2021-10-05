using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct NamedColour
{
    [SerializeField]
    public Color color;
    [SerializeField]
    public string name;
    
    // Put this in a namespace???
    public static System.Converter<object, TMPro.TMP_Dropdown.OptionData> TMPDropdownOptionDataConverter = (object namedColour) => new TMPro.TMP_Dropdown.OptionData(((NamedColour)namedColour).name, null); 

    public NamedColour(string name, Color color) { this.name = name; this.color = color; }

    public override string ToString()
    {
        return "Name: " + name + "\n Colour: " + color.ToString() + "\n" + base.ToString();
    }

}
