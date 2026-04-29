using System.Collections;
using UnityEngine;

namespace Assets.Scripts.EnemyActions
{
    public class EnemySpawner : MonoBehaviour
    {
        [SerializeField] private Transform _instantiatePosition;
        [SerializeField] private EnemyMovement _enemyPrefab;

        private WaitForSeconds _waiter;

        private void Start()
        {
            _waiter = new WaitForSeconds(3f);
            StartCoroutine(SpawnRoutine());
        }

        private IEnumerator SpawnRoutine()
        {
            while (true)
            {
                SpawnEnemy();
                yield return _waiter;
            }
        }

        private void SpawnEnemy()
        {
            var enemy = Instantiate(_enemyPrefab, _instantiatePosition.position, Quaternion.identity);
            // Generate random values
            float speed = Random.Range(5f, 10f);          // float → 5.0 to 12.0
            int distance = Random.Range(5, 50);           // int → max is exclusive, so 41
            int materialID = Random.Range(0, 4);          // 1 to 4

            // Initialize enemy
            enemy.Init(speed, distance, materialID);

        }


    }
}