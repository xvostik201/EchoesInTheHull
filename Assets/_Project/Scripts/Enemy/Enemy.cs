using System;
using System.Collections;
using System.Collections.Generic;
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
    [SerializeField] private AudioSource[] _allKnowSounds;

    [SerializeField] private float _newSoundWaitTime = 5f;
    [SerializeField] private int _maxListeningSounds = 2;
    
    private int _currentListeningSound;
    private int _lastListeningSound;
    private Vector3 _lastSoundPosition;
    private bool _isListening = false;
    
    [Header("Chase")]
    
    [SerializeField] private float _chaseDistance = 5f;
    [SerializeField] private float _chaseAcceleration = 10f;
    [SerializeField] private float _defaultAcceleration = 8f;
    private Vector3 _lastChasePosition;
    private Transform _chaseTarget;

    private bool _isWaiting;

    [Header("Body")] 
    [SerializeField] private Transform _head;
    
    [Header("FOV")]
    [SerializeField, Range(0, 180)] private float _fovAngle;
    [SerializeField] private float _fovDistance = 15f;
    
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
        _agent.SetDestination(_enemyPaths[_currentPathIndex].points[_currentPointIndex].position);
    }

    void Update()
    {
        bool canSee = IsPlayerVisible();

        if (canSee && _currentState != EnemyStates.Chase)
        {
            StopAllCoroutines();
            ChangeState(EnemyStates.Chase);
        }
        else if (!canSee && _currentState == EnemyStates.Chase)
        {
            _chaseTarget = null; 
        }

        switch (_currentState)
        {
            case EnemyStates.Patrol: PathMovement(); break;
            case EnemyStates.Chase: Chase(); break;
            case EnemyStates.Check: Checking(); break;
        }

        HandleHearingLogic();
    }

    private void Checking()
    {
        _agent.SetDestination(_lastSoundPosition);

        if (Vector3.Distance(_lastSoundPosition, transform.position) < 0.5f && !_isWaiting)
        {
            _agent.isStopped = true;
            _isWaiting = true;
            StartCoroutine(WaitAndLookAround());
        }
    }

    private void Listen()
    {
        _isListening = true;
        _lastListeningSound = _currentListeningSound;
        StartCoroutine(WaitAndListenAround());
    }

    private void HandleHearingLogic()
    {
        if (_currentListeningSound > 0 && !_isListening)
        {
            Listen();
        }
    }

    private void Chase()
    {
        if (_chaseTarget != null)
        {
            _agent.SetDestination(_chaseTarget.position);
            _lastChasePosition = _chaseTarget.position;
            
            if (Vector3.Distance(transform.position,
                    _chaseTarget.position) <= 1f)
            {
                Debug.Log("PLAYER CAUGHT");                
            }
        }
        else
        {
            _agent.SetDestination(_lastChasePosition);

            if (Vector3.Distance(transform.position,
                    _lastChasePosition) <= 1f)
            {
                ChangeState(EnemyStates.Check);
            }
        }
    }

    private void PathMovement()
    {
        if (Vector3.Distance(_enemyPaths[_currentPathIndex].points[_currentPointIndex].position,
            transform.position) < 0.5f)
        {
            _currentPointIndex++;

            if (_currentPointIndex >= _enemyPaths[_currentPathIndex].points.Length)
            {
                _currentPointIndex = 0; 
                _currentPathIndex++;    
            
                if (_currentPathIndex >= _enemyPaths.Length)
                {
                    _currentPathIndex = 0;
                }
            }

            _agent.SetDestination(_enemyPaths[_currentPathIndex].points[_currentPointIndex].position);
        }
    }

    private IEnumerator WaitAndLookAround()
    {
        yield return new WaitForSeconds(3f);

        _isWaiting = false;
        ChangeState(EnemyStates.Patrol);
    }
    private IEnumerator WaitAndListenAround()
    {
        yield return new WaitForSeconds(_newSoundWaitTime);
        if (_currentListeningSound == _lastListeningSound)
        {
            ChangeState(EnemyStates.Patrol);
        }
        else
        {
            ChangeState(EnemyStates.Check);
        }
        
        _currentListeningSound = 0;
        _isListening = false;
    }

    private bool IsPlayerVisible()
    {
        RaycastHit hit;
        PlayerController player = _cachedPlayer;
        
        Vector3 directionToPlayer = (player.transform.position - _head.position).normalized;

        float angle = Vector3.Angle(_head.forward, directionToPlayer);

        if (angle < _fovAngle * 0.5f)
        {
            if (Physics.Raycast(_head.position, directionToPlayer, out hit, _fovDistance))
            {
                if (hit.collider.TryGetComponent<PlayerController>(out PlayerController p))
                {
                    _chaseTarget = p.transform;
                    return true;
                }
            }
        }
        return false;
    }
    
    public void GetNewSound(Vector3 soundPosition)
    {
        _currentListeningSound++;
        _lastSoundPosition = soundPosition;
    }

    private void ChangeState(EnemyStates newState)
    {
        _currentState = newState;
        switch (_currentState)
        {
            case EnemyStates.Patrol:
                break;
            case EnemyStates.Listen:
                break;
            case EnemyStates.Chase:
                break;
            case EnemyStates.Check:
                break;
        }

        UpdateAgentSettings();
    }

    private void UpdateAgentSettings()
    {
        _agent.isStopped = (_currentState == EnemyStates.Listen);
        _agent.acceleration = _currentState == EnemyStates.Chase ?  _chaseAcceleration : _defaultAcceleration;
    }

    private void OnDrawGizmos()
    {
        if (_head != null)
        {
            Gizmos.color = Color.yellow;
            
            Vector3 forward =  _head.forward * _fovDistance;
            
            Quaternion leftRayRotation = Quaternion.AngleAxis(-_fovAngle * 0.5f, Vector3.up);
            Quaternion rightRayRotation = Quaternion.AngleAxis(_fovAngle * 0.5f, Vector3.up);
            
            Vector3 leftRay = leftRayRotation * forward;
            Vector3 rightRay = rightRayRotation * forward;

            Vector3 leftDirection = _head.position + leftRay;
            Vector3 rightDirection = _head.position + rightRay;
            Gizmos.DrawLine(leftDirection, rightDirection);
            
            Gizmos.DrawRay(_head.position, leftRay);
            Gizmos.DrawRay(_head.position, rightRay);
            
            Gizmos.color = Color.blue;
            Gizmos.DrawRay(_head.position, forward);
        }
    }
}
