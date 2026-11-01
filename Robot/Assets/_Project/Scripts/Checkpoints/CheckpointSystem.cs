using System;
using System.Collections.Generic;
using Robot;
using UnityEngine;

public class CheckpointSystem : MonoBehaviour
{
    public static CheckpointSystem Instance;
    
    private Checkpoint currentCheckpoint;
    private Vector3 defaultRespawnPosition;
    private List<EnemyState> enemyStates = new List<EnemyState>();

    public event Action OnPlayerSpawn;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        defaultRespawnPosition = Vector3.zero;
    }

    public void SetCurrentCheckpoint(Checkpoint checkpoint)
    {
        currentCheckpoint = checkpoint;

        SaveEnemyStates();
        Debug.Log("Checkpoint guardado: " + checkpoint.roomName);
    }

    public void RespawnPlayer()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if(player == null) return;

        Vector3 respawnPosition = defaultRespawnPosition;
        if (currentCheckpoint != null)
        {
            respawnPosition = currentCheckpoint.transform.position;
        }
        
        player.transform.position = respawnPosition;

        OnPlayerSpawn?.Invoke();

        RespawnEnemies();
    }

    void SaveEnemyStates()
    {
        enemyStates.Clear();
        
        EnemigoMelee[] enemigoMelees = FindObjectsOfType<EnemigoMelee>();
        RangeEnemy[] rangeEnemies = FindObjectsOfType<RangeEnemy>();
        FlyingEnemy[] flyingEnemies = FindObjectsOfType<FlyingEnemy>();
        HeavyEnemy[] heavyEnemies = FindObjectsOfType<HeavyEnemy>();
        
        foreach (EnemigoMelee enemy in enemigoMelees)
        {
            enemyStates.Add(new EnemyState
            {
                enemyType = EnemyType.Melee,
                position = enemy.transform.position,
                rotation = enemy.transform.rotation,
                enemyReference = enemy
            });
        }
        foreach (RangeEnemy enemy in rangeEnemies)
        {
            enemyStates.Add(new EnemyState
            {
                enemyType = EnemyType.Ranged,
                position = enemy.transform.position,
                rotation = enemy.transform.rotation,
                enemyReference = enemy
            });
        }
        
        foreach (FlyingEnemy enemy in flyingEnemies)
        {
            enemyStates.Add(new EnemyState
            {
                enemyType = EnemyType.Flying,
                position = enemy.transform.position,
                rotation = enemy.transform.rotation,
                enemyReference = enemy
            });
        }
        
        foreach (HeavyEnemy enemy in heavyEnemies)
        {
            enemyStates.Add(new EnemyState
            {
                enemyType = EnemyType.Heavy,
                position = enemy.transform.position,
                rotation = enemy.transform.rotation,
                enemyReference = enemy
            });
        }
    }

    void RespawnEnemies()
    {

        foreach (EnemyState state in enemyStates)
        {
            if (state.enemyReference != null)
            {
                ResetEnemy(state.enemyReference, state.position, state.rotation);
            }
        }
    }

    void ResetEnemy(MonoBehaviour enemy, Vector3 position, Quaternion rotation)
    {
        if(enemy == null) return;
        
        enemy.transform.position = position;
        enemy.transform.rotation = rotation;
        
        switch (enemy)
        {
            case EnemigoMelee melee:
                melee.TakeDamage(-1000, false); // "Curar" al enemigo
                break;
            case RangeEnemy ranged:
                // Resetear estado de ranged enemy
                break;
            case FlyingEnemy flying:
                // Resetear estado de flying enemy
                break;
            case HeavyEnemy heavy:
                // Resetear estado de heavy enemy
                break;
        }
    }

    public void OnPlayerDeath()
    {
        RespawnPlayer();
    }
}

[System.Serializable]
public class EnemyState
{
    public EnemyType enemyType;
    public Vector3 position;
    public Quaternion rotation;
    public MonoBehaviour enemyReference;
}

public enum EnemyType
{
    Melee,
    Ranged,
    Flying,
    Heavy
}
