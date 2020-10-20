using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class Dragon : MonoBehaviour {
    [Header ("Settings - please tweak!")]
    [Range (.001F, .05F)]
    public float hungerDepletionSpeed = 0.001F;

    [Range (.1F, .5F)]
    public float hungerIncreaseWhenFed = 0.1F;

    [Range (.001F, .7F)]
    public float hungerIncreaseFavoriteFoodBonus = 0.1F;

    [Range (.001F, .05F)]
    public float patienceIncreaseBonus = 0.001F;

    [Range (.001F, .05F)]
    public float patienceIncreaseSpeed = 0.001F;

    [Range (.0F, 1.0F)]
    public float chanceToGetAStatusAilment = 0.0F;

    [HideInInspector]
    public string dragonId;

    [Header ("DO NOT TOUCH BELOW THIS LINE")]
    public GameObject poop;
    public FoodType likedFood;
    public FoodType dislikedFood;

    private Image hotIcon;
    private Image coldIcon;

    private Image sickIcon;

    private Image happyIcon;
    private Image sadIcon;

    private Slider hungrySlider;
    private Slider patientTimer;

    private bool mouseIsCurrentlyOnMe = false;

    private bool beingPet = false;
    private bool hasBeenPetOnce = false;

    private bool hasBeenFedFavoriteFoodOnce = false;

    public GameObject heartsParticleEffect;
    public GameObject hotParticleEffect;
    public GameObject coldParticleEffect;
    [Range (0, 1)]
    public float hungerMeter = 0.5F;

    [Range (0, 1)]
    public float patienceMeter = 0;

    private GameObject _player = null;

    private StatusAilment status = StatusAilment.None;

    void Start () {
        var canvas = GetComponentInChildren<CanvasUI> ();
        happyIcon = canvas.happyIcon;
        sadIcon = canvas.sadIcon;
        sickIcon = canvas.sickIcon;

        hotIcon = canvas.hotIcon;
        coldIcon = canvas.coldIcon;

        hungrySlider = canvas.hungrySlider;
        patientTimer = canvas.patientTimer;

        ResetDragon ();
        StartCoroutine (ProcessEmotions ());
    }

    void Update () {
        if (!canBeReleased ()) {
            if (hungerMeter > 0) {
                hungerMeter -= (hungerDepletionSpeed * Time.deltaTime);
            }

            if (patienceMeter < 1) {
                patienceMeter += (patienceIncreaseSpeed * CalculateMood () * Time.deltaTime);
            }

            patientTimer.value = patienceMeter;
            hungrySlider.value = hungerMeter;
        } else {
            happyIcon.enabled = true;
            hungrySlider.value = 1.0F;

            var fill = hungrySlider.GetComponentsInChildren<UnityEngine.UI.Image> ()
                .FirstOrDefault (t => t.name == "Fill");
            if (fill != null) {
                fill.color = Color.green;
            }

            fill = patientTimer.GetComponentsInChildren<UnityEngine.UI.Image> ()
                .FirstOrDefault (t => t.name == "Fill");
            if (fill != null) {
                fill.color = Color.green;
            }

        }
    }

    private void OnTriggerEnter (Collider other) {

        if (other.tag == "Food" && hungerMeter < 1.0F) {
            Destroy (other.gameObject);
            Feed (other.GetComponent<Food> ().type);
        }

        if (other.tag == "Player") {
            _player = gameObject;
        }

    }

    private void OnTriggerStay (Collider other) {

        if (_player != null && Input.GetMouseButton (0)) {
            hasBeenPetOnce = true;

            if (!heartsParticleEffect.GetComponent<ParticleSystem> ().isPlaying) {
                Debug.Log ("play");
                heartsParticleEffect.GetComponent<ParticleSystem> ().Play ();
                beingPet = true;
            }

            Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
            var holder = ray.GetPoint (1);
            Debug.Log (holder);
            // Debug.Log (Vector3.Distance (prefferedPettingSpot.transform.position, holder));

        } else if (heartsParticleEffect.GetComponent<ParticleSystem> ().isPlaying) {
            heartsParticleEffect.GetComponent<ParticleSystem> ().Stop ();
        }
    }

    private void OnTriggerExit (Collider other) {
        beingPet = false;
        _player = null;
    }

    void OnMouseEnter () {
        Debug.Log ("enter");
        mouseIsCurrentlyOnMe = true;
        Cursor.SetCursor (GameController.Instance.mouseHandImage, Vector2.zero, CursorMode.Auto);
    }

    void OnMouseExit () {
        Debug.Log ("exit");
        mouseIsCurrentlyOnMe = false;
        Cursor.SetCursor (null, Vector2.zero, CursorMode.Auto);
    }

    private IEnumerator ProcessEmotions () {
        while (!canBeReleased ()) {
            EnableCorrectMoodIcon ();

            if (patienceMeter >.5F && patienceMeter < .8F) {
                if (Random.Range (0, 4) < 2) {
                    Poop ();
                } else {
                    AddStatusAilment ();
                }
            }

            yield return new WaitForSeconds (3.0F);
        }
    }

    bool hasGottenAStatusAilment = false;
    private void AddStatusAilment () {

        if (!hasGottenAStatusAilment) {

            if (Random.Range (0.0F, 1.0F) < chanceToGetAStatusAilment) {
                hasGottenAStatusAilment = true;
                Debug.Log ("SICK");

                if (Random.Range (0, 5) < 2) {
                    status = StatusAilment.Hot;
                    hotIcon.enabled = true;
                    hotParticleEffect.GetComponent<ParticleSystem> ().Play ();

                } else {
                    status = StatusAilment.Cold;
                    coldIcon.enabled = true;
                    coldParticleEffect.GetComponent<ParticleSystem> ().Play ();
                }
            }
        }

    }

    public void HealDragon () {
        status = StatusAilment.None;
        hotIcon.enabled = false;
        hotParticleEffect.GetComponent<ParticleSystem> ().Stop ();
        coldIcon.enabled = false;
        hotParticleEffect.GetComponent<ParticleSystem> ().Stop ();
    }

    bool hasPooped = false;
    bool cleanedUpPoop = false;

    GameObject poopObj;
    private void Poop () {

        if (hungerMeter >.6F && !hasPooped) {
            hasPooped = true;
            poopObj = GameController.Instance.SpawnObject (poop);
            var pos = transform.position;
            poopObj.transform.position = new Vector3 (pos.x, pos.y, pos.z);
        }
    }

    public bool NeedToCleanupPoop () {
        return (hasPooped && !cleanedUpPoop);
    }

    public void CleanupPoop () {
        cleanedUpPoop = true;
        Destroy (poopObj, 0.5F);
    }

    private void EnableCorrectMoodIcon () {
        happyIcon.enabled = false;
        sadIcon.enabled = false;
        sickIcon.enabled = false;

        if (status != StatusAilment.None) {
            sickIcon.enabled = true;
        } else {
            float mood = CalculateMood ();

            if (mood < .2F) {
                sadIcon.enabled = true;
            } else if (mood >.8F) {
                happyIcon.enabled = true;
            }
        }

    }

    private float CalculateMood () {
        var mood = 1.0F;

        if (status != StatusAilment.None) {
            mood -= .5F;
        }
        if (NeedToCleanupPoop ()) {
            return 0;
        }

        if (hungerMeter < .3F) {
            return 0;
        } else if (hungerMeter >.7F) {
            mood += .5F;
        }

        if (hasBeenPetOnce == true) {
            mood += 2.5F;
        }

        if (hasBeenFedFavoriteFoodOnce == true) {
            mood += 2.5F;
        }

        return mood;
    }

    public bool canBeReleased () {
        return patienceMeter >.95F && status == StatusAilment.None && !NeedToCleanupPoop ();
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
        hasBeenFedFavoriteFoodOnce = true;
        patienceMeter += patienceIncreaseBonus;
        hungerMeter += hungerIncreaseFavoriteFoodBonus;
    }

    private void feedDragon () {
        hungerMeter += hungerIncreaseWhenFed;
    }

    private void feedDragonDislikedFood () {
        patienceMeter -= patienceIncreaseBonus;
        hungerMeter += hungerIncreaseWhenFed;
    }

    public int CaluclateFinalScore () {
        int startScore = 10;

        if (hasBeenPetOnce) {
            startScore += 5;
        }

        if (hasBeenFedFavoriteFoodOnce) {
            startScore += 5;
        }

        return startScore;
    }

    public void ResetDragon () {
        if (heartsParticleEffect != null && hungrySlider != null) {
            hotParticleEffect.GetComponent<ParticleSystem> ().Stop ();
            coldParticleEffect.GetComponent<ParticleSystem> ().Stop ();
            heartsParticleEffect.GetComponent<ParticleSystem> ().Stop ();

            patienceMeter = 0;
            patientTimer.value = 0;
            patientTimer.maxValue = 1.0f;

            hungerMeter = 0.5F;
            hungrySlider.value = 0.5F;
            hungrySlider.maxValue = 1.0f;

            hotIcon.enabled = false;
            coldIcon.enabled = false;
            happyIcon.enabled = false;
            sadIcon.enabled = false;
            sickIcon.enabled = false;

            hasBeenPetOnce = false;
            hasPooped = false;
            cleanedUpPoop = false;
            hasGottenAStatusAilment = false;

            hasBeenFedFavoriteFoodOnce = false;
        } else {
            Debug.Log ("Null values on reset dragon");
        }

    }
}