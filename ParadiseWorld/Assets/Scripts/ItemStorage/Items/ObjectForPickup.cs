using Unity.Collections;
using UnityEngine;

[RequireComponent(typeof(BoxCollider),typeof(Rigidbody))]
public class ObjectForPickup : MonoBehaviour
{
    [SerializeField] Item Item;
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            SoldierComponents.InventoryComponent.AddItem(Item);
            Destroy(gameObject);
        }
    }
}
