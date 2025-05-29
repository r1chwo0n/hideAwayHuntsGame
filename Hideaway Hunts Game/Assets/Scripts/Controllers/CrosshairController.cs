using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CrosshairController : MonoBehaviour
{
    public Image crosshairImage;
    public Color defaultColor = Color.white;
    public Color targetColor = Color.red;
    public float detectionDistance = 100f;
    public LayerMask enemyLayer;

    void Update()
    {
        Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
        RaycastHit hit;

        bool isTarget = Physics.Raycast(ray, out hit, detectionDistance, enemyLayer);
        crosshairImage.color = isTarget ? targetColor : defaultColor;

        if (Input.GetMouseButtonDown(0) && isTarget)
        {
            BotController enemy = hit.collider.GetComponent<BotController>();
            if (enemy != null)
            {
                enemy.TakeDamage(50); // Damage bot on shoot
            }
        }
    }

    public void FlashDamage()
    {
        StartCoroutine(DamageFlashRoutine());
    }

    private IEnumerator DamageFlashRoutine()
    {
        crosshairImage.color = Color.red;
        yield return new WaitForSeconds(0.2f);
        crosshairImage.color = defaultColor;
    }
}
