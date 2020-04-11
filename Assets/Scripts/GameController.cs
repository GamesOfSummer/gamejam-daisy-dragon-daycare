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

    [SerializeField]
    public Vector2 DirectionToShoot;
}

[Serializable]
public class Round {
    [SerializeField]
    public List<GameObject> DragonsToSpawn;

    [SerializeField]
    public float TimeToLast;
}

[Serializable]
public class SpawnPoint {
    [SerializeField]
    public Vector3 SpawnHere;
    [SerializeField]
    public Vector2 DirectionToShoot;
}

public class GameController : MonoBehaviour {

    [HideInInspector]
    static public GameController Instance { get; private set; }

    public Rounds rounds;

    public GameObject[] spawnPoints;
    public GameObject starPrefab;
    private PoolManager _pool { get { return PoolManager.Instance; } }

    private void Awake () {
        Instance = this;
        spawnPoints = GameObject.FindGameObjectsWithTag ("Spawn Point");
    }

    // Start is called before the first frame update
    void Start () {
        StartCoroutine (SpawnStarsWhileGameIsRunning (1.0f));
    }

    // Update is called once per frame
    void Update () {

    }

    private IEnumerator SpawnStarsWhileGameIsRunning (float waitTime) {

        yield return new WaitForSeconds (2.0F);
        // if (GameObject.FindGameObjectsWithTag ("Item").Length >= (Mathf.Clamp (PlayerManager.Instance.PlayerCount, 0, 4) + 3))
        //     continue;
        var star = SpawnStar ();
        StartCoroutine (Release (star));

    }

    private GameObject SpawnStar () {
        SpawnPoint location = getRandomStarSpawnLocation ();
        var star = _pool.spawnObject (starPrefab, location.SpawnHere, Quaternion.identity);
        star.GetComponentInChildren<SpriteRenderer> ().enabled = true;
        star.GetComponent<TrailRenderer> ().enabled = true;
        star.GetComponent<ParticleSystem> ().Play ();

        var forceVariance = new Vector2 (
            location.DirectionToShoot.x + Random.Range (-1.0F, 1.0F),
            location.DirectionToShoot.y + Random.Range (-1.0F, 1.0F)
        );

        star.GetComponent<Rigidbody2D> ().AddForce (forceVariance);
        // star.GetComponent<Star> ().itemToSpawn = ReturnItem ();
        return star;
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

    private SpawnPoint getRandomStarSpawnLocation () {
        //GameObject go = this.spawnPoints.GetRandom ();
        return getSpawnPoint (Vector3.zero);
    }

    SpawnPoint getSpawnPoint (Vector3 point) {
        // GameObject planet;
        // Vector2 direction = (planet.transform.position - point).normalized;
        return new SpawnPoint () { SpawnHere = point };
    }

}