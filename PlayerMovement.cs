using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float _normalSpeed;
    [SerializeField] private float _sprintingSpeed;
    [SerializeField] private float _perspectiveSize;
    private bool _isSprinting;
    private float _currentSpeed;
    private Rigidbody _rigidbody;
    [Header("Jumping Settings")]
    [SerializeField] private float _jumpHeight;
    [SerializeField] private Transform _legs;
    [SerializeField] private bool _isGrounded;
    [Header("Rotating Settings")]
    [SerializeField] private float _sensitivityX;
    [SerializeField] private float _sensitivityY;
    [SerializeField] private float _headBobMultiplier;
    [SerializeField] private float _headBobSpeed;
    [SerializeField] private float _jumpHeadImpact;
    [SerializeField] private Transform _camera;
    float rotationX = 0;

    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _currentSpeed = _normalSpeed;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {
        _isGrounded = Physics.CheckSphere(_legs.position, 0.1f, 1);
        Debug.DrawRay(_legs.position, Vector3.down, Color.red, 0.3f);
        Move();
        if (_isGrounded && Input.GetButtonDown("Jump"))
            Jump();
        ControlDrag();
        ControlCameraPerspective();
        Rotate();
        ControlSpeed();
        if (!_isGrounded) 
         Camera.main.transform.localEulerAngles = new Vector3(Mathf.Lerp(Camera.main.transform.localEulerAngles.x, Mathf.Clamp(_rigidbody.velocity.y * _jumpHeadImpact , 0, 7), .05f), Camera.main.transform.localEulerAngles.y, Camera.main.transform.localEulerAngles.z);
        else
            Camera.main.transform.localEulerAngles = new Vector3(Mathf.Lerp(Camera.main.transform.localEulerAngles.x, 0, .05f), Camera.main.transform.localEulerAngles.y, Camera.main.transform.localEulerAngles.z);
    }

    private void ControlSpeed()
    {
        if (!_isGrounded)
            _currentSpeed = _currentSpeed / 10;
        else if (_isGrounded && !Input.GetKey(KeyCode.LeftShift))
        {
            _currentSpeed = _normalSpeed;
            _isSprinting = false;
        }
        else if (_isGrounded && Input.GetKey(KeyCode.LeftShift))
        {
            _currentSpeed = _sprintingSpeed;
            _isSprinting = true;
        }
    }

    private void Move()
    {
        Vector2 movementVector = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")) * _currentSpeed;
        if (movementVector.magnitude > 0)
            Camera.main.transform.localEulerAngles = new Vector3(Camera.main.transform.localEulerAngles.x, Camera.main.transform.localEulerAngles.y, Camera.main.transform.localEulerAngles.z + Mathf.Sin(Time.time * _headBobSpeed * _currentSpeed / 5) * _headBobMultiplier);
        else
            Camera.main.transform.localEulerAngles = new Vector3(Camera.main.transform.localEulerAngles.x, Camera.main.transform.localEulerAngles.y, 0);
        _rigidbody.AddForce(transform.right * movementVector.x + transform.forward * movementVector.y);
    }

    private void Rotate()
    {
        rotationX -= Input.GetAxis("Mouse Y") * _sensitivityY;
        rotationX = Mathf.Clamp(rotationX, -90, 85);
        float rotationY = Input.GetAxis("Mouse X") * _sensitivityX;
        _camera.transform.localEulerAngles = new Vector3(rotationX, 0, 0);
        transform.Rotate(0, rotationY, 0);
    }

    private void Jump()
    {
        _rigidbody.AddForce(transform.up * _jumpHeight, ForceMode.Impulse);
    }

    private void ControlDrag()
    {
        if (_isGrounded)
            _rigidbody.drag = 6;
        if (!_isGrounded)
            _rigidbody.drag = 0;
    }

    private void ControlCameraPerspective()
    {
        float currentPerspectiveSize;
        if (_isSprinting)
            currentPerspectiveSize = _perspectiveSize + (_sprintingSpeed - _normalSpeed) * 2;
        else
            currentPerspectiveSize = _perspectiveSize;
        Camera.main.fieldOfView = Mathf.Lerp(Camera.main.fieldOfView, currentPerspectiveSize, 0.15f);
    }
}
