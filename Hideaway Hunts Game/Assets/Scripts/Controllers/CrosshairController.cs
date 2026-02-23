using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CrosshairController : MonoBehaviour
{
    [Header("UI")]
    public Image crosshairImage;

    [Header("Colors")]
    public Color defaultColor = Color.white;
    public Color targetColor = Color.red;
    public Color hitFlashColor = Color.red;

    [Header("Raycast")]
    public float detectionDistance = 100f;
    public LayerMask enemyLayer;

    private Camera cam;
    private Coroutine flashRoutine;

    void Start()
    {
        // Get main camera safely
        cam = Camera.main;

        if (cam == null)
        {
            Debug.LogError("❌ No MainCamera found! Make sure camera tag = MainCamera");
        }

        if (crosshairImage == null)
        {
            Debug.LogError("❌ Crosshair Image not assigned!");
        }

        crosshairImage.color = defaultColor;
    }

    void Update()
    {
        if (cam == null || crosshairImage == null) return;

        Ray ray = new Ray(cam.transform.position, cam.transform.forward);
        RaycastHit hit;

        bool isTarget = Physics.Raycast(
            ray,
            out hit,
            detectionDistance,
            enemyLayer
        );

        // Change crosshair color
        crosshairImage.color = isTarget ? targetColor : defaultColor;

        // Shoot
        // if (Input.GetMouseButtonDown(0) && isTarget)
        // {
        //     BotBody body = hit.collider.GetComponent<BotBody>();
        //     if (body != null)
        //     {
        //         body.TakeDamage(25);
        //         FlashDamage();
        //     }
        // }
    }

    // =========================
    // Hit Feedback
    // =========================
    public void FlashDamage()
    {
        if (flashRoutine != null)
            StopCoroutine(flashRoutine);

        flashRoutine = StartCoroutine(DamageFlashRoutine());
    }

    private IEnumerator DamageFlashRoutine()
    {
        crosshairImage.color = hitFlashColor;
        yield return new WaitForSeconds(0.15f);
        crosshairImage.color = defaultColor;
    }
}
