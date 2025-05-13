using System;
using System.Linq;
using System.Threading;
using _MyGame.Scripts.Core;
using _MyGame.Scripts.Systems.Factories;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Serialization;
using VContainer;

namespace _MyGame.Scripts.Gameplay.Player
{
    public class Player : MonoBehaviour, IDamageable
    {
        [SerializeField] private PlayerSettings playerSettings;

        private const float AttackOffset = 0.5f;

        private CancellationTokenSource _cts;
        private NewControls _newControls;
        private Rigidbody2D _rb;
        private float _playerHalfWidth;
        private float _playerHalfHeight;
        private float _leftBound;
        private float _rightBound;
        private float _bottomBound;
        private bool _isDead;

        public event Action LoseAction;
        
        [Inject] private Camera _cam;
        [Inject] private IObjectFactory<Bullet.Bullet> _objectFactory;

        public float Health { get; private set; }

        private void Awake()
        {
            _cts = new CancellationTokenSource();
            
            _rb = GetComponent<Rigidbody2D>();

            _newControls = new NewControls();
            _newControls.Enable();
            
            var component = GetComponent<Collider2D>();
            
            if (component == null) return;
            
            _playerHalfWidth = component.bounds.extents.x;
            _playerHalfHeight = component.bounds.extents.y;
        }
        
        private void Start()
        {
            InitializePlayer();
        }

        private void InitializePlayer()
        {
            Health = 100;
            _isDead = false;
            CalculateBounds();
            StartShooting().Forget();
        }

        private async UniTaskVoid StartShooting()
        {
            while (!_cts.IsCancellationRequested && !_isDead) 
            { 
                await AutoShoot();
                await UniTask.Delay((int)(playerSettings.attackSpeed * 1000), 
                        cancellationToken: _cts.Token); 
            }
        }

        private async UniTask AutoShoot()
        {
            if (_isDead) return;

            var nearestAsteroid = FindNearestAsteroid();
            Vector2 direction;

            if (nearestAsteroid != null)
            {
                direction = (nearestAsteroid.transform.position - transform.position).normalized;
            }
            else
            {
                direction = Vector2.up;
            }

            await _objectFactory.CreateObject(
                transform.position + (Vector3)(direction * AttackOffset), 
                direction, _cts.Token);
        }

        //todo: optimize if bottleneck
        private GameObject FindNearestAsteroid()
        {
            return FindObjectsOfType<_MyGame.Scripts.Gameplay.Asteroid.Asteroid>()
                .Where(a => a != null && a.gameObject.activeInHierarchy)
                .Select(a => a.gameObject)
                .Where(go => IsInAttackRectangle(go.transform.position))
                .OrderBy(go => Vector3.Distance(transform.position, go.transform.position))
                .FirstOrDefault();
        }

        private bool IsInAttackRectangle(Vector2 enemyPosition)
        {
            var toEnemy = enemyPosition - (Vector2)transform.position;
            return toEnemy.magnitude <= playerSettings.attackRange && 
                   Mathf.Abs(toEnemy.x) <= playerSettings.attackWidth / 2f;
        }

        private void CalculateBounds()
        {
            if (_cam == null)
            {
                _cam = Camera.main;
                if (_cam == null) return;
            }
        
            var screenAspect = (float)Screen.width / Screen.height;
            var camHeight = _cam.orthographicSize;
        
            _leftBound = -camHeight * screenAspect + _playerHalfWidth;
            _rightBound = camHeight * screenAspect - _playerHalfWidth;
            _bottomBound = -camHeight + _playerHalfHeight;
        }

        private Vector2 GetMovementVector()
        {
            return _newControls.Player.Move.ReadValue<Vector2>().normalized;
        }

        private void Update()
        {
            if (_isDead) return;
            
            var input = GetMovementVector();
            var newPosition = _rb.position + input * (playerSettings.moveSpeed * Time.deltaTime);

            newPosition.x = Mathf.Clamp(newPosition.x, _leftBound, _rightBound);
            newPosition.y = Mathf.Clamp(newPosition.y, _bottomBound, playerSettings.finishLine - _playerHalfHeight);

            _rb.MovePosition(newPosition);
        }
        
        public void TakeDamage(float damage)
        {
            Health -= damage;

            if (!(Health <= 0)) return;
            
            _isDead = true;
            LoseAction?.Invoke();
            _cts?.Cancel();
        }
        
        private void OnDestroy()
        {
            _cts?.Cancel();
            _cts?.Dispose();
            _newControls?.Dispose();
        }
        
        private void OnDrawGizmos()
        {
            if (Camera.main == null) return;
            
            var left = -Camera.main.orthographicSize * Camera.main.aspect;
            var right = Camera.main.orthographicSize * Camera.main.aspect;
            Gizmos.color = Color.green;
            Gizmos.DrawLine(new Vector3(left, playerSettings.finishLine), new Vector3(right, playerSettings.finishLine));
            
            Gizmos.color = new Color(1, 0, 0, 0.3f);
            Vector2 center = transform.position + Vector3.up * (playerSettings.attackRange / 2f + AttackOffset);
            Vector2 size = new Vector2(playerSettings.attackWidth, playerSettings.attackRange);
            Gizmos.DrawCube(center, size);
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(center, size);
        }
    }
}