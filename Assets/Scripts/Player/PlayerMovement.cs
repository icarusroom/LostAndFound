using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.XR;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField]
    private GameObject _playerModel;
    private CharacterController _controller;
    private Vector3 _movementDir;
    private Vector3 _direction;
    private Vector3 _jumpVector;

    private bool _isJumping = false;
    private bool _isGrounded = true;

    [SerializeField]
    private float _movementSpeed;
    private float _tempSpeed;
    [SerializeField]
    private float _jumpForce;
    [SerializeField]
    private float _rotSpeed;
    [SerializeField]
    private float _turnSmoothTime = 0.1f;
    private float _turnSmoothVelocity;

    private int _jumpCount = 0;

    private Transform _mainCameraTransform;
    void Awake()
    {
        this._controller = GetComponent<CharacterController>();
        this._mainCameraTransform = Camera.main.transform;
    }

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        this.RaycastGroundCheck();
        this.HandleGravity();
        this.HandleMovement();
    }

    private void RaycastGroundCheck()
    {
        if (Physics.Raycast(transform.position, -transform.up, out RaycastHit hit, ((this._controller.height / 2) + 0.1f)))
        {
            this._isGrounded = true;
        }
        else
        {
            this._isGrounded = false;
        }
    }

    private void HandleGravity()
    {
        if (this._isGrounded)
        {
            this._jumpCount = 0;
        }

        if (!this._isJumping)
        {
            this._jumpVector.y += Physics.gravity.y * Time.deltaTime;
            this._controller.Move(this._jumpVector * Time.deltaTime);
        }
    }
    public void OnMove(InputAction.CallbackContext value)
    {
        if(this._isGrounded)
        {
            //this._movementDir = new Vector3(value.ReadValue<Vector2>().x, 0f, value.ReadValue<Vector2>().y).normalized;
            this._direction = new Vector3(value.ReadValue<Vector2>().x, 0f, value.ReadValue<Vector2>().y).normalized;
        }
    }

    private void HandleMovement()
    {
        if(this._direction.magnitude >= 0.1f)
        {
            //Quaternion targetRotation = Quaternion.LookRotation(this._movementDir);
            float targetAngle = Mathf.Atan2(this._direction.x, this._direction.z) * Mathf.Rad2Deg + this._mainCameraTransform.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(this.transform.eulerAngles.y, targetAngle, ref this._turnSmoothVelocity, this._turnSmoothTime);
            //this.transform.rotation = Quaternion.RotateTowards(this.transform.rotation, targetRotation, this._rotSpeed * Time.deltaTime);
            this.transform.rotation = Quaternion.Euler(0f, angle, 0f);
            this._movementDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;

            this._controller.Move(this._movementDir * this._movementSpeed * Time.deltaTime);
        }
    }

    public void OnJump(InputAction.CallbackContext value)
    {
        if (value.performed)
        {
            this.HandleJump();
        }
    }

    private void HandleJump()
    {
        if (this._jumpCount != 2)
        {
            this._jumpVector.y = Mathf.Sqrt(1f * -this._jumpForce * Physics.gravity.y);
            this._isGrounded = false;
            this._jumpCount++;
        }
    }

    public void OnDash(InputAction.CallbackContext value)
    {
        float dashSpeed = this._movementSpeed * 2f;
        this._tempSpeed = this._movementSpeed / 2f;

        if (value.performed)
        {
            this._movementSpeed = dashSpeed;
            Debug.Log("Dashing");
            Debug.Log(this._movementSpeed);
        }

        if (value.canceled)
        {
            this._movementSpeed = this._tempSpeed;
            Debug.Log("Default speed");
            Debug.Log(this._movementSpeed);
        }

    }
}
