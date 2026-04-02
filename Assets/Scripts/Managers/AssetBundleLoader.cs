using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

using System.IO;
using System.Collections;
public class AssetBundleLoader : MonoBehaviour
{
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
        string extension = string.IsNullOrEmpty(variantName) ? string.Empty : '.'+variantName;
        string url = Path.Combine(Application.streamingAssetsPath, bundleName,extension);

        using UnityWebRequest webRequest = UnityWebRequestAssetBundle.GetAssetBundle(url);
        yield return webRequest.SendWebRequest();
        if(webRequest.result!=UnityWebRequest.Result.Success)
        {
            Debug.LogError($"Failed to download AssetBundle: {webRequest.error}");
            yield break;
        }
        AssetBundle bundle = DownloadHandlerAssetBundle.GetContent(webRequest);
        for (int i =0; i<assetNames.Length; i++)
        {
            yield return StartCoroutine(LoadSpritesFromBundle(bundle,assetNames[i],icons[i]));
        }
        bundle.Unload(false);
    }

    private IEnumerator LoadSpritesFromBundle(AssetBundle bundle, string assetName, Image icon)
    {
        AssetBundleRequest bundleRequest = bundle.LoadAssetAsync<Sprite>(assetName);
        yield return bundleRequest;
        if (bundleRequest.asset!=null)
        {
            icon.sprite = (Sprite)bundleRequest.asset;
            Debug.Log($"Loaded {assetName} from {bundleName}");
        }
        else Debug.LogError($"Failed to load {assetName} from {bundleName}");
    }
    public void TriggerLoadFromOtherScript()
    {
        StartCoroutine(LoadBundleFromURL());
    }
}
