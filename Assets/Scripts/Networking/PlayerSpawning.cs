﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerSpawning : Photon.PunBehaviour {

    public static PlayerSpawning Instance;
    private PhotonView PhotonView;
    private GameObject defaultSpawnPoint;
    public static GameObject localPlayer;
    private int PlayersInGame = 0;

    // Use this for initialization
    void Awake () {
        Instance = this;
        PhotonView = GetComponent<PhotonView>();

        PhotonNetwork.sendRate = 60;
        PhotonNetwork.sendRateOnSerialize = 30;
        
    }
    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneFinishedLoading;
    }
    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneFinishedLoading;
    }
    private void OnSceneFinishedLoading(Scene scene, LoadSceneMode mode)
    {
        Scene currentScene = SceneManager.GetActiveScene();
        if (scene.name == currentScene.name)
        {
            if (PhotonNetwork.isMasterClient)
                MasterLoadedGame();
            else
                NonMasterLoadedGame();
        }
    }
    private void MasterLoadedGame()
    {
        PhotonView.RPC("RPC_LoadedGameScene", PhotonTargets.MasterClient, PhotonNetwork.player);
        //PhotonView.RPC("RPC_LoadGameOthers", PhotonTargets.Others);
    }
    private void NonMasterLoadedGame()
    {
        //PhotonView.RPC("RPC_LoadedGameScene", PhotonTargets.MasterClient, PhotonNetwork.player);

    }

    //[PunRPC]
    //private void RPC_LoadGameOthers()
    //{
    //    PhotonNetwork.LoadLevel(1);
    //}

    [PunRPC]
    private void RPC_LoadedGameScene(PhotonPlayer player)
    {
        PhotonView.RPC("RPC_CreatePlayer", PhotonTargets.All);
    }

    [PunRPC]
    private void RPC_CreatePlayer()
    {
        var spawnPoint = GetRandomSpawnPoint();
        localPlayer = PhotonNetwork.Instantiate("Player", spawnPoint.position, spawnPoint.rotation, 0);
        Debug.Log(localPlayer.GetPhotonView().owner);
        
        // Setup UI
        PlayerUI ui = FindObjectOfType<PlayerUI>();
        ui.SetupUI();

        // HACK
        PlayerStats ps = localPlayer.GetComponent<PlayerStats>();
        ps.Invulnerable = true;
        ps.CanRespawn = true;
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
    // Update is called once per frame
    
}
