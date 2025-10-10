using UnityEngine;
using Utils;

public class Health : MonoBehaviour
{

    [SerializeField] float MaxHealth = 100f;
    float CurrentHealth = 0;

    void Start()
    {
        CurrentHealth = MaxHealth;
    }

    public void TakeDamage(float damage)
    {
        CurrentHealth -= damage;
        Debugger.Log($"{transform.root.name} has taken {damage} damage");
        if (CurrentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Debugger.Log($"{transform.root.name} has died");
        SendMessage("Die", SendMessageOptions.DontRequireReceiver);
    }
}
