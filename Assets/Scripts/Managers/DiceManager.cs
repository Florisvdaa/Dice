using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DiceManager : MonoBehaviour
{
    [Header("Possible dice prefabs")]
    [SerializeField] private GameObject[] dicePrefabs; // List of all available dice prefabs (D4, D6, etc.)

    [Header("Spawn Settings")]
    [SerializeField] private Transform spawnPoint; // Point where the dice will spawn

    [Header("Remove VFX")]
    [SerializeField] private GameObject removeParticle; // Particle effect for dice removal

    // Runtime state
    private List<DiceRoller> activeDice = new(); // List of currently spawned dice
    private bool canRoll = false;                // Flag to determine if rolling is allowed

    private void Update()
    {
        // Allows manual re-roll by pressing Spacebar (for debug/testing)
        if (Input.GetKeyDown(KeyCode.Space))
        {
            RollAll();
        }
    }

    // Coroutine that spawns dice in a rolling direction
    public IEnumerator SpawnDice(Vector3 spawnDirection)
    {
        yield return new WaitForSeconds(1f); // Small delay before starting

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
            // Find the prefab matching the selected number of sides
            var prefab = dicePrefabs.FirstOrDefault(d => d.name.Contains($"D{sides}"));
            if (prefab == null) yield break;

            // Instantiate the die at spawn point with random rotation
            GameObject newDice = Instantiate(prefab, spawnPoint.position, Random.rotation);
            DiceRoller roller = newDice.GetComponent<DiceRoller>();

            // Apply randomized force and torque to simulate rolling
            Vector3 randomForce = spawnDirection * Random.Range(6f, 20f);
            Vector3 randomTorque = Random.onUnitSphere * Random.Range(3f, 6f);

            roller.RollDice(randomForce, randomTorque);
            activeDice.Add(roller);

            yield return new WaitForSeconds(0.2f); // Stagger dice spawns for better visual pacing
        }

        yield return new WaitForSeconds(1.5f); // Wait before closing gate

        FeedbackManager.Instance.CloseGateFeedback().PlayFeedbacks(); // Visual/audio cue
    }

    // Handles initiating a full roll
    public void RollAll()
    {
        if (!canRoll) return; // Prevent rolling while dice are still moving

        canRoll = false; // Disable further rolls temporarily
        UIManager.Instance.ClearBoard(); // Clear previous results

        Vector3 spawnDirection = spawnPoint.forward;

        FeedbackManager.Instance.OpenGateFeedback().PlayFeedbacks(); // Feedback before roll
        StartCoroutine(SpawnDice(spawnDirection));                   // Roll dice
        StartCoroutine(WaitForDiceToSettle());                       // Wait until they're done rolling
    }

    // Monitors active dice until they've all settled before showing results
    private IEnumerator WaitForDiceToSettle()
    {
        yield return new WaitForSeconds(3f); // Initial delay to allow movement

        while (activeDice.Any(d => !d.IsDiceSettled()))
        {
            yield return new WaitForSeconds(0.5f); // Wait and check again
        }

        DisplayResults(); // All dice are still—show the outcome
    }

    // Removes all dice from the board with visual effects
    public void ClearDice()
    {
        foreach (var die in activeDice)
        {
            Instantiate(removeParticle, die.transform.position, Quaternion.identity);
            Destroy(die.gameObject);
        }

        activeDice.Clear();
    }

    // Reads each active die's result and name along with its reference
    public List<(int result, string diceName, DiceRoller dice)> ReadAllResults()
    {
        return activeDice.Select(d => (d.GetTopFace(), d.gameObject.name, d)).ToList();
    }

    // Communicates results to the UI for display
    public void DisplayResults()
    {
        var results = ReadAllResults();
        UIManager.Instance.ShowDiceResults(results); // Pass data to UI
        canRoll = true;                              // Allow another roll
    }

    // Called from UI to unlock rolling after intro/start
    public void ActivateRoll()
    {
        if (!canRoll)
            canRoll = true;
    }

    // Generates a random spawn position (currently unused, but could be handy for scatter effects)
    Vector3 RandomSpawnPoint() => new Vector3(Random.Range(-2, 2), 2f, Random.Range(-2, 2));

    // External access to whether rolling is enabled
    public bool GetCanRoll() => canRoll;
}