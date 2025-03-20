using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    [Header("UI Panel References")]
    [SerializeField] private GameObject playerNamePanel;
    [SerializeField] private GameObject gamePanel;
    [SerializeField] private GameObject loadingPanel;
    [SerializeField] private GameObject gameCompletionPanel;
    [SerializeField] private GameObject reconnectingPanel;

    [Header("Player Name Elements")]
    [SerializeField] private TMP_InputField playerNameInput;
    [SerializeField] private Button playButton;
    [SerializeField] private TextMeshProUGUI nameErrorText;
    [SerializeField] private TextMeshProUGUI yourNameText;

    [Header("Loading Elements")]
    [SerializeField] private TextMeshProUGUI loadingText;
    [SerializeField] private Button cancelButton;

    [Header("Game Elements")]
    [SerializeField] private TextMeshProUGUI statusText;
    [SerializeField] private Button leaveButton;

    [Header("Game Completion Elements")]
    [SerializeField] private TextMeshProUGUI resultText;
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private Button playAgainButton;
    [SerializeField] private Button mainMenuButton;

    [Header("Reconnection Elements")]
    [SerializeField] private TextMeshProUGUI reconnectingText;
    [SerializeField] private Button reconnectButton;

    [Header("Color Settings")]
    [SerializeField] private Color winColor = new Color(0.2f, 0.8f, 0.2f, 1f);
    [SerializeField] private Color loseColor = new Color(0.8f, 0.2f, 0.2f, 1f);
    [SerializeField] private Color drawColor = new Color(0.4f, 0.4f, 0.4f, 1f);

    [Header("References")]
    [SerializeField] private ConnectionManager connectionManager;
    [SerializeField] private GameStateController gameController;

    private void Awake()
    {
        if (connectionManager == null)
        {
            connectionManager = GetComponent<ConnectionManager>();
            if (connectionManager == null)
            {
                Debug.LogError("ConnectionManager not found on the same GameObject!");
                return;
            }
        }

        SetupButtonListeners();
    }

    private void SetupButtonListeners()
    {
        // Player Name Panel
        if (playButton != null)
            playButton.onClick.AddListener(OnPlayButtonClicked);

        // Game Panel
        if (leaveButton != null)
            leaveButton.onClick.AddListener(OnLeaveButtonClicked);

        // Loading Panel
        if (cancelButton != null)
            cancelButton.onClick.AddListener(OnCancelButtonClicked);

        // Game Completion Panel
        if (playAgainButton != null)
            playAgainButton.onClick.AddListener(OnPlayAgainClicked);
        if (mainMenuButton != null)
            mainMenuButton.onClick.AddListener(OnMainMenuClicked);

        // Reconnection Panel
        if (reconnectButton != null)
            reconnectButton.onClick.AddListener(OnReconnectClicked);
    }

    private void Start()
    {
        ShowPlayerNameInput();

        // Set up input field behavior
        if (playerNameInput != null)
        {
            playerNameInput.onSubmit.AddListener((value) =>
            {
                if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
                {
                    OnPlayButtonClicked();
                }
            });
        }

        // Hide panels by default
        if (gameCompletionPanel != null)
            gameCompletionPanel.SetActive(false);
        if (reconnectingPanel != null)
            reconnectingPanel.SetActive(false);
    }

    private void OnDestroy()
    {
        // Player Name Panel
        if (playButton != null)
            playButton.onClick.RemoveListener(OnPlayButtonClicked);

        // Game Panel
        if (leaveButton != null)
            leaveButton.onClick.RemoveListener(OnLeaveButtonClicked);

        // Loading Panel
        if (cancelButton != null)
            cancelButton.onClick.RemoveListener(OnCancelButtonClicked);

        // Game Completion Panel
        if (playAgainButton != null)
            playAgainButton.onClick.RemoveListener(OnPlayAgainClicked);
        if (mainMenuButton != null)
            mainMenuButton.onClick.RemoveListener(OnMainMenuClicked);

        // Reconnection Panel
        if (reconnectButton != null)
            reconnectButton.onClick.RemoveListener(OnReconnectClicked);
    }

    #region Player Name Panel Methods
    private void OnPlayButtonClicked()
    {
        string playerName = playerNameInput.text.Trim();

        if (string.IsNullOrEmpty(playerName))
        {
            ShowNameError("Please enter a name!");
            return;
        }

        if (playerName.Length < 2)
        {
            ShowNameError("Name must be at least 2 characters!");
            return;
        }

        if (playerName.Length > 20)
        {
            ShowNameError("Name must be less than 20 characters!");
            return;
        }

        if (playerName.Contains(" "))
        {
            ShowNameError("Name cannot contain spaces!");
            return;
        }

        // Hide any previous error
        nameErrorText.gameObject.SetActive(false);

        // Set the player name in PhotonNetwork
        PhotonNetwork.playerName = playerName;

        // Show loading panel and try to join a game
        ShowLoadingPanel("Searching for game...");
        connectionManager.JoinGame();
    }

    private void ShowNameError(string message)
    {
        nameErrorText.text = message;
        nameErrorText.gameObject.SetActive(true);
    }
    #endregion

    #region Loading Panel Methods
    public void ShowLoadingPanel(string message = "Loading...")
    {
        loadingText.text = message;
        ShowPanel(loadingPanel);
    }

    public void HideLoadingPanel()
    {
        loadingPanel.SetActive(false);
    }

    private void OnCancelButtonClicked()
    {
        connectionManager.LeaveGame();
    }
    #endregion

    #region Game Panel Methods
    private void OnLeaveButtonClicked()
    {
        connectionManager.LeaveGame();
    }

    public void UpdateStatusText(string status)
    {
        if (statusText != null)
        {
            statusText.text = status;
        }
    }
    #endregion

    #region Game Completion Methods
    public void ShowResult(string result, int score = 0)
    {
        gameCompletionPanel.SetActive(true);

        // Set result text and color based on result
        switch (result.ToUpper())
        {
            case "WIN":
                resultText.text = "YOU WIN!";
                resultText.color = winColor;
                break;
            case "LOSE":
                resultText.text = "YOU LOSE!";
                resultText.color = loseColor;
                break;
            case "DRAW":
                resultText.text = "DRAW!";
                resultText.color = drawColor;
                break;
            default:
                resultText.text = result;
                resultText.color = drawColor;
                break;
        }

        // Update score if provided
        if (score > 0)
        {
            scoreText.text = $"Score: {score}";
            scoreText.gameObject.SetActive(true);
        }
        else
        {
            scoreText.gameObject.SetActive(false);
        }

        // Show/hide play again button based on if we're the master client
        playAgainButton.gameObject.SetActive(PhotonNetwork.isMasterClient);
    }

    private void OnPlayAgainClicked()
    {
        if (PhotonNetwork.isMasterClient && gameController != null)
        {
            gameController.RestartGame();
            gameCompletionPanel.SetActive(false);
        }
    }

    private void OnMainMenuClicked()
    {
        // Leave the room and return to main menu
        if (PhotonNetwork.inRoom)
        {
            PhotonNetwork.LeaveRoom();
        }
        ShowPlayerNameInput();
    }

    // Helper methods for showing game results
    public void ShowWin(int score = 0) => ShowResult("WIN", score);
    public void ShowLose(int score = 0) => ShowResult("LOSE", score);
    public void ShowDraw(int score = 0) => ShowResult("DRAW", score);
    #endregion

    #region Reconnection Methods
    public void ShowReconnecting(string message = "Reconnecting...")
    {
        reconnectingText.text = message;
        ShowPanel(reconnectingPanel);
    }

    public void HideReconnecting()
    {
        reconnectingPanel.SetActive(false);
    }

    public void UpdateReconnectingText(string message)
    {
        if (reconnectingText != null)
        {
            reconnectingText.text = message;
        }
    }

    private void OnReconnectClicked()
    {
        if (connectionManager != null)
        {
            connectionManager.ReconnectAndRejoin();
        }
    }
    #endregion

    #region Panel Management
    public void ShowPanel(GameObject panel)
    {
        playerNamePanel.SetActive(panel == playerNamePanel);
        gamePanel.SetActive(panel == gamePanel);
        loadingPanel.SetActive(panel == loadingPanel);
        reconnectingPanel.SetActive(panel == reconnectingPanel);
        if (panel != gameCompletionPanel) // Don't hide completion panel here
            gameCompletionPanel.SetActive(false);
    }

    public void ShowGame()
    {
        ShowPanel(gamePanel);
    }

    public void ShowPlayerNameInput()
    {
        if (playerNameInput != null)
        {
            playerNameInput.text = "";  // Clear the input field
            playerNameInput.Select();   // Focus the input field
            playerNameInput.ActivateInputField();
        }

        if (nameErrorText != null)
        {
            nameErrorText.gameObject.SetActive(false);
        }

        ShowPanel(playerNamePanel);
    }
    #endregion
}