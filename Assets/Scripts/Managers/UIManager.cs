using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    // References to DiceManager and all UI elements
    [Header("Dice Manager")]
    [SerializeField] private DiceManager diceManager;

    [Header("Dice Buttons")]
    [SerializeField] private Button fourDiceButton;
    [SerializeField] private Button sixDiceButton;
    [SerializeField] private Button eightDiceButton;
    [SerializeField] private Button tenDiceButton;
    [SerializeField] private Button twelveDiceButton;
    [SerializeField] private Button twentyDiceButton;
    [SerializeField] private Button rollAllButton;

    [Header("Panel Buttons")]
    [SerializeField] private Button openMenuButton;
    [SerializeField] private Button closeMenuButton;
    [SerializeField] private Button resetButton;

    [Header("Slider")]
    [SerializeField] private Slider amountSlider;
    [SerializeField] private TextMeshProUGUI sliderText;

    [Header("Result Display")]
    [SerializeField] private GameObject resultPrefab;
    [SerializeField] private Transform resultPanel;

    [Header("Start Screen")]
    [SerializeField] private Button startButton;

    // Internally tracked currently selected dice type (default is D6)
    private int selectedDice = 6;

    private void Awake()
    {
        // Singleton pattern to ensure there's only one UIManager in the scene
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }

        // Set slider to default value
        amountSlider.value = 1;

        // Set up all UI buttons and their listeners
        SetupButtons();
    }

    private void Start()
    {
        // Play start feedback when the game begins
        FeedbackManager.Instance.GameStartFeedback().PlayFeedbacks();
    }

    private void Update()
    {
        // Continuously update the displayed slider value as it changes
        sliderText.text = amountSlider.value.ToString();
    }

    // Displays each rolled dice result in the UI
    public void ShowDiceResults(List<(int result, string diceName, DiceRoller dice)> results)
    {
        foreach (var (result, diceName, dice) in results)
        {
            // Instantiate new result UI object
            GameObject newResult = Instantiate(resultPrefab, resultPanel);

            // Populate the text fields with dice name and result
            TextMeshProUGUI[] textElements = newResult.GetComponentsInChildren<TextMeshProUGUI>();
            if (textElements.Length >= 2)
            {
                textElements[0].text = diceName.Replace("(Clone)", ""); // Clean up dice name
                textElements[1].text = $"Rolled: {result}";
            }

            // Add click listener to result to highlight corresponding dice
            Button resultButton = newResult.GetComponent<Button>();
            if (resultButton != null)
            {
                resultButton.onClick.AddListener(() =>
                {
                    Debug.Log($"Clicked result for: {diceName} (Rolled {result})");
                    dice.FlashDice(); // Highlight the dice in the scene
                });
            }
        }
    }

    // Clears the dice from the scene and UI
    public void ClearBoard()
    {
        diceManager.ClearDice(); // Destroy dice in the scene
        ClearResultPanel();      // Remove result UI objects
    }

    // Removes all result prefab children from the result panel
    private void ClearResultPanel()
    {
        foreach (Transform child in resultPanel)
        {
            Destroy(child.gameObject);
        }
    }

    // Assigns functionality to all buttons in the UI
    private void SetupButtons()
    {
        // Start button launches the game and activates rolling
        startButton.onClick.AddListener(() =>
        {
            FeedbackManager.Instance.GameStartCloseFeedback().PlayFeedbacks();
            diceManager.ActivateRoll();
        });

        Debug.Log("Button setup");

        // Dice selection buttons set the dice sides
        fourDiceButton.onClick.AddListener(() => selectedDice = 4);
        sixDiceButton.onClick.AddListener(() => selectedDice = 6);
        eightDiceButton.onClick.AddListener(() => selectedDice = 8);
        tenDiceButton.onClick.AddListener(() => selectedDice = 10);
        twelveDiceButton.onClick.AddListener(() => selectedDice = 12);
        twentyDiceButton.onClick.AddListener(() => selectedDice = 20);

        // Rolls all dice if rolling is enabled
        rollAllButton.onClick.AddListener(() =>
        {
            if (diceManager.GetCanRoll())
            {
                diceManager.RollAll();
            }
        });

        // Menu and reset controls
        openMenuButton.onClick.AddListener(() => FeedbackManager.Instance.MenuOpenFeedback().PlayFeedbacks());
        closeMenuButton.onClick.AddListener(() => FeedbackManager.Instance.MenuCloseFeedback().PlayFeedbacks());
        resetButton.onClick.AddListener(ClearBoard);
    }

    #region References
    public int GetCurrentSelectedDice() => selectedDice;
    public int GetCurrentSliderValue() => (int)amountSlider.value;
    #endregion
}