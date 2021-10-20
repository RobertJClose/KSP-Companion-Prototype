using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HelpButton : MonoBehaviour
{
    [SerializeField]
    private GameObject prefab_HelpScreen;

    private Transform _mainCanvas; 

    private void Awake()
    {
        _mainCanvas = GameObject.FindGameObjectWithTag("Main Canvas").transform;
    }

    public void ShowHelpScreen()
    {
        Instantiate(prefab_HelpScreen, _mainCanvas);
    }
}
