using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlanetInfoUI : MonoBehaviour
{
    public static PlanetInfoUI Instance;

    [Header("UI (auto-built if empty)")]
    public GameObject infoPanel;
    public TextMeshProUGUI infoText;
    public UnityEngine.UI.Image planetImage;
    public Button nextButton;
    public Button closeButton;

    [Header("Earth")]
    public AudioClip[] earthClips;
    public Sprite earthInfoSprite;
    public string[] earthTexts = {
        "Earth is the third planet from the Sun and the only planet known to support life. About 71% of its surface is covered by water.",
        "It has a protective atmosphere of nitrogen and oxygen, which regulates temperature and shields life from harmful radiation.",
        "Earth rotates once every 24 hours causing day and night, and orbits the Sun every 365 days creating the seasons."
    };

    [Header("Mars")]
    public AudioClip[] marsClips;
    public Sprite marsInfoSprite;
    public string[] marsTexts = {
        "Mars is the fourth planet from the Sun, called the Red Planet due to iron oxide (rust) covering its surface.",
        "It has a thin atmosphere mostly of carbon dioxide. Mars has the largest volcano in the solar system: Olympus Mons.",
        "Scientists study Mars because it may have once had liquid water and could have supported microbial life in the past."
    };

    private AudioSource audioSource;
    private string[] currentTexts;
    private AudioClip[] currentClips;
    private Sprite currentSprite;
    private int currentIndex = 0;

    void Awake() { Instance = this; }

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();

        if (infoPanel == null) BuildUI();

        infoPanel.SetActive(false);
        nextButton.onClick.AddListener(OnNext);
        closeButton.onClick.AddListener(ClosePanel);
    }

    void BuildUI()
    {
        // Find or create Canvas
        Canvas canvas = FindFirstObjectByType<Canvas>();
        GameObject canvasGO;
        if (canvas == null)
        {
            canvasGO = new GameObject("Canvas");
            canvas = canvasGO.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasGO.AddComponent<CanvasScaler>();
            canvasGO.AddComponent<GraphicRaycaster>();
        }
        else canvasGO = canvas.gameObject;

        // Main Panel (dark)
        infoPanel = new GameObject("InfoPanel");
        infoPanel.transform.SetParent(canvasGO.transform, false);
        RectTransform pr = infoPanel.AddComponent<RectTransform>();
        pr.anchorMin = new Vector2(0.05f, 0.05f);
        pr.anchorMax = new Vector2(0.95f, 0.6f);
        pr.offsetMin = pr.offsetMax = Vector2.zero;
        Image bg = infoPanel.AddComponent<Image>();
        bg.color = new Color(0.05f, 0.05f, 0.1f, 0.92f);

        // Left: Planet Image
        GameObject imgGO = new GameObject("PlanetImage");
        imgGO.transform.SetParent(infoPanel.transform, false);
        RectTransform imgR = imgGO.AddComponent<RectTransform>();
        imgR.anchorMin = new Vector2(0, 0.15f);
        imgR.anchorMax = new Vector2(0.4f, 0.95f);
        imgR.offsetMin = new Vector2(15, 0);
        imgR.offsetMax = new Vector2(-5, -10);
        planetImage = imgGO.AddComponent<UnityEngine.UI.Image>();
        planetImage.preserveAspect = true;

        // Right: Info Text
        GameObject textGO = new GameObject("InfoText");
        textGO.transform.SetParent(infoPanel.transform, false);
        RectTransform textR = textGO.AddComponent<RectTransform>();
        textR.anchorMin = new Vector2(0.42f, 0.15f);
        textR.anchorMax = new Vector2(0.98f, 0.95f);
        textR.offsetMin = new Vector2(5, 0);
        textR.offsetMax = new Vector2(-10, -10);
        infoText = textGO.AddComponent<TextMeshProUGUI>();
        infoText.fontSize = 20;
        infoText.color = Color.white;
        infoText.alignment = TextAlignmentOptions.TopLeft;

        // Next Button
        GameObject nextGO = CreateButton(infoPanel.transform, "Next -->",
            new Vector2(0.58f, 0), new Vector2(0.82f, 0.13f), new Color(0.2f, 0.55f, 1f));
        nextButton = nextGO.GetComponent<Button>();

        // Close Button
        GameObject closeGO = CreateButton(infoPanel.transform, "Close",
            new Vector2(0.84f, 0), new Vector2(0.99f, 0.13f), new Color(0.8f, 0.2f, 0.2f));
        closeButton = closeGO.GetComponent<Button>();
    }

    GameObject CreateButton(Transform parent, string label, Vector2 anchorMin, Vector2 anchorMax, Color color)
    {
        GameObject go = new GameObject(label);
        go.transform.SetParent(parent, false);
        RectTransform r = go.AddComponent<RectTransform>();
        r.anchorMin = anchorMin; r.anchorMax = anchorMax;
        r.offsetMin = new Vector2(5, 5); r.offsetMax = new Vector2(-5, -5);
        Image img = go.AddComponent<Image>(); img.color = color;
        Button btn = go.AddComponent<Button>();
        GameObject t = new GameObject("Text"); t.transform.SetParent(go.transform, false);
        RectTransform tr = t.AddComponent<RectTransform>();
        tr.anchorMin = Vector2.zero; tr.anchorMax = Vector2.one;
        TextMeshProUGUI txt = t.AddComponent<TextMeshProUGUI>();
        txt.text = label; txt.fontSize = 18;
        txt.color = Color.white; txt.alignment = TextAlignmentOptions.Center;
        return go;
    }

    public void ShowEarthInfo()
    {
        currentTexts = earthTexts;
        currentClips = earthClips;
        currentSprite = earthInfoSprite;
        currentIndex = 0;
        ShowCurrent();
    }

    public void ShowMarsInfo()
    {
        currentTexts = marsTexts;
        currentClips = marsClips;
        currentSprite = marsInfoSprite;
        currentIndex = 0;
        ShowCurrent();
    }

    void ShowCurrent()
    {
        infoPanel.SetActive(true);

        if (infoText != null && currentTexts != null && currentIndex < currentTexts.Length)
            infoText.text = currentTexts[currentIndex];

        if (planetImage != null)
            planetImage.sprite = currentSprite;

        if (audioSource != null && currentClips != null && currentIndex < currentClips.Length && currentClips[currentIndex] != null)
        {
            audioSource.Stop();
            audioSource.clip = currentClips[currentIndex];
            audioSource.Play();
        }

        nextButton.gameObject.SetActive(currentTexts != null && currentIndex < currentTexts.Length - 1);
    }

    void OnNext()
    {
        if (currentTexts == null) return;
        if (currentIndex < currentTexts.Length - 1) { currentIndex++; ShowCurrent(); }
    }

    public void ClosePanel()
    {
        infoPanel.SetActive(false);
        audioSource?.Stop();
    }
}
