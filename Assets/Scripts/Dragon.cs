using System.Collections;
using System.Linq;
using TMPro;
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
    public static Dragon instance;
    public AudioClip[] sfx;
    public AudioSource audioSource;
    //SFX Variables
    [HideInInspector]
    public int chirp1 = 0;
    [HideInInspector]
    public int chirp2 = 1;
    [HideInInspector]
    public int hungry = 2;
    [HideInInspector]
    public int notPleased = 3;
    [HideInInspector]
    public int pleased = 4;
    [HideInInspector]
    public int snore = 5;

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

    private bool startedCountdown;
    private TextMeshProUGUI thankyouText;
    private TextMeshProUGUI countdownText;

    private bool mouseIsCurrentlyOnMe = false;
    private bool hasBeenPetOnce = false;

    private bool hasBeenFedWrongFood = false;

    private bool hasBeenFedFavoriteFoodOnce = false;

    public GameObject heartsParticleEffect;
    public GameObject hotParticleEffect;
    public GameObject coldParticleEffect;

    public GameObject favoriteFoodEffect;
    [Range (0, 1)]
    public float hungerMeter = 0.5F;

    [Range (0, 1)]
    public float patienceMeter = 0;

    private GameObject _player = null;

    private StatusAilment status = StatusAilment.None;

    private void Awake () {
        instance = this;
    }
    void Start () {
        ResetDragon ();
        StartDragon ();
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

            if (!startedCountdown) {
                startedCountdown = true;
                thankyouText.enabled = true;
                countdownText.enabled = true;
                StartCoroutine (StartCoRoutineToRelease ());
            }
        }
    }

    private IEnumerator StartCoRoutineToRelease () {
        countdownText.text = "3";
        yield return new WaitForSeconds (1.0f);
        countdownText.text = "2";
        yield return new WaitForSeconds (1.0f);
        countdownText.text = "1";
        yield return new WaitForSeconds (1.0f);
        GameController.Instance.ReleaseDragon (this);
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

        if (other.tag == "Player") {
            _player = gameObject;
        }

        if (_player != null && mouseIsCurrentlyOnMe) {

            Vector2 screenPosition = new Vector2 (Input.mousePosition.x, Input.mousePosition.y);
            Vector2 worldPosition = Camera.main.ScreenToViewportPoint (screenPosition);

            if (worldPosition.x > 0.3F && worldPosition.x < 0.7F) {
                Cursor.SetCursor (GameController.Instance.mouseHandImage, new Vector2 (1, 1), CursorMode.Auto);
            } else {
                Cursor.SetCursor (null, Vector2.zero, CursorMode.Auto);
            }

            hasBeenPetOnce = true;

            if (status != StatusAilment.None || !heartsParticleEffect.GetComponent<ParticleSystem> ().isPlaying) {
                heartsParticleEffect.GetComponent<ParticleSystem> ().Play ();
            }

        } else if (status != StatusAilment.None || heartsParticleEffect.GetComponent<ParticleSystem> ().isPlaying) {
            heartsParticleEffect.GetComponent<ParticleSystem> ().Stop ();
        }
    }

    private void OnTriggerExit (Collider other) {
        _player = null;
        Cursor.SetCursor (null, Vector2.zero, CursorMode.Auto);
    }

    void OnMouseEnter () {
        mouseIsCurrentlyOnMe = true;
    }

    void OnMouseExit () {
        mouseIsCurrentlyOnMe = false;
        Cursor.SetCursor (null, Vector2.zero, CursorMode.Auto);
    }

    private IEnumerator ProcessEmotions () {
        while (!canBeReleased ()) {
            EnableCorrectMoodIcon ();

            if (patienceMeter >.5F && patienceMeter < .9F) {
                if (chanceToGetAStatusAilment < .1F) {
                    Poop ();
                } else {
                    if (Random.Range (0, 4) <= 1) {
                        Poop ();
                    } else {
                        AddStatusAilment ();
                    }
                }

            }

            yield return new WaitForSeconds (0.5F);
        }
    }

    bool hasGottenAStatusAilment = false;
    private void AddStatusAilment () {

        if (!hasGottenAStatusAilment) {
            if (Random.Range (0.0F, 1.0F) < chanceToGetAStatusAilment) {
                hasGottenAStatusAilment = true;
                sickIcon.enabled = true;
                happyIcon.enabled = false;
                sadIcon.enabled = false;

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
        sickIcon.enabled = false;

        hotIcon.enabled = false;
        hotParticleEffect.GetComponent<ParticleSystem> ().Stop ();
        coldIcon.enabled = false;
        coldParticleEffect.GetComponent<ParticleSystem> ().Stop ();
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
            StartCoroutine (DragonMoodSFX (notPleased));
        } else {
            float mood = CalculateMood ();

            if (mood < .2F) {
                sadIcon.enabled = true;
                StartCoroutine (DragonMoodSFX (notPleased));

            } else if (mood >.8F) {
                happyIcon.enabled = true;
                StartCoroutine (DragonMoodSFX (pleased));
            }
        }

    }

    private float CalculateMood () {
        if (NeedToCleanupPoop () || status != StatusAilment.None || hungerMeter < .3F) {
            return 0;
        }

        var mood = 1.0F;

        if (hungerMeter >.7F) {
            mood += .5F;
        }

        if (hasBeenPetOnce == true) {
            mood += 1.5F;
        }

        if (hasBeenFedFavoriteFoodOnce == true) {
            mood += 1.5F;
        }

        return mood;
    }

    public bool canBeReleased () {
        return patienceMeter >.95F && status == StatusAilment.None && !NeedToCleanupPoop ();
    }

    private void Feed (FoodType type) {
        if (type == likedFood) {
            feedDragonLikedFood ();
            StartCoroutine (DragonMoodSFX (pleased));

        } else if (type == dislikedFood) {
            feedDragonDislikedFood ();
            StartCoroutine (DragonMoodSFX (notPleased));
        }
    }

    private void feedDragonLikedFood () {
        hasBeenFedFavoriteFoodOnce = true;
        patienceMeter += patienceIncreaseBonus;
        hungerMeter += hungerIncreaseFavoriteFoodBonus;
        StartCoroutine (PlayFavoriteFoodEffect ());
    }

    private IEnumerator PlayFavoriteFoodEffect () {
        if (!favoriteFoodEffect.GetComponent<ParticleSystem> ().isPlaying) {
            favoriteFoodEffect.GetComponent<ParticleSystem> ().Play ();
            yield return new WaitForSeconds (3.0f);
            favoriteFoodEffect.GetComponent<ParticleSystem> ().Stop ();
        }
    }

    private void feedDragonDislikedFood () {
        hasBeenFedWrongFood = true;
        patienceMeter -= patienceIncreaseBonus;

        hungerMeter = 0;

        hungerMeter -= hungerIncreaseWhenFed;
    }

    public int CaluclateFinalScore () {

        if (hasBeenFedWrongFood) {
            return 0;
        }

        int startScore = 10;

        if (hasBeenPetOnce) {
            startScore += 5;
        }

        if (hasBeenFedFavoriteFoodOnce) {
            startScore += 5;
        }

        return startScore;
    }

    public void StartDragon () {
        var canvas = GetComponentInChildren<CanvasUI> ();
        happyIcon = canvas.happyIcon;
        sadIcon = canvas.sadIcon;
        sickIcon = canvas.sickIcon;

        hotIcon = canvas.hotIcon;
        coldIcon = canvas.coldIcon;

        hungrySlider = canvas.hungrySlider;
        patientTimer = canvas.patientTimer;

        thankyouText = canvas.thankyouText;
        countdownText = canvas.countdownText;

        StartCoroutine (ProcessEmotions ());
        StartCoroutine (DragonSpawnSFX (chirp1));
    }

    public void ResetDragon () {
        StopAllCoroutines ();

        if (heartsParticleEffect != null && hungrySlider != null) {
            hotParticleEffect.GetComponent<ParticleSystem> ().Stop ();
            coldParticleEffect.GetComponent<ParticleSystem> ().Stop ();
            heartsParticleEffect.GetComponent<ParticleSystem> ().Stop ();
            favoriteFoodEffect.GetComponent<ParticleSystem> ().Stop ();

            patienceMeter = 0;
            patientTimer.value = 0;
            patientTimer.maxValue = 1.0f;

            hungerMeter = 0.5F;
            hungrySlider.value = 0.5F;
            hungrySlider.maxValue = 1.0f;

            var fill = hungrySlider.GetComponentsInChildren<UnityEngine.UI.Image> ()
                .FirstOrDefault (t => t.name == "Fill");
            if (fill != null) {
                fill.color = Color.red;
            }

            fill = patientTimer.GetComponentsInChildren<UnityEngine.UI.Image> ()
                .FirstOrDefault (t => t.name == "Fill");
            if (fill != null) {
                fill.color = Color.blue;
            }

            hotIcon.enabled = false;
            coldIcon.enabled = false;
            happyIcon.enabled = false;
            sadIcon.enabled = false;
            sickIcon.enabled = false;

            startedCountdown = false;
            thankyouText.enabled = false;
            countdownText.text = "3";
            countdownText.enabled = false;

            hasBeenPetOnce = false;
            hasPooped = false;
            cleanedUpPoop = false;
            hasGottenAStatusAilment = false;

            hasBeenFedFavoriteFoodOnce = false;
            hasBeenFedWrongFood = false;

            _player = null;
            mouseIsCurrentlyOnMe = false;
        }
    }

    public IEnumerator DragonSpawnSFX (int sfxToPlay) {
        if (gameObject.name == "Dragon-Berry-A(Clone)") {
            int timeToWait = (0);
            yield return new WaitForSeconds (timeToWait);
            audioSource.PlayOneShot (sfx[sfxToPlay], 1f);
            Debug.Log ("Played " + sfx[sfxToPlay] + " after waiting for " + timeToWait + " seconds!");
            Debug.Log (gameObject.name);
        } else if (gameObject.name == "Dragon-Moth-A(Clone)") {
            float timeToWait = (.6f);
            yield return new WaitForSeconds (timeToWait);
            audioSource.PlayOneShot (sfx[sfxToPlay], 1f);
            Debug.Log ("Played " + sfx[sfxToPlay] + " after waiting for " + timeToWait + " seconds!");
            Debug.Log (gameObject.name);
        } else if (gameObject.name == "Dragon-Koi-A(Clone)") {
            float timeToWait = (1.2f);
            yield return new WaitForSeconds (timeToWait);
            audioSource.PlayOneShot (sfx[sfxToPlay], 1f);
            Debug.Log ("Played " + sfx[sfxToPlay] + " after waiting for " + timeToWait + " seconds!");
            Debug.Log (gameObject.name);
        }
    }

    public IEnumerator DragonMoodSFX (int sfxToPlay) {

        int timeToWait = (0);
        yield return new WaitForSeconds (timeToWait);
        audioSource.clip = sfx[sfxToPlay];
        audioSource.loop = false;
        audioSource.Play();
        Debug.Log ("Played " + sfx[sfxToPlay] + " after waiting for " + timeToWait + " seconds!");
        Debug.Log (gameObject.name);

    }
}