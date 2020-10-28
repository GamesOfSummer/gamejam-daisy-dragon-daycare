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

	Vector3 _targetScale;

	float timeAlive = 0f;

	private void OnEnable()
	{
		_targetScale = this.transform.localScale;
		timeAlive = 0f;
	}

	private void OnDisable()
	{
		this.transform.localScale = _targetScale;
	}

	private void Update()
	{
		timeAlive += Time.deltaTime;
		this.transform.localScale = Vector3.Lerp(Vector3.zero, _targetScale, Mathf.Clamp01(timeAlive * 4.5f));
	}

	private void Start () {
        startPosition = transform.position;

        Destroy (this.gameObject, 5.0F);
    }

}