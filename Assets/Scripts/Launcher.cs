using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

namespace Jogo.FPS.Multiplayer
{
    public class Launcher : MonoBehaviourPunCallbacks
    {
        bool isConnecting;

        private void Awake()
        {
            // #NotImportant
            // Force Full LogLevel
            PhotonNetwork.LogLevel = PunLogLevel.Informational;

            // #Critical
            // this makes sure we can use PhotonNetwork.Loadlevel() on the master client and all clients in the same room synch their level automatically
            PhotonNetwork.AutomaticallySyncScene = true;
        }

        public void Start()
        {
            PhotonNetwork.AutomaticallySyncScene = true;
            Connect();
        }

        public void Connect()
        {
            Debug.Log("Connecting...");
            PhotonNetwork.GameVersion = "0.0.0";

            isConnecting = true;
            if (PhotonNetwork.IsConnected)
                PhotonNetwork.JoinRandomRoom();
            else
                PhotonNetwork.ConnectUsingSettings();
        }

        public override void OnConnectedToMaster()
        {
            if (isConnecting)
                PhotonNetwork.JoinRandomRoom();
        }

        public override void OnJoinRandomFailed(short x, string y)
        {
            PhotonNetwork.CreateRoom(null);
        }
    }
}