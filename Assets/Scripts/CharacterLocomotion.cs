using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterLocomotion : MonoBehaviour
{
    [SerializeField] private Rigidbody rb = default;
    Animator animator;
    [SerializeField] private float speed;
    [SerializeField] private float runSpeed;

    private Vector2 input;
    private Vector3 movementDir;
    private float inputAmount;

    void Start()
    {
        animator = GetComponent<Animator>();
    }


    void FixedUpdate()
    {
        Move();
    }

    void Update()
    {
        
    }

    private void Move()
    {
        input.x = Input.GetAxis("Horizontal");
        input.y = Input.GetAxis("Vertical");

        Vector3 forward = input.y * transform.forward;
        Vector3 sideway = input.x * transform.right;

        Vector3 combinedInput = (forward + sideway).normalized;

        movementDir = new Vector3(combinedInput.x, 0f, combinedInput.z);

        float inputMagnitude = Mathf.Abs(input.y) + Mathf.Abs(input.x);
        inputAmount = Mathf.Clamp01(inputMagnitude);

        if (Input.GetButton("Run") && !animator.GetBool("isArmed"))
        {
            rb.velocity = movementDir * Mathf.Clamp(runSpeed, -1f, runSpeed) * inputAmount;
            animator.SetFloat("inputX", input.x * 2f);
            animator.SetFloat("inputY", input.y * 2f);
        }
        else
        {
            rb.velocity = movementDir * speed * inputAmount;
            animator.SetFloat("inputX", input.x);
            animator.SetFloat("inputY", input.y);
        }

        
    }

    private void Run()
    {
        
    }

    private void OnAnimatorMove()
    {

    }
}
