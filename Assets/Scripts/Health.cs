using UnityEngine;
using Utils;

public class Health : MonoBehaviour
{

    [SerializeField] float MaxHealth = 100f;
    float CurrentHealth = 0;

    bool isDead;

    public bool IsDead => isDead;

    void Start()
    {
        CurrentHealth = MaxHealth;
    }

    public void TakeDamage(float damage)
    {
        if(isDead) return; // Ignore damage if already dead

        CurrentHealth -= damage;
        //Debugger.Log($"{transform.root.name} has taken {damage} damage");
        if (CurrentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        if (isDead) return; // Prevent multiple death triggers
        isDead = true;
        //Debugger.Log($"{gameObject.name} has died");
        SendMessage("Die", SendMessageOptions.DontRequireReceiver);
    }
}
