using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


public class Player : MonoBehaviour
{
    private PlayerInput _playerInput;
    private CharacterController _characterController;
    private bool _isRunPressed;

    [SerializeField] [Range (0f, 1f)] private float _rotationSpeed = 0.5f;
    [SerializeField] [Range(0f, 100f)] private float _runSpeed;
    [SerializeField] [Range(0f, 100f)] private float _speed;
    [SerializeField] [Range(-10f, 0f)] private float _gravity = -9.8f;
    [SerializeField] [Range(-0.05f, 0)] private float _groundedGravity = -0.05f;
    [SerializeField] [Range(0f, 100f)] private float _massCharacter = 5.0f;

    private Transform _cameraTransform;

    private Vector2 _input;
    private Vector3 _currentMovement;
    private Vector3 _currentRunMovement;
    private Vector3 _playerVelocity;

    private bool _isMovementPressed;
    private bool _isJumpPressed = false;
    private bool _isJumping = false;
    private float _initialJumpVelocity;
    private float _maxJumpHeight = 0.5f;
    private float _maxJumpTime = 0.75f;

    private void Awake()
    {
        _playerInput = new PlayerInput();
        _characterController = GetComponent<CharacterController>();

        _cameraTransform = Camera.main.transform;


        _playerInput.PlayerController.Move.started += OnMovementInput;
        _playerInput.PlayerController.Move.canceled += OnMovementInput;
        _playerInput.PlayerController.Move.performed += OnMovementInput;
        _playerInput.PlayerController.Run.started += OnRun;
        _playerInput.PlayerController.Run.canceled += OnRun;
        _playerInput.PlayerController.Jump.started += OnJump;
        _playerInput.PlayerController.Jump.canceled += OnJump;

        SetUpJumpVariables();

    }
 
    private  void SetUpJumpVariables()
    {
        float timeToApex = _maxJumpTime / 2;
        _gravity = (-2 * _maxJumpHeight) / (timeToApex * timeToApex);
        _initialJumpVelocity = (2 * _maxJumpHeight) / timeToApex;
    }


    private void Start()
    {
        
    }

    private void Update()
    {
        HandleRotation();
        HandleGravity();
        HandleJump();
        HandleMovement();      
    }

    private void HandleMovement()
    {
        _currentMovement = new Vector3(_input.x, 0, _input.y);

        _currentMovement = _currentMovement.x * _cameraTransform.right.normalized + _currentMovement.z * _cameraTransform.forward.normalized;
        _currentMovement.y = 0f;

        if (_isRunPressed)
        {
            _characterController.Move(_currentMovement * _runSpeed * Time.deltaTime);
        }
        else
        {
            _characterController.Move(_currentMovement * _speed * Time.deltaTime);
        }
    }
   private void HandleRotation()
   {
        Quaternion targetRotation = Quaternion.Euler(0, _cameraTransform.eulerAngles.y, 0);


        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, _rotationSpeed * Time.deltaTime);
   }
    private void HandleJump()
    {
        if (!_isJumping && _characterController.isGrounded && _isJumpPressed)
        {
            _isJumping = true;

            _currentMovement.y = _initialJumpVelocity * 0.5f;
            _currentRunMovement.y = _initialJumpVelocity * 0.5f * _speed;
        }
        else if (!_isJumpPressed && _isJumping && _characterController.isGrounded)
        {
            _isJumping = false;
        }
    }
    private void HandleGravity()
    {
        bool isFalling = _currentMovement.y <= 0.0f || !_isJumpPressed;
        float fallMultiplier = 2.0f;
        if (_characterController.isGrounded)
        {
            _currentMovement.y = _groundedGravity;
            _currentRunMovement.y = _groundedGravity;
        }
        else if (isFalling)
        {
            float previousYVelocity = _currentMovement.y;
            float newYVelocity = _currentMovement.y + (_gravity * fallMultiplier * Time.deltaTime);
            float nextYVelocity = (previousYVelocity + newYVelocity) * 0.5f;
            _currentMovement.y = nextYVelocity;
            _currentRunMovement.y = nextYVelocity;
        }
        else
        {
            float previousYVelocity = _currentMovement.y;
            float newYVelocity = _currentMovement.y + (_gravity * Time.deltaTime);
            float nextYVelocity = (previousYVelocity + newYVelocity) * 0.5f;
            _currentMovement.y = nextYVelocity;
            _currentRunMovement.y = nextYVelocity;
        }
    }
    private void OnJump(InputAction.CallbackContext obj)
    {
        _isJumpPressed = obj.ReadValueAsButton();
    }
    private void OnRun(InputAction.CallbackContext obj)
    {
        _isRunPressed = obj.ReadValueAsButton();
    }

    private void OnMovementInput(InputAction.CallbackContext obj)
    {
        _input = obj.ReadValue<Vector2>();

       _isMovementPressed = _input.x != 0 || _input.y != 0;

    }

    private void OnEnable()
    {
        _playerInput.PlayerController.Enable();
    }

    private void OnDisable()
    {
        _playerInput.PlayerController.Disable();
    }
}
