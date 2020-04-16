using System.Collections;
using UnityEngine;

// This script moves the character controller forward
// and sideways based on the arrow keys.
// It also jumps when pressing space.
// Make sure to attach a character controller to the same game object.
// It is recommended that you make only one call to Move or SimpleMove per frame.

public class PlayerController : MonoBehaviour {
    private CharacterController _controller;

    public float _speed = 10;
    public float _rotationSpeed = 180;

    private Vector3 rotation;
    private Vector3 moveDirection = Vector3.zero;

    private GameObject fruit;

    void Start () {
        _controller = GetComponent<CharacterController> ();
    }

    public void Update () {
        this.rotation = new Vector3 (0, Input.GetAxisRaw ("Horizontal") * _rotationSpeed * Time.deltaTime, 0);

        Vector3 move = new Vector3 (0, 0, Input.GetAxisRaw ("Vertical") * Time.deltaTime);
        move = this.transform.TransformDirection (move);
        _controller.Move (move * _speed);
        this.transform.Rotate (this.rotation);

    }

}