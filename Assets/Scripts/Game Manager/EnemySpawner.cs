using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public static EnemySpawner Instance;

    const string ENEMIES_CONTAINER_NAME = "Enemies";

    #region Congiguration
    [SerializeField] float spawnInterval = 0.5f;
    [SerializeField] int spawnCount = 10;
    [SerializeField] GameObject enemyPrefab;
    [SerializeField] GameObject[] spawnLocations;
    [SerializeField] GameObject enemyGoal;
    #endregion

    #region Cached References
    GameObject enemiesContainer;
    #endregion

    #region Runtime Variables
    float nextSpawnTime;
    int spawnedCount = 0;
    public bool doneSpawning => spawnedCount >= spawnCount;
    #endregion

    #region Properties
    public static GameObject EnemyGoal => Instance.enemyGoal;
    #endregion

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        nextSpawnTime = Time.time + spawnInterval;

        // Find object called "Enemies" and spawn it if not found
        enemiesContainer = GameObject.Find(ENEMIES_CONTAINER_NAME);
        if (enemiesContainer == null)
        {
            enemiesContainer = new GameObject(ENEMIES_CONTAINER_NAME);
        }

    }

    // Update is called once per frame
    void Update()
    {
        SpawnEnemies();
    }

    private void SpawnEnemies()
    {
        float t = Time.time;
        if (nextSpawnTime > t) return;
        if (doneSpawning) return;

        spawnedCount++;
        nextSpawnTime = Time.time + spawnInterval;

        int i = Random.Range(0, spawnLocations.Length);
        Transform spawnLocation = spawnLocations[i].transform;

        var newEnemy = Instantiate(enemyPrefab, spawnLocation.position, Quaternion.identity);
        newEnemy.transform.parent = enemiesContainer.transform;
    }

    private void OnDestroy()
    {
        Instance = null;
    }
}
