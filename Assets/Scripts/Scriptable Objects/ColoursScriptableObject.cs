using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Colour Swatch", menuName = "ScriptableObjects/ColoursScriptableObject", order = 1)]
public class ColoursScriptableObject : ScriptableObject
{
    public List<NamedColour> colours;
}
