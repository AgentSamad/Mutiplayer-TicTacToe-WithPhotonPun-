///-----------------------------------------------------------------
///   Class:          GameStateController
///   Description:    Handles the current state of the game and whos turn it is
///   Author:         VueCode
///   GitHub:         https://github.com/ivuecode/
///-----------------------------------------------------------------
using UnityEngine;
using UnityEngine.UI;
using Photon;
using Photon.Realtime;
using System.Collections;
using Photon;
using TMPro;

public class GameStateController : PunBehaviour
{
    [Header("TitleBar References")]
    public Image playerXIcon;               // Reference to the playerX icon
    public Image           playerOIcon;     // Reference to the playerO icon
    public TextMeshProUGUI statusText;      // Reference to the status text
    public TextMeshProUGUI player1NameText; // Reference to the status text
    public TextMeshProUGUI player2NameText; // Reference to the status text
    [Header("Asset References")]
    public Sprite tilePlayerO;                                       // Sprite reference to O tile
    public Sprite tilePlayerX;                                       // Sprite reference to X tile
    public Sprite tileEmpty;                                         // Sprite reference to empty tile
    public Text[] tileList;                                          // Gets a list of all the tiles in the scene

    [Header("GameState Settings")]
    public Color inactivePlayerColor;                                // Color to display for the inactive player icon
    public Color activePlayerColor;                                  // Color to display for the active player icon
    public string whoPlaysFirst;                                     // Who plays first (X : 0) {NOTE! no checks are made to ensure this is either X or O}

    [Header("Private Variables")]
    private string playerTurn;                                       // Internal tracking whos turn is it
    private string player1Name;                                      // Player1 display name
    private string player2Name;                                      // Player2 display name
    private int moveCount;                                           // Internal move counter
    private bool isGameStarted;
    private bool isLocalPlayerTurn;
    private PhotonView photonView;
    [SerializeField] UIManager uiManager;

    private void Awake()
    {
        photonView = GetComponent<PhotonView>();
        if (photonView == null)
        {
            photonView = gameObject.AddComponent<PhotonView>();
        }
    }

    /// <summary>
    /// Start is called on the first active frame
    /// </summary>
    private void Start()
    {
        // Set the internal tracker of whos turn is first and setup UI icon feedback for whos turn it is
        playerTurn = whoPlaysFirst;
        if (playerTurn == "X") playerOIcon.color = inactivePlayerColor;
        else playerXIcon.color = inactivePlayerColor;

        //Adds a listener to the name input fields and invokes a method when the value changes. This is a callback.
       
        

        // Initialize game state
        isGameStarted = false;
        moveCount = 0;
        ToggleButtonState(false);
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("Joined Room");
        if (PhotonNetwork.room.PlayerCount == 2)
        {
            StartGame();
        }
        else
        {
            UpdateStatusText("Waiting for another player...");
        }
    }

    public override void OnPhotonPlayerConnected(PhotonPlayer newPlayer)
    {
        Debug.Log("Player Connected: " + newPlayer.NickName);
        
        if (newPlayer.GetFinishedTurn() > 0)
        {
            Debug.Log("Player is reconnecting: " + newPlayer.NickName);
            UpdateStatusText("Player reconnected: " + newPlayer.NickName);
        }
        else
        {
            Debug.Log("Player joined for the first time: " + newPlayer.NickName);
            UpdateStatusText("Player joined: " + newPlayer.NickName);
        }

        if (PhotonNetwork.room.PlayerCount == 2)
        {
            StartGame();
        }
    }

    public override void OnPhotonPlayerDisconnected(PhotonPlayer otherPlayer)
    {
        Debug.Log("Player Disconnected: " + otherPlayer.NickName);
        //EndGame("Opponent disconnected!");
        uiManager.ShowLoadingPanel("Waiting for opponent...");
    }

    private void StartGame()
    {
        Debug.Log("Starting Game __  Now hide UI");
        isGameStarted = true;
        moveCount = 0;
        ToggleButtonState(true);
        ResetBoard();

        // Determine if it's local player's turn
        isLocalPlayerTurn = (PhotonNetwork.player.IsMasterClient && whoPlaysFirst == "X") ||
                           (!PhotonNetwork.player.IsMasterClient && whoPlaysFirst == "O");
        player1Name          = PhotonNetwork.player.IsMasterClient ? PhotonNetwork.player.NickName : PhotonNetwork.player.GetNext().NickName;
        player2Name          = PhotonNetwork.player.IsMasterClient ? PhotonNetwork.player.GetNext().NickName : PhotonNetwork.player.NickName;
        player1NameText.text = player1Name;
        player2NameText.text = player2Name;
        UpdateStatusText(isLocalPlayerTurn ? "Your turn!" : "Opponent's turn");
    }

