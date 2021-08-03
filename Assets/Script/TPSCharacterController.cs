using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TPSCharacterController : MonoBehaviour
{
    private Transform characterBody;
    struct CharacterPosition
    {
        public float x;
        public float y;
        public float z;
        public float angle_y;
    }

    public float speed = 3.0f;
    CharacterPosition charaPos;
    private Vector3 moveDirection;
    Animator animator;
    CharacterController characterController;

    void Start()
    {
        characterBody = transform.GetChild(0).transform;
        animator = characterBody.GetComponent<Animator>();
        Cursor.lockState = CursorLockMode.Locked;
        characterController = GetComponent<CharacterController>();
    }
    
    void Update()
    {
        MoveCharacter();
    }

    private float ClampAngle(float angle, float min, float max)
    {
        if (angle < -360) angle += 360;
        if (angle > 360) angle -= 360;

        return Mathf.Clamp(angle, min, max);
    }

    private void MoveCharacter()
    {
        Vector2 moveInput = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        bool isMove = moveInput.magnitude != 0;
        animator.SetBool("isRun", isMove);

        if (isMove)
        {
            Vector3 moveDir = new Vector3(moveInput.x, 0, moveInput.y);
            transform.Rotate(Vector3.up, moveInput.x * 300f * Time.deltaTime);
            //transform.Translate(moveDir * speed * Time.deltaTime);
            if(moveInput.y != 0)
            {
                characterController.SimpleMove(transform.forward * speed * moveInput.y);
            }
        }
    }
}
