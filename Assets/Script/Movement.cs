using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Movement : MonoBehaviour
{
    public Transform playerCamera;
    [SerializeField]
    [Range(0.0f, 0.5f)] float mouseSmoothTime = 0.03f;
    [SerializeField] bool cursorLock = true;
    [SerializeField] float mouseSensitivity = 3.5f;
    [SerializeField] float Speed = 6.0f;
    [SerializeField]
    [Range(0.0f, 0.5f)] float moveSmoothTime = 0.03f;
    [SerializeField] float gravity = 30f;
    [SerializeField] Transform groundCheck;
    [SerializeField] LayerMask ground;
    [SerializeField]
    float throwForce;

    public float itemPickupDistance;
    [SerializeField] float telepathySpeed = 5f;
    public Transform attachedObject = null;

    float attachedDistance = 0f;
    public Transform head;
    float velocityY;
    bool isGrounded;

    float cameraCap;
    Vector2 currentMouseDelta;
    Vector2 currentMouseDeltaVelocity;

    Vector2 currentDir;
    Vector2 currentDirVelocity;
    Vector3 velocity;

    Rigidbody rb; // Changed from CharacterController to Rigidbody

    void Start()
    {
        rb = GetComponent<Rigidbody>(); // Changed from CharacterController to Rigidbody

        if (cursorLock)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = true;
        }
    }

    void Update()
    {
        UpdateMouse();
        UpdateMove();

        RaycastHit hit;
        bool cast = Physics.Raycast(playerCamera.position, playerCamera.forward, out hit, itemPickupDistance);

       

        if (attachedObject != null)
        {
            if (Input.GetKeyDown(KeyCode.F))
            {
                DetachObject();
            }
            else
            {
                TelepathicallyMoveObject();
            }
        }
        else
        {
            

            if (Input.GetKeyDown(KeyCode.F))
            {
                if (cast && hit.transform.CompareTag("telepathy"))
                {
                    AttachObject(hit.transform);
                }
            }
        }
    }

    void TelepathicallyMoveObject()
    {
        Vector3 targetPosition = head.position + head.forward * attachedDistance;
        attachedObject.position = Vector3.Lerp(attachedObject.position, targetPosition, telepathySpeed * Time.deltaTime);
    }

    void AttachObject(Transform obj)
    {
        attachedObject = obj;
        attachedObject.SetParent(transform);

        if (attachedObject.GetComponent<Rigidbody>() != null)
        {
            attachedObject.GetComponent<Rigidbody>().isKinematic = true;
        }

        if (attachedObject.GetComponent<Collider>() != null)
        {
            attachedObject.GetComponent<Collider>().enabled = false;
        }
    }

    void DetachObject()
    {
        if (attachedObject != null)
        {
            // Release the object from being a child of the player
            attachedObject.SetParent(null);

            // Enable Rigidbody and Collider
            Rigidbody attachedRigidbody = attachedObject.GetComponent<Rigidbody>();
            Collider attachedCollider = attachedObject.GetComponent<Collider>();

            if (attachedRigidbody != null)
            {
                attachedRigidbody.isKinematic = false;
                // Add a force to throw the object away from the player
                attachedRigidbody.AddForce(playerCamera.forward * throwForce, ForceMode.Impulse);
            }

            if (attachedCollider != null)
            {
                attachedCollider.enabled = true;
            }

            // Reset the attachedObject reference
            attachedObject = null;
        }
    }

    void UpdateMouse()
    {
        Vector2 targetMouseDelta = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));

        currentMouseDelta = Vector2.SmoothDamp(currentMouseDelta, targetMouseDelta, ref currentMouseDeltaVelocity, mouseSmoothTime);

        cameraCap -= currentMouseDelta.y * mouseSensitivity;

        cameraCap = Mathf.Clamp(cameraCap, -90.0f, 90.0f);

        playerCamera.localEulerAngles = Vector3.right * cameraCap;

        transform.Rotate(Vector3.up * currentMouseDelta.x * mouseSensitivity);
    }

    void UpdateMove()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, 0.2f, ground);

        float horizontalInput = Input.GetAxisRaw("Horizontal");
        float verticalInput = Input.GetAxisRaw("Vertical");

        Vector2 targetDir = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        targetDir.Normalize();

        currentDir = Vector2.SmoothDamp(currentDir, targetDir, ref currentDirVelocity, moveSmoothTime);

        velocityY += gravity * 2f * Time.deltaTime;

      

        if (isGrounded && rb.velocity.y < -1f)
        {
            velocityY = -8f;
        }

        Vector3 inputDir = new Vector3(horizontalInput, 0f, verticalInput).normalized;
        Vector3 moveDirection = transform.TransformDirection(inputDir);
        rb.MovePosition(rb.position + moveDirection * Speed * Time.deltaTime + Vector3.up * velocityY * Time.deltaTime);
    }
}
