using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class Gamemanager : MonoBehaviour
{
	public static Gamemanager instance;

	//Spawning
	private Trash _trash = new Trash();

	//keeps track of all the different objects that can been spawn
	private  Dictionary<Guid, GameObject> trashObjects = new Dictionary<Guid, GameObject>();

	//the queue of witch the trash has come in
	private  Queue<Trash> TrashQueue = new Queue<Trash>();

	//Player settings
	private int _currentLives = 3;

	private GameObject[] _obstacleObjects;
	private float _sceneSwitchTime;

	//trash objects to spawn
	private GameObject[] _trashObject;
	private float basetime;

	//obstacle objects to spawn

	[SerializeField] private int chanceToSpawn;

	[SerializeField] private Transform[] spawmLocations = new Transform[3];
	public Vector3 spawnpoint2;

	[Tooltip("time in seconds")] public float spawnRate;

	//World settings
	[SerializeField] private GameObject world;
	[SerializeField] [Range(20, 70)] private float worldSpeed;


	private void Awake()
	{
		if (instance == null)
		{
			instance = this;
			DontDestroyOnLoad(this);
			basetime = spawnRate;
			_trash.GetFirstTime(spawnRate);
			SetTrashObjects();
			SetObstacleObjects();
		}
		else
		{
			Destroy(this);
		}
	}

	private void FixedUpdate()
	{
		if (SceneManager.GetActiveScene().buildIndex == 2)
		{
			//Debug.Log("spawning");
			//SpawnBasedOnTime();
			TempSpawn();
		}
		else
		{
			SpinWorld();
			SpawnObstacles();
		}

	}

	public void LifeTracking(GameObject col)
	{
		_currentLives--;
		Destroy(col);
		Debug.Log(_currentLives);
		if (_currentLives != 0) return;
		SceneManager.LoadScene(2);
		_sceneSwitchTime = Time.time;
	}

	private void TrashSetTime(GameObject t)
	{
		var x = t.GetComponent<TrashConfig>();
		x._trash.SetTime();
		Debug.Log(_trash.pickUpTime);
	}

	public void AddTrash(GameObject t)
	{
		var x = t.GetComponent<TrashConfig>();
		x._trash.SetTime();
		TrashQueue.Enqueue(x._trash);
		Debug.Log("adding trash");
		Destroy(t);
	}

	private void SpinWorld()
	{
		world.transform.Rotate(0, worldSpeed * Time.deltaTime, 0, Space.Self);
	}


	private void SpawnObstacles()
	{
		spawnRate -= 1 * Time.deltaTime;

		if (!(spawnRate <= 0)) return;
		var k = Random.Range(0, 200);
		var i = Random.Range(0, spawmLocations.Length);

		if (k < chanceToSpawn)
		{
			var j = Random.Range(0, _trashObject.Length);
			var obs = Instantiate(_trashObject[j], spawmLocations[i].position, Quaternion.identity);
			obs.transform.SetParent(world.transform, true);
			TrashSetTime(obs);
		}
		else if (k > chanceToSpawn)
		{
			var j = Random.Range(0, _obstacleObjects.Length);
			var obs = Instantiate(_obstacleObjects[j], spawmLocations[i].position, Quaternion.identity);
			obs.transform.SetParent(world.transform, true);
		}
		else
		{
			Debug.LogError("Object index out of range");
			//EditorApplication.isPlaying = false;
		}

		spawnRate += basetime;
	}

	private void SetTrashObjects()
	{
		var allObj = Resources.LoadAll("TrashPrefabs/", typeof(GameObject));
		_trashObject = new GameObject[allObj.Length];

		foreach (GameObject obj in allObj)
		{
			var uid = obj.GetComponent<TrashConfig>().Generateuid();
			trashObjects.Add(uid, obj);
			Debug.Log(uid.ToString());
		}

		for (var i = 0; i < allObj.Length; i++) _trashObject[i] = allObj[i] as GameObject;
	}

	private void SetObstacleObjects()
	{
		//load the assets in
		var allObs = Resources.LoadAll("ObstaclesPrefabs/", typeof(GameObject));

		//SetLength Array
		_obstacleObjects = new GameObject[allObs.Length];

		//assign the objects to the array
		for (var i = 0; i < allObs.Length; i++) _obstacleObjects[i] = allObs[i] as GameObject;
	}

	private void SpawnBasedOnTime()
	{
		Debug.Log("Hello");
		//dequeue the next trash object
		var curTrash = TrashQueue.Dequeue();
		if (curTrash == null)
		{
			Debug.LogError("No object had been picked up");
		}
		//get the right GameObject to spawn
		var curTrashObject = trashObjects[curTrash.GUID];

		if (curTrash.pickUpTime <= Time.time - _sceneSwitchTime)
		{
			GameObject i;
			i = Instantiate(curTrashObject, new Vector3(0, 0, 0), Quaternion.identity) as GameObject;
			i.AddComponent<Rigidbody>().useGravity = true;
		}
	}

	private Transform pos()
	{
		return GameObject.Find("spawnpos").transform;
	}

	/// <summary>
	/// this is used to fake the trash spawning
	/// </summary>
	private void TempSpawn()
	{
		spawnRate -= 1 * Time.deltaTime;
		if (!(spawnRate <= 0)) return;
		var i = Random.Range(0, _trashObject.Length);

		var obj = Instantiate(_trashObject[i], spawnpoint2, Quaternion.identity) as GameObject;
		
		obj.transform.localScale = new Vector3(0.09f,0.09f,0.09f);
		
		obj.AddComponent<Rigidbody>();

		spawnRate += basetime;
		//obj.AddComponent<FixedJoint>().;
	}
}