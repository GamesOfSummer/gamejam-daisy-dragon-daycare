using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dragon : MonoBehaviour {
    void Start () {

    }

    void Update () {

    }

    private void OnTriggerEnter (Collider other) {
        Debug.Log ("enter");
    }

    private void OnTriggerStay (Collider other) {

        if (Input.GetMouseButtonDown (0)) {
            Debug.Log ("been pet");
        }

    }
}