// Assets/Editor/DetailDataConverterWindow.cs
using System;
using System.Collections.Generic;
using System.Linq;
using Gameplay; // старые классы: Detail, PointParentConnector, Parent, Point
using UnityEditor;
using UnityEngine;
using UnityEditorInternal;
using UnityEditor.Presets;
using _1_LEVEL_REWORK.New.Data; // DetailData, PointData, ParentConstraint
using _1_LEVEL_REWORK.New.Instances; // DetailPrefab

public class DetailDataConverterWindow : EditorWindow
{
    private List<GameObject> prefabs = new List<GameObject>();
    private Vector2 scroll;
    private UnityEngine.Object outputFolderAsset;
    private ReorderableList rList;
    private const float FLOAT_EPS = 0.001f;

    // NEW: folder with original prefabs (variant B: only load prefabs that contain Detail component)
    private UnityEngine.Object originalPrefabsFolder;

    [MenuItem("Tools/Detail Data Converter")]
    public static void OpenWindow()
    {
        var w = GetWindow<DetailDataConverterWindow>("DetailData Converter");
        w.minSize = new Vector2(600, 300);
    }

    private void OnEnable()
    {
        rList = new ReorderableList(prefabs, typeof(GameObject), true, true, true, true);
        rList.drawHeaderCallback = rect => EditorGUI.LabelField(rect, "Prefabs to convert (drag/drop prefab assets here)");
        rList.drawElementCallback = (rect, index, active, focused) =>
        {
            prefabs[index] = (GameObject)EditorGUI.ObjectField(new Rect(rect.x, rect.y + 2, rect.width, EditorGUIUtility.singleLineHeight),
                prefabs[index], typeof(GameObject), false);
        };
        rList.onAddCallback = _ => prefabs.Add(null);
        rList.onRemoveCallback = rl =>
        {
            prefabs.RemoveAt(rl.index);
        };
    }

    private void OnGUI()
    {
        EditorGUILayout.Space();
        EditorGUILayout.HelpBox("Конвертер префабов Detail -> ScriptableObject DetailData\n\n" +
            "1) Перетащи префабы в список или загрузите из папки с кнопки. 2) Выбери папку (Assets/...) для сохранения SO. 3) Нажми Convert.\n\n" +
            "Лог будет содержать предупреждения о неразрешённых родителях/точках для ручной доработки.", MessageType.Info);

        EditorGUILayout.Space();

        scroll = EditorGUILayout.BeginScrollView(scroll);

        // --- NEW: UI for source folder selection and loading prefabs that contain Detail component ---
        EditorGUILayout.LabelField("Original Prefabs Folder (load only prefabs with Detail component)", EditorStyles.boldLabel);
        EditorGUILayout.BeginHorizontal();
        originalPrefabsFolder = EditorGUILayout.ObjectField(originalPrefabsFolder, typeof(DefaultAsset), false);
        if (GUILayout.Button("Load Prefabs from Folder", GUILayout.Width(180)))
        {
            LoadPrefabsFromSelectedFolder();
        }
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.Space();
        // --- end new UI ---

        rList.DoLayoutList();

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Output folder (select a folder inside Assets)", EditorStyles.boldLabel);
        outputFolderAsset = EditorGUILayout.ObjectField(outputFolderAsset, typeof(DefaultAsset), false);

        EditorGUILayout.Space();
        EditorGUI.BeginDisabledGroup(prefabs.Count == 0 || outputFolderAsset == null);
        if (GUILayout.Button("Convert"))
        {
            ConvertAll();
        }
        EditorGUI.EndDisabledGroup();

        EditorGUILayout.EndScrollView();

        EditorGUILayout.Space();
        if (GUILayout.Button("Clear list"))
        {
            prefabs.Clear();
        }
    }

    /// <summary>
    /// Loads all prefab assets from the selected folder (recursively) and adds only those prefabs
    /// that contain a Detail component (variant B). Avoids duplicates.
    /// </summary>
    private void LoadPrefabsFromSelectedFolder()
    {
        if (originalPrefabsFolder == null)
        {
            Debug.LogError("[Converter] Source folder is not selected.");
            return;
        }

        var folderPath = AssetDatabase.GetAssetPath(originalPrefabsFolder);
        if (string.IsNullOrEmpty(folderPath) || !AssetDatabase.IsValidFolder(folderPath))
        {
            Debug.LogError("[Converter] Выбранный объект не является папкой в проекте (Assets/...).");
            return;
        }

        // Find all prefabs in folder (recursively)
        var guids = AssetDatabase.FindAssets("t:Prefab", new[] { folderPath });
        int added = 0, skippedNoDetail = 0, duplicates = 0;

        foreach (var guid in guids)
        {
            var path = AssetDatabase.GUIDToAssetPath(guid);
            var go = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            if (go == null) continue;

            // Only add prefabs that contain the Detail component (on self or children)
            // Use AssetDatabase.LoadAssetAtPath<GameObject> because scene instances might be present otherwise
            var containsDetail = PrefabContainsDetail(go);
            if (!containsDetail)
            {
                skippedNoDetail++;
                continue;
            }

            if (prefabs.Contains(go))
            {
                duplicates++;
                continue;
            }

            prefabs.Add(go);
            added++;
        }

        // Update the ReorderableList reference (safe)
        rList.list = prefabs;
        Repaint();

        Debug.Log($"[Converter] LoadPrefabsFromSelectedFolder: added {added}, skipped (no Detail): {skippedNoDetail}, duplicates: {duplicates} from '{folderPath}'.");
    }

    /// <summary>
    /// Checks whether the prefab asset contains the Detail component on the root or in children.
    /// </summary>
    private static bool PrefabContainsDetail(GameObject prefabAsset)
    {
        if (prefabAsset == null) return false;
        // Use GetComponentInChildren on the loaded prefab asset
        var detail = prefabAsset.GetComponentInChildren<Detail>(true);
        return detail != null;
    }

    private void ConvertAll()
    {
        // Validate folder path
        var outputPath = AssetDatabase.GetAssetPath(outputFolderAsset);
        if (string.IsNullOrEmpty(outputPath) || !AssetDatabase.IsValidFolder(outputPath))
        {
            Debug.LogError("Выбранный объект не является папкой проекта (Assets/...). Выбери папку.");
            return;
        }

        // Prepare lists: keep mapping from prefabName -> original Detail (component inside prefab contents) and to created DetailData
        var mappingNameToDetailComp = new Dictionary<string, Detail>();
        var mappingNameToDetailData = new Dictionary<string, DetailData>();
        var mappingNameToPrefabAsset = new Dictionary<string, GameObject>();
        var prefabToOriginalDetail = new Dictionary<GameObject, Detail>(); // used to fill constraints later
        var warnings = new List<string>();

        // First pass: create all DetailData assets (without parent constraints)
        foreach (var prefab in prefabs)
        {
            if (prefab == null) continue;

            var prefabPath = AssetDatabase.GetAssetPath(prefab);
            if (string.IsNullOrEmpty(prefabPath))
            {
                warnings.Add($"Prefab '{prefab.name}' не является ассетом в проекте. Пропущен.");
                Debug.LogWarning($"[Converter] Prefab '{prefab.name}' skipped: not an asset.");
                continue;
            }

            // Load prefab contents to access/Add components safely
            var prefabStageGO = PrefabUtility.LoadPrefabContents(prefabPath);
            if (prefabStageGO == null)
            {
                warnings.Add($"Не удалось загрузить содержимое префаба '{prefab.name}'.");
                Debug.LogWarning($"[Converter] Failed to load prefab contents for '{prefab.name}'.");
                continue;
            }

            try
            {
                // Try to find Detail component on root or in children
                var detailComp = prefabStageGO.GetComponentInChildren<Detail>(true);
                if (detailComp == null)
                {
                    // Add a Detail component to root if it's missing
                    detailComp = prefabStageGO.AddComponent<Detail>();
                    Debug.LogWarning($"[Converter] В префабе '{prefab.name}' не найден компонент Detail. Добавлен автоматически на корень префаба.");
                    warnings.Add($"Detail отсутствовал у '{prefab.name}' — добавлен автоматически. Проверь заполненные поля (count, icon и т.д.).");
                    // Note: we don't set fields for the new Detail; user may want to edit later.
                }

                // Ensure DetailPrefab component exists on root (or same GameObject)
                var detailPrefabComp = prefabStageGO.GetComponentInChildren<DetailPrefab>(true);
                if (detailPrefabComp == null)
                {
                    // add to root
                    detailPrefabComp = prefabStageGO.AddComponent<DetailPrefab>();
                    Debug.LogWarning($"[Converter] В префабе '{prefab.name}' не найден компонент DetailPrefab. Добавлен автоматически.");
                    warnings.Add($"DetailPrefab отсутствовал у '{prefab.name}' — добавлен автоматически.");
                }

                // Save modifications back to prefab asset if we added components
                PrefabUtility.SaveAsPrefabAsset(prefabStageGO, prefabPath);
            }
            catch (Exception e)
            {
                Debug.LogError($"[Converter] Ошибка при обработке префаба '{prefab.name}': {e}");
                PrefabUtility.UnloadPrefabContents(prefabStageGO);
                continue;
            }

            // Reload the prefab asset as GameObject to get real components from asset
            var prefabAsset = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
            if (prefabAsset == null)
            {
                Debug.LogError($"[Converter] Не удалось загрузить ассет префаба по пути {prefabPath}.");
                PrefabUtility.UnloadPrefabContents(prefabStageGO);
                continue;
            }

            // Get the Detail and DetailPrefab components from the asset
            var detail = prefabAsset.GetComponentInChildren<Detail>(true);
            var detailPrefab = prefabAsset.GetComponentInChildren<DetailPrefab>(true);

            if (detail == null)
            {
                // This shouldn't usually happen since we added earlier, but handle gracefully
                Debug.LogWarning($"[Converter] После сохранения префаба не найден Detail на '{prefab.name}'. Создаём пустую SO с минимальными полями.");
            }

            // Create DetailData instance
            var detailData = ScriptableObject.CreateInstance<DetailData>();
            // id is auto-generated inside DetailData; set other public properties

            if (detail != null)
            {
                detailData.Count = detail.count;
                detailData.Icon = detail.icon;
            }

            if (detailPrefab != null)
            {
                detailData.Prefab = detailPrefab;
            }
            else
            {
                Debug.LogWarning($"[Converter] Для префаба '{prefab.name}' не найден компонент DetailPrefab во время заполнения DetailData.");
                warnings.Add($"DetailPrefab не найден у '{prefab.name}' при создании DetailData.");
            }

            // Try to fill Mesh & Material from prefab asset
            var meshFilter = prefabAsset.GetComponentInChildren<MeshFilter>(true);
            var meshRenderer = prefabAsset.GetComponentInChildren<MeshRenderer>(true);
            if (meshFilter != null)
            {
                detailData.Mesh = meshFilter.sharedMesh;
            }
            if (meshRenderer != null)
            {
                detailData.Material = meshRenderer.sharedMaterial;
            }

            // Fill points: create PointData from old structure
            if (detail != null)
            {
                detailData.points = new List<PointData>();
                if (detail.points != null)
                {
                    foreach (var ppc in detail.points)
                    {
                        var pd = new PointData
                        {
                            position = ppc.point.position,
                            rotation = ppc.point.rotation,
                            constraints = new List<ParentConstraint>()
                        };
                        detailData.points.Add(pd);
                    }
                }
            }

            // Create asset file
            var safeName = MakeSafeAssetName(prefabAsset.name) + ".asset";
            var assetPath = AssetDatabase.GenerateUniqueAssetPath(outputPath + "/" + safeName);
            AssetDatabase.CreateAsset(detailData, assetPath);
            AssetDatabase.SaveAssets();
            AssetDatabase.ImportAsset(assetPath);

            // Save mappings
            mappingNameToDetailData[prefabAsset.name] = detailData;
            mappingNameToPrefabAsset[prefabAsset.name] = prefabAsset; // asset reference for later
            // Also keep original Detail component in prefab contents for matching indices later
            var originalDetail = prefabAsset.GetComponentInChildren<Detail>(true);
            if (originalDetail != null)
            {
                mappingNameToDetailComp[prefabAsset.name] = originalDetail;
                prefabToOriginalDetail[prefabAsset] = originalDetail;
            }
            else
            {
                mappingNameToDetailComp[prefabAsset.name] = null;
            }

            Debug.Log($"[Converter] Создан DetailData для '{prefabAsset.name}' -> '{assetPath}'");
        } // end first pass

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        // Second pass: fill ParentConstraints by resolving parents and mapping points -> indices
        foreach (var kv in mappingNameToDetailData)
        {
            var prefabName = kv.Key;
            var detailData = kv.Value;
            // Need original Detail to read parent relationships (old Parent objects)
            // We'll load the prefab asset (GameObject) and get its Detail component
            if (!mappingNameToPrefabAsset.TryGetValue(prefabName, out var prefabAsset)) continue;
            var originalDetail = prefabAsset.GetComponentInChildren<Detail>(true);
            if (originalDetail == null)
            {
                // nothing to fill
                continue;
            }

            // For each point in originalDetail, look at its PointParentConnector.parentList (List<Parent>)
            if (originalDetail.points == null) continue;
            for (int pointIndex = 0; pointIndex < originalDetail.points.Count; pointIndex++)
            {
                var ppc = originalDetail.points[pointIndex];
                if (ppc == null) continue;

                // Ensure we have a corresponding PointData in the created detailData (should be same count)
                if (pointIndex >= detailData.points.Count)
                {
                    Debug.LogWarning($"[Converter] В '{prefabName}' количество точек в SO меньше, чем в оригинале. Пропускаем точку {pointIndex}.");
                    continue;
                }

                var targetPointData = detailData.points[pointIndex];

                if (ppc.parentList == null) continue;

                foreach (var parent in ppc.parentList)
                {
                    if (parent == null)
                    {
                        continue;
                    }

                    if (parent.parentDetail == null)
                    {
                        warnings.Add($"[{prefabName}] Точка {pointIndex}: Parent.parentDetail == null — пропущено.");
                        Debug.LogWarning($"[Converter] {prefabName} point#{pointIndex}: Parent.parentDetail == null, skipped.");
                        continue;
                    }

                    // Use the parentDetail's GameObject name as ID to find created DetailData
                    var parentGoName = parent.parentDetail.gameObject.name;
                    if (!mappingNameToDetailData.TryGetValue(parentGoName, out var parentDetailData))
                    {
                        // parent not converted (not in provided prefabs) — skip but log for manual fix
                        warnings.Add($"[{prefabName}] Точка {pointIndex}: родитель '{parentGoName}' не найден среди конвертируемых префабов. Пропущено.");
                        Debug.LogWarning($"[Converter] {prefabName} point#{pointIndex}: parent detail '{parentGoName}' not found among converted prefabs. Skipped constraint.");
                        continue;
                    }

                    // Create constraint and fill indices by comparing parent's parentPointList entries to parent's original points
                    var constraint = new ParentConstraint
                    {
                        ParentDetail = parentDetailData
                    };

                    // We'll need the original parent Detail component to find indices by comparing points
                    if (!mappingNameToDetailComp.TryGetValue(parentGoName, out var parentOriginalDetail) || parentOriginalDetail == null)
                    {
                        // no original detail to compare to (shouldn't happen if mapping present), log and add empty index
                        warnings.Add($"[{prefabName}] Точка {pointIndex}: Для родителя '{parentGoName}' отсутствует оригинальный Detail (не удалось сопоставить индексы).");
                        Debug.LogWarning($"[Converter] {prefabName} point#{pointIndex}: parent original Detail missing for '{parentGoName}', cannot match indices.");
                        // Still add constraint but no indices
                        targetPointData.constraints.Add(constraint);
                        continue;
                    }

                    // parent.parentPointList contains Point copies (position/rotation). For each of them find index in parentOriginalDetail.points
                    if (parent.parentPointList != null && parent.parentPointList.Count > 0)
                    {
                        foreach (var parentPoint in parent.parentPointList)
                        {
                            // Find match index in parentOriginalDetail.points list
                            var foundIdx = -1;
                            if (parentOriginalDetail.points != null)
                            {
                                for (int i = 0; i < parentOriginalDetail.points.Count; i++)
                                {
                                    var cand = parentOriginalDetail.points[i];
                                    if (cand == null || cand.point == null || parentPoint == null) continue;
                                    if (ApproximatelyEqualVectors(cand.point.position, parentPoint.position, FLOAT_EPS) &&
                                        ApproximatelyEqualVectors(cand.point.rotation, parentPoint.rotation, FLOAT_EPS))
                                    {
                                        foundIdx = i;
                                        break;
                                    }
                                }
                            }

                            if (foundIdx >= 0)
                            {
                                constraint.AddIndex(foundIdx);
                            }
                            else
                            {
                                // Not found: try also to match by position only (fallback), log for manual check
                                var foundByPos = -1;
                                if (parentOriginalDetail.points != null)
                                {
                                    for (int i = 0; i < parentOriginalDetail.points.Count; i++)
                                    {
                                        var cand = parentOriginalDetail.points[i];
                                        if (cand == null || cand.point == null || parentPoint == null) continue;
                                        if (ApproximatelyEqualVectors(cand.point.position, parentPoint.position, FLOAT_EPS))
                                        {
                                            foundByPos = i;
                                            break;
                                        }
                                    }
                                }

                                if (foundByPos >= 0)
                                {
                                    constraint.AddIndex(foundByPos);
                                    warnings.Add($"[{prefabName}] Точка {pointIndex}: в parent '{parentGoName}' совпадение по позиции (но не по ротации) найдено для точки индекса {foundByPos}. Проверьте вручную.");
                                    Debug.LogWarning($"[Converter] {prefabName} point#{pointIndex}: parent '{parentGoName}' matched by position to index {foundByPos} (rotation mismatch).");
                                }
                                else
                                {
                                    warnings.Add($"[{prefabName}] Точка {pointIndex}: не удалось найти соответствующую точку в родителе '{parentGoName}' для parentPoint (pos:{parentPoint.position}, rot:{parentPoint.rotation}).");
                                    Debug.LogWarning($"[Converter] {prefabName} point#{pointIndex}: Could not match parent point in '{parentGoName}' (pos {parentPoint.position}).");
                                }
                            }
                        } // end foreach parentPoint
                    }
                    else
                    {
                        // If parent.parentPointList is empty, fallback: maybe condition referenced "all" or used checkToggleList earlier.
                        // We cannot resolve -> log and still add constraint with no indices.
                        warnings.Add($"[{prefabName}] Точка {pointIndex}: parent.parentPointList пуст у родителя '{parentGoName}'. Constraint добавлен без индексов.");
                        Debug.LogWarning($"[Converter] {prefabName} point#{pointIndex}: parent.parentPointList empty for '{parentGoName}'.");
                    }

                    // Add the constraint to target point (ensure no duplicates), only if it contains something or even empty (we add anyway)
                    targetPointData.constraints.Add(constraint);
                } // foreach parent in ppc.parentList
            } // foreach pointIndex

            // mark asset dirty
            EditorUtility.SetDirty(detailData);
        } // end second pass

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        // Print summary of warnings (if any)
        if (warnings.Count > 0)
        {
            Debug.LogWarning($"[Converter] Завершено с предупреждениями ({warnings.Count}). См. лог для деталей. Примеры:\n - {string.Join("\n - ", warnings.Take(10))}" + (warnings.Count > 10 ? $"\n... (ещё {warnings.Count - 10})" : ""));
        }
        else
        {
            Debug.Log($"[Converter] Конвертация завершена успешно без предупреждений.");
        }
    }

    private static bool ApproximatelyEqualVectors(Vector3 a, Vector3 b, float eps)
    {
        return Mathf.Abs(a.x - b.x) <= eps && Mathf.Abs(a.y - b.y) <= eps && Mathf.Abs(a.z - b.z) <= eps;
    }

    private static string MakeSafeAssetName(string name)
    {
        var invalid = System.IO.Path.GetInvalidFileNameChars();
        foreach (var c in invalid)
            name = name.Replace(c.ToString(), "_");
        return name;
    }
}
