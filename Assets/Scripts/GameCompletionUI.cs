using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameCompletionUI : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI resultText;
    public TextMeshProUGUI scoreText;
    public Button playAgainButton;
    public Button mainMenuButton;

    [Header("Color Settings")]
    public Color winColor = new Color(0.2f, 0.8f, 0.2f, 1f);
    public Color loseColor = new Color(0.8f, 0.2f, 0.2f, 1f);
    public Color drawColor = new Color(0.4f, 0.4f, 0.4f, 1f);
    
    [SerializeField]private GameStateController gameController;

    private void Awake()
    {
        if (playAgainButton != null)
            playAgainButton.onClick.AddListener(OnPlayAgainClicked);
        if (mainMenuButton != null)
            mainMenuButton.onClick.AddListener(OnMainMenuClicked);
    }

    private void OnDestroy()
    {
        // Remove button listeners
        if (playAgainButton != null)
            playAgainButton.onClick.RemoveListener(OnPlayAgainClicked);
        if (mainMenuButton != null)
            mainMenuButton.onClick.RemoveListener(OnMainMenuClicked);
    }

    public void ShowResult(string result, int score = 0)
    {
        gameObject.SetActive(true);

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
            gameObject.SetActive(false);
        }
    }

    private void OnMainMenuClicked()
    {
        // Leave the room and return to main menu
        if (PhotonNetwork.inRoom)
        {
            PhotonNetwork.LeaveRoom();
        }
    }

    public void OnLeftRoom()
    {
        // Handle any cleanup when leaving the room
        gameObject.SetActive(false);
    }

    // Helper method to show a win result 
    public void ShowWin(int score = 0)
    {
        ShowResult("WIN", score);
    }

    // Helper method to show a lose result
    public void ShowLose(int score = 0)
    {
        ShowResult("LOSE", score);
    }

    // Helper method to show a draw result
    public void ShowDraw(int score = 0)
    {
        ShowResult("DRAW", score);
    }
}