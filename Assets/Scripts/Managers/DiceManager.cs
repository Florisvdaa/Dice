using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DiceManager : MonoBehaviour
{
    [SerializeField] private GameObject[] dicePrefabs;
    
    [SerializeField] private List<DiceRoller> activeDice = new();

    [SerializeField] private Button sixDiceButton;
    [SerializeField] private Button rollAllButton;
    [SerializeField] private Button readResult;
    [SerializeField] private TextMeshProUGUI resultText;

    private void Awake()
    {
        sixDiceButton.onClick.AddListener(() => SpawnDice(20));
        rollAllButton.onClick.AddListener(RollAll);
        readResult.onClick.AddListener(DisplayResults);
    }

    public void SpawnDice(int sides)
    {
        var prefab = dicePrefabs.FirstOrDefault(d => d.name.Contains($"D{sides}"));
        if (prefab == null) return;

        GameObject newDice = Instantiate(prefab, RandomSpawnPoint(), Random.rotation);
        DiceRoller roller = newDice.GetComponent<DiceRoller>();
        activeDice.Add(roller);
    }

    public void RollAll()
    {
        foreach (var die in activeDice)
        {
            die.RollDice(Random.onUnitSphere * 5f, Random.onUnitSphere * 10f);
        }
    }

    public List<int> ReadAllResults()
    {
        var results = activeDice.Select(d => d.GetTopFace()).ToList();
        activeDice.Clear(); // Remove all dice after reading
        return results;
    }
    public void DisplayResults()
    {
        var results = ReadAllResults();
        resultText.text = results.Count > 0 ? $"Results: {string.Join(", ", results)}" : "No dice rolled!";
    }

    Vector3 RandomSpawnPoint() => new Vector3(Random.Range(-2, 2), 2f, Random.Range(-2, 2));
}
