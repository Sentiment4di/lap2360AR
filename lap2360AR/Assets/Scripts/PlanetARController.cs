using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using Unity.XR.CoreUtils;

/// <summary>
/// CPIS 360 - Planet AR Controller
/// Attach to XR Origin. Auto-finds ARTrackedImageManager.
/// Spawns Earth or Mars sphere when image card is detected.
/// </summary>
public class PlanetARController : MonoBehaviour
{
    [Header("Planet Prefabs")]
    public GameObject earthPrefab;
    public GameObject marsPrefab;

    [Header("Audio (optional)")]
    public AudioClip earthAudio;
    public AudioClip marsAudio;

    private ARTrackedImageManager imageManager;
    private AudioSource audioSource;
    private readonly Dictionary<TrackableId, GameObject> spawnedPlanets = new();

    void Awake()
    {
        // Get or add ARTrackedImageManager
        imageManager = GetComponent<ARTrackedImageManager>();
        if (imageManager == null)
            imageManager = gameObject.AddComponent<ARTrackedImageManager>();

        // Add AudioSource
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.spatialBlend = 0f;
    }

    void OnEnable()  => imageManager.trackablesChanged.AddListener(OnChanged);
    void OnDisable() => imageManager.trackablesChanged.RemoveListener(OnChanged);

    void OnChanged(ARTrackablesChangedEventArgs<ARTrackedImage> args)
    {
        foreach (var img in args.added)   OnAdded(img);
        foreach (var img in args.updated) OnUpdated(img);
        foreach (var kvp in args.removed) OnRemoved(kvp.Key);
    }

    void OnAdded(ARTrackedImage image)
    {
        if (spawnedPlanets.ContainsKey(image.trackableId)) return;

        string     n      = image.referenceImage.name;
        GameObject prefab = n == "Earth" ? earthPrefab : n == "Mars" ? marsPrefab : null;
        AudioClip  clip   = n == "Earth" ? earthAudio  : n == "Mars" ? marsAudio  : null;

        if (prefab == null)
        {
            Debug.LogWarning($"[AR] No prefab for image: {n}");
            return;
        }

        GameObject planet = Instantiate(prefab,
            image.transform.position + Vector3.up * 0.12f,
            Quaternion.identity);

        planet.transform.SetParent(image.transform, true);
        planet.transform.localScale = Vector3.one * 0.08f;

        spawnedPlanets[image.trackableId] = planet;

        if (clip != null) audioSource.PlayOneShot(clip);

        Debug.Log($"[AR] Spawned: {n}");
    }

    void OnUpdated(ARTrackedImage image)
    {
        if (spawnedPlanets.TryGetValue(image.trackableId, out GameObject go))
            go.SetActive(image.trackingState == TrackingState.Tracking);
    }

    void OnRemoved(TrackableId id)
    {
        if (spawnedPlanets.TryGetValue(id, out GameObject go))
        {
            Destroy(go);
            spawnedPlanets.Remove(id);
        }
    }
}
