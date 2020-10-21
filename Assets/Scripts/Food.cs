using UnityEngine;

public enum FoodType {
    Pepper,
    Pizza,
    Berries,
    None,
}

public class Food : MonoBehaviour {

    public FoodType type;

    private bool attachedToPlayer = false;

    private Vector3 mousePosition;

    private Vector3 startPosition;
    private void Start () {
        startPosition = transform.position;

        Destroy (this.gameObject, 5.0F);
    }

}