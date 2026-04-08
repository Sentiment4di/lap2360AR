using UnityEngine;
using UnityEngine.InputSystem;

public class PlanetClickHandler : MonoBehaviour
{
    public enum PlanetType { Earth, Mars }
    public PlanetType planetType;

    private Camera mainCamera;

    void Start()
    {
        mainCamera = Camera.main;
        if (GetComponent<Collider>() == null)
            gameObject.AddComponent<SphereCollider>();
    }

    void Update()
    {
        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
        {
            if (mainCamera == null) mainCamera = Camera.main;
            Vector2 mousePos = Mouse.current.position.ReadValue();
            Ray ray = mainCamera.ScreenPointToRay(mousePos);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider.gameObject == gameObject)
                {
                    if (PlanetInfoUI.Instance == null) return;
                    if (planetType == PlanetType.Earth)
                        PlanetInfoUI.Instance.ShowEarthInfo();
                    else
                        PlanetInfoUI.Instance.ShowMarsInfo();
                }
            }
        }
    }
}
