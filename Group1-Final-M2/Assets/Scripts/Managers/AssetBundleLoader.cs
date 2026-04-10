using UnityEngine;
using UnityEngine.UI;

using System.IO;
using System.Collections;
using System.Collections.Generic;
public class AssetBundleLoader : MonoBehaviour
{
    private sealed class BundleHandle
    {
        public AssetBundle Bundle;
        public int RefCount;
    }

    private static readonly Dictionary<string, BundleHandle> LoadedBundles = new();
    private static readonly HashSet<string> BundlesLoading = new();

    public string loadWhat;
    public string bundleName;
    public string variantName;
    public string [] assetNames;
    public Image [] icons;
    public bool loadOnStart;

    void Start()
    {
        if (loadOnStart)
            StartCoroutine(LoadBundleFromURL());
    }

    private IEnumerator LoadBundleFromURL()
    {
        string bundlePath = GetBundlePath();
        AssetBundle bundle = null;

        yield return StartCoroutine(AcquireBundle(bundlePath, loadedBundle => bundle = loadedBundle));

        if (bundle == null)
        {
            yield break;
        }

        int assetCount = Mathf.Min(
            assetNames != null ? assetNames.Length : 0,
            icons != null ? icons.Length : 0);

        if (assetCount == 0)
        {
            Debug.LogWarning($"No assets configured to load from bundle {bundleName} on {name}.");
            ReleaseBundle(bundlePath);
            yield break;
        }

        if (assetNames.Length != icons.Length)
        {
            Debug.LogWarning($"AssetBundleLoader on {name} has mismatched assetNames ({assetNames.Length}) and icons ({icons.Length}) lengths. Loading the first {assetCount} entries.");
        }

        for (int i = 0; i < assetCount; i++)
        {
            yield return StartCoroutine(LoadSpritesFromBundle(bundle, assetNames[i], icons[i]));
        }

        ReleaseBundle(bundlePath);
    }

    private IEnumerator LoadSpritesFromBundle(AssetBundle bundle, string assetName, Image icon)
    {
        if (bundle == null)
        {
            Debug.LogError($"Cannot load {assetName} because bundle {bundleName} is null.");
            yield break;
        }

        if (icon == null)
        {
            Debug.LogWarning($"Icon target for {assetName} on {name} is missing.");
            yield break;
        }

        AssetBundleRequest bundleRequest = bundle.LoadAssetAsync<Sprite>(assetName);
        yield return bundleRequest;

        if (bundleRequest.asset != null)
        {
            icon.sprite = (Sprite)bundleRequest.asset;
            Debug.Log($"Loaded {assetName} from {bundleName}");
        }
        else
        {
            Debug.LogError($"Failed to load {assetName} from {bundleName}");
        }
    }

    public void TriggerLoadFromOtherScript()
    {
        StartCoroutine(LoadBundleFromURL());
    }

    private string GetBundlePath()
    {
        string fileName = string.IsNullOrWhiteSpace(variantName)
            ? bundleName
            : $"{bundleName}.{variantName}";

        return Path.Combine(Application.streamingAssetsPath, fileName);
    }

    private IEnumerator AcquireBundle(string bundlePath, System.Action<AssetBundle> onLoaded)
    {
        while (BundlesLoading.Contains(bundlePath))
        {
            yield return null;
        }

        if (LoadedBundles.TryGetValue(bundlePath, out BundleHandle existingHandle) && existingHandle.Bundle != null)
        {
            existingHandle.RefCount++;
            onLoaded?.Invoke(existingHandle.Bundle);
            yield break;
        }

        if (!File.Exists(bundlePath))
        {
            Debug.LogError($"AssetBundle not found at path: {bundlePath}");
            onLoaded?.Invoke(null);
            yield break;
        }

        BundlesLoading.Add(bundlePath);

        AssetBundleCreateRequest request = AssetBundle.LoadFromFileAsync(bundlePath);
        yield return request;

        BundlesLoading.Remove(bundlePath);

        if (request.assetBundle == null)
        {
            Debug.LogError($"Failed to load AssetBundle from path: {bundlePath}");
            onLoaded?.Invoke(null);
            yield break;
        }

        LoadedBundles[bundlePath] = new BundleHandle
        {
            Bundle = request.assetBundle,
            RefCount = 1
        };

        onLoaded?.Invoke(request.assetBundle);
    }

    private void ReleaseBundle(string bundlePath)
    {
        if (!LoadedBundles.TryGetValue(bundlePath, out BundleHandle handle))
        {
            return;
        }

        handle.RefCount--;

        if (handle.RefCount > 0)
        {
            return;
        }

        handle.Bundle.Unload(false);
        LoadedBundles.Remove(bundlePath);
    }
}
