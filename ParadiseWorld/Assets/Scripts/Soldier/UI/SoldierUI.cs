
using UnityEngine;
using static UnityEditorInternal.ReorderableList;

public class SoldierUI : MonoBehaviour
{
    [SerializeField] GameObject equipmentCanvas;
    [SerializeField] GameObject inventoryCanvas;
    private void Awake()
    {
        SoldierComponents.InterfaceComponent = this;
    }
    public void StorageUI(GameObject canvas)
    {
        if (canvas.activeSelf)
        {
            SoldierComponents.CurrentUI = UIType.None;
            canvas.SetActive(false);
            Cursor.lockState = CursorLockMode.Locked;
            SoldierComponents.CameraComponent.enabled = true;
            StorageSetting.SlotTooltip.HideTooltip();
            SoldierComponents.ControlComponent.EnableControlInputs();
        }
        else
        {
            canvas.SetActive(true);
            Cursor.lockState = CursorLockMode.Confined;
            SoldierComponents.CameraComponent.enabled = false;
            SoldierComponents.ControlComponent.DisableControlInputs();
        }
    }
    public void CombatEquipmentWindow()
    {
        SoldierComponents.CurrentUI = UIType.Equipment;
        StorageUI(equipmentCanvas);
        StorageUI(inventoryCanvas);
        if (!equipmentCanvas.activeSelf)
        {
            SetBasicInventoryUI();
            SoldierComponents.InventoryComponent.SetDefaultInventory();
        }
        else
        {
            SetHalfInventoryUI(false);
            SoldierComponents.InventoryComponent.SortBy(typeof(Sword));
        }
    }
    public void InventoryWindow()
    {
        SoldierComponents.CurrentUI = UIType.Inventory;
        StorageUI(inventoryCanvas);
    }
    private void SetBasicInventoryUI()
    {
        SoldierComponents.InventoryComponent.inventoryScrollView.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, SoldierComponents.InventoryComponent.defaultScale);
        SoldierComponents.InventoryComponent.inventoryScrollView.SetLocalPositionAndRotation(new Vector3(SoldierComponents.InventoryComponent.defaultPos, SoldierComponents.InventoryComponent.inventoryScrollView.localPosition.y, 0), Quaternion.identity);
    }
    private void SetHalfInventoryUI(bool isLeftSide)
    {
        SoldierComponents.InventoryComponent.inventoryScrollView.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, SoldierComponents.InventoryComponent.secondScale);
        SoldierComponents.InventoryComponent.inventoryScrollView.SetLocalPositionAndRotation(new Vector3(SoldierComponents.InventoryComponent.secondPos * (isLeftSide ? 1 : -1), SoldierComponents.InventoryComponent.inventoryScrollView.localPosition.y, 0), Quaternion.identity);
    }
    public void ChestWindow(Chest chest)
    {
        SoldierComponents.CurrentUI = UIType.Chest;
        StorageUI(chest.chestCanvas);
        StorageUI(inventoryCanvas);
        if (!chest.chestCanvas.activeSelf)
        {
            SetBasicInventoryUI();
            SoldierComponents.CurrentChestComponent = null;
            Slot.StopDragging();
        }
        else
        {
            SetHalfInventoryUI(true);
            SoldierComponents.CurrentChestComponent = chest;
        }
    }

}
