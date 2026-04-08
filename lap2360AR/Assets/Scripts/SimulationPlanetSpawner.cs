using UnityEngine;

/// <summary>
/// For PC simulation only - spawns planets in front of camera so you can see them in Play mode
/// Remove or disable this script when building for Android
/// </summary>
public class SimulationPlanetSpawner : MonoBehaviour
{
    public GameObject earthPrefab;
    public GameObject marsPrefab;
    public AudioClip earthAudio;
    public AudioClip marsAudio;

    private AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();

        // Spawn Earth 1.5m in front and slightly to the left
        if (earthPrefab != null)
        {
            GameObject earth = Instantiate(earthPrefab);
            earth.transform.position = new Vector3(-0.3f, 0f, 1.5f);
            earth.transform.localScale = Vector3.one * 0.15f;
        }

        // Spawn Mars 1.5m in front and slightly to the right
        if (marsPrefab != null)
        {
            GameObject mars = Instantiate(marsPrefab);
            mars.transform.position = new Vector3(0.3f, 0f, 1.5f);
            mars.transform.localScale = Vector3.one * 0.15f;
        }

        // Play earth audio on start
        if (earthAudio != null)
        {
            audioSource.clip = earthAudio;
            audioSource.loop = true;
            audioSource.Play();
        }
    }
}
