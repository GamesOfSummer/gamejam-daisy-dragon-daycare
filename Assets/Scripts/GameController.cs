using System;
using System.Collections;
using System.Collections.Generic;
using PKG;
using UnityEngine;
using Random = UnityEngine.Random;
using System.Linq;
using UnityEngine.UI;

[Serializable]
public class Rounds
{
	[SerializeField]
	public List<Round> rounds;

}

[Serializable]
public class Round
{

	[SerializeField]
	public string roundName;

	[SerializeField]
	public List<GameObject> DragonsToSpawn;
}

[Serializable]
public class SpawnPoint
{
	[SerializeField]
	public Vector3 SpawnHere;
}

public class SpawnPointsTracker
{
	public SpawnPointsTracker(int ind)
	{
		index = ind;
	}
	public int index;
	public Boolean locked = false;

	public Boolean wasUsedLastRound = false;

	public string dragonId;

	public Vector3 SpawnHere;
}

public class GameController : MonoBehaviour
{

	[HideInInspector]
	static public GameController Instance { get; private set; }

	public Rounds round;

	private GameObject[] spawnPoints;

	private SpawnPointsTracker[] spawnPointTrackers;

	public GameObject titleScreenUI;
	public GameObject gameScreenUI;
	public GameObject tutorialScreenUIPg1, tutorialScreenUIPg2;

	public GameObject endScreenUI;

	private PoolManager _pool { get { return PoolManager.Instance; } }

	public GameObject pizzaFood;
	public GameObject pepperFood;
	public GameObject berriesFood;

	public Texture2D mouseHandImage;

	private GameObject _player;

	private bool allDragonsSpawned = false;

	bool tutorialActive1 = false;
	bool tutorialActive2 = false;

	//Present Drop System Variables
	public float presentDropChance = 25f;
	public GameObject[] presentPool;
	public GameObject presentBtn, newPresentBtn;
	public Image present1, present2, present3;

	private int finalScore = 0;

	private void Awake()
	{
		Instance = this;

		_player = GameObject.FindGameObjectWithTag("Player");
		spawnPoints = GameObject.FindGameObjectsWithTag("Spawn Point");
		spawnPointTrackers = new SpawnPointsTracker[spawnPoints.Length - 1];

		for (int i = 0; i < spawnPointTrackers.Length; i++)
		{
			spawnPointTrackers[i] = new SpawnPointsTracker(i);
			spawnPointTrackers[i].SpawnHere = spawnPoints[i].transform.position;
			spawnPointTrackers[i].dragonId = (Random.Range(0, 100) + Random.Range(0, 100) + Random.Range(0, 100)).ToString();
		}

		StartGameUI();
	}

	void Update()
	{
		if (GameState.Instance.IsCurrentStateTitle() && Input.GetMouseButton(0))
		{
			if (tutorialActive1 != true && tutorialActive2 != true)
			{
				GameState.Instance.ChangeState_Play();
				Debug.Log(tutorialActive1 + " " + tutorialActive2);
				ToggleGameplayUI();
				StartCoroutine(SpawnDragonsWhileGameIsRunning(3.0f));
			}
		}

		if (Input.GetKeyDown(KeyCode.H))
		{
			if (!GameState.Instance.IsCurrentStatePlay())
			{
				Debug.Log("H key pressed");
				if (tutorialActive1 == false)
				{
					tutorialScreenUIPg1.SetActive(true);
					tutorialActive1 = true;
					Time.timeScale = 0f;
				}
				else if (tutorialActive1 == true || tutorialActive2 == true)
				{
					tutorialScreenUIPg1.SetActive(false);
					tutorialScreenUIPg2.SetActive(false);
					tutorialActive1 = false;
					Time.timeScale = 1f;
				}
			}
		}
		if (Input.GetKeyDown(KeyCode.N))
		{
			Debug.Log("N key pressed");
			if (tutorialActive2 == false && tutorialActive1 == true)
			{
				tutorialScreenUIPg1.SetActive(false);
				tutorialScreenUIPg2.SetActive(true);
				tutorialActive2 = true;
			}
		}

		if (Input.GetKeyDown(KeyCode.P))
		{
			if (tutorialActive2 == true)
			{
				tutorialScreenUIPg1.SetActive(true);
				tutorialScreenUIPg2.SetActive(false);
				tutorialActive2 = false;
			}
		}

	}

