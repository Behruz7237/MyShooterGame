using System;
using UnityEngine;
using UnityEngine.AI;

namespace Assets.Scripts.EnemyActions
{
    public class EnemyMovement : MonoBehaviour
    {
        [SerializeField] private NavMeshAgent _agent;
        [SerializeField] private Animator _animator;
        [SerializeField] private float _distanceToChase = 10;
        [SerializeField] private float _distanceToLose = 20;
        [SerializeField] private float _distanceToStop = 2f;
        [SerializeField] private float _keepChasingTime = 5f;
        [SerializeField] private GameObject _bullet;
        [SerializeField] private Transform _firePoint;
        [SerializeField] private float _fireRate = 0.3f;
        [SerializeField] private float _waitTimeBetweenShots = 2f;
        [SerializeField] private float _timeToShoot = 1f;

        private float _chaseCounter, _fireCount, _shotWaitCounter, _shootTimeCounter;
        private bool _chasing;
        private Transform _player;
        private Vector3 _targetPoint, _startPoint;
        private float speed;
        private Transform target;
        private int damage;

        public void Init(float speed, Transform target, int damage)
        {
            this.speed = speed;
            this.target = target;
            this.damage = damage;
        }

        internal void Init(float speed, int distance, int materialID)
        {
            throw new NotImplementedException();
        }

        private void Start()
        {
            _startPoint = transform.position;
            _shootTimeCounter = _timeToShoot;
            _shotWaitCounter = _waitTimeBetweenShots;
            _player = PlayerController.instance.transform;
        }

        private void Update()
        {
            _targetPoint = _player.position;
            _targetPoint.y = transform.position.y;

            if (!_chasing)
            {
                if (Vector3.Distance(transform.position, _targetPoint) < _distanceToChase)
                {
                    _chasing = true;

                    _shootTimeCounter = _timeToShoot;
                    _shotWaitCounter = _waitTimeBetweenShots;
                }

                if (_chaseCounter > 0)
                {
                    _chaseCounter -= Time.deltaTime;

                    if (_chaseCounter <= 0)
                    {
                        _agent.destination = _startPoint;
                    }
                }

                if (_agent.remainingDistance < 0.25f)
                {
                    _animator.SetBool("isMoving", false);
                }
                else
                {
                    _animator.SetBool("isMoving", true);
                }
            }
            else
            {
                //transform.LookAt(_targetPoint);
                //_rigidBody.velocity = transform.forward * _speed;

                if (Vector3.Distance(transform.position, _targetPoint) > _distanceToStop)
                {
                    _agent.destination = _targetPoint;
                }
                else
                {
                    _agent.destination = transform.position;
                }

                if (Vector3.Distance(transform.position, _targetPoint) > _distanceToLose)
                {
                    _chasing = false;

                    _chaseCounter = _keepChasingTime;
                }

                if (_shotWaitCounter > 0)
                {
                    _shotWaitCounter -= Time.deltaTime;

                    if (_shotWaitCounter <= 0)
                    {
                        _shootTimeCounter = _timeToShoot;
                    }

                    _animator.SetBool("isMoving", true);
                }
                else
                {
                    if (_player.gameObject.activeInHierarchy)
                    {
                        _shootTimeCounter -= Time.deltaTime;

                        if (_shootTimeCounter > 0)
                        {
                            _fireCount -= Time.deltaTime;

                            if (_fireCount <= 0)
                            {
                                _fireCount = _fireRate;

                                _firePoint.LookAt(_player.position + new Vector3(0f, 1.5f, 0f));

                                //check the angle to the player
                                Vector3 targetDir = _player.position - transform.position;
                                float angle = Vector3.SignedAngle(targetDir, transform.forward, Vector3.up);

                                if (Mathf.Abs(angle) < 30f)
                                {
                                    Instantiate(_bullet, _firePoint.position, _firePoint.rotation);
                                    _animator.SetTrigger("fireShot");
                                }
                                else
                                {
                                    _shotWaitCounter = _waitTimeBetweenShots;
                                }
                            }

                            _agent.destination = transform.position;
                        }
                        else
                        {
                            _shotWaitCounter = _waitTimeBetweenShots;
                        }

                        _animator.SetBool("isMoving", false);
                    }
                    else
                    {

                    }
                }
            }



        }
    }
}