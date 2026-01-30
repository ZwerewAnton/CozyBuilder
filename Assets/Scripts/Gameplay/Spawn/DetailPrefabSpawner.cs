using System.Collections.Generic;
using Gameplay.Spawn;
using UnityEngine;

namespace Gameplay
{
    public class DetailPrefabSpawner : MonoBehaviour
    {
        [SerializeField] private Transform rootObject;

        public void SpawnPrefabs(List<DetailPrefabSpawnInfo> spawnInfoList)
        {
            spawnInfoList.ForEach(SpawnPrefab);
        }

        public void SpawnPrefab(DetailPrefabSpawnInfo spawnInfo)
        {
            Instantiate(spawnInfo.Prefab, spawnInfo.Position, spawnInfo.Rotation, rootObject);
        }
    }
}