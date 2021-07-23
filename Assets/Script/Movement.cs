using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
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
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        MoveCharacter();
    }

    private void MoveCharacter()
    {
        GetKey();
        moveDirection = new Vector3(charaPos.x, 0, charaPos.z);
        transform.position += moveDirection * Time.deltaTime;
        transform.Rotate(new Vector3(0, charaPos.angle_y, 0) * Time.deltaTime);
    }

    private void GetKey()
    {
        if (Input.GetKey(KeyCode.A) && Input.GetKey(KeyCode.W))
        {
            charaPos.angle_y = -90;
            charaPos.x = speed * Mathf.Sin(transform.eulerAngles.y * Mathf.PI / 180);
            charaPos.z = speed * Mathf.Cos(transform.eulerAngles.y * Mathf.PI / 180);
        }
        else if (Input.GetKey(KeyCode.D) && Input.GetKey(KeyCode.W))
        {
            charaPos.angle_y = 90;
            charaPos.x = speed * Mathf.Sin(transform.eulerAngles.y * Mathf.PI / 180);
            charaPos.z = speed * Mathf.Cos(transform.eulerAngles.y * Mathf.PI / 180);
        }
        else if (Input.GetKey(KeyCode.D) && Input.GetKey(KeyCode.S))
        {
            charaPos.angle_y = 90;
            charaPos.x = -speed * Mathf.Sin(transform.eulerAngles.y * Mathf.PI / 180);
            charaPos.z = -speed * Mathf.Cos(transform.eulerAngles.y * Mathf.PI / 180);
        }
        else if (Input.GetKey(KeyCode.A) && Input.GetKey(KeyCode.S))
        {
            charaPos.angle_y = -90;
            charaPos.x = -speed * Mathf.Sin(transform.eulerAngles.y * Mathf.PI / 180);
            charaPos.z = -speed * Mathf.Cos(transform.eulerAngles.y * Mathf.PI / 180);
        }
        else if (Input.GetKey(KeyCode.A))
        {
            charaPos.angle_y = -90;
            charaPos.x = 0;
            charaPos.z = 0;
        }
        else if (Input.GetKey(KeyCode.D))
        {
            charaPos.angle_y = 90;
            charaPos.x = 0;
            charaPos.z = 0;
        }
        else if (Input.GetKey(KeyCode.W))
        {
            charaPos.x = speed * Mathf.Sin(transform.eulerAngles.y * Mathf.PI / 180);
            charaPos.z = speed * Mathf.Cos(transform.eulerAngles.y * Mathf.PI / 180);
            charaPos.angle_y = 0;
        }
        else if (Input.GetKey(KeyCode.S))
        {
            charaPos.x = -speed * Mathf.Sin(transform.eulerAngles.y * Mathf.PI / 180);
            charaPos.z = -speed * Mathf.Cos(transform.eulerAngles.y * Mathf.PI / 180);
            charaPos.angle_y = 0;
        }
        else
        {
            charaPos.x = 0;
            charaPos.z = 0;
            charaPos.angle_y = 0;
        }
    }
}
