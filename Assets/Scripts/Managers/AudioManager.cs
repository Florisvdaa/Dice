using System.Collections;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

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
    }

    public void PlayClipAt(Vector3 pos, AudioClip clip, float volume)
    {
        GameObject tempGO = new GameObject("TempAudio");
        tempGO.transform.position = pos;

        AudioSource source = tempGO.AddComponent<AudioSource>();
        source.clip = clip;
        source.volume = volume;
        source.Play();

        StartCoroutine(DestoryAfterPlay(source));
    }

    private IEnumerator DestoryAfterPlay(AudioSource source)
    {
        yield return new WaitForSeconds(source.clip.length);
        Destroy(source.gameObject);
    }    
}
