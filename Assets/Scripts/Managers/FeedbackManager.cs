using MoreMountains.Feedbacks;
using UnityEngine;

public class FeedbackManager : MonoBehaviour
{
    public static FeedbackManager Instance { get; private set; }

    [Header("Feedbacks")]
    [SerializeField] private MMF_Player gameStartFeedback;
    [SerializeField] private MMF_Player gameStartCloseFeedback;
    [SerializeField] private MMF_Player menuOpenFeedback;
    [SerializeField] private MMF_Player menuCloseFeedback;
    [SerializeField] private MMF_Player openGateFeedback;
    [SerializeField] private MMF_Player closeGateFeedback;

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

    public MMF_Player GameStartFeedback() => gameStartFeedback;
    public MMF_Player GameStartCloseFeedback() => gameStartCloseFeedback;
    public MMF_Player MenuOpenFeedback() => menuOpenFeedback;
    public MMF_Player MenuCloseFeedback() => menuCloseFeedback;
    public MMF_Player OpenGateFeedback() => openGateFeedback;
    public MMF_Player CloseGateFeedback() => closeGateFeedback;
}
