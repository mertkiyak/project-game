using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform _orientationTransform;
    
    [Header("Setting")]
    [SerializeField] private float _movementSpeed;

    [Header("Jump Setting")]
    [SerializeField] private KeyCode _jumpkey;
    [SerializeField] private float _jumpForce;
    [SerializeField] private bool _canJump;
    [SerializeField] private float _jumpCooldown;

    [Header("Geround Check")]
    [SerializeField]private float _playerHeight;
    [SerializeField] private LayerMask _groundLayer;


    private Rigidbody _playerRigibody;
    
    private float _horizontalInput , _verticalInput;

    private Vector3 _movementDirection;
    

    private void Awake()
    {
        _playerRigibody = GetComponent<Rigidbody>();
    }
    private void Update()
    {
        SetInputs();
    }
    private void FixedUpdate()
    {
        SetPlayerMovement();
    }
    private void SetInputs()
    {
        _horizontalInput = Input.GetAxis("Horizontal");
        _verticalInput = Input.GetAxis("Vertical");

        if (Input.GetKey(_jumpkey)&& _canJump && IsGrounded())
        {
            _canJump = false;
            SetPlayerJumping();
            Invoke(nameof(ResetJumping),_jumpCooldown);
        }
    }

    private void SetPlayerMovement()
    {
        _movementDirection = _orientationTransform.forward * _verticalInput  + _orientationTransform.right * _horizontalInput;
        _playerRigibody.AddForce(_movementDirection.normalized * _movementSpeed, ForceMode.Force);
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

}
