using System;
using System.Collections.Generic;
using UnityEngine;

namespace _1_LEVEL_REWORK.New.Data
{
    [Serializable]
    public class ParentConstraint
    {
        [SerializeField] private DetailData parentDetail;
        [SerializeField] private List<int> parentPointIndexes = new();

        public DetailData ParentDetail
        {
            get => parentDetail;
            set => parentDetail = value;
        }

        public List<int> ParentPointIndexes => parentPointIndexes;

        public bool ContainsIndex(int index) => parentPointIndexes.Contains(index);

        public void AddIndex(int index)
        {
            if (!parentPointIndexes.Contains(index))
                parentPointIndexes.Add(index);
        }

        public void RemoveIndex(int index)
        {
            parentPointIndexes.Remove(index);
        }
    }
}