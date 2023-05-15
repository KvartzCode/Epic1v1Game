using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(CharacterController), typeof(Alteruna.Avatar))]
public class PlayerController : MonoBehaviour
{
    private Vector2 direction;
    private CharacterController charController;
    private Alteruna.Avatar avatar;
    [SerializeField] Transform cameraTransform;


    void Awake()
    {
        avatar = GetComponent<Alteruna.Avatar>();
        charController = GetComponent<CharacterController>();
    }

    void Start()
    {
        if (!avatar.IsMe)
        {
            enabled = false;
            return;
        }

        Camera.main.transform.SetParent(transform);
        Camera.main.transform.SetPositionAndRotation(cameraTransform.position, cameraTransform.rotation);
    }

    void Update()
    {
        direction = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        charController.Move(new Vector3(direction.x * Time.deltaTime, 0, direction.y * Time.deltaTime));
    }
}