    public void MakeMove(int tileIndex)
    {
        if (!isGameStarted || !isLocalPlayerTurn) return;

        photonView.RPC("UpdateTile",PhotonTargets.All, tileIndex, playerTurn);
        isLocalPlayerTurn = false;
        UpdateStatusText("Opponent's turn");
    }

    [PunRPC]
    private void UpdateTile(int tileIndex, string player)
    {
        Debug.Log("UpdateTile: " + tileIndex + " " + player);
        tileList[tileIndex-1].text = player;
        tileList[tileIndex-1].GetComponentInParent<TileController>().UpdateUI();
        moveCount++;

        if (CheckWinCondition(player))
        {
            EndGame(player);
        }
        else if (moveCount >= 9)
        {
            EndGame("D");
        }
        else
        {
            ChangeTurn();
        }
    }

    private bool CheckWinCondition(string player)
    {
        // Check rows
        if (tileList[0].text == player && tileList[1].text == player && tileList[2].text == player) return true;
        if (tileList[3].text == player && tileList[4].text == player && tileList[5].text == player) return true;
        if (tileList[6].text == player && tileList[7].text == player && tileList[8].text == player) return true;

        // Check columns
        if (tileList[0].text == player && tileList[3].text == player && tileList[6].text == player) return true;
        if (tileList[1].text == player && tileList[4].text == player && tileList[7].text == player) return true;
        if (tileList[2].text == player && tileList[5].text == player && tileList[8].text == player) return true;

        // Check diagonals
        if (tileList[0].text == player && tileList[4].text == player && tileList[8].text == player) return true;
        if (tileList[2].text == player && tileList[4].text == player && tileList[6].text == player) return true;

        return false;
    }

    /// <summary>
    /// Changes the internal tracker for whos turn it is
    /// </summary>
    public void ChangeTurn()
    {
        Debug.Log("Change Icons");
        // This is called a Ternary operator which evaluates "X" and results in "O" or "X" based on truths
        // We then just change some ui feedback like colors.
        playerTurn = (playerTurn == "X") ? "O" : "X";
        if (playerTurn == "X")
        {
            playerXIcon.color = activePlayerColor;
            playerOIcon.color = inactivePlayerColor;
        }
        else
        {
            playerXIcon.color = inactivePlayerColor;
            playerOIcon.color = activePlayerColor;
        }

        isLocalPlayerTurn = (playerTurn == "X" && PhotonNetwork.player.IsMasterClient) ||
                           (playerTurn == "O" && !PhotonNetwork.player.IsMasterClient);

        UpdateStatusText(isLocalPlayerTurn ? "Your turn!" : "Opponent's turn");
    }

    /// <summary>
    /// Called when the game has found a win condition or draw
    /// </summary>
    /// <param name="winningPlayer">X O D</param>
    private void EndGame(string winningPlayer)
    {
        isGameStarted = false;
        ToggleButtonState(false);
        

        switch (winningPlayer)
        {
            case "D":
                uiManager.ShowDraw();
                break;
            case "X":
                if (PhotonNetwork.player.IsMasterClient)
                {
                    uiManager.ShowWin();
                }
                else
                {
                    uiManager.ShowLose();
                }
                break;
            case "O":
                if (PhotonNetwork.player.IsMasterClient)
                {
                    uiManager.ShowWin();
                }
                else
                {
                    uiManager.ShowLose();
                }
                break;
            default:
                uiManager.ShowLose();
                break;
        }
    }

    /// <summary>
    /// Restarts the game state
    /// </summary>
    public void RestartGame()
    {
        if (PhotonNetwork.isMasterClient)
        {
            photonView.RPC("ResetGameState", PhotonTargets.All);
        }
    }

    [PunRPC]
    private void ResetGameState()
    {
        StartGame();
    }

    private void ResetBoard()
    {
        for (int i = 0; i < tileList.Length; i++)
        {
            tileList[i].GetComponentInParent<TileController>().ResetTile();
        }
    }

    /// <summary>
    /// Enables or disables all the buttons
    /// </summary>
    private void ToggleButtonState(bool state)
    {
        for (int i = 0; i < tileList.Length; i++)
        {
            tileList[i].GetComponentInParent<Button>().interactable = state && isLocalPlayerTurn;
        }
    }

    /// <summary>
    /// Returns the current players turn (X / O)
    /// </summary>
    public string GetPlayersTurn()
    {
        return playerTurn;
    }

    /// <summary>
    /// Retruns the display sprite (X / 0)
    /// </summary>
    public Sprite GetPlayerSprite()
    {
        if (playerTurn == "X") return tilePlayerX;
        else return tilePlayerO;
    }
    

    private void UpdateStatusText(string status)
    {
        if (statusText != null)
        {
            statusText.text = status;
        }
    }
}
