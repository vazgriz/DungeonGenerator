using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{

    public CharacterController Controller;

    [SerializeField]
    public float MovementSpeed = 2f;
    public float Speed = 12f;
    public float Gravity = -9.81f;
    public float JumpHeight = 3f;

    public Transform GroundCheck;
    public float GroundDistance = 4f;
    public LayerMask GroundMask;

    private Vector3 _velocity;

    void Start()
    {
        
    }

    void Update()
    {
        bool isGrounded = Physics.CheckSphere(GroundCheck.position, GroundDistance, GroundMask);

        if(isGrounded && velocity.y < 0)
        {
            velocity.y = -MovementSpeed;
        }

        float xMovement = Input.GetAxis("Horizontal");
        float zMovement = Input.GetAxis("Vertical");

        Vector3 move = transform.right * xMovement + transform.forward * zMovement;

        Controller.Move(move * Speed * Time.deltaTime);

        if(Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y = Mathf.Sqrt(JumpHeight * -MovementSpeed * Gravity);
        }

        velocity.y += Gravity * Time.deltaTime;

        Controller.Move(velocity * Time.deltaTime);
    }
}
