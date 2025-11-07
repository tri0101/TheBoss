using SojaExiles;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class ColliderEnter : MonoBehaviour
{
    [SerializeField] private EnemyPatrolNav enemy;
    private void Start()
    {
        enemy = transform.parent.GetComponent<EnemyPatrolNav>();
    }
    private void OnTriggerEnter(Collider collision)
    {
        if (enemy.IsDead) return;
        if (collision != null && collision.transform.name.Contains("Dart"))
        {
            enemy.EnableRagdoll();
        }
    }
}
