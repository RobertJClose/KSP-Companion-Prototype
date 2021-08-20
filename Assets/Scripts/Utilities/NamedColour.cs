using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class NamedColour
{
    [SerializeField]
    public Color color;
    [SerializeField]
    public string name;
    
    // Put this in a namespace???
    public static System.Converter<NamedColour, TMPro.TMP_Dropdown.OptionData> TMPDropdownOptionDataConverter = (NamedColour namedColour) => new TMPro.TMP_Dropdown.OptionData(namedColour.name, null); 

    public NamedColour(string name, Color color) { this.name = name; this.color = color; }

    public override string ToString()
    {
        return "Name: " + name + "\n Colour: " + color.ToString() + "\n" + base.ToString();
    }

}