	public void ShouldPresentDrop()
	{

		//drop present
		float dropChance = Random.Range(0f, 100f);
		//        Debug.Log ("Is " + dropChance + " < " + presentDropChance + "?");

		if (dropChance < presentDropChance)
		{
			//            Debug.Log ("Present Dropped!");
			int randomItem = Random.Range(0, presentPool.Length);
			//            Debug.Log (randomItem);
			presentBtn.SetActive(false);
			newPresentBtn.SetActive(true);

			switch (randomItem)
			{
				case 0:
					present1.color = new Color(present1.color.r, present1.color.g, present1.color.b, 1f);
					//                    Debug.Log (presentPool[randomItem] + " Item 0");

					break;

				case 1:
					present2.color = new Color(present2.color.r, present2.color.g, present2.color.b, 1f);
					//   Debug.Log (presentPool[randomItem] + " Item 1");
					break;

				case 2:
					present3.color = new Color(present3.color.r, present3.color.g, present3.color.b, 1f);
					//     Debug.Log (presentPool[randomItem] + " Item 2");
					break;
			}

			//Instantiate(presentPool[randomItem], transform.position, transform.rotation);
		}
		else
		{
			Debug.Log("Sorry no present this time =/");
		}

	}

	public void clearNewPresentIdicator()
	{
		newPresentBtn.SetActive(false);
		presentBtn.SetActive(true);
	}

	private IEnumerator SpawnDragonsWhileGameIsRunning(float waitTime)
	{
		yield return new WaitForSeconds(3.0F);
		foreach (Round round in round.rounds)
		{

			while (GameObject.FindGameObjectsWithTag("Dragon").Length != 0)
			{
				yield return new WaitForSeconds(2.0F);
			}

			foreach (GameObject dragon in round.DragonsToSpawn)
			{
				SpawnDragon(dragon);
			}
		}

		allDragonsSpawned = true;
		Debug.Log(" ** DONE SPAWNING DRAGONS **");
	}

	private void SpawnDragon(GameObject dragonPrefab)
	{
		SpawnPointsTracker location = getRandomDragonSpawnLocation();
		var dragon = _pool.spawnObject(dragonPrefab, location.SpawnHere, Quaternion.identity);

		dragon.GetComponent<Dragon>().dragonId = location.dragonId;
		dragon.GetComponent<Dragon>().ResetDragon();
		dragon.GetComponent<Dragon>().StartDragon();
	}

	public void ReleaseDragon(Dragon dragon)
	{
		//var dragon = _player.GetComponent<PlayerController> ().GetCurrentDragon ();

		if (dragon != null && dragon.GetComponent<Dragon>().canBeReleased())
		{

			var id = dragon.GetComponent<Dragon>().dragonId;
			IncrementFinalScore(dragon.GetComponent<Dragon>().CaluclateFinalScore());

			foreach (SpawnPointsTracker s in spawnPointTrackers)
			{
				if (s.dragonId == id)
				{
					s.locked = false;
				}
			}

			_pool.releaseObject(dragon.gameObject);
			_player.GetComponent<PlayerController>().ReleaseDragon();

			if (isGameOver())
			{
				GameState.Instance.ChangeState_End();
				ToggleEndUI();
				ResetGameStartCoRoutine();
			}
		}
		else
		{
			Debug.Log("No dragon to release");
		}
	}

	public void CleanupPoop()
	{
		var dragon = _player.GetComponent<PlayerController>().GetCurrentDragon();

		if (dragon != null && dragon.GetComponent<Dragon>().NeedToCleanupPoop())
		{
			dragon.GetComponent<Dragon>().CleanupPoop();
		}
	}

	public void HealDragon()
	{
		var dragon = _player.GetComponent<PlayerController>().GetCurrentDragon();

		if (dragon != null)
		{
			dragon.GetComponent<Dragon>().HealDragon();
		}
	}

	private bool isGameOver()
	{
		return allDragonsSpawned && !spawnPointTrackers.Where(x => x.locked == true).Any();
	}

