﻿using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PlayerListing : MonoBehaviour
{
    //Finished
    public PhotonPlayer PhotonPlayer { get; private set; }

    [SerializeField]
    private Text _playerName;

    private Text PlayerName
    {
        get { return _playerName; }
    }

    [SerializeField]
    private Text _playerPing;

    private Text m_playerPing
    {
        get { return _playerPing; }
    }

    public void ApplyPhotonPlayer(PhotonPlayer photonPlayer)
    {
        PhotonPlayer = photonPlayer;
        PlayerName.text = photonPlayer.NickName;

        StartCoroutine(C_ShowPing());
    }

    private IEnumerator C_ShowPing()
    {
        while (PhotonNetwork.connected)
        {
            int ping = PhotonNetwork.GetPing();
            m_playerPing.text = ping.ToString();
            yield return new WaitForSeconds(1f);
        }

        yield break;
    }
}