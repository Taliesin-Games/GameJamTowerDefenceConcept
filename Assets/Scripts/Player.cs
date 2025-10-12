using StarterAssets;
using UnityEngine;
using Utils;

public class Player : MonoBehaviour
{
    #region Chached components
    Health health;
    #endregion

    #region properties
    public bool IsDead => health?.IsDead ?? true;       // Using Player as a common interface for subcomponents.
    #endregion
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        health = GetComponent<Health>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
}
