﻿using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace PUNTutorial
{
    public class OLD_GameManager : Photon.PunBehaviour
    {
        public static OLD_GameManager instance;
        public static GameObject localPlayer;
        private GameObject defaultSpawnPoint;

        private void Awake()
        {
            if (instance != null)
            {
                DestroyImmediate(gameObject);
                return;
            }
            DontDestroyOnLoad(gameObject);
            instance = this;

            defaultSpawnPoint = new GameObject("Default SpawnPoint");
            defaultSpawnPoint.transform.position = new Vector3(0, 0.5f, 0);
            defaultSpawnPoint.transform.SetParent(transform, false);

            //PhotonNetwork.automaticallySyncScene = true;
        }

        private void Start()
        {
            //PhotonNetwork.ConnectUsingSettings("0.0.0");
        }

        public void JoinGame()
        {
            RoomOptions ro = new RoomOptions();
            ro.MaxPlayers = 8;
            PhotonNetwork.JoinOrCreateRoom("Default Room", ro, null);
        }
        
        private void OnLevelWasLoaded(int levelNumber)
        {
            if (!PhotonNetwork.inRoom) return;

            var spawnPoint = GetRandomSpawnPoint();
            Scene currentScene = SceneManager.GetActiveScene();
            string sceneName = currentScene.name;
            if (sceneName == "Sandbox")
            {
                localPlayer = PhotonNetwork.Instantiate(
                    "BasicPlayer",
                    spawnPoint.position,
                    spawnPoint.rotation, 0);
            }
        }
        

        public Transform GetRandomSpawnPoint()
        {
            var spawnPoints = GetAllObjectsOfTypeInScene<SpawnPoint>();
            if (spawnPoints.Count == 0)
            {
                return defaultSpawnPoint.transform;
            }
            else
            {
                return spawnPoints[Random.Range(0, spawnPoints.Count)].transform;
            }
        }

        public static List<GameObject> GetAllObjectsOfTypeInScene<T>()
        {
            List<GameObject> objectsInScene = new List<GameObject>();

            foreach (GameObject go in Resources.FindObjectsOfTypeAll(typeof(GameObject))
                     as GameObject[])
            {
                if (go.hideFlags == HideFlags.NotEditable ||
                    go.hideFlags == HideFlags.HideAndDontSave)
                    continue;

                if (go.GetComponent<T>() != null)
                    objectsInScene.Add(go);
            }

            return objectsInScene;
        }
    }
}