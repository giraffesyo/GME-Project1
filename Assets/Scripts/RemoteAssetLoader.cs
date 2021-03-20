using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class RemoteAssetLoader : MonoBehaviour
{
    public static RemoteAssetLoader Instance;
    private string _label = "remote";
    private List<GameObject> _assets;
    public List<GameObject> Assets => new List<GameObject>(_assets);
    private void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(Instance);
    }

    private async void Start()
    {
        await LoadAllAssets(_label);
    }

    private async Task LoadAllAssets(string label)
    {
        _assets = new List<GameObject>();
        var locations = await Addressables.LoadResourceLocationsAsync(label).Task;

        foreach (var location in locations)
        {
            var prefab = await Addressables.LoadAssetAsync<GameObject>(location).Task;
            await Addressables.InstantiateAsync(location).Task;
            _assets.Add(prefab);
        }
    }
}