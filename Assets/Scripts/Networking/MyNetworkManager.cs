using Assets.Scripts.Game;
using Assets.Scripts.Models;
using Mirror;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MyNetworkManager : NetworkManager
{
    [SerializeField] private GameOverHandler gameOverHandler = null;

    public bool IsGameInProgress = false;
    public List<DominoPlayer> Players = new List<DominoPlayer>();

    public static event Action ClientOnConnected;
    public static event Action ClientOnDisconnected;

    public void SetTransport(Transport t) => transport = t;

    #region Server

    public override void OnServerAddPlayer(NetworkConnectionToClient conn)
    {
        base.OnServerAddPlayer(conn);

        DominoPlayer player = conn.identity.GetComponent<DominoPlayer>();
        player.ID = (int)conn.identity.netId;
        Players.Add(player);

        player.SetDisplayName($"Player {Players.Count}");
        player.SetPartyOwner(Players.Count == 1);
    }

    public override void OnServerConnect(NetworkConnectionToClient conn)
    {
        if (!IsGameInProgress)
        {
            return;
        }

        conn.Disconnect();
    }

    public void StartGame()
    {
        if (Players.Count < 2)
        {
            return;
        }

        IsGameInProgress = true;

        ServerChangeScene("Main_Scene");
    }

    public override void OnServerSceneChanged(string sceneName)
    {
        if (SceneManager.GetActiveScene().name.StartsWith("Main_Scene"))
        {
            GameOverHandler gameOverHandlerInstance = Instantiate(gameOverHandler);
            NetworkServer.Spawn(gameOverHandlerInstance.gameObject);

            //gameSessionInstance = Instantiate(gameSessionPrefab);

        }
    }

    public override void OnServerDisconnect(NetworkConnectionToClient conn)
    {
        DominoPlayer player = conn.identity.GetComponent<DominoPlayer>();

        Players.Remove(player);

        base.OnServerDisconnect(conn);
    }

    public override void OnStopServer()
    {
        Players.Clear();

        IsGameInProgress = false;
    }

    #endregion


    #region Client

    public override void OnClientConnect()
    {
        base.OnClientConnect();
        ClientOnConnected?.Invoke();
    }

    public override void OnClientDisconnect()
    {
        base.OnClientDisconnect();
        ClientOnDisconnected?.Invoke();
    }

    public override void OnStopClient()
    {
        Players.Clear();
    }


    #endregion
}
