using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using TMPro;

[CustomEditor(typeof(UIManager))]
public class ConnectionManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        UIManager manager = (UIManager)target;

        if (GUILayout.Button("Generate UI Elements"))
        {
            GenerateUIElements(manager);
        }
    }

    private void GenerateUIElements(UIManager manager)
    {
        // Find or create Canvas
        Canvas canvas = FindObjectOfType<Canvas>();
        if (canvas == null)
        {
            GameObject canvasObj = new GameObject("Canvas");
            canvas = canvasObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            
            // Add Canvas Scaler
            CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
            scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
            scaler.matchWidthOrHeight = 0.5f;
            
            // Add Graphic Raycaster
            canvasObj.AddComponent<GraphicRaycaster>();
        }

        // Get or add ConnectionManager component
        ConnectionManager connectionManager = manager.gameObject.GetComponent<ConnectionManager>();
        if (connectionManager == null)
        {
            connectionManager = manager.gameObject.AddComponent<ConnectionManager>();
        }

        // Create panels
        GameObject playerNamePanel = CreatePanel("PlayerNamePanel", canvas.transform);
        GameObject gamePanel = CreatePanel("GamePanel", canvas.transform);
        GameObject loadingPanel = CreatePanel("LoadingPanel", canvas.transform);

        // Set panel colors
        Color panelColor = new Color(0.18f, 0.18f, 0.18f, 0.8f);
        playerNamePanel.GetComponent<Image>().color = panelColor;
        gamePanel.GetComponent<Image>().color = panelColor;
        loadingPanel.GetComponent<Image>().color = panelColor;

        // Setup panels
        SetupPlayerNamePanel(playerNamePanel, manager);
        SetupGamePanel(gamePanel, manager);
        SetupLoadingPanel(loadingPanel, manager);

        // Initially show only player name panel
        playerNamePanel.SetActive(true);
        gamePanel.SetActive(false);
        loadingPanel.SetActive(false);

        // Assign panel references to the UI manager
        manager.GetType().GetField("playerNamePanel", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).SetValue(manager, playerNamePanel);
        manager.GetType().GetField("gamePanel", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).SetValue(manager, gamePanel);
        manager.GetType().GetField("loadingPanel", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).SetValue(manager, loadingPanel);

        // Set up references between managers
        manager.GetType().GetField("connectionManager", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).SetValue(manager, connectionManager);
        connectionManager.GetType().GetField("uiManager", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).SetValue(connectionManager, manager);

        EditorUtility.SetDirty(manager);
        EditorUtility.SetDirty(connectionManager);
    }

    private void SetupPlayerNamePanel(GameObject panel, UIManager manager)
    {
        // Create Your Name Text
        GameObject yourNameText = CreateTMPText("YourNameText", panel.transform);
        RectTransform yourNameRect = yourNameText.GetComponent<RectTransform>();
        yourNameRect.sizeDelta = new Vector2(300, 50);
        yourNameRect.anchoredPosition = new Vector2(0, 50);
        TextMeshProUGUI yourNameTMP = yourNameText.GetComponent<TextMeshProUGUI>();
        yourNameTMP.text = "Your Name";
        yourNameTMP.fontSize = 36;
        yourNameTMP.color = Color.white;
        yourNameTMP.alignment = TextAlignmentOptions.Center;
        yourNameTMP.fontStyle = FontStyles.Bold;

        // Create Input Field
        GameObject inputField = CreateTMPInputField("PlayerNameInput", panel.transform);
        RectTransform inputRect = inputField.GetComponent<RectTransform>();
        inputRect.sizeDelta = new Vector2(300, 40);
        inputRect.anchoredPosition = new Vector2(0, -20);
        TMP_InputField input = inputField.GetComponent<TMP_InputField>();
        input.characterLimit = 20;
        
        // Style the input field
        Image inputImage = inputField.GetComponent<Image>();
        inputImage.color = Color.white;

        // Set up text area for input field
        GameObject textArea = new GameObject("Text Area", typeof(RectTransform));
        textArea.transform.SetParent(inputField.transform, false);
        RectTransform textAreaRect = textArea.GetComponent<RectTransform>();
        textAreaRect.anchorMin = new Vector2(0, 0);
        textAreaRect.anchorMax = new Vector2(1, 1);
        textAreaRect.offsetMin = new Vector2(10, 5);
        textAreaRect.offsetMax = new Vector2(-10, -5);

        TextMeshProUGUI inputText = textArea.AddComponent<TextMeshProUGUI>();
        inputText.color = Color.black;
        inputText.fontSize = 24;
        inputText.alignment = TextAlignmentOptions.Left;
        input.textComponent = inputText;
        input.textViewport = textAreaRect;

        GameObject placeholder = new GameObject("Placeholder", typeof(RectTransform));
        placeholder.transform.SetParent(textArea.transform, false);
        RectTransform placeholderRect = placeholder.GetComponent<RectTransform>();
        placeholderRect.anchorMin = Vector2.zero;
        placeholderRect.anchorMax = Vector2.one;
        placeholderRect.offsetMin = Vector2.zero;
        placeholderRect.offsetMax = Vector2.zero;

        TextMeshProUGUI placeholderText = placeholder.AddComponent<TextMeshProUGUI>();
        placeholderText.text = "Enter your name";
        placeholderText.fontSize = 24;
        placeholderText.fontStyle = FontStyles.Italic;
        placeholderText.color = new Color(0.5f, 0.5f, 0.5f, 0.5f);
        placeholderText.alignment = TextAlignmentOptions.Left;
        input.placeholder = placeholderText;

        // Create Play Button
        GameObject playButton = CreateButton("PlayButton", panel.transform);
        RectTransform buttonRect = playButton.GetComponent<RectTransform>();
        buttonRect.sizeDelta = new Vector2(160, 40);
        buttonRect.anchoredPosition = new Vector2(0, -90);
        Button button = playButton.GetComponent<Button>();
        ColorBlock colors = button.colors;
        colors.normalColor = new Color(0.2f, 0.2f, 0.2f, 1);
        colors.highlightedColor = new Color(0.3f, 0.3f, 0.3f, 1);
        colors.pressedColor = new Color(0.15f, 0.15f, 0.15f, 1);
        button.colors = colors;

        SetupButtonWithText(playButton, "Play", 24);

        // Create Error Text
        GameObject errorText = CreateTMPText("NameErrorText", panel.transform);
        RectTransform errorRect = errorText.GetComponent<RectTransform>();
        errorRect.sizeDelta = new Vector2(300, 30);
        errorRect.anchoredPosition = new Vector2(0, -140);
        TextMeshProUGUI errorTMP = errorText.GetComponent<TextMeshProUGUI>();
        errorTMP.color = Color.red;
        errorTMP.fontSize = 18;
        errorTMP.alignment = TextAlignmentOptions.Center;
        errorText.SetActive(false);

        // Center all elements
        RectTransform mainContainer = new GameObject("MainContainer", typeof(RectTransform)).GetComponent<RectTransform>();
        mainContainer.SetParent(panel.transform, false);
        mainContainer.anchorMin = new Vector2(0.5f, 0.5f);
        mainContainer.anchorMax = new Vector2(0.5f, 0.5f);
        mainContainer.sizeDelta = new Vector2(300, 300);
        
        yourNameText.transform.SetParent(mainContainer, true);
        inputField.transform.SetParent(mainContainer, true);
        playButton.transform.SetParent(mainContainer, true);
        errorText.transform.SetParent(mainContainer, true);

        // Assign references
        manager.GetType().GetField("playerNameInput", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).SetValue(manager, input);
        manager.GetType().GetField("playButton", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).SetValue(manager, button);
        manager.GetType().GetField("nameErrorText", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).SetValue(manager, errorTMP);
        manager.GetType().GetField("yourNameText", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).SetValue(manager, yourNameTMP);
    }

    private void SetupGamePanel(GameObject panel, UIManager manager)
    {
        // Create status text
        GameObject statusObj = CreateTMPText("StatusText", panel.transform);
        RectTransform statusRect = statusObj.GetComponent<RectTransform>();
        statusRect.sizeDelta = new Vector2(400, 50);
        statusRect.anchoredPosition = new Vector2(0, 200);
        TextMeshProUGUI statusTMP = statusObj.GetComponent<TextMeshProUGUI>();
        statusTMP.text = "Waiting for opponent...";
        statusTMP.fontSize = 24;
        statusTMP.color = Color.white;
        statusTMP.alignment = TextAlignmentOptions.Center;

        // Create leave button
        GameObject leaveButton = CreateButton("LeaveButton", panel.transform);
        RectTransform leaveRect = leaveButton.GetComponent<RectTransform>();
        leaveRect.sizeDelta = new Vector2(160, 40);
        leaveRect.anchoredPosition = new Vector2(0, -200);
        SetupButtonWithText(leaveButton, "Leave Game", 24);

        // Assign references
        manager.GetType().GetField("statusText", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).SetValue(manager, statusTMP);
        manager.GetType().GetField("leaveButton", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).SetValue(manager, leaveButton.GetComponent<Button>());
    }

    private void SetupLoadingPanel(GameObject panel, UIManager manager)
    {
        // Create loading text
        GameObject loadingTextObj = CreateTMPText("LoadingText", panel.transform);
        RectTransform loadingRect = loadingTextObj.GetComponent<RectTransform>();
        loadingRect.sizeDelta = new Vector2(400, 50);
        loadingRect.anchoredPosition = new Vector2(0, 50);
        TextMeshProUGUI loadingTMP = loadingTextObj.GetComponent<TextMeshProUGUI>();
        loadingTMP.text = "Searching for game...";
        loadingTMP.fontSize = 32;
        loadingTMP.color = Color.white;
        loadingTMP.alignment = TextAlignmentOptions.Center;

        // Create loading spinner (simple rotating text for now)
        GameObject spinnerObj = CreateTMPText("LoadingSpinner", panel.transform);
        RectTransform spinnerRect = spinnerObj.GetComponent<RectTransform>();
        spinnerRect.sizeDelta = new Vector2(100, 100);
        spinnerRect.anchoredPosition = new Vector2(0, 0);
        TextMeshProUGUI spinnerTMP = spinnerObj.GetComponent<TextMeshProUGUI>();
        spinnerTMP.text = "○";
        spinnerTMP.fontSize = 48;
        spinnerTMP.color = Color.white;
        spinnerTMP.alignment = TextAlignmentOptions.Center;
        
        // Add rotation animation component
        spinnerObj.AddComponent<LoadingSpinner>();

        // Create cancel button
        GameObject cancelButton = CreateButton("CancelButton", panel.transform);
        RectTransform cancelRect = cancelButton.GetComponent<RectTransform>();
        cancelRect.sizeDelta = new Vector2(160, 40);
        cancelRect.anchoredPosition = new Vector2(0, -50);
        SetupButtonWithText(cancelButton, "Cancel", 24);

        // Assign references
        manager.GetType().GetField("loadingText", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).SetValue(manager, loadingTMP);
        manager.GetType().GetField("cancelButton", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).SetValue(manager, cancelButton.GetComponent<Button>());
    }

    private void SetupButtonWithText(GameObject buttonObj, string text, int fontSize)
    {
        Button button = buttonObj.GetComponent<Button>();
        Image buttonImage = buttonObj.GetComponent<Image>();
        buttonImage.color = new Color(0.2f, 0.2f, 0.2f, 1);
        
        ColorBlock colors = button.colors;
        colors.normalColor = new Color(0.2f, 0.2f, 0.2f, 1);
        colors.highlightedColor = new Color(0.3f, 0.3f, 0.3f, 1);
        colors.pressedColor = new Color(0.15f, 0.15f, 0.15f, 1);
        button.colors = colors;

        GameObject textObj = CreateTMPText("Text", buttonObj.transform);
        RectTransform textRect = textObj.GetComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = Vector2.zero;
        textRect.offsetMax = Vector2.zero;

        TextMeshProUGUI tmp = textObj.GetComponent<TextMeshProUGUI>();
        tmp.text = text;
        tmp.fontSize = fontSize;
        tmp.color = Color.white;
        tmp.fontStyle = FontStyles.Bold;
        tmp.alignment = TextAlignmentOptions.Center;
    }

    private GameObject CreatePanel(string name, Transform parent)
    {
        GameObject panel = new GameObject(name, typeof(RectTransform));
        panel.transform.SetParent(parent, false);
        RectTransform rect = panel.GetComponent<RectTransform>();
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.sizeDelta = Vector2.zero;
        rect.anchoredPosition = Vector2.zero;
        
        Image image = panel.AddComponent<Image>();
        image.color = new Color(0.18f, 0.18f, 0.18f, 0.8f);
        
        return panel;
    }

    private GameObject CreateTMPText(string name, Transform parent)
    {
        GameObject obj = new GameObject(name, typeof(RectTransform));
        obj.transform.SetParent(parent, false);
        TextMeshProUGUI tmp = obj.AddComponent<TextMeshProUGUI>();
        RectTransform rect = obj.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        return obj;
    }

    private GameObject CreateTMPInputField(string name, Transform parent)
    {
        GameObject obj = new GameObject(name, typeof(RectTransform));
        obj.transform.SetParent(parent, false);
        
        // Add required components
        Image image = obj.AddComponent<Image>();
        TMP_InputField inputField = obj.AddComponent<TMP_InputField>();
        
        // Create text area
        GameObject textArea = CreateTMPText("Text Area", obj.transform);
        TextMeshProUGUI textComponent = textArea.GetComponent<TextMeshProUGUI>();
        
        // Create placeholder
        GameObject placeholder = CreateTMPText("Placeholder", obj.transform);
        TextMeshProUGUI placeholderComponent = placeholder.GetComponent<TextMeshProUGUI>();
        
        // Setup input field
        inputField.textViewport = textArea.GetComponent<RectTransform>();
        inputField.textComponent = textComponent;
        inputField.placeholder = placeholderComponent;
        
        return obj;
    }

    private GameObject CreateButton(string name, Transform parent)
    {
        GameObject obj = new GameObject(name, typeof(RectTransform));
        obj.transform.SetParent(parent, false);
        
        // Add required components
        Image image = obj.AddComponent<Image>();
        Button button = obj.AddComponent<Button>();
        
        // Create text
        GameObject textObj = CreateTMPText("Text", obj.transform);
        
        return obj;
    }
}

