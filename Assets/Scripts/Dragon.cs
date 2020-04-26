using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum StatusAilment {
    None,
    Hot,
    Cold,
    Sick
}

public class Dragon : MonoBehaviour {

    [Header ("Settings - please tweak!")]
    public float hungerDepletionSpeed = 15.0F;
    public float hungerIncreaseAmount = 70.0F;

    public float paienceIncreaseBonus = 15.0F;
    public float paienceDepletionSpeed = 15.0F;
    public float patienceIncreaseSpeed = 50.0F;

    [Header ("Range => 1 - 100")]
    public float chanceToGetAStatusAilment = 5.0F;

    [HideInInspector]
    public string dragonId;

    [Header ("DO NOT TOUCH BELOW THIS LINE")]
    public GameObject poop;

    public GameObject prefferedPettingSpot;

    public FoodType likedFood;
    public FoodType dislikedFood;

    public Image hotIcon;
    public Image coldIcon;

    public Image sickIcon;

    public Image happyIcon;
    public Image sadIcon;

    public Slider hungrySlider;
    public Slider patientTimer;

    private bool beingPet = false;

    public ParticleSystem particle;

    [Range (0, 1)]
    public float hungerMeter = 0.5F;

    [Range (0, 1)]
    public float paitenceMeter = 0;

    private float internalMoodSetting = 1.0F;

    private GameObject _player = null;

    private StatusAilment status = StatusAilment.None;

    void Start () {
        particle.Stop ();

        hungrySlider.maxValue = 1.0f;
        hungerMeter = 0.7F;
        patientTimer.maxValue = 1.0f;

        hotIcon.enabled = false;
        coldIcon.enabled = false;

        happyIcon.enabled = false;
        sadIcon.enabled = false;
        StartCoroutine (ProcessEmotions ());
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

            Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
            var holder = ray.GetPoint (1);
            // Debug.Log (holder);

            // Debug.Log (Vector3.Distance (prefferedPettingSpot.transform.position, holder));

        }

        if (particle.isPlaying) {
            particle.Stop ();
        }
    }

    private void OnTriggerExit (Collider other) {
        beingPet = false;
        _player = null;
    }

    private IEnumerator ProcessEmotions () {

        while (true) {
            yield return new WaitForSeconds (2.0F);
            CalculateMoodSummaryInstantly ();
            Poop ();
            AddStatusAilment ();
        }
    }

    bool hasGottenAStatusAilment = false;
    private void AddStatusAilment () {

        if (!hasGottenAStatusAilment && paitenceMeter >.5F && paitenceMeter < .9F) {

            if (Random.Range (0, 101) < chanceToGetAStatusAilment) {
                hasGottenAStatusAilment = true;
                Debug.Log ("SICK");
                internalMoodSetting = 0;

                if (Random.Range (1, 3) < 2) {
                    status = StatusAilment.Hot;
                    hotIcon.enabled = true;

                } else {
                    status = StatusAilment.Cold;
                    coldIcon.enabled = true;
                }

            }

        }

    }

    bool hasPooped = false;
    bool cleanedUpPoop = false;
    private void Poop () {

        if (hungerMeter >.7F && !hasPooped) {
            hasPooped = true;

            var poopObj = GameController.Instance.SpawnObject (poop);
            var pos = transform.position;
            poopObj.transform.position = new Vector3 (pos.x, pos.y, pos.z);

        }

    }

    private void CalculateMoodSummaryInstantly () {
        float mood = CalculateMood () * internalMoodSetting;

        //Debug.Log (mood);

        happyIcon.enabled = false;
        sadIcon.enabled = false;

        if (mood < .2F) {
            sadIcon.enabled = true;
        } else if (mood >.7F) {
            happyIcon.enabled = true;
        }
    }

    private float CalculateMood () {
        if (hungerMeter < .3F) {
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
        internalMoodSetting = 2.0F;
    }

    private void feedDragon () {
        hungerMeter += hungerIncreaseAmount;
        internalMoodSetting = 1.0F;
    }

    private void feedDragonDislikedFood () {
        paitenceMeter -= paienceIncreaseBonus;
        hungerMeter += hungerIncreaseAmount;
        internalMoodSetting = 0.5F;
    }
}