using System;
using System.Collections;
using System.Collections.Generic;
using PKG;
using UnityEngine;
using Random = UnityEngine.Random;

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
    [SerializeField]
    public Vector2 DirectionToShoot;
}

public class SpawnPointsTracker {
    public SpawnPointsTracker (int ind) {
        index = ind;
    }
    public int index;
    public Boolean locked = false;
}

public class GameController : MonoBehaviour {

    [HideInInspector]
    static public GameController Instance { get; private set; }

    public Rounds round;

    private GameObject[] spawnPoints;

    private SpawnPointsTracker[] spawnPointTrackers;
    private PoolManager _pool { get { return PoolManager.Instance; } }

    private void Awake () {
        Instance = this;
        spawnPoints = GameObject.FindGameObjectsWithTag ("Spawn Point");
        spawnPointTrackers = new SpawnPointsTracker[spawnPoints.Length - 1];

        for (int i = 0; i < spawnPointTrackers.Length; i++) {
            spawnPointTrackers[i] = new SpawnPointsTracker (i);
        }

    }

    // Start is called before the first frame update
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
        SpawnPoint location = getRandomDragonSpawnLocation ();
        var dragon = _pool.spawnObject (dragonPrefab, location.SpawnHere, Quaternion.identity);
    }

    private IEnumerator Release (GameObject star) {
        yield return new WaitForSeconds (4.0F);
        _pool.releaseObject (star);
    }

    private GameObject ReturnItem () {
        // var index = Random.Range (0, items.Count);
        // index = index < 0 ? 0 : index;
        // return items[index];
        return new GameObject ();
    }

    private SpawnPoint getRandomDragonSpawnLocation () {

        int breakCounter = 0;
        Boolean placed = false;
        while (!placed && breakCounter < 100) {

            breakCounter++;

            int index = Random.Range (0, spawnPoints.Length - 1);
            if (!spawnPointTrackers[index].locked) {
                GameObject go = spawnPoints[index];
                spawnPointTrackers[index].locked = true;
                return getSpawnPoint (go.transform.position);
            }

        }

        Debug.LogError ("Not enough spawn points!");
        return new SpawnPoint ();
    }

    SpawnPoint getSpawnPoint (Vector3 point) {
        return new SpawnPoint () { SpawnHere = point };
    }

}