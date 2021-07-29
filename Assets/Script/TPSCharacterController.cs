using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TPSCharacterController : MonoBehaviour
{
    [SerializeField]
    private Transform characterBody;
    [SerializeField]
    private Transform cameraArm;

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

    void Start()
    {
        animator = characterBody.GetComponent<Animator>();
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        //LookAround();
        MoveCharacter();
    }

    private void LookAround()
    {
        Vector2 mouseDelta = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y")); // *로 감도 조절
        Vector3 camAngle = cameraArm.rotation.eulerAngles;

        float x = camAngle.x - mouseDelta.y;
        if (x < 180f)
        { 
            x = Mathf.Clamp(x, -1f, 70f);
        }
        else
        {
            x = Mathf.Clamp(x, 335f, 361f);
        }

        //cameraArm.RotateAround(characterBody.position, Vector3.forward, mouseDelta.x);
        cameraArm.rotation = Quaternion.Euler(camAngle.x - mouseDelta.y, camAngle.y + mouseDelta.x, 0);
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
            transform.Rotate(Vector3.up, moveInput.x * 100f * Time.deltaTime);
            transform.Translate(moveDir * speed * Time.deltaTime);
        }
    }
}
