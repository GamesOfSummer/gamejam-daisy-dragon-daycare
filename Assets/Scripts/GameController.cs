using System;
using System.Collections;
using System.Collections.Generic;
using PKG;
using UnityEngine;
using Random = UnityEngine.Random;
using System.Linq;
using UnityEngine.UI;

[Serializable]
public class Rounds {
    [SerializeField]
    public List<Round> rounds;

}

[Serializable]
public class Round {

    [SerializeField]
    public string roundName;

    [SerializeField]
    public List<GameObject> DragonsToSpawn;
}

[Serializable]
public class SpawnPoint {
    [SerializeField]
    public Vector3 SpawnHere;
}

public class SpawnPointsTracker {
    public SpawnPointsTracker (int ind) {
        index = ind;
    }
    public int index;
    public Boolean locked = false;

    public string dragonId;

    public Vector3 SpawnHere;
}

public class GameController : MonoBehaviour {

    [HideInInspector]
    static public GameController Instance { get; private set; }

    public Rounds round;

    private GameObject[] spawnPoints;

    private SpawnPointsTracker[] spawnPointTrackers;

    public GameObject titleScreenUI;
    public GameObject gameScreenUI;

    public GameObject endScreenUI;

    private PoolManager _pool { get { return PoolManager.Instance; } }

    public GameObject redFruit;
    public GameObject blueFruit;
    public GameObject yellowFruit;

    public Texture2D mouseHandImage;

    private GameObject _player;

    private GameObject releaseDragonButton;
    private bool allDragonsSpawned = false;

    //Present Drop System Variables
    public float presentDropChance = 25f;
    public GameObject[] presentPool;
    public GameObject presentBtn, newPresentBtn;
    public Image present1, present2, present3;

    private int finalScore = 0;

    private void Awake () {

        Instance = this;

        releaseDragonButton = GameObject.FindGameObjectWithTag ("ReleaseDragonButton");
        gameScreenUI.SetActive (false);
        endScreenUI.SetActive (false);

        _player = GameObject.FindGameObjectWithTag ("Player");
        spawnPoints = GameObject.FindGameObjectsWithTag ("Spawn Point");
        spawnPointTrackers = new SpawnPointsTracker[spawnPoints.Length - 1];

        for (int i = 0; i < spawnPointTrackers.Length; i++) {
            spawnPointTrackers[i] = new SpawnPointsTracker (i);
            spawnPointTrackers[i].SpawnHere = spawnPoints[i].transform.position;
            spawnPointTrackers[i].dragonId = (Random.Range (0, 100) + Random.Range (0, 100) + Random.Range (0, 100)).ToString ();
        }

    }

    void Update () {

        if (GameState.Instance.IsCurrentStateTitle () && Input.GetMouseButton (0)) {
            GameState.Instance.ChangeState_Play ();
            ToggleGameplayUI ();
            StartCoroutine (SpawnDragonsWhileGameIsRunning (3.0f));
        }

    }

    public void TurnOnReleaseButton () {
        releaseDragonButton.SetActive (true);
    }

    public void TurnOffReleaseButton () {
        releaseDragonButton.SetActive (false);
    }

    public void ShouldPresentDrop () {

        //drop present
        float dropChance = Random.Range (0f, 100f);
        //        Debug.Log ("Is " + dropChance + " < " + presentDropChance + "?");

        if (dropChance < presentDropChance) {
            //            Debug.Log ("Present Dropped!");
            int randomItem = Random.Range (0, presentPool.Length);
            //            Debug.Log (randomItem);
            presentBtn.SetActive (false);
            newPresentBtn.SetActive (true);

            switch (randomItem) {
                case 0:
                    present1.color = new Color (present1.color.r, present1.color.g, present1.color.b, 1f);
                    //                    Debug.Log (presentPool[randomItem] + " Item 0");

                    break;

                case 1:
                    present2.color = new Color (present2.color.r, present2.color.g, present2.color.b, 1f);
                    //   Debug.Log (presentPool[randomItem] + " Item 1");
                    break;

                case 2:
                    present3.color = new Color (present3.color.r, present3.color.g, present3.color.b, 1f);
                    //     Debug.Log (presentPool[randomItem] + " Item 2");
                    break;
            }

            //Instantiate(presentPool[randomItem], transform.position, transform.rotation);
        } else {
            Debug.Log ("Sorry no present this time =/");
        }

    }

    public void clearNewPresentIdicator () {
        newPresentBtn.SetActive (false);
        presentBtn.SetActive (true);
    }

