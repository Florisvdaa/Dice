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

    [Header("Spawn Settings")]
    [SerializeField] private Transform spawnPoint;

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

    //public void SpawnDice(Vector3 spawnDirection)
    //{
    //    int sides = UIManager.Instance.GetCurrentSelectedDice();
    //    int amount = UIManager.Instance.GetCurrentSliderValue();

    //    if (spawnPoint == null)
    //    {
    //        Debug.LogError("Spawn point is not set!");
    //        return;
    //    }

    //    Debug.Log($"Spawn Dice: {sides}, Amount: {amount}");

    //    for (int i = 0; i < amount; i++)
    //    {
    //        var prefab = dicePrefabs.FirstOrDefault(d => d.name.Contains($"D{sides}"));
    //        if (prefab == null) return;

    //        GameObject newDice = Instantiate(prefab, spawnPoint.position, Random.rotation);
    //        DiceRoller roller = newDice.GetComponent<DiceRoller>();

    //        // Apply directional force based on spawnPoint's forward direction
    //        Vector3 randomForce = spawnDirection * Random.Range(6f, 20f);
    //        Vector3 randomTorque = Random.onUnitSphere * Random.Range(3f, 6f);

    //        roller.RollDice(randomForce, randomTorque);

    //        activeDice.Add(roller);
    //    }
    //}

    public IEnumerator SpawnDice(Vector3 spawnDirection)
    {
        yield return new WaitForSeconds(1f);

        int sides = UIManager.Instance.GetCurrentSelectedDice();
        int amount = UIManager.Instance.GetCurrentSliderValue();

        if (spawnPoint == null)
        {
            Debug.LogError("Spawn point is not set!");
            yield break;
        }

        Debug.Log($"Spawning {amount} D{sides} dice...");
        
        for (int i = 0; i < amount; i++)
        {
            var prefab = dicePrefabs.FirstOrDefault(d => d.name.Contains($"D{sides}"));
            if (prefab == null) yield break;

            GameObject newDice = Instantiate(prefab, spawnPoint.position, Random.rotation);
            DiceRoller roller = newDice.GetComponent<DiceRoller>();

            // Apply directional force based on spawnPoint's forward direction
            Vector3 randomForce = spawnDirection * Random.Range(6f, 20f);
            Vector3 randomTorque = Random.onUnitSphere * Random.Range(3f, 6f);

            roller.RollDice(randomForce, randomTorque);
            activeDice.Add(roller);

            yield return new WaitForSeconds(0.2f); // Wait before spawning next dice
        }

        yield return new WaitForSeconds(1.5f);

        FeedbackManager.Instance.CloseGateFeedback().PlayFeedbacks();
    }

    public void RollAll()
    {
        if (!canRoll) return; // Prevent rolling if already in progress

        canRoll = false; // Disable rolling until dice settle
        UIManager.Instance.ClearBoard();

        Vector3 spawnDirection = spawnPoint.forward; // Dice roll forward from spawnPoint

        FeedbackManager.Instance.OpenGateFeedback().PlayFeedbacks();

        StartCoroutine(SpawnDice(spawnDirection)); // Start dice spawning with delay

        StartCoroutine(WaitForDiceToSettle()); // Start waiting for dice to stop

    }
    private IEnumerator WaitForDiceToSettle()
    {
        yield return new WaitForSeconds(3f); // Give dice time to roll

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
