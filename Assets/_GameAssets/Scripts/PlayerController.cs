using UnityEngine;

public class PlayerController : MonoBehaviour
{

    private Rigidbody playerRigidbody;
    private float horizontalInput, verticalInput;
    private Vector3 movementDirection;

    [Header("References")]
    [SerializeField] private Transform orientationTransform;

    [Header("Movement Settings")]
    [SerializeField] private KeyCode movementKey;
    [SerializeField] private float movementSpeed;

    [Header("Jump Settings")]
    [SerializeField] private KeyCode jumpKey;
    [SerializeField] private float jumpForce;
    [SerializeField] private float jumpCooldown;
    private bool canJump = true;

    [Header("Ground Check Settings")]
    [SerializeField] private float playerHeight;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float groundDrag;

    [Header("Slider Settings")]
    [SerializeField] private KeyCode slideKey;
    [SerializeField] private float slideMultiplier;
    [SerializeField] private float slideDrag;
    private bool isSliding;



    private void Awake()
    {
        playerRigidbody = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        playerRigidbody.freezeRotation = true;
    }


    private void Update()
    {
        SetInput();
        SetPlayerDrag();
        LimitPlayerSpeed();
    }

    private void FixedUpdate()
    {
        SetPlayerMovement();
        
    }



    private void SetInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        if (Input.GetKeyDown(slideKey))
        {
            isSliding = true;

        }
        else if (Input.GetKeyDown(movementKey))
        {
            isSliding = false;
        }

        else if (Input.GetKeyDown(jumpKey) && canJump && IsGrounded())
        {
            canJump = false;
            SetPlayerJump();
            Invoke("SetCanJump", jumpCooldown);
        }
    }

    private void SetPlayerMovement()
    {
        movementDirection = orientationTransform.forward * verticalInput + orientationTransform.right * horizontalInput;

        if (isSliding)
        {
            playerRigidbody.AddForce(movementDirection.normalized * movementSpeed * slideMultiplier, ForceMode.Force);
        }
        else
        {
            playerRigidbody.AddForce(movementDirection.normalized * movementSpeed , ForceMode.Force);
        }
    }

    private void SetPlayerDrag()
    {
        if (isSliding)
        {
            playerRigidbody.linearDamping = slideDrag;
        }
        else
        {
            playerRigidbody.linearDamping = groundDrag;

        }
    }

    private void LimitPlayerSpeed()
    {
        Vector3 flatVelocity = new Vector3(playerRigidbody.linearVelocity.x, 0f, playerRigidbody.linearVelocity.z);

        if(flatVelocity.magnitude > movementSpeed)
        {
            Vector3 limitedVelocity = flatVelocity.normalized * movementSpeed;
            playerRigidbody.linearVelocity = limitedVelocity;
        }
    }

    private void SetPlayerJump()
    {
        playerRigidbody.linearVelocity = new Vector3(playerRigidbody.linearVelocity.x, 0f, playerRigidbody.linearVelocity.z); // make the y velocity 0 so that our jump will not be disturbed
        playerRigidbody.AddForce(transform.up * jumpForce, ForceMode.Impulse);
    }

    private void SetCanJump()
    {
        canJump = true;
    }

    private bool IsGrounded()
    {
        return Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.2f, groundLayer);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, new Vector3(transform.position.x, transform.position.y - (playerHeight * 0.5f + 0.2f),transform.position.z));
    }


}
