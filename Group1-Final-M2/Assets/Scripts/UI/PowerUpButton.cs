using UnityEngine;
using UnityEngine.UI;

public class PowerUpButton : MonoBehaviour
{
    public enum PowerType
    {
        MoveSpeed,
        Ammo,
        HP
    }

    public PowerType powerType;
    public int powerCost = 500;

    public void ActivatePower()
    {
        if (ChipManager.Instance.chipCount >= powerCost)
        {
            ChipManager.Instance.UpdateChipCount(-powerCost);

            switch (powerType)
            {
                case PowerType.MoveSpeed:
                    ModsManager.Instance.ActivateMoveSpeed();
                    break;
                case PowerType.Ammo:
                    ModsManager.Instance.ActivateAmmo();
                    break;
                case PowerType.HP:
                    ModsManager.Instance.ActivateHP();
                    break;
            }
            AnalyticsManager.Instance.LogModBought(powerType.ToString());

            Debug.Log(powerType + " activated!");
        }
        else
        {
            Debug.Log("Not enough chips to activate " + powerType);
        }
    }
}