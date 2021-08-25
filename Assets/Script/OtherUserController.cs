using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OtherUserController : MonoBehaviour
{
    public int moveDir;
    public float speed = 3.0f;
    public float rotationSpeed = 180f;
    private CharacterController characterController;
    Animator animator;
    // Start is called before the first frame update
    void Start()
    {
        characterController = GetComponent<CharacterController>();
        animator = transform.GetChild(0).transform.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        MoveByPacket();
    }

    void MoveByPacket()
    {
        Vector2 moveInput = new Vector2(0, 0);
        if (moveDir == 0)
        {
            moveInput = new Vector2(0, 0);
        }
        if (moveDir == 1)
        {
            moveInput = new Vector2(-1, 0);
        }
        if (moveDir == 2)
        {
            moveInput = new Vector2(1, 0);
        }
        if (moveDir == 3)
        {
            moveInput = new Vector2(0, 1);
        }
        if (moveDir == 4)
        {
            moveInput = new Vector2(0, -1);
        }
        if (moveDir == 5)
        {
            moveInput = new Vector2(-1, 1);
        }
        if (moveDir == 6)
        {
            moveInput = new Vector2(1, 1);
        }
        if (moveDir == 7)
        {
            moveInput = new Vector2(-1, -1);
        }
        if (moveDir == 8)
        {
            moveInput = new Vector2(1, -1);
        }
        bool isMove = moveInput.magnitude != 0;
        animator.SetBool("isRun", isMove);
        Vector3 move = new Vector3(0, 0, moveInput.y * Time.deltaTime);
        move = this.transform.TransformDirection(move);
        characterController.Move(speed * move);
        characterController.Move(new Vector3(0, -1, 0));
        transform.Rotate(new Vector3(0, moveInput.x * rotationSpeed * Time.deltaTime, 0));
    }
}
