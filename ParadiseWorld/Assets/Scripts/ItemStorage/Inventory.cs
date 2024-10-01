
using UnityEngine;

public class Inventory : Storage
{
    [SerializeField] public Transform canvasForDraggingItem;
    [SerializeField] RectTransform view;
    [SerializeField] float secondPos;
    [SerializeField] float secondScale;
    float firstScale;
    float firstPos;

    private void Awake()
    {
        StaticSoldier.Inventory = this;
        SlotsCreating("InventorySlot");
        firstScale = view.localPosition.x;
        firstPos = view.rect.width;
    }
    internal void SetFirstUI()
    {
        view.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, firstPos);
        view.SetLocalPositionAndRotation(new Vector3(firstScale, view.localPosition.y, 0), Quaternion.identity);
    }
    internal void SetSecondUI(bool isLeft)
    {
        view.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, secondScale);
        view.SetLocalPositionAndRotation(new Vector3(isLeft ? secondPos : -secondPos, view.localPosition.y, 0), Quaternion.identity);
    }
}
