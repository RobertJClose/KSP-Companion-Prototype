using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

public class AddButton : GapStep, IPointerClickHandler
{
    [SerializeField]
    AddMenu prefab_AddMenu;
    Transform mainCanvas;

    protected override void Awake()
    {
        base.Awake();
        mainCanvas = GameObject.FindGameObjectWithTag("Main Canvas").transform;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        OpenAddMenu();
    }

    private void OpenAddMenu()
    {
        AddMenu addMenu = Instantiate(prefab_AddMenu, mainCanvas);
        addMenu.parentAddButtonIndexInTimeline = IndexInTimeline;
        (addMenu.transform as RectTransform).anchoredPosition = Mouse.current.position.ReadValue();
    }
}
