using UnityEngine;

public class Enemy : MonoBehaviour
{
    static int count = 0;

    public static int EnemyCount => count;

    private void OnEnable()
    {
        count++;
    }

    private void Update()
    {
        // Here for debugging to manually trigger death
        if (Input.GetKeyDown(KeyCode.P))
        {
            Die();
        }
    }

    public void Die()
    {
        Debug.Log($"{gameObject.name} (Enemy) is handling death logic.");
        count--;
        // Example: play animation, spawn loot, disable AI, etc.
        Destroy(gameObject, 1f); // simple cleanup for demo
    }

    private void OnDestroy()
    {
        GameManager.Instance.CheckWinGame();
    }

}
