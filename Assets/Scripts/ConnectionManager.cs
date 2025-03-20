using UnityEngine;
using Photon;
using Photon.Realtime;
using System.Collections;

public class ConnectionManager : Photon.PunBehaviour
{
    [Header("References")]
    [SerializeField] private UIManager uiManager;

    [Header("Connection Settings")]
    [SerializeField] public byte maxPlayersPerRoom = 2;

    private bool isReconnecting = false;
    private string _gameVersion = "1";

    private void Start()
    {
        // Check for UI Manager reference
        if (uiManager == null)
        {
            uiManager = GetComponent<UIManager>();
            if (uiManager == null)
            {
                Debug.LogError("UIManager reference not set! Please assign it in the inspector.");
                return;
            }
        }

        PhotonNetwork.autoJoinLobby = false;
        PhotonNetwork.automaticallySyncScene = true;
        PhotonNetwork.ConnectUsingSettings(_gameVersion);
    }

    public void JoinGame()
    {
        if (string.IsNullOrEmpty(PhotonNetwork.playerName))
        {
            Debug.LogError("Player name not set!");
            return;
        }

        PhotonNetwork.JoinRandomRoom();
    }

    public void LeaveGame()
    {
        if (PhotonNetwork.inRoom)
        {
            PhotonNetwork.LeaveRoom();
        }
        uiManager.HideLoadingPanel();
        uiManager.ShowPlayerNameInput();
    }

    public void ReconnectAndRejoin()
    {
        if (!isReconnecting)
        {
            isReconnecting = true;
            StartCoroutine(AttemptReconnection());
        }
    }

    private IEnumerator AttemptReconnection()
    {
        int attempts = 0;
        const int maxAttempts = 3;
        const float delayBetweenAttempts = 2f;

        while (attempts < maxAttempts && !PhotonNetwork.connected)
        {
            attempts++;
            uiManager.UpdateReconnectingText($"Reconnecting... Attempt {attempts}/{maxAttempts}");

            PhotonNetwork.ReconnectAndRejoin();
            yield return new WaitForSeconds(delayBetweenAttempts);
        }

        if (!PhotonNetwork.connected)
        {
            uiManager.UpdateReconnectingText("Reconnection failed. Returning to main menu...");
            yield return new WaitForSeconds(2f);
            uiManager.HideReconnecting();
            uiManager.ShowPlayerNameInput();
        }

        isReconnecting = false;
    }

    #region Photon Callbacks
    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected to master server");
    }

    public override void OnJoinedRoom()
    {
        Debug.Log($"Player {PhotonNetwork.playerName} joined room: {PhotonNetwork.room.Name}");

        // Check if we have enough players
        if (PhotonNetwork.room.PlayerCount < maxPlayersPerRoom)
        {
            // Still waiting for players
            uiManager.ShowLoadingPanel("Waiting for opponent...");
        }
        else
        {
            // We have enough players, start the game
            uiManager.HideLoadingPanel();
            uiManager.ShowGame();
        }
    }

    public override void OnPhotonPlayerConnected(PhotonPlayer newPlayer)
    {
        Debug.Log($"Player {newPlayer.NickName} joined the room");

        // Check if we now have enough players
        if (PhotonNetwork.room.PlayerCount >= maxPlayersPerRoom)
        {
            // We have enough players, start the game
            uiManager.HideLoadingPanel();
            uiManager.ShowGame();
        }
    }

    public override void OnPhotonPlayerDisconnected(PhotonPlayer otherPlayer)
    {
        Debug.Log($"Player {otherPlayer.NickName} left the room");
        uiManager.UpdateStatusText($"Player {otherPlayer.NickName} disconnected");
        LeaveGame();
    }

    public override void OnDisconnectedFromPhoton()
    {
        Debug.Log("Disconnected from Photon");
        if (!isReconnecting)
        {
            uiManager.ShowReconnecting();
        }
    }

    public override void OnPhotonRandomJoinFailed(object[] codeAndMsg)
    {
        Debug.Log("No random room available, creating one...");
        RoomOptions roomOptions = new RoomOptions
        {
            MaxPlayers = this.maxPlayersPerRoom,
            IsVisible = true,
            IsOpen = true
        };
        PhotonNetwork.CreateRoom(null, roomOptions, null);
    }
    #endregion
}