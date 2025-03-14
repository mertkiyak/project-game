using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform _orientationTransform;
    
    [Header("Setting")]
    [SerializeField] private float _movementSpeed;
    [SerializeField] private KeyCode _movementKey;

    [Header("Jump Setting")]
    [SerializeField] private KeyCode _jumpkey;
    [SerializeField] private float _jumpForce;
    [SerializeField] private bool _canJump;
    [SerializeField] private float _jumpCooldown;

    [Header("Sliding Setting")]
    [SerializeField] private KeyCode _slideKey;
    [SerializeField] private float _slideMultiplier;

    [Header("Geround Check")]
    [SerializeField]private float _playerHeight;
    [SerializeField] private LayerMask _groundLayer;


    private StateContoller _stateContoller;

    private Rigidbody _playerRigibody;
    
    private float _horizontalInput , _verticalInput;

    private Vector3 _movementDirection;

    private bool _isSliding;
    

    private void Awake()
    {
        _stateContoller = GetComponent<StateContoller>(); 
        _playerRigibody = GetComponent<Rigidbody>();
    }
    private void Update()
    {
        SetInputs();
        SetStates();
    }
    private void FixedUpdate()
    {
        SetPlayerMovement();
    }
    private void SetInputs()
    {
        _horizontalInput = Input.GetAxis("Horizontal");
        _verticalInput = Input.GetAxis("Vertical");

        if (Input.GetKeyDown(_slideKey))
        {
            _isSliding = true;
        }
        else if (Input.GetKeyDown(_movementKey)) 
        { 
            _isSliding= false;
            _isSliding= false;
        
        }

        if (Input.GetKey(_jumpkey)&& _canJump && IsGrounded())
        {
            _canJump = false;
            SetPlayerJumping();
            Invoke(nameof(ResetJumping),_jumpCooldown);
        }
    }


    private void SetStates()
    {
        var movementDireciton = GetMovementDirection();
        var isGrounded = IsGrounded();
        var currentState = _stateContoller.GetCurrentState();
        var isSliding = IsSliding();

        var newState = currentState switch
        {
            _ when movementDireciton == Vector3.zero && isGrounded && !isSliding => PlayerState.Idle,
            _ when movementDireciton != Vector3.zero && isGrounded && !isSliding => PlayerState.Move,
            _ when movementDireciton != Vector3.zero && isGrounded && isSliding => PlayerState.Slide,
            _ when movementDireciton == Vector3.zero && isGrounded && isSliding => PlayerState.SlideIdle,
            _ when _canJump && !isGrounded => PlayerState.Jump,
            _ => currentState
        };

        if (newState != currentState)
        {
            _stateContoller.ChangeState(newState);
        }
       
    }

    private void SetPlayerMovement()
    {
        _movementDirection = _orientationTransform.forward * _verticalInput  + _orientationTransform.right * _horizontalInput;


        if (_isSliding)
        {
            _playerRigibody.AddForce(_movementDirection.normalized * _movementSpeed * _slideMultiplier, ForceMode.Force);
        }
        else 
        {
            _playerRigibody.AddForce(_movementDirection.normalized * _movementSpeed, ForceMode.Force);
        }
    }

  
    private void SetPlayerJumping()
    {
        //_playerRigibody.linearVelocity = new Vector3(_playerRigibody.linearVelocity.x, 0f, _playerRigibody.linearVelocity.z);
        _playerRigibody.AddForce(transform.up * _jumpForce, ForceMode.Impulse);

    }
    private void ResetJumping()
    {
        _canJump = true;
    }
    private bool IsGrounded()
    {
        return Physics.Raycast(transform.position, Vector3.down, _playerHeight * 0.5f + 0.2f, _groundLayer);
    }

    private Vector3 GetMovementDirection()
    {
        return _movementDirection.normalized;
    }


    private bool IsSliding()
    {
        return _isSliding;
    }

}
