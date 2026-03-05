using UnityEngine;

public class AmmoPool : MonoBehaviour
{
    public int maxAmmo = 10;
    public int currentAmmo = 10;

    public float AmmoRatio =>
        maxAmmo > 0 ? (float)currentAmmo / maxAmmo : 0f;

    void Awake()
    {
        currentAmmo = maxAmmo;
    }

    public bool HasAmmo()
    {
        return currentAmmo > 0;
    }

    public void ConsumeAmmo()
    {
        if (currentAmmo > 0)
            currentAmmo--;
    }
}