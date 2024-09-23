using UnityEngine;
using UnityEngine.UI;

public class StartupFile : MonoBehaviour
{
    [SerializeField] EnemyInitData enemyInitData;
    private void Awake()
    {
        if (enemyInitData != null)
            Instantiate(enemyInitData.enemyPrefab.gameObject, enemyInitData.position, Quaternion.identity);
    }
}
