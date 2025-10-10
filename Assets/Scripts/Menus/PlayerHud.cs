using UnityEngine;

public class PlayerHud : MonoBehaviour
{
    public int currentPlayerHealth;
    public int currentWave;
    public int currentMana;
    public void SetCurrentPlayerHealth(int value)
    {
        Debug.Log("Current Player Health: " + value);
    }
    public void SetCurrentWave(int value)
    {
        Debug.Log("Current Wave" + value);
    }
    public void SetCurrentMana(int value)
    {
        Debug.Log("Current Mana" + value);
    }
}