// UI Manager component to handle panel states
public class ConnectionUIManager : MonoBehaviour
{
    private GameObject mainMenuPanel;
    private GameObject lobbyPanel;
    private GameObject gamePanel;
    private GameObject reconnectingPanel;

    public void InitializePanels(GameObject mainMenu, GameObject lobby, GameObject game, GameObject reconnecting)
    {
        mainMenuPanel = mainMenu;
        lobbyPanel = lobby;
        gamePanel = game;
        reconnectingPanel = reconnecting;
    }

    public void ShowMainMenu()
    {
        mainMenuPanel.SetActive(true);
        lobbyPanel.SetActive(false);
        gamePanel.SetActive(false);
        reconnectingPanel.SetActive(false);
    }

    public void ShowLobby()
    {
        mainMenuPanel.SetActive(false);
        lobbyPanel.SetActive(true);
        gamePanel.SetActive(false);
        reconnectingPanel.SetActive(false);
    }

    public void ShowGame()
    {
        mainMenuPanel.SetActive(false);
        lobbyPanel.SetActive(false);
        gamePanel.SetActive(true);
        reconnectingPanel.SetActive(false);
    }

    public void ShowReconnecting()
    {
        mainMenuPanel.SetActive(false);
        lobbyPanel.SetActive(false);
        gamePanel.SetActive(false);
        reconnectingPanel.SetActive(true);
    }
}

// Add LoadingSpinner component
public class LoadingSpinner : MonoBehaviour
{
    private RectTransform rectTransform;
    private float rotationSpeed = 180f; // degrees per second

    private void Start()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    private void Update()
    {
        rectTransform.Rotate(0, 0, -rotationSpeed * Time.deltaTime);
    }
}