using System.Collections.Generic;
using UnityEngine;

public class Tower : MonoBehaviour
{
    [SerializeField] float range = 5f;
    [Tooltip("Projectiles per second")]
    [Range(0.01f, 100f)]
    [SerializeField] float fireRate = 1f;
    [SerializeField] GameObject projectileEmitter;
    [SerializeField] Projectile projectilePrefab;

    public static List<Enemy> enemies = new List<Enemy>();

    float nextAttackTime = 0f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        nextAttackTime = Time.time + 1f / fireRate;
    }

    // Update is called once per frame
    void Update()
    {
        if(Time.time >= nextAttackTime)
        {
            Attack();
            nextAttackTime = Time.time + 1f / fireRate;
        }
    }

    void Attack()
    {
        Enemy enemy = GetNearestEnemy();
        if(!enemy) return;

        Vector3 directionToEnemy = (enemy.transform.position - projectileEmitter.transform.position);

        Projectile proj = Instantiate(projectilePrefab, projectileEmitter.transform.position, Quaternion.identity);
        proj.Direction = new Vector2(directionToEnemy.x, directionToEnemy.z);
        proj.FiringParent = this.gameObject;
    }
    Enemy GetNearestEnemy()
    {
        if (enemies.Count == 0) return null;
        Enemy closest = null;

        foreach(var enemy in enemies)
        {
            if (!enemy) continue;
            
            float distToEnemy = (enemy.transform.position - projectileEmitter.transform.position).sqrMagnitude;
            if (distToEnemy > range * range) continue; // Out of range
            if (!closest) closest = enemy;
            else
            {
                float distToClosest = (closest.transform.position - projectileEmitter.transform.position).sqrMagnitude;
                if (distToEnemy < distToClosest) closest = enemy;
            }
        }

        return closest;
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Tower collided with " + collision.gameObject.name);
    }
}