    private IEnumerator SpawnDragonsWhileGameIsRunning (float waitTime) {
        yield return new WaitForSeconds (2.0F);
        foreach (Round round in round.rounds) {

            while (GameObject.FindGameObjectsWithTag ("Dragon").Length != 0) {
                yield return new WaitForSeconds (2.0F);
            }

            foreach (GameObject dragon in round.DragonsToSpawn) {
                SpawnDragon (dragon);
            }
        }

        allDragonsSpawned = true;
        Debug.Log (" ** DONE SPAWNING DRAGONS **");
    }

    private void SpawnDragon (GameObject dragonPrefab) {
        SpawnPointsTracker location = getRandomDragonSpawnLocation ();
        var dragon = _pool.spawnObject (dragonPrefab, location.SpawnHere, Quaternion.identity);

        dragon.GetComponent<Dragon> ().dragonId = location.dragonId;
        dragon.GetComponent<Dragon> ().ResetDragon ();
        dragon.GetComponent<Dragon> ().StartDragon ();
    }

    public void ReleaseDragon () {
        var dragon = _player.GetComponent<PlayerController> ().GetCurrentDragon ();

        if (dragon != null) {

            var id = dragon.GetComponent<Dragon> ().dragonId;
            IncrementFinalScore (dragon.GetComponent<Dragon> ().CaluclateFinalScore ());

            foreach (SpawnPointsTracker s in spawnPointTrackers) {
                if (s.dragonId == id) {
                    s.locked = false;
                }
            }

            _pool.releaseObject (dragon);
            _player.GetComponent<PlayerController> ().ReleaseDragon ();
            TurnOffReleaseButton ();

            if (isGameOver ()) {
                GameState.Instance.ChangeState_End ();
                ToggleEndUI ();
            }

        } else {
            Debug.Log ("No dragon to release");
        }

    }

    public void CleanupPoop () {
        var dragon = _player.GetComponent<PlayerController> ().GetCurrentDragon ();

        if (dragon != null && dragon.GetComponent<Dragon> ().NeedToCleanupPoop ()) {
            dragon.GetComponent<Dragon> ().CleanupPoop ();
        }
    }

    public void HealDragon () {
        var dragon = _player.GetComponent<PlayerController> ().GetCurrentDragon ();

        if (dragon != null) {
            dragon.GetComponent<Dragon> ().HealDragon ();
        }
    }

    private bool isGameOver () {
        return allDragonsSpawned && !spawnPointTrackers.Where (x => x.locked == true).Any ();
    }

    private SpawnPointsTracker getRandomDragonSpawnLocation () {

        int breakCounter = 0;
        Boolean placed = false;
        while (!placed && breakCounter < 100) {

            breakCounter++;

            int index = Random.Range (0, spawnPointTrackers.Length - 1);
            if (!spawnPointTrackers[index].locked) {
                spawnPointTrackers[index].locked = true;
                return spawnPointTrackers[index];
            }

        }

        Debug.LogError ("Not enough spawn points!");
        return new SpawnPointsTracker (0);
    }

    SpawnPoint getSpawnPoint (Vector3 point) {
        return new SpawnPoint () { SpawnHere = point };
    }

    public void ClickRedFoodButton () {
        spawnFruit (redFruit);
    }

    public void ClickBlueFoodButton () {
        spawnFruit (blueFruit);
    }

    public void ClickYellowFoodButton () {
        spawnFruit (yellowFruit);
    }

    private void spawnFruit (GameObject prefab) {
        var fruit = _pool.spawnObject (prefab, _player.transform.position + new Vector3 (0, 1.0F, 0.0F), Quaternion.identity);
        fruit.GetComponent<Rigidbody> ().AddForce (Camera.main.transform.forward * Random.Range (10.0F, 12.0F), ForceMode.Impulse);
    }

    public void IncrementFinalScore (int value) {
        finalScore += value;
    }

    public int GetFinalScore () {
        return finalScore;
    }

    public void ToggleGameplayUI () {
        titleScreenUI.SetActive (false);
        gameScreenUI.SetActive (true);
        releaseDragonButton.SetActive (false);
    }

    public void ToggleEndUI () {
        titleScreenUI.SetActive (false);
        gameScreenUI.SetActive (false);
        endScreenUI.SetActive (true);
        endScreenUI.GetComponentInChildren<Text> ().text = "Thanks for dragon-sitting! Final Score - " + GetFinalScore ().ToString ();
    }

    public GameObject SpawnObject (GameObject obj) {
        return _pool.spawnObject (obj, new Vector3 (0, 0, 0), Quaternion.identity);
    }

    public void ReleaseObject (GameObject obj) {
        _pool.releaseObject (obj);
    }

}