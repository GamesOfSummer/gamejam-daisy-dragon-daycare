using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum FoodType {
    RedFruit,
    YellowFruit,
    BlueFruit,

    None,
}

public class Food : MonoBehaviour {

    public FoodType type;

    private bool attachedToPlayer = false;

    private Vector3 mousePosition;

    private Vector3 startPosition;
    private void Start () {
        startPosition = transform.position;
    }

    private void Update () {

        if (attachedToPlayer) {

        }

        if (Input.GetMouseButton (1) || (Input.GetButton ("Fire1"))) {

            Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
            transform.position = ray.GetPoint (1);

            //Debug.Log (ray + "-----" + transform.position);
        }

    }

}