using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class Chest : Storage
{
    [SerializeField] Transform panel;
    [SerializeField] InputAction action;
    [SerializeField] public GameObject chestCanvas;
    private void Start()
    {
        panel.gameObject.SetActive(false);
        action.started += (obj) => SoldierComponents.InterfaceComponent.ChestWindow(this);
        SlotsCreating("ChestSlot");
    }

    private void LateUpdate()
    {
        if (panel.gameObject.activeSelf)
        {
            panel.LookAt(SoldierComponents.CameraComponent.transform);
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            panel.gameObject.SetActive(true);
            action.Enable();
        }
    }
    private void OnTriggerExit(Collider other)
    {
        panel.gameObject.SetActive(false);
        action.Disable();
    }
}    
