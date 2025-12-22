using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    public Transform cameraPivot;
    public Camera playerCamera;
    public Animator PlayerAnim;
    public float walkSpeed = 6f;
    public float runSpeed = 12f;
    public float jumpPower = 7f;
    public float gravity = 20f;

    public float mouseSensitivity = 0.15f;
    public float minY = -35f;
    public float maxY = 60f;

    public float defaultHeight = 2f;
    public float crouchHeight = 1f;
    public float crouchSpeed = 3f;

    private CharacterController controller;
    private Vector3 velocity;
    private float cameraPitch;

    void Awake()
    {
        controller = GetComponent<CharacterController>();

        if (PlayerAnim == null)
        {
            PlayerAnim = GetComponentInChildren<Animator>();
        }
    }

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        HandleCamera();
        HandleMovement();
    }

    void HandleMovement()
    {
        float inputX = 0f;
        float inputZ = 0f;

        if (Keyboard.current.aKey.isPressed) inputX = -1;
        if (Keyboard.current.dKey.isPressed) inputX = 1;
        if (Keyboard.current.wKey.isPressed) inputZ = 1;
        if (Keyboard.current.sKey.isPressed) inputZ = -1;

        Vector3 inputDir = new Vector3(inputX, 0, inputZ).normalized;
        bool hasMovement = inputDir.magnitude > 0.1f;

        bool isRunning = Keyboard.current.leftShiftKey.isPressed;
        bool isCrouching = Keyboard.current.rKey.isPressed;

        if (isCrouching)
        {
            PlayerAnim.SetBool("AnimCrouch", true);
            PlayerAnim.SetBool("AnimWalking", false);
            PlayerAnim.SetBool("AnimRunning", false);
        }
        else if (hasMovement && isRunning)
        {
            
            PlayerAnim.SetBool("AnimCrouch", false);
            PlayerAnim.SetBool("AnimWalking", false);
            PlayerAnim.SetBool("AnimRunning", true);
        }
        else if (hasMovement)
        {
            PlayerAnim.SetBool("AnimCrouch", false);
            PlayerAnim.SetBool("AnimWalking", true);
            PlayerAnim.SetBool("AnimRunning", false);
        }
        else
        {
            PlayerAnim.SetBool("AnimCrouch", false);
            PlayerAnim.SetBool("AnimWalking", false);
            PlayerAnim.SetBool("AnimRunning", false);
        }

        float speed = walkSpeed;
        if (isCrouching) speed = crouchSpeed;
        else if (isRunning && hasMovement) speed = runSpeed;

        if (controller.isGrounded)
        {
            if (velocity.y < 0) velocity.y = -2f;
            if (Keyboard.current.spaceKey.wasPressedThisFrame) velocity.y = jumpPower;
        }

        velocity.y -= gravity * Time.deltaTime;

        Vector3 camForward = cameraPivot.forward;
        Vector3 camRight = cameraPivot.right;
        camForward.y = 0;
        camRight.y = 0;

        Vector3 move = (camForward * inputDir.z + camRight * inputDir.x).normalized;
        controller.Move((move * speed + velocity) * Time.deltaTime);

        if (move.magnitude > 0.1f)
        {
            Quaternion targetRot = Quaternion.LookRotation(move);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, Time.deltaTime * 10f);
        }

        controller.height = isCrouching ? crouchHeight : defaultHeight;
    }

    void HandleCamera()
    {
        Vector2 mouse = Mouse.current.delta.ReadValue() * mouseSensitivity;
        cameraPitch -= mouse.y;
        cameraPitch = Mathf.Clamp(cameraPitch, minY, maxY);
        cameraPivot.localRotation = Quaternion.Euler(cameraPitch, 0, 0);
        transform.Rotate(Vector3.up * mouse.x);
    }
}