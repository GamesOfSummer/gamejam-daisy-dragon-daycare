using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Dragon : MonoBehaviour {

    public Slider hungrySlider;
    public Slider moodSlider;

    public float hungerDepletionSpeed = 15.0F;
    public float hungerIncreaseAmount = 70.0F;
    public float moodDepletionSpeed = 15.0F;
    public float moodIncreaseSpeed = 50.0F;

    private bool beingPet = false;

    public ParticleSystem particle;

    [Range (0, 100)]
    public float hungerMeter = 100;

    [Range (0, 100)]
    public float moodMeter = 100;

    void Start () {
        particle.Stop ();

        hungrySlider.maxValue = 100.0f;
        moodSlider.maxValue = 100.0f;
    }

    void Update () {

        if (hungerMeter > 0) {
            hungerMeter -= (hungerDepletionSpeed * Time.deltaTime);
        }

        if (moodMeter > 0 && !beingPet) {
            moodMeter -= (moodDepletionSpeed * Time.deltaTime);
        }

        moodSlider.value = moodMeter;
        hungrySlider.value = hungerMeter;
    }

    private void OnTriggerEnter (Collider other) {
        Debug.Log ("enter");

        if (other.tag == "Food") {
            Destroy (other.gameObject);
            hungerMeter += hungerIncreaseAmount;
        }

    }

    private void OnTriggerStay (Collider other) {

        if (Input.GetMouseButton (0)) {
            if (!particle.isPlaying) {
                particle.Play ();
                moodMeter += (moodIncreaseSpeed * Time.deltaTime);
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
    }
}