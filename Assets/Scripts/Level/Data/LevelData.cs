using System;
using System.Collections.Generic;
using UnityEngine;

namespace _1_LEVEL_REWORK.New.Data
{
    [CreateAssetMenu(menuName = "Level/LevelData")]
    public class LevelData : ScriptableObject
    {
        [SerializeField] private string levelName;
        [SerializeField] private Sprite icon;
        [SerializeField] private DetailData ground;
        [SerializeField] private List<DetailData> details = new();
        
        public string LevelName => levelName;
        
        public Sprite Icon => icon;

        public DetailData Ground => ground;

        public List<DetailData> Details => details;
    }
}