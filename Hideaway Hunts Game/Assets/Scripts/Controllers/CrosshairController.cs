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
    public float detectionDistance = 30f;
    public LayerMask BotLayer;

    private Camera cam;
    private Coroutine flashRoutine;

    void Start()
    {
        cam = Camera.main;

        if (cam == null)
        {
            Debug.LogError("❌ No MainCamera found! Make sure camera tag = MainCamera");
        }
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
            BotLayer
        );

        // Change crosshair color
        crosshairImage.color = isTarget ? targetColor : defaultColor;

        // Shoot
        if (Input.GetMouseButtonDown(0) && isTarget)
        {

            Killable killable = hit.transform.GetComponent<Killable>();

            if (killable != null)
            {
                killable.TakeHit();
            }

        }
    }

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
