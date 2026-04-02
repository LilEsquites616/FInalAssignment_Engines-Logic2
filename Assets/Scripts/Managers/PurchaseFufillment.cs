using System.Diagnostics;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.Purchasing.Extension;
using TMPro;
using UnityEngine.UI;
public class PurchaseFufillment : MonoBehaviour
{
    private const string CHIPS_500 = "buy500chipcoin";
    private const string CHIPS_1000 = "buy1000chipcoin";
    public ChipManager chipManager;
   public void OnConfirmedOrder(ConfirmedOrder confirmedOrder)
    {
        var purchasedProductInfo = confirmedOrder.Info.PurchasedProductInfo;

        foreach (IPurchasedProductInfo info in purchasedProductInfo)
        {
            switch (info.productId)
            {
                case CHIPS_500:
                    GrantsGems(500);
                    AnalyticsManager.Instance.LogChipPurchase(500);
                    break;
                case CHIPS_1000:
                    GrantsGems(1000);
                    AnalyticsManager.Instance.LogChipPurchase(1000);
                    break;
            }
        }
        
    }
    public void OnFailedOrder(FailedOrder failedOrder)
    {
        var purchaseProductInfo = failedOrder.Info.PurchasedProductInfo;
        string items = string.Empty;

        foreach (IPurchasedProductInfo info in purchaseProductInfo)
        {
            items += ' ' + info.productId;
        }

        UnityEngine.Debug.Log($"Failed to purchase the following items:{items}");
        UnityEngine.Debug.Log($"Reason: '{failedOrder.FailureReason}', Details: '{failedOrder.Details}'");
    }
    private void GrantsGems(int chipAmount)
    {
        chipManager.UpdateChipCount(chipAmount);
        UnityEngine.Debug.Log($"You purchased {chipAmount} chipcoin.");
    }
}
