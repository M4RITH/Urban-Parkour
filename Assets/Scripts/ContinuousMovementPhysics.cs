using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


public class ContinuousMovementPhysics : MonoBehaviour
{
    public float speed = 1;
    public float turnSpeed = 60;
    private float jumpVelocity = 7.0f;
    public float jumpHeight = 1.5f;
    public float jumpHeightWhileClimbing = 2.5f;

    public bool onlyMoveWhenGrounded = false;

    public InputActionProperty moveInputSource;
    public InputActionProperty turnInputSource;
    public InputActionProperty jumpInputSource;

    public Rigidbody rb;

    public Transform directionSource;
    public Transform turnSource;

    public CapsuleCollider bodyCollider;
    public BoxCollider leftHandCollider;
    public BoxCollider rightHandCollider;

    private Vector2 inputMoveAxis;
    private float inputTurnAxis;
    private bool isGrounded;


    // Update is called once per frame
    void Update()
    {
        inputMoveAxis = moveInputSource.action.ReadValue<Vector2>();
        inputTurnAxis = turnInputSource.action.ReadValue<Vector2>().x;

        bool jumpInput = jumpInputSource.action.WasPressedThisFrame();

        if(jumpInput && isGrounded )
        {
            jumpVelocity = Mathf.Sqrt(2 * -Physics.gravity.y * jumpHeight);
            rb.velocity = Vector3.up * jumpVelocity;
        }

        if (jumpInput && IsHoldingOntoGrabbable())
        {
            jumpVelocity = Mathf.Sqrt(2 * -Physics.gravity.y * jumpHeightWhileClimbing);
            rb.velocity = Vector3.up * jumpVelocity;
        }
    }


    private void FixedUpdate()
    {
        isGrounded = CheckIfGrounded();
        bool isHoldingGrabbable = IsHoldingOntoGrabbable();

        if (!onlyMoveWhenGrounded || (onlyMoveWhenGrounded && isGrounded))
        {
            Quaternion yaw = Quaternion.Euler(0, directionSource.eulerAngles.y, 0);
            Vector3 direction = yaw * new Vector3(inputMoveAxis.x, 0, inputMoveAxis.y);

            Vector3 targetMovePosition = rb.position + direction * Time.fixedDeltaTime * speed;

            Vector3 axis = Vector3.up;
            float angle = turnSpeed * Time.fixedDeltaTime * inputTurnAxis;

            Quaternion q = Quaternion.AngleAxis(angle, axis);

            rb.MoveRotation(rb.rotation * q);  
            
            Vector3 newPosition = q*(targetMovePosition - turnSource.position) + turnSource.position;

            rb.MovePosition(newPosition);
        }
    }


    bool IsHoldingOntoGrabbable()
    {
        return IsHoldingOntoGrabbable(leftHandCollider) || IsHoldingOntoGrabbable(rightHandCollider);
    }


    bool IsHoldingOntoGrabbable(BoxCollider handCollider)
    {
        if (handCollider == null)
        {
            // Handles the case where the collider is not assigned
            return false;
        }

        Vector3 handPosition = handCollider.transform.TransformPoint(handCollider.center);
        float sphereCastRadius = 0.05f;  
        float sphereCastDistance = 0.05f;

        int defaultLayer = LayerMask.NameToLayer("Default");
        int grabbableLayer = LayerMask.NameToLayer("Grabbable");

        int layerMask = 1 << defaultLayer | 1 << grabbableLayer;

        RaycastHit hitInfo;
        bool hasHit = Physics.SphereCast(handPosition, sphereCastRadius, -handCollider.transform.up, out hitInfo, sphereCastDistance, layerMask);

        return hasHit;
    }


    public bool CheckIfGrounded()
    {
        Vector3 start = bodyCollider.transform.TransformPoint(bodyCollider.center);
        float rayLength = bodyCollider.height / 2 - bodyCollider.radius + 0.05f;
       
        int layerMask = 1 << LayerMask.NameToLayer("Default") | 1 << LayerMask.NameToLayer("Grabbable");

        bool hasHit = Physics.SphereCast(start, bodyCollider.radius, Vector3.down, out RaycastHit hitInfo, rayLength, layerMask);

        return hasHit;
    }
}
