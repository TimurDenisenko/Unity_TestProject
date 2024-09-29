
using UnityEngine;

public class Inventory : Storage
{
    [SerializeField] public Transform canvasForDraggingItem;
    [SerializeField] RectTransform view;
    [SerializeField] float secondScale;
    [SerializeField] float secondPos;
    internal bool isChestOpen;
    float firstScale;
    float firstPos;

    private void Awake()
    {
        StaticSoldier.Inventory = this;
        SlotsCreating();
        firstScale = view.localPosition.x;
        firstPos = view.rect.width;
    }
    internal void SetFirstUI()
    {
        view.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, firstPos);
        view.SetLocalPositionAndRotation(new Vector3(firstScale, view.localPosition.y, 0), Quaternion.identity);
        isChestOpen = false;
    }
    internal void SetSecondUI()
    {
        view.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, secondPos);
        view.SetLocalPositionAndRotation(new Vector3(secondScale, view.localPosition.y, 0), Quaternion.identity);
        isChestOpen = true;
    }
}
