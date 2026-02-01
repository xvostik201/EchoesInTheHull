using System.Collections;
using Echoes.Player;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    [Header("Patrol && Path")]
    [SerializeField] private EnemyPath[] _enemyPaths;
    private int _currentPathIndex;
    private int _currentPointIndex;

    [Header("Listen")]
    [SerializeField] private float _newSoundWaitTime = 5f;
    [SerializeField] private int _maxListeningSounds = 2;
    [SerializeField] private float _maxSoundDistance = 25f;
    
    private int _currentListeningSound;
    private Vector3 _lastSoundPosition;
    private bool _isListening = false;
    
    [Header("Chase")]
    [SerializeField] private float _chaseAcceleration = 10f;
    [SerializeField] private float _defaultAcceleration = 8f;
    private Vector3 _lastTargetPosition;
    private Transform _chaseTarget;

    private bool _isWaiting;

    [Header("Body")] 
    [SerializeField] private Transform _head;
    
    [Header("FOV")]
    [SerializeField, Range(0, 180)] private float _fovAngle;
    [SerializeField] private float _fovDistance = 15f;
    [SerializeField] private LayerMask _obstructionMask;
    
    private NavMeshAgent _agent;
    private PlayerController _cachedPlayer;
    private EnemyStates _currentState = EnemyStates.Patrol;

    private void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
        _cachedPlayer = FindObjectOfType<PlayerController>();
    }

    void Start()
    {
        if (_enemyPaths.Length > 0)
            _agent.SetDestination(_enemyPaths[_currentPathIndex].points[_currentPointIndex].position);
    }

    void Update()
    {
        bool canSee = IsPlayerVisible();

        if (canSee && _currentState != EnemyStates.Chase)
        {
            StopAllCoroutines();
            _isWaiting = false;
            _isListening = false;
            ChangeState(EnemyStates.Chase);
        }

        switch (_currentState)
        {
            case EnemyStates.Patrol: PathMovement(); break;
            case EnemyStates.Chase: Chase(canSee); break;
            case EnemyStates.Check: Checking(); break;
            case EnemyStates.Listen:  break;
        }

        HandleHearingLogic();
    }

    private void HandleHearingLogic()
    {
        if (_currentListeningSound > 0 && !_isListening && _currentState != EnemyStates.Chase)
        {
            ChangeState(EnemyStates.Listen);
            StartCoroutine(WaitAndListenAround());
        }
    }

    private void Chase(bool canSee)
    {
        if (canSee && _chaseTarget != null)
        {
            _lastTargetPosition = _chaseTarget.position; 
            _agent.SetDestination(_lastTargetPosition);
            
            if (Vector3.Distance(transform.position, _chaseTarget.position) <= 1.2f)
            {
                Debug.Log("PLAYER CAUGHT");
            }
        }
        else
        {
            _agent.SetDestination(_lastTargetPosition);

            if (!_agent.pathPending && _agent.remainingDistance <= _agent.stoppingDistance + 0.5f)
            {
                ChangeState(EnemyStates.Check);
            }
        }
    }

    private void Checking()
    {
        _agent.SetDestination(_lastTargetPosition);

        if (!_agent.pathPending && _agent.remainingDistance <= _agent.stoppingDistance + 0.5f && !_isWaiting)
        {
            StartCoroutine(WaitAndLookAround());
        }
    }

    private void PathMovement()
    {
        if (_enemyPaths.Length == 0) return;

        if (!_agent.pathPending && _agent.remainingDistance < 0.5f)
        {
            _currentPointIndex++;

            if (_currentPointIndex >= _enemyPaths[_currentPathIndex].points.Length)
            {
                _currentPointIndex = 0; 
                _currentPathIndex = (_currentPathIndex + 1) % _enemyPaths.Length;
            }

            _agent.SetDestination(_enemyPaths[_currentPathIndex].points[_currentPointIndex].position);
        }
    }

    private IEnumerator WaitAndLookAround()
    {
        _isWaiting = true;
        _agent.isStopped = true;
        
        yield return new WaitForSeconds(3f);

        _agent.isStopped = false;
        _isWaiting = false;
        ChangeState(EnemyStates.Patrol);
    }

    private IEnumerator WaitAndListenAround()
    {
        _isListening = true;
        _agent.isStopped = true;
        
        yield return new WaitForSeconds(_newSoundWaitTime);
        
        _agent.isStopped = false;
        _isListening = false;
        _currentListeningSound = 0;

        _lastTargetPosition = _lastSoundPosition;
        ChangeState(EnemyStates.Check);
    }

    private bool IsPlayerVisible()
    {
        if (_cachedPlayer == null) return false;

        Vector3 directionToPlayer = (_cachedPlayer.transform.position - _head.position).normalized;
        float angle = Vector3.Angle(_head.forward, directionToPlayer);

        if (angle < _fovAngle * 0.5f)
        {
            if (Physics.Raycast(_head.position, directionToPlayer, out RaycastHit hit, _fovDistance, _obstructionMask))
            {
                if (hit.collider.CompareTag("Player") 
                    || hit.collider.TryGetComponent<PlayerController>(out _))
                {
                    _chaseTarget = _cachedPlayer.transform;
                    return true;
                }
            }
        }
        return false;
    }
    
    public void GetNewSound(Vector3 soundPosition)
    {
        if(Vector3.Distance(soundPosition, transform.position) <= _maxSoundDistance)
            return;
        
        if (_currentState == EnemyStates.Chase)
            return;
        
        _currentListeningSound++;
        _lastSoundPosition = soundPosition;
    }

    private void ChangeState(EnemyStates newState)
    {
        if (_currentState == newState) return;

        _currentState = newState;
        
        _agent.isStopped = false;
        _agent.acceleration = (_currentState == EnemyStates.Chase) ? _chaseAcceleration : _defaultAcceleration;
        
        if (_currentState == EnemyStates.Patrol)
        {
            _agent.SetDestination(_enemyPaths[_currentPathIndex].points[_currentPointIndex].position);
        }
    }

    private void OnDrawGizmos()
    {
        if (_head != null)
        {
            Gizmos.color = _currentState == EnemyStates.Chase ? Color.red : Color.yellow;
            
            Vector3 forward = _head.forward * _fovDistance;
            Quaternion leftRayRotation = Quaternion.AngleAxis(-_fovAngle * 0.5f, Vector3.up);
            Quaternion rightRayRotation = Quaternion.AngleAxis(_fovAngle * 0.5f, Vector3.up);
            
            Gizmos.DrawRay(_head.position, leftRayRotation * forward);
            Gizmos.DrawRay(_head.position, rightRayRotation * forward);
            Gizmos.DrawLine(_head.position + (leftRayRotation * forward), _head.position + (rightRayRotation * forward));
            
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(_lastTargetPosition, 0.5f);
        }
    }
}