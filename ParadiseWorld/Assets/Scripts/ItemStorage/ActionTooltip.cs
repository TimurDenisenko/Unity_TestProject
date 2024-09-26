using TMPro;
using UnityEngine;

public class ActionTooltip : MonoBehaviour
{
    private void LateUpdate()
    {
        transform.LookAt(StaticSoldier.CameraComponent.transform);
    }
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log(1); 
    }
}
