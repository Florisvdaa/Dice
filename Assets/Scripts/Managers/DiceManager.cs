using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DiceManager : MonoBehaviour
{
    [Header("Possible dice prefabs")]
    [SerializeField] private GameObject[] dicePrefabs;                  // All possible dice

    // Private
    private List<DiceRoller> activeDice = new();                        // Currently active dice

    private bool canRoll = true;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            RollAll();
        }
    }

    public void SpawnDice()
    {
        int sides = UIManager.Instance.GetCurrentSelectedDice();
        int amount = UIManager.Instance.GetCurrentSliderValue();

        Debug.Log($"Spawn Dice: {sides}, Amount: {amount}");

        for (int i = 0; i < amount; i++) // Spawn based on slider value
        {
            var prefab = dicePrefabs.FirstOrDefault(d => d.name.Contains($"D{sides}"));
            if (prefab == null) return;

            GameObject newDice = Instantiate(prefab, RandomSpawnPoint(), Random.rotation);
            DiceRoller roller = newDice.GetComponent<DiceRoller>();
            activeDice.Add(roller);
        }
    }
    public void RollAll()
    {
        if (!canRoll) return; // Prevent rolling if already in progress

        canRoll = false; // Disable rolling until dice settle

        UIManager.Instance.ClearBoard();

        SpawnDice();

        foreach (var die in activeDice)
        {
            die.RollDice(Random.onUnitSphere * 5f, Random.onUnitSphere * 10f);
        }

        StartCoroutine(WaitForDiceToSettle()); // Start waiting for dice to stop
    }
    private IEnumerator WaitForDiceToSettle()
    {
        yield return new WaitForSeconds(1f); // Give dice time to roll

        while (activeDice.Any(d => !d.IsDiceSettled())) // Keep checking until all dice settle
        {
            yield return new WaitForSeconds(0.5f); // Check every 0.5 seconds
        }

        DisplayResults(); // Once all dice are settled, show results
    }

    public void ClearDice()
    {
        foreach (var die in activeDice)
        {
            Destroy(die.gameObject);
        }

        activeDice.Clear();
    }

    public List<(int result, string diceName)> ReadAllResults()
    {
        var results = activeDice.Select(d => (d.GetTopFace(), d.gameObject.name)).ToList();
        return results;
    }
    public void DisplayResults()
    {
        var results = ReadAllResults();
        UIManager.Instance.ShowDiceResults(results);
        canRoll = true;
    }

    Vector3 RandomSpawnPoint() => new Vector3(Random.Range(-2, 2), 2f, Random.Range(-2, 2));
    public bool GetCanRoll() => canRoll;
}   
