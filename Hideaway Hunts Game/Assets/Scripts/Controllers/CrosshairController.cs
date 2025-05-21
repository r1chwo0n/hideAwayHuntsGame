using UnityEngine;
using UnityEngine.UI;

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

        if (Physics.Raycast(ray, out hit, detectionDistance, enemyLayer))
        {
            crosshairImage.color = targetColor;
        }
        else
        {
            crosshairImage.color = defaultColor;
        }
    }
}
