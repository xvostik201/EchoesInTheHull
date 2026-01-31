using System;
using System.Collections;
using System.Collections.Generic;
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
    
    private NavMeshAgent _agent;
    private EnemyStates _currentState = EnemyStates.Patrol;

    private void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
    }

    void Start()
    {
        _agent.SetDestination(_enemyPaths[_currentPathIndex].points[_currentPointIndex].position);
    }

    void Update()
    {
        switch (_currentState)
        {
            case EnemyStates.Patrol:
                PathMovement();
                break;
            case EnemyStates.Chase:
                Chase();
                break;
            case EnemyStates.Check:
                Checking();
                break;
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
}
