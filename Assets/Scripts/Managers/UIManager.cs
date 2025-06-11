using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

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

    [Header("Pannel Buttons")]
    [SerializeField] private Button openMenuButton;
    [SerializeField] private Button closeMenuButton;
    [SerializeField] private Button resetButton;

    [Header("Slider")]
    [SerializeField] private Slider amountSlider;
    [SerializeField] private TextMeshProUGUI sliderText;

    [Header("Result Display")]
    [SerializeField] private GameObject resultPrefab;
    [SerializeField] private Transform resultPanel;

    //Private
    private int selectedDice = 6;

    private void Awake()
    {
        // If there is an instance, and it's not me, delete myself.
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }

        amountSlider.value = 1;

        SetupButtons();
    }

    private void Update()
    {
        sliderText.text = amountSlider.value.ToString();
    }
    public void ShowDiceResults(List<(int result, string diceName)> results)
    {
        foreach (var (result, diceName) in results)
        {
            GameObject newResult = Instantiate(resultPrefab, resultPanel);

            // Get all TextMeshProUGUI elements in the prefab
            TextMeshProUGUI[] textElements = newResult.GetComponentsInChildren<TextMeshProUGUI>();

            if (textElements.Length >= 2)
            {
                textElements[0].text = diceName.Replace("(Clone)", ""); // Replaces (Clone) with a blank space
                textElements[1].text = $"Rolled: {result}"; // Assign the dice result
            }
        }
    }

    public void ClearBoard()
    {
        diceManager.ClearDice();
        ClearResultPanel();
    }
    private void ClearResultPanel()
    {
        foreach (Transform child in resultPanel)
        {
            Destroy(child.gameObject);
        }
    }

    private void SetupButtons()
    {
        Debug.Log("Button setup");

        fourDiceButton.onClick.AddListener(() => selectedDice = 4);
        sixDiceButton.onClick.AddListener(() => selectedDice = 6);
        eightDiceButton.onClick.AddListener(() => selectedDice = 8);
        tenDiceButton.onClick.AddListener(() => selectedDice = 10);
        twelveDiceButton.onClick.AddListener(() => selectedDice = 12);
        twentyDiceButton.onClick.AddListener(() => selectedDice = 20);

        rollAllButton.onClick.AddListener(() =>
        {
            if (diceManager.GetCanRoll()) // Check if rolling is allowed
            {
                diceManager.RollAll();
            }
        });


        openMenuButton.onClick.AddListener(() => FeedbackManager.Instance.MenuOpenFeedback().PlayFeedbacks());
        closeMenuButton.onClick.AddListener(() => FeedbackManager.Instance.MenuCloseFeedback().PlayFeedbacks());
        resetButton.onClick.AddListener(ClearBoard);
    }

    #region References
    public int GetCurrentSelectedDice() => selectedDice;
    public int GetCurrentSliderValue() => (int)amountSlider.value;
    #endregion
}
