using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ResourceBox : MonoBehaviour
{
    [SerializeField] SerializedDictionary<int, Item> Items;
    [SerializeField] GameObject Box;
    [SerializeField] GameObject CrushedBox;
    Dictionary<int, Item> uDict;
    bool isLaunched = false;
    Animator animator;
    private void Awake()
    {
        animator = GetComponent<Animator>();
        uDict = Items.ToDictionary();
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (isLaunched)
            return;
        if (collision.gameObject.CompareTag("Player"))
        {
            Animation();
            Action();
        }
    }

    private void Action()
    {
        int value = Random.Range(1, 100);
        Item item = uDict[uDict.Keys.Where(x => x >= value).Min()];
        SoldierComponents.InventoryComponent.AddItem(item);
        Destroy(this);
    }

    private void Animation()
    {
        isLaunched = true;
        Box.SetActive(false);
        CrushedBox.SetActive(true);
        animator.PlayAnimation("Bang");
    }
}
