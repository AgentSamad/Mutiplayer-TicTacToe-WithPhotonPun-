using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using TMPro;

public class GameCompletionUIEditor : EditorWindow
{
    private Color winColor = new Color(0.2f, 0.8f, 0.2f, 1f);     // Green
    private Color loseColor = new Color(0.8f, 0.2f, 0.2f, 1f);    // Red
    private Color drawColor = new Color(0.4f, 0.4f, 0.4f, 1f);    // Gray

    [MenuItem("Tools/Create Game Completion UI")]
    public static void ShowWindow()
    {
        GetWindow<GameCompletionUIEditor>("Game Completion UI Creator");
    }

    private void OnGUI()
    {
        GUILayout.Label("Game Completion UI Creator", EditorStyles.boldLabel);

        if (GUILayout.Button("Create Game Completion UI"))
        {
            CreateGameCompletionUI();
        }
    }

    private void CreateGameCompletionUI()
    {
        // Create the main panel
        GameObject completionPanel = CreatePanel("GameCompletionPanel");
        RectTransform panelRect = completionPanel.GetComponent<RectTransform>();
        panelRect.anchorMin = new Vector2(0, 0);
        panelRect.anchorMax = new Vector2(1, 1);
        panelRect.offsetMin = Vector2.zero;
        panelRect.offsetMax = Vector2.zero;

        // Add background image
        Image panelImage = completionPanel.GetComponent<Image>();
        panelImage.color = new Color(0, 0, 0, 0.9f);

        // Create result container
        GameObject resultContainer = CreatePanel("ResultContainer");
        resultContainer.transform.SetParent(completionPanel.transform, false);
        RectTransform resultRect = resultContainer.GetComponent<RectTransform>();
        resultRect.anchorMin = new Vector2(0.5f, 0.5f);
        resultRect.anchorMax = new Vector2(0.5f, 0.5f);
        resultRect.sizeDelta = new Vector2(600, 400);
        resultRect.anchoredPosition = Vector2.zero;

        // Create result text
        GameObject resultTextObj = new GameObject("ResultText");
        resultTextObj.transform.SetParent(resultContainer.transform, false);
        TextMeshProUGUI resultText = resultTextObj.AddComponent<TextMeshProUGUI>();
        resultText.fontSize = 72;
        resultText.fontStyle = FontStyles.Bold;
        resultText.alignment = TextAlignmentOptions.Center;
        resultText.text = "YOU WIN!";

        RectTransform resultTextRect = resultText.GetComponent<RectTransform>();
        resultTextRect.anchorMin = new Vector2(0, 0.6f);
        resultTextRect.anchorMax = new Vector2(1, 0.9f);
        resultTextRect.sizeDelta = Vector2.zero;

        // Create score text
        GameObject scoreTextObj = new GameObject("ScoreText");
        scoreTextObj.transform.SetParent(resultContainer.transform, false);
        TextMeshProUGUI scoreText = scoreTextObj.AddComponent<TextMeshProUGUI>();
        scoreText.fontSize = 36;
        scoreText.alignment = TextAlignmentOptions.Center;
        scoreText.text = "Score: 0";

        RectTransform scoreTextRect = scoreText.GetComponent<RectTransform>();
        scoreTextRect.anchorMin = new Vector2(0, 0.4f);
        scoreTextRect.anchorMax = new Vector2(1, 0.6f);
        scoreTextRect.sizeDelta = Vector2.zero;

        // Create buttons container
        GameObject buttonsContainer = CreatePanel("ButtonsContainer");
        buttonsContainer.transform.SetParent(resultContainer.transform, false);
        RectTransform buttonsRect = buttonsContainer.GetComponent<RectTransform>();
        buttonsRect.anchorMin = new Vector2(0, 0);
        buttonsRect.anchorMax = new Vector2(1, 0.4f);
        buttonsRect.sizeDelta = Vector2.zero;

        // Remove image component from buttons container
        DestroyImmediate(buttonsContainer.GetComponent<Image>());

        // Create Play Again button
        GameObject playAgainBtn = CreateButton("PlayAgainButton", "Play Again", buttonsContainer.transform);
        RectTransform playAgainRect = playAgainBtn.GetComponent<RectTransform>();
        playAgainRect.anchorMin = new Vector2(0.1f, 0.5f);
        playAgainRect.anchorMax = new Vector2(0.45f, 0.8f);
        playAgainRect.sizeDelta = Vector2.zero;

        // Create Main Menu button
        GameObject mainMenuBtn = CreateButton("MainMenuButton", "Main Menu", buttonsContainer.transform);
        RectTransform mainMenuRect = mainMenuBtn.GetComponent<RectTransform>();
        mainMenuRect.anchorMin = new Vector2(0.55f, 0.5f);
        mainMenuRect.anchorMax = new Vector2(0.9f, 0.8f);
        mainMenuRect.sizeDelta = Vector2.zero;

        // Add GameCompletionUI component
        GameCompletionUI completionUI = completionPanel.AddComponent<GameCompletionUI>();
        completionUI.resultText = resultText;
        completionUI.scoreText = scoreText;
        completionUI.playAgainButton = playAgainBtn.GetComponent<Button>();
        completionUI.mainMenuButton = mainMenuBtn.GetComponent<Button>();
        completionUI.winColor = winColor;
        completionUI.loseColor = loseColor;
        completionUI.drawColor = drawColor;

        // Add animation
        Animator animator = completionPanel.AddComponent<Animator>();
        // You would need to assign an appropriate animation controller here

        // Hide the panel by default
        completionPanel.SetActive(false);

        Debug.Log("Game Completion UI created successfully!");
    }

    private GameObject CreatePanel(string name)
    {
        GameObject panel = new GameObject(name);
        panel.AddComponent<RectTransform>();
        Image image = panel.AddComponent<Image>();
        image.color = Color.clear;
        return panel;
    }

    private GameObject CreateButton(string name, string text, Transform parent)
    {
        GameObject buttonObj = new GameObject(name);
        buttonObj.transform.SetParent(parent, false);

        // Add button component
        Button button = buttonObj.AddComponent<Button>();

        // Add image component for button background
        Image image = buttonObj.AddComponent<Image>();
        image.sprite = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/UISprite.psd");
        image.type = Image.Type.Sliced;
        image.color = new Color(0.2f, 0.2f, 0.2f, 1f);

        // Setup button colors
        ColorBlock colors = button.colors;
        colors.normalColor = new Color(0.2f, 0.2f, 0.2f, 1f);
        colors.highlightedColor = new Color(0.3f, 0.3f, 0.3f, 1f);
        colors.pressedColor = new Color(0.1f, 0.1f, 0.1f, 1f);
        button.colors = colors;

        // Create text
        GameObject textObj = new GameObject("Text");
        textObj.transform.SetParent(buttonObj.transform, false);
        TextMeshProUGUI buttonText = textObj.AddComponent<TextMeshProUGUI>();
        buttonText.text = text;
        buttonText.fontSize = 24;
        buttonText.alignment = TextAlignmentOptions.Center;
        buttonText.color = Color.white;

        // Position text
        RectTransform textRect = buttonText.GetComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.sizeDelta = Vector2.zero;
        textRect.anchoredPosition = Vector2.zero;

        return buttonObj;
    }
}