using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using UnityEditor;
using UnityEngine;
namespace AssetList
{
    public static class AssetsListGenerator
    {
        public static void SaveFileList(bool includeResource)
        {
            List<AssetCollection> assetCollectionList = new List<AssetCollection>()
            {
                new AssetCollection()
                {
                    rootDirectory = Path.Combine(BuildConst.ProjectPath, BuildConst.FullPathForUploadingData),
                    savePath = $"{BuildConst.ProjectPath}/{BuildConst.FullPathForUploadingData}/{ApplicationConst.ListFile}",
                }
            };
            if (includeResource)
            {
                assetCollectionList.Add(new AssetCollection()
                {
                    rootDirectory = Path.Combine(BuildConst.ProjectPath, BuildConst.FullPathForUploadingDataRes),
                    savePath = $"{BuildConst.ProjectPath}/{BuildConst.FullPathForUploadingDataRes}/{ApplicationConst.ListFile}",
                });
            }

            foreach (var curAssetCollection in assetCollectionList)
            {
                curAssetCollection.GenerateList();
                curAssetCollection.Redirect();
                curAssetCollection.SaveInfosToFile();
            }
        }
    }
}
