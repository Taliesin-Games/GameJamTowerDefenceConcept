using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    #region Congiguration
    [SerializeField] float spawnInterval = 0.5f;
    [SerializeField] int spawnCount = 10;
    [SerializeField] GameObject enemyPrefab;
    [SerializeField] GameObject[] spawnLocations;
    #endregion

    #region Cached References
    GameObject enemiesContainer;
    #endregion

    #region Runtime Variables
    float nextSpawnTime;
    int spawnedCount = 0;
    bool doneSpawning => spawnedCount >= spawnCount;
    #endregion

    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        nextSpawnTime = Time.time + spawnInterval;

        // Find object called "Enemies" and spawn it if not found
        enemiesContainer = GameObject.Find("Enemies");
        if (enemiesContainer == null)
        {

        }

    }

    // Update is called once per frame
    void Update()
    {
        SpawnEnemies();
    }

    private void SpawnEnemies()
    {
        if (nextSpawnTime < Time.time) return;
        if (doneSpawning) return;

        int i = Random.Range(0, spawnLocations.Length);
        Transform spawnLocation = spawnLocations[i].transform;

        var newEnemy = Instantiate(enemyPrefab, spawnLocation.position, Quaternion.identity);
    }
}
