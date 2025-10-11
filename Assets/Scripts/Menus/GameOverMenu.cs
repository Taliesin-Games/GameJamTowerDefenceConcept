using TMPro;
using UnityEngine;

public class GameOverMenu : MonoBehaviour
{
    [SerializeField] TMP_Text finalScoreText;

    private void OnEnable()
    {
        if (GameManager.Instance != null)
        {
            finalScoreText.text = $"Final Score: {GameManager.Score}";
        }
        else
        {
            finalScoreText.text = "Final Score: 0";
        }
    }
    public void ReturnToMenu()
    {
        LevelManager.LoadMainMenu();

    }
}
