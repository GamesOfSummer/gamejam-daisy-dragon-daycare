using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FaceCamera : MonoBehaviour {

    public float Ease = .15f;

    Transform _transform;

    void Awake () {
        _transform = transform;
    }

    // void Update () {

    //     var screenPoint = Camera.main.WorldToScreenPoint (_transform.localPosition);
    //     var offset = new Vector2 (screenPoint.x, screenPoint.y);
    //     var angle = Mathf.Atan2 (offset.y, offset.x) * Mathf.Rad2Deg;
    //     _transform.rotation = Quaternion.Slerp (_transform.rotation, Quaternion.Euler (0, -angle, 0), Ease);

    // }

    //Orient the camera after all movement is completed this frame to avoid jittering
    void LateUpdate () {
        transform.LookAt (transform.position + Camera.main.transform.rotation * Vector3.forward,
            Camera.main.transform.rotation * Vector3.up);
    }

}