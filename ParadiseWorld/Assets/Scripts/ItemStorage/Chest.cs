using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class Chest : Storage
{
    [SerializeField] Transform panel;
    [SerializeField] InputAction action;
    [SerializeField] GameObject chestUI;
    [SerializeField] Transform inventoryContent;
    private void Awake()
    {
        panel.gameObject.SetActive(false);
        action.started += OpenAction;
        SlotsCreating();
    }
    private void OpenAction(InputAction.CallbackContext obj)
    {
        StaticSoldier.ControlComponent.StorageUI(chestUI.activeSelf, chestUI);
        if (chestUI.activeSelf)
        {
            foreach (Slot slot in StaticSoldier.Inventory.slots)
            {
                Instantiate(slot, inventoryContent);
            }
        }
        else
        {
            foreach (Transform slot in inventoryContent)
            {
                Destroy(slot.gameObject);
            }
        }
    }

    private void LateUpdate()
    {
        if (panel.gameObject.activeSelf)
        {
            panel.LookAt(StaticSoldier.CameraComponent.transform);
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
