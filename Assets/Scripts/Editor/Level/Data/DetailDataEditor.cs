using System.Collections.Generic;
using _1_LEVEL_REWORK.New.Instances;
using UnityEditor;
using UnityEngine;

namespace _1_LEVEL_REWORK.New.Data.Editor
{
    [CustomEditor(typeof(DetailData))]
    [CanEditMultipleObjects]
    public class DetailDataEditor : UnityEditor.Editor
    {
        private readonly List<List<bool>> _showConstraints = new();
        private readonly List<bool> _showPoints = new();

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            var detail = (DetailData)target;

            DrawParameters(detail);
            DrawPoints(detail);

            if (GUI.changed)
                EditorUtility.SetDirty(detail);

            serializedObject.ApplyModifiedProperties();
        }

        private static void DrawParameters(DetailData detail)
        {
            EditorGUILayout.LabelField("Detail", EditorStyles.boldLabel);
            EditorGUILayout.Space(5);

            EditorGUILayout.LabelField("Id", detail.Id);
            detail.name = EditorGUILayout.TextField("Name", detail.name);
            detail.Prefab =
                (DetailPrefab)EditorGUILayout.ObjectField("Prefab", detail.Prefab, typeof(DetailPrefab), false);
            detail.Icon = (Sprite)EditorGUILayout.ObjectField("Icon", detail.Icon, typeof(Sprite), false);
            detail.Count = EditorGUILayout.IntField("Count", detail.Count);
        }

        private void DrawPoints(DetailData detail)
        {
            EditorGUILayout.Space(10);
            EditorGUILayout.LabelField("Points", EditorStyles.boldLabel);

            var points = detail.points;
            EnsureListSize(_showPoints, points.Count, true);
            EnsureNestedListSize(_showConstraints, points.Count);

            for (var i = 0; i < points.Count; i++)
            {
                var point = points[i];
                _showPoints[i] = EditorGUILayout.Foldout(_showPoints[i], $"Point {i}", true);
                if (_showPoints[i])
                    DrawPoint(point, _showConstraints[i]);
            }

            if (GUILayout.Button("Add Point"))
                points.Add(new PointData());

            if (GUILayout.Button("Remove Point"))
                if (points.Count > 0)
                    points.RemoveAt(points.Count - 1);
        }

        private static void DrawPoint(PointData point, List<bool> showConstrains)
        {
            EditorGUI.indentLevel++;

            point.position = EditorGUILayout.Vector3Field("Position", point.position);
            point.rotation = EditorGUILayout.Vector3Field("Rotation", point.rotation);

            DrawConstrains(point.constraints, showConstrains);

            EditorGUI.indentLevel--;
        }

        private static void DrawConstrains(List<ParentConstraint> constraints, List<bool> showConstrains)
        {
            EditorGUILayout.Space(5);
            EditorGUILayout.LabelField("Constraints", EditorStyles.miniBoldLabel);

            EnsureListSize(showConstrains, constraints.Count, false);

            for (var j = 0; j < constraints.Count; j++)
            {
                var constraint = constraints[j];
                showConstrains[j] = EditorGUILayout.Foldout(showConstrains[j], $"Constraint {j}", true);
                if (showConstrains[j])
                {
                    EditorGUI.indentLevel++;

                    DrawConstraint(constraint);

                    EditorGUI.indentLevel--;
                }
            }

            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(EditorGUI.indentLevel * 30);

            if (GUILayout.Button("Add Constraint"))
                constraints.Add(new ParentConstraint());
            if (GUILayout.Button("Remove Constraint"))
                if (constraints.Count > 0)
                    constraints.RemoveAt(constraints.Count - 1);

            EditorGUILayout.EndHorizontal();
        }

        private static void DrawConstraint(ParentConstraint constraint)
        {
            constraint.ParentDetail = (DetailData)EditorGUILayout.ObjectField(
                "Parent Detail",
                constraint.ParentDetail,
                typeof(DetailData),
                false
            );

            if (constraint.ParentDetail == null)
                return;

            var parentPoints = constraint.ParentDetail.points;

            EditorGUILayout.Space(5);
            EditorGUILayout.LabelField("Parent Points", EditorStyles.miniBoldLabel);

            for (var i = 0; i < parentPoints.Count; i++)
            {
                var point = parentPoints[i];

                EditorGUILayout.BeginVertical("box");

                EditorGUILayout.BeginHorizontal();
                GUILayout.Space(5);

                var selected = constraint.ContainsIndex(i);
                var newSelected = GUILayout.Toggle(selected, GUIContent.none, GUILayout.Width(20));

                switch (newSelected)
                {
                    case true when !selected:
                        constraint.AddIndex(i);
                        break;
                    case false when selected:
                        constraint.RemoveIndex(i);
                        break;
                }

                EditorGUILayout.LabelField($"Point {i}", EditorStyles.boldLabel);
                EditorGUILayout.EndHorizontal();

                using (new EditorGUI.DisabledScope(true))
                {
                    EditorGUI.indentLevel++;
                    EditorGUILayout.Vector3Field("Position", point.position);
                    EditorGUILayout.Vector3Field("Rotation", point.rotation);
                    EditorGUI.indentLevel--;
                }

                EditorGUILayout.EndVertical();
            }
        }

        private static void EnsureListSize(List<bool> list, int size, bool defaultValue)
        {
            while (list.Count < size)
                list.Add(defaultValue);
            while (list.Count > size)
                list.RemoveAt(list.Count - 1);
        }

        private static void EnsureNestedListSize(List<List<bool>> list, int size)
        {
            while (list.Count < size)
                list.Add(new List<bool>());
            while (list.Count > size)
                list.RemoveAt(list.Count - 1);
        }
    }
}