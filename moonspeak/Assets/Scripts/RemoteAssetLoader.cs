using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;


public static class AssetLabels
{
    public static string Remote = "remote"; // common label for all remote assets
    public static string LearningObjects = "learning_objects";
    public static string PlayerProjectiles = "player_projectiles";
    public static string BossProjectiles = "boss_projectiles";
}

public class RemoteAssetLoader : MonoBehaviour
{
    public static RemoteAssetLoader Instance;
    private Dictionary<string, List<GameObject>> _assetMap;

    private void Awake()
    {
        Instance = this;
        Instance._assetMap = new Dictionary<string, List<GameObject>>();
        DontDestroyOnLoad(Instance);
    }

    private async void Start()
    {
        // await LoadAllAssets();
    }

    public async Task LoadAllAssets()
    {
        await LoadAndStoreAssets(AssetLabels.LearningObjects);
        await LoadAndStoreAssets(AssetLabels.PlayerProjectiles);
        await LoadAndStoreAssets(AssetLabels.BossProjectiles);
    }

    private async Task LoadAndStoreAssets(string label)
    {
        var locations = await Addressables.LoadResourceLocationsAsync(label).Task;
        List<GameObject> assets = new List<GameObject>();
        foreach (var location in locations)
        {
            var prefab = await Addressables.LoadAssetAsync<GameObject>(location).Task;
            assets.Add(prefab);
        }
        _assetMap.Add(label, new List<GameObject>(assets));
    }

    public List<GameObject> GetAssets(string assetLabel)
    {
        return _assetMap.TryGetValue(assetLabel, out var assets) ? new List<GameObject>(assets) : null;
    }
}