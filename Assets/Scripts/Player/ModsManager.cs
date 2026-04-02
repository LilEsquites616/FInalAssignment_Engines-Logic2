using UnityEngine;

public class ModsManager : MonoBehaviour
{
    public static ModsManager Instance;
    public bool moveSpeedActive = false;
    public bool ammoActive = false;
    public bool hpActive = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    public void ActivateMoveSpeed()
    {
        moveSpeedActive = true;
        Debug.Log("Move Speed Power Activated");
    }

    public void ActivateAmmo()
    {
        ammoActive = true;
        Debug.Log("Ammo Power Activated");
    }

    public void ActivateHP()
    {
        hpActive = true;
        Debug.Log("HP Power Activated");
    }
    public void ResetAllPowers()
    {
        moveSpeedActive = false;
        ammoActive = false;
        hpActive = false;

        Debug.Log("All powers reset");
    }
}