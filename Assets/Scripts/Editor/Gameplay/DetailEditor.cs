using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Gameplay.Editor
{
    [CustomEditor(typeof(Detail))]
    [CanEditMultipleObjects]
    public class DetailEditor : UnityEditor.Editor
    {
        private bool _showPoints, _showParentPoints = true;
        private List<bool> _showPointList, _showParentDetailList, _showParentPointList, _showParentsList;

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            var detail = (Detail) target;
            _showPointList = detail.showPointList;
            _showParentsList = detail.showParentsList;
            
            DrawParameters(detail);

            _showPoints = EditorGUILayout.Foldout(_showPoints, "Points", true);
            if (_showPoints)
            {
                EditorGUI.indentLevel++;
                var pointsListSp = serializedObject.FindProperty("points");

                var pointParentConnectorList = detail.points;
                //TODO Focus on field size
                EditorGUI.BeginChangeCheck();
                var pointParentConnectorSize = Mathf.Max(0, EditorGUILayout.IntField("Size", pointParentConnectorList.Count));
                if (EditorGUI.EndChangeCheck()) 
                {
                    EditorUtility.SetDirty(target);
                }

                if (pointParentConnectorList.Count != pointParentConnectorSize)
                {
                    while (pointParentConnectorSize > pointParentConnectorList.Count)
                    {
                        pointParentConnectorList.Add(null);
                    }
                    while (pointParentConnectorSize < pointParentConnectorList.Count)
                    {
                        pointParentConnectorList.RemoveAt(pointParentConnectorList.Count - 1);
                    }
                }
                if (pointParentConnectorSize != _showPointList.Count)
                {
                    while (pointParentConnectorSize > _showPointList.Count)
                    {
                        _showPointList.Add(false);
                        _showParentsList.Add(false);
                    }
                    while (pointParentConnectorSize < _showPointList.Count)
                    {
                        _showPointList.RemoveAt(_showPointList.Count - 1);
                        _showParentsList.RemoveAt(_showParentsList.Count - 1);
                    }
                }

                serializedObject.Update();
                for (var i = 0; i < pointParentConnectorList.Count; i++)
                {
                    var pointSP = pointsListSp.GetArrayElementAtIndex(i).FindPropertyRelative("point");
                    var parentListSP = pointsListSp.GetArrayElementAtIndex(i).FindPropertyRelative("parentList");

                    _showPointList[i] = EditorGUILayout.Foldout(_showPointList[i],new GUIContent("Point " + i), true);
                    if (_showPointList[i])
                    {   
                        EditorGUI.indentLevel++;
                        EditorGUI.BeginChangeCheck();
                        GUILayout.BeginHorizontal();
                        GUILayout.BeginVertical();
                        pointParentConnectorList[i].point.position = EditorGUILayout.Vector3Field("Pos", pointParentConnectorList[i].point.position);
                        pointParentConnectorList[i].point.rotation = EditorGUILayout.Vector3Field("Rot" , pointParentConnectorList[i].point.rotation);
                        GUILayout.EndVertical();
                        if (GUILayout.Button("C", GUILayout.ExpandHeight(true), GUILayout.ExpandWidth(false)))
                        {
                            var objectTransform = detail.gameObject.GetComponent<Transform>();
                            pointParentConnectorList[i].point.position = objectTransform.position;
                            pointParentConnectorList[i].point.rotation = objectTransform.rotation.eulerAngles;
                        }
                        GUILayout.EndHorizontal();
                        if (EditorGUI.EndChangeCheck()) 
                        {
                            EditorUtility.SetDirty(target);
                        }
                        _showParentsList[i] = EditorGUILayout.Foldout(_showParentsList[i], "Parents", true);
                        if(_showParentsList[i])
                        {
                            EditorGUI.indentLevel++;
                            EditorGUI.BeginChangeCheck();
                            _showParentDetailList = detail.points[i].showParentDetailList;
                            var parentListSize = Mathf.Max(0, EditorGUILayout.IntField("Size", pointParentConnectorList[i].parentList.Count));
                            if(EditorGUI.EndChangeCheck()) 
                            {
                                EditorUtility.SetDirty(target);
                            }
                            if (parentListSize != _showParentDetailList.Count)
                            {
                                while (parentListSize > _showParentDetailList.Count)
                                {
                                    _showParentDetailList.Add(false);
                                }
                                while (parentListSize < _showParentDetailList.Count)
                                {
                                    _showParentDetailList.RemoveAt(_showParentDetailList.Count - 1);
                                }
                            }
                            if (pointParentConnectorList[i].parentList.Count != parentListSize)
                            {
                                while (parentListSize > pointParentConnectorList[i].parentList.Count)
                                {
                                    pointParentConnectorList[i].parentList.Add(null);
                                }
                                while (parentListSize < pointParentConnectorList[i].parentList.Count)
                                {
                                    pointParentConnectorList[i].parentList.RemoveAt(pointParentConnectorList[i].parentList.Count - 1);
                                }
                            }

                            serializedObject.Update();
                            for (var j = 0; j < pointParentConnectorList[i].parentList.Count; j++)
                            {
                                var parentDetailSP = parentListSP.GetArrayElementAtIndex(j).FindPropertyRelative("parentDetail");
                                var parentPointListSP = parentListSP.GetArrayElementAtIndex(j).FindPropertyRelative("parentPointList");
                                var parentPPCFlags = parentListSP.GetArrayElementAtIndex(j).FindPropertyRelative("parentPPCFlags");

                                _showParentDetailList[j] = EditorGUILayout.Foldout(_showParentDetailList[j], new GUIContent("Parent Detail Points " + j), true);
                                if (_showParentDetailList[j])
                                { 
                                    EditorGUI.indentLevel++;                           
                                    EditorGUI.BeginChangeCheck();
                                    EditorGUILayout.PropertyField(parentDetailSP, new GUIContent("Parent" + "_" + j), true);
                                    
                                    if (EditorGUI.EndChangeCheck())
                                    {
                                        EditorUtility.SetDirty(target);
                                    }

                                    var ob = parentDetailSP.objectReferenceValue as Detail;
                                    pointParentConnectorList[i].parentList[j].parentDetail = ob;
                                    if (ob && ob.points != null)
                                    {
                                        EditorGUI.BeginChangeCheck(); 
                                    
                                        _showParentPointList = detail.points[i].parentList[j].showParentPointList;   
                                        var thisParentPointList = detail.points[i].parentList[j].parentPointList;
                                        var checkToggleList = detail.points[i].parentList[j].checkToggleList;
                                        var parentPointListSize = ob.points.Count;
                                        
                                        if(EditorGUI.EndChangeCheck())
                                        {
                                            EditorUtility.SetDirty(target);
                                        }
                                        if (parentPointListSize != _showParentPointList.Count)
                                        {
                                            while (parentPointListSize > _showParentPointList.Count)
                                            {
                                                _showParentPointList.Add(false);
                                                checkToggleList.Add(false);
                                            }
                                            while (parentPointListSize < _showParentPointList.Count)
                                            {
                                                _showParentPointList.RemoveAt(_showParentPointList.Count - 1);
                                                checkToggleList.RemoveAt(checkToggleList.Count - 1);
                                            }
                                        }
                                        _showParentPoints = EditorGUILayout.Foldout(_showParentPoints, "Parent", true);
                                        if (_showParentPoints)
                                        {
                                            EditorGUI.indentLevel++;
                                            serializedObject.Update();
                                            for (var k = 0; k < ob.points.Count; k++)
                                            {
                                                _showParentPointList[k] = EditorGUILayout.Foldout(_showParentPointList[k], ("\"" + ob.gameObject.name + "\"" + " point " + k), true);
                                                if (_showParentPointList[k])
                                                {  
                                                    EditorGUI.BeginChangeCheck();
                                                    GUILayout.BeginHorizontal();
                                                    GUILayout.BeginVertical();   
                                                    EditorGUI.BeginChangeCheck();
                                                    EditorGUI.BeginDisabledGroup(true);
                                                    EditorGUILayout.Vector3Field("Pos", ob.points[k].point.position);
                                                    EditorGUILayout.Vector3Field("Rot", ob.points[k].point.rotation);
                                                    EditorGUI.EndDisabledGroup();
                                                    GUILayout.EndVertical();
                                                    checkToggleList[k] = GUILayout.Toggle(checkToggleList[k], "");                                  
                                                    GUILayout.EndHorizontal();
                                                    if (EditorGUI.EndChangeCheck()) 
                                                    {
                                                        if(checkToggleList[k])
                                                        {
                                                            thisParentPointList.Add(ob.points[k].point);
                                                        }
                                                        else
                                                        {
                                                            for (var l = 0; l < thisParentPointList.Count; l++)
                                                            {
                                                                if (thisParentPointList[l].position == ob.points[k].point.position &&
                                                                   thisParentPointList[l].rotation == ob.points[k].point.rotation)
                                                                {
                                                                    thisParentPointList.RemoveAt(l);
                                                                }
                                                            }
                                                        }
                                                        EditorUtility.SetDirty(target);
                                                    }
                                                }
                                            }
                                            EditorGUI.indentLevel--;
                                        }
                                    }
                                    EditorGUI.indentLevel--;
                                }
                            }
                            EditorGUI.indentLevel--;
                        }
                        EditorGUI.indentLevel--;
                    }
                } 
                EditorGUI.indentLevel--;  
            }
        
            serializedObject.ApplyModifiedProperties();
        }
        
        private static void DrawParameters(Detail detail)
        {
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.LabelField("Detail", EditorStyles.boldLabel);

            EditorGUILayout.Space(10f);
            EditorGUILayout.LabelField("Detail features");

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Count");
            detail.count = EditorGUILayout.IntField(detail.count);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Prefab");
            detail.prefab = (GameObject)EditorGUILayout.ObjectField(detail.prefab, typeof(GameObject), true);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Icon");
            detail.icon = (Sprite)EditorGUILayout.ObjectField(detail.icon, typeof(Sprite), true);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Is Root");
            detail.isRoot = EditorGUILayout.Toggle(detail.isRoot);
            EditorGUILayout.EndHorizontal();

            if (EditorGUI.EndChangeCheck())
            {
                EditorUtility.SetDirty(detail);
            }
        }
    }
}