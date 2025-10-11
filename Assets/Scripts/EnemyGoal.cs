using UnityEngine;

public class EnemyGoal : MonoBehaviour
{

    GameManager gameManager;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        gameManager = FindFirstObjectByType<GameManager>();
    }

    private void Update()
    {
        // For testing purposes, press G to simulate reaching the goal
        if (Input.GetKeyDown(KeyCode.G))
        {
            GetComponent<Health>()?.TakeDamage(10); // Inflict massive damage to ensure death
        }
    }

    public void Die()
    {
        Debug.Log($"{gameObject.name} (EnemyGoal) is handling death logic.");
        
        // Example: play animation, spawn loot, disable AI, etc.
        GameManager.Instance.LoseGame();
    }
}