	private SpawnPointsTracker getRandomDragonSpawnLocation()
	{
		int breakCounter = 0;
		Boolean placed = false;
		while (!placed && breakCounter < 200)
		{

			//reset all the points if they have all been used!
			if (spawnPointTrackers.All(x => x.wasUsedLastRound == true))
			{
				foreach (SpawnPointsTracker s in spawnPointTrackers)
				{
					s.wasUsedLastRound = false;
				}
			};

			breakCounter++;

			int index = Random.Range(0, spawnPointTrackers.Length - 1);
			if (!spawnPointTrackers[index].locked || !spawnPointTrackers[index].wasUsedLastRound)
			{
				spawnPointTrackers[index].locked = true;
				spawnPointTrackers[index].wasUsedLastRound = true;
				return spawnPointTrackers[index];
			}

		}

		Debug.LogError("Not enough spawn points!");
		return new SpawnPointsTracker(0);
	}

	SpawnPoint getSpawnPoint(Vector3 point)
	{
		return new SpawnPoint() { SpawnHere = point };
	}

	public void ClickPizzaButton()
	{
		spawnFruit(pizzaFood);
	}

	public void ClickPepperButton()
	{
		spawnFruit(pepperFood);
	}

	public void ClickBerryButton()
	{
		spawnFruit(berriesFood);
	}

	public float RandomThrowArc = 10f;

	private void spawnFruit(GameObject prefab)
	{
		var fruit = _pool.spawnObject(prefab, _player.transform.position + new Vector3(0f, 0.0f, 0.0f), Quaternion.identity);
		var dir = Camera.main.transform.forward;
		dir = Quaternion.Euler(Random.Range(-RandomThrowArc, RandomThrowArc), Random.Range(-RandomThrowArc, RandomThrowArc), Random.Range(-RandomThrowArc, RandomThrowArc)) * dir;
		fruit.GetComponent<Rigidbody>().AddForce(dir * Random.Range(10.0F, 12.0F), ForceMode.Impulse);
	}

	public void IncrementFinalScore(int value)
	{
		finalScore += value;
	}

	public int GetFinalScore()
	{
		return finalScore;
	}

	public void ToggleGameplayUI()
	{
		titleScreenUI.SetActive(false);
		gameScreenUI.SetActive(true);

	}

	public void ToggleEndUI()
	{
		titleScreenUI.SetActive(false);
		gameScreenUI.SetActive(false);
		endScreenUI.SetActive(true);
		endScreenUI.GetComponentInChildren<Text>().text = "Thanks for dragon-sitting! Final Score - " + GetFinalScore().ToString();
	}

	public void ResetGameStartCoRoutine()
	{
		StartCoroutine(ResetGameCoroutine());
	}

	private IEnumerator ResetGameCoroutine()
	{
		yield return new WaitForSeconds(3.0F);
		endScreenUI.GetComponentInChildren<Text>().text = "Resetting game - 3 - try again!";
		yield return new WaitForSeconds(1.0F);
		endScreenUI.GetComponentInChildren<Text>().text = "Resetting game - 2 - try again!";
		yield return new WaitForSeconds(1.0F);
		endScreenUI.GetComponentInChildren<Text>().text = "Resetting game - 1 - try again!";
		yield return new WaitForSeconds(1.0F);

		GameState.Instance.ChangeState_Start();
		StartGameUI();
	}

	private void StartGameUI()
	{

		tutorialScreenUIPg1.SetActive(false);
		tutorialScreenUIPg2.SetActive(false);
		tutorialActive1 = false;
		tutorialActive2 = false;

		titleScreenUI.SetActive(true);
		gameScreenUI.SetActive(false);
		endScreenUI.SetActive(false);

		titleScreenUI.SetActive(true);
		gameScreenUI.SetActive(false);
		endScreenUI.SetActive(false);

		titleScreenUI.SetActive(true);
		gameScreenUI.SetActive(false);
		endScreenUI.SetActive(false);
	}

	public GameObject SpawnObject(GameObject obj)
	{
		return _pool.spawnObject(obj, new Vector3(0, 0, 0), Quaternion.identity);
	}

	public void ReleaseObject(GameObject obj)
	{
		_pool.releaseObject(obj);
	}

	public void ResetMouseCursor()
	{
		Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
	}
}