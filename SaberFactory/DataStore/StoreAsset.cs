﻿using System.IO;
using SaberFactory.Helpers;
using UnityEngine;

namespace SaberFactory.DataStore
{
    /// <summary>
    ///     Keeps information about the origin of the asset
    /// </summary>
    public class StoreAsset
    {
        public readonly AssetBundle AssetBundle;
        public readonly string Extension;
        public readonly string Name;
        public readonly string NameWithoutExtension;
        public readonly string RelativePath;
        public readonly string SubDirName;
        public readonly bool IsStoredOnDisk;

        public GameObject Prefab;

        public StoreAsset(string relativePath, GameObject prefab, AssetBundle assetBundle)
        {
            RelativePath = relativePath;
            IsStoredOnDisk = !relativePath.StartsWith("External");
            Name = Path.GetFileName(RelativePath);
            NameWithoutExtension = Path.GetFileNameWithoutExtension(Name);
            Extension = Path.GetExtension(Name);
            SubDirName = PathTools.GetSubDir(relativePath);

            Prefab = prefab;
            AssetBundle = assetBundle;
        }

        public void Unload()
        {
            if (AssetBundle)
            {
                AssetBundle.Unload(true);
            }
        }
    }
}