using System.Collections;
using UnityEngine;

namespace Assets.Scripts.Managers
{
    [System.Serializable]
    public class Wave
    {
        public int count;
        public float rate;
        public Transform enemy;
    }

    public class Spawner : MonoBehaviour
    {
        public enum SpawnState
        {
            Spawning,
            Counting,
            Waiting
        };

        public Wave[] waves;

        public Transform[] spawnPoints;

        public float timeBetweenWaves = 5.0f;
        public float waveCountdown;

        private int _nextWave;
        private float _searchCountdown;

        public SpawnState spawnState = SpawnState.Counting;

        // Start is called before the first frame update
        private void Start()
        {
            if (spawnPoints == null)
            {
                Debug.LogError("No spawn points provide to the spawn-er script");
            }
            _nextWave = 0;
            _searchCountdown = 1.5f;
            waveCountdown = timeBetweenWaves;
        }

        // Update is called once per frame
        private void Update()
        {
            if (spawnState == SpawnState.Waiting)
            {
                if (!EnemyIsAlive())
                {
                    WaveCompleted();
                }
                else
                {
                    return;
                }
            }
            if (waveCountdown < 0)
            {
                if (spawnState != SpawnState.Spawning)
                {
                    StartCoroutine(SpawnWave(waves[_nextWave]));
                }
            }
            else
            {
                waveCountdown -= Time.deltaTime;
            }
        }

        private bool EnemyIsAlive()
        {
            _searchCountdown -= Time.deltaTime;

            if (!(_searchCountdown <= 0)) return true;
            return GameObject.FindGameObjectWithTag("Enemy") != null;
        }

        private void WaveCompleted()
        {
            spawnState = SpawnState.Counting;
            waveCountdown = timeBetweenWaves;

            _nextWave++;
            if (_nextWave >= waves.Length)
            {
                _nextWave = 0;
            }
        }

        private IEnumerator SpawnWave(Wave wave)
        {
            spawnState = SpawnState.Spawning;

            for (var i = 0; i < wave.count; i++)
            {
                SpawnEnemy(wave.enemy);
                yield return new WaitForSeconds(1f / wave.rate);
            }

            spawnState = SpawnState.Waiting;
        }

        private void SpawnEnemy(Transform enemy)
        {
            Debug.Log("Spawning enemy");
            var randomSpawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
            Instantiate(enemy, randomSpawnPoint.position, randomSpawnPoint.rotation);
        }
    }
}