using UnityEngine;

public class FullMapToggle : MonoBehaviour
{
    public GameObject fullMapUI;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            fullMapUI.SetActive(!fullMapUI.activeSelf);
        }
    }
}