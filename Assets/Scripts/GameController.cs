using System;
using System.Collections;
using System.Collections.Generic;
using PKG;
using UnityEngine;
using Random = UnityEngine.Random;
using System.Linq;

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

    [SerializeField]
    public float TimeToLastInSeconds;
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
    private PoolManager _pool { get { return PoolManager.Instance; } }

    public GameObject redFruit;

    private GameObject _player;

    private void Awake () {
        Instance = this;
        _player = GameObject.FindGameObjectWithTag ("Player");
        spawnPoints = GameObject.FindGameObjectsWithTag ("Spawn Point");
        spawnPointTrackers = new SpawnPointsTracker[spawnPoints.Length - 1];

        for (int i = 0; i < spawnPointTrackers.Length; i++) {
            spawnPointTrackers[i] = new SpawnPointsTracker (i);
            spawnPointTrackers[i].SpawnHere = spawnPoints[i].transform.position;
            spawnPointTrackers[i].dragonId = (Random.Range (0, 100) + Random.Range (0, 100) + Random.Range (0, 100)).ToString ();
        }

    }

    void Start () {
        StartCoroutine (SpawnDragonsWhileGameIsRunning (1.0f));
    }

    // Update is called once per frame
    void Update () {

    }

    private IEnumerator SpawnDragonsWhileGameIsRunning (float waitTime) {

        yield return new WaitForSeconds (2.0F);

        int counter = 1;
        foreach (Round round in round.rounds) {

            Debug.Log (counter + " - Spawning dragons - > " + round.roundName);

            foreach (GameObject dragon in round.DragonsToSpawn) {
                SpawnDragon (dragon);
            }

            yield return new WaitForSeconds (round.TimeToLastInSeconds);
        }

        Debug.Log (" ** DONE SPAWNING DRAGONS **");

    }

    private void SpawnDragon (GameObject dragonPrefab) {
        SpawnPointsTracker location = getRandomDragonSpawnLocation ();
        var dragon = _pool.spawnObject (dragonPrefab, location.SpawnHere, Quaternion.identity);
        dragon.GetComponent<Dragon> ().dragonId = location.dragonId;
    }

    public void ReleaseDragon () {
        var dragon = _player.GetComponent<PlayerController> ().GetCurrentDragon ();

        if (dragon != null) {
            Debug.Log ("Releasing dragon");

            var id = dragon.GetComponent<Dragon> ().dragonId;

            foreach (SpawnPointsTracker s in spawnPointTrackers) {
                if (s.dragonId == id) {
                    s.locked = false;
                    //Debug.Log ("SpawnPointsTracker point unlocked");
                }
            }

            _pool.releaseObject (dragon);
            _player.GetComponent<PlayerController> ().ReleaseDragon ();

            Debug.Log ("isGameOver ()" + isGameOver ());

            if (isGameOver ()) {
                GameState.Instance.ChangeState_End ();
            }

        } else {
            Debug.Log ("No dragon to release");
        }

    }

    private bool isGameOver () {
        return !spawnPointTrackers.Where (x => x.locked == true).Any ();
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

    public void ClickFoodButton () {

        Debug.Log ("clicked");
        var fruit = _pool.spawnObject (redFruit, _player.transform.position + new Vector3 (0, 0, 1.0F), Quaternion.identity);
    }

}