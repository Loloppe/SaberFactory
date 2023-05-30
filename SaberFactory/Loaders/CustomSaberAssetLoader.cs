﻿using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using AssetBundleLoadingTools.Utilities;
using CustomSaber;
using HarmonyLib;
using IPA.Utilities;
using SaberFactory.DataStore;
using SaberFactory.Helpers;
using UnityEngine;

namespace SaberFactory.Loaders
{
    internal class CustomSaberAssetLoader : AssetBundleLoader
    {
        public override string HandledExtension => ".saber";

        public override ISet<AssetMetaPath> CollectFiles(PluginDirectories dirs)
        {
            var paths = new HashSet<AssetMetaPath>();

            foreach (var path in dirs.CustomSaberDir.EnumerateFiles("*.saber", SearchOption.AllDirectories))
            {
                paths.Add(new AssetMetaPath(path, dirs.Cache.GetFile(path.Name+".meta").FullName));
            }

            return paths;
        }

        public override async Task<StoreAsset> LoadStoreAssetAsync(string relativePath)
        {
            var fullPath = PathTools.ToFullPath(relativePath);
            if (!File.Exists(fullPath))
            {
                return null;
            }

            var result = await Readers.LoadAssetFromAssetBundleSafeAsync<GameObject>(fullPath, "_CustomSaber");
            if (result == null)
            {
                return null;
            }

            var info = await ShaderRepair.FixShadersOnGameObjectAsync(result.Item1);
            if (!info.AllShadersReplaced)
            {
                Debug.LogWarning($"Missing shader replacement data for {relativePath}:");
                foreach (var shaderName in info.MissingShaderNames)
                {
                    Debug.LogWarning($"\t- {shaderName}");
                }
            }


            var trailsList = result.Item1.GetComponentsInChildren<CustomTrail>();
            var matDict = new Dictionary<Material, List<CustomTrail>>();
            
            foreach (var trail in trailsList)
            {
                if (!trail.TrailMaterial)
                {
                    continue;
                }
                
                if (!matDict.ContainsKey(trail.TrailMaterial))
                {
                    matDict.Add(trail.TrailMaterial, new List<CustomTrail>());
                }
                
                matDict[trail.TrailMaterial].Add(trail);
            }
            
            foreach (var (mat, trails) in matDict)
            {
                var trailInfo = await ShaderRepair.FixShaderOnMaterialAsync(mat);

                if (!trailInfo.AllShadersReplaced)
                {
                    Debug.LogWarning("Missing trail shader replacement data. Using default trail");
                    trails.Do(x=>x.TrailMaterial = null);
                }
            }
            
            return new StoreAsset(relativePath, result.Item1, result.Item2);
        }

        public override async Task<StoreAsset> LoadStoreAssetFromBundleAsync(AssetBundle bundle, string saberName)
        {
            var result = await bundle.LoadAssetFromAssetBundleAsync<GameObject>("_CustomSaber");
            if (result == null)
            {
                return null;
            }

            return new StoreAsset("External\\" + saberName, result, bundle);
        }
    }
}