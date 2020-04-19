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

    public Image happyIcon;
    public Image neutralIcon;
    public Image sadIcon;

    public Slider hungrySlider;
    public Slider patientTimer;

    public float hungerDepletionSpeed = 15.0F;
    public float hungerIncreaseAmount = 70.0F;

    public float paienceIncreaseBonus = 15.0F;
    public float paienceDepletionSpeed = 15.0F;
    public float patienceIncreaseSpeed = 50.0F;

    private bool beingPet = false;

    public ParticleSystem particle;

    [Range (0, 1)]
    public float hungerMeter = 1.0F;

    [Range (0, 1)]
    public float paitenceMeter = 0;

    private float internalMoodSetting = 1.0F;

    private GameObject _player = null;

    void Start () {
        particle.Stop ();

        hungrySlider.maxValue = 1.0f;
        patientTimer.maxValue = 1.0f;

        hotIcon.enabled = false;
        coldIcon.enabled = false;

        happyIcon.enabled = false;
        neutralIcon.enabled = true;
        sadIcon.enabled = false;
        StartCoroutine (CalculateMoodSummary ());
    }

    void Update () {

        if (hungerMeter > 0) {
            hungerMeter -= (hungerDepletionSpeed * Time.deltaTime);
        }

        if (paitenceMeter < 1) {
            paitenceMeter += (patienceIncreaseSpeed * CalculateMood () * internalMoodSetting * Time.deltaTime);
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
        beingPet = false;
        _player = null;
    }

    private IEnumerator CalculateMoodSummary () {

        while (true) {
            yield return new WaitForSeconds (2.0F);
            float mood = CalculateMood () * internalMoodSetting;

            Debug.Log (mood);

            happyIcon.enabled = false;
            neutralIcon.enabled = false;
            sadIcon.enabled = false;

            if (mood < .3F) {
                sadIcon.enabled = true;
            } else if (mood >.9F) {
                happyIcon.enabled = true;
            } else {
                neutralIcon.enabled = true;
            }
        }
    }

    private float CalculateMood () {
        if (hungerMeter < .5F) {
            return 0;
        } else if (hungerMeter < .9F) {
            return 0.7F;
        }

        return 1;
    }

    public bool canBeReleased () {
        return paitenceMeter >.95F;
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