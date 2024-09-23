using UnityEngine;

[CreateAssetMenu(fileName = "EnemyData", menuName = "Unit initialize/Enemy", order = 0)]
public class EnemyInitData : ScriptableObject
{
    [SerializeField] public EnemyControl enemyPrefab;
    [SerializeField] public Vector3 position;
}
