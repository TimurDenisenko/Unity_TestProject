using TMPro;
using UnityEngine;

public class Chest : MonoBehaviour
{
    [SerializeField] Transform panel;
    private void Awake()
    {
        panel.gameObject.SetActive(false);
    }
    private void LateUpdate()
    {
        if (panel.gameObject.activeSelf)
            panel.LookAt(StaticSoldier.CameraComponent.transform);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
            panel.gameObject.SetActive(true);
    }
    private void OnTriggerExit(Collider other)
    {
        panel.gameObject.SetActive(false);
    }
}    
