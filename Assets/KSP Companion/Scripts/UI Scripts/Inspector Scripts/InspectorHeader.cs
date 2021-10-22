using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;

public class InspectorHeader : MonoBehaviour
{
    // Child GameObject Component References
    [SerializeField]
    TextMeshProUGUI headerTextComponent;
    [SerializeField]
    Button deleteButton;

    public void SetText(string newText)
    {
        headerTextComponent.text = newText;
    }

    public void SetDeleteButtonListener(UnityAction listener)
    {
        if (listener != null)
        {
            deleteButton.gameObject.SetActive(true);
            deleteButton.onClick.AddListener(listener);
        }
    }

    public void ResetHeader()
    {
        deleteButton.onClick.RemoveAllListeners();
        deleteButton.gameObject.SetActive(false);
    }

    private void Start()
    {
        ResetHeader();
    }
}
