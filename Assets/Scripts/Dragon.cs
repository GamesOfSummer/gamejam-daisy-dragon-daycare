using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Dragon : MonoBehaviour {

    public string dragonId;

    public FoodType likedFood;
    public FoodType dislikedFood;

    public Image hotIcon;
    public Image coldIcon;

    public Image moodIcon;

    public Slider hungrySlider;
    public Slider patientTimer;

    public float hungerDepletionSpeed = 15.0F;
    public float hungerIncreaseAmount = 70.0F;

    public float paienceIncreaseBonus = 15.0F;
    public float paienceDepletionSpeed = 15.0F;
    public float patienceIncreaseSpeed = 50.0F;

    private bool beingPet = false;

    public ParticleSystem particle;

    [Range (0, 100)]
    public float hungerMeter = 100;

    [Range (0, 100)]
    public float paitenceMeter = 0;

    private GameObject _player = null;

    void Start () {
        particle.Stop ();

        hungrySlider.maxValue = 100.0f;
        patientTimer.maxValue = 100.0f;

        hotIcon.enabled = false;
        coldIcon.enabled = false;

    }

    void Update () {

        if (hungerMeter > 0) {
            hungerMeter -= (hungerDepletionSpeed * Time.deltaTime);
        }

        if (paitenceMeter < 100) {
            paitenceMeter += (patienceIncreaseSpeed * Time.deltaTime);
        }

        patientTimer.value = paitenceMeter;
        hungrySlider.value = hungerMeter;
    }

    private void OnTriggerEnter (Collider other) {

        if (other.tag == "Food") {
            Destroy (other.gameObject);
            Feed (other.GetComponent<Food> ().type);
        }

        if (other.tag == "Player") {
            _player = gameObject;
        }
    }

    private void OnTriggerStay (Collider other) {

        if (_player != null && Input.GetMouseButton (0)) {
            if (!particle.isPlaying) {
                particle.Play ();
                beingPet = true;
            }
        }

        if (particle.isPlaying) {
            particle.Stop ();
        }
    }

    private void OnTriggerExit (Collider other) {
        Debug.Log ("exit");
        beingPet = false;
        _player = null;
    }

    public void ClickReleaseButton () {

        Debug.Log ("clicked");
        // GameController.Instance.ReleaseDragon (this.gameObject);
    }

    private void Feed (FoodType type) {

        if (type == likedFood) {
            feedDragonLikedFood ();
        } else if (type == dislikedFood) {
            feedDragonDislikedFood ();
        } else {
            feedDragon ();
        }

    }

    private void feedDragonLikedFood () {
        paitenceMeter += paienceIncreaseBonus;
        hungerMeter += hungerIncreaseAmount;
    }

    private void feedDragon () {
        hungerMeter += hungerIncreaseAmount;
    }

    private void feedDragonDislikedFood () {
        paitenceMeter -= paienceIncreaseBonus;
        hungerMeter += hungerIncreaseAmount;
    }
}