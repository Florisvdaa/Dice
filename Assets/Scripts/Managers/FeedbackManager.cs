using MoreMountains.Feedbacks;
using UnityEngine;

public class FeedbackManager : MonoBehaviour
{
    public static FeedbackManager Instance { get; private set; }

    [Header("Feedbacks")]
    [SerializeField] private MMF_Player menuOpenFeedback;
    [SerializeField] private MMF_Player menuCloseFeedback;

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
    }

    public MMF_Player MenuOpenFeedback() => menuOpenFeedback;
    public MMF_Player MenuCloseFeedback() => menuCloseFeedback;
}
