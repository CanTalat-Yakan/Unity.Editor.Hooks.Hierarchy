# Unity Essentials

This module is part of the Unity Essentials ecosystem and follows the same lightweight, editor-first approach.
Unity Essentials is a lightweight, modular set of editor utilities and helpers that streamline Unity development. It focuses on clean, dependency-free tools that work well together.

All utilities are under the `UnityEssentials` namespace.

```csharp
using UnityEssentials;
```

## Installation

Install the Unity Essentials entry package via Unity's Package Manager, then install modules from the Tools menu.

- Add the entry package (via Git URL)
    - Window → Package Manager
    - "+" → "Add package from git URL…"
    - Paste: `https://github.com/CanTalat-Yakan/UnityEssentials.git`

- Install or update Unity Essentials packages
    - Tools → Install & Update UnityEssentials
    - Install all or select individual modules; run again anytime to update

---

# Hierarchy Hook

> Quick overview: Editor-only selection hook for the Hierarchy. Subscribe to `HierarchyHook.OnSelectionChanged` and use helpers to get/set the current selection.

A tiny helper that centralizes selection handling for the Hierarchy. It raises a simple event whenever the selection changes to one or more GameObjects and provides convenience methods to read or set the selection.

![screenshot](Documentation/Screenshot.png)

## Features
- Initialize-on-load and editor-only
- Event-based selection updates
  - `public static event Action<GameObject[]> OnSelectionChanged`
  - Invoked when the selection contains at least one GameObject
- Convenience APIs
  - `GetSelectedGameObjects()` returns the current selection
  - `SetSelectedGameObjects(GameObject[])` updates the selection
- Minimal surface: zero setup; subscribe and go

## Requirements
- Unity Editor 6000.0+ (Editor-only; no runtime code)

## Usage

Subscribe to selection changes from an editor script or window:

```csharp
using UnityEditor;
using UnityEngine;
using UnityEssentials;

public class HierarchyHookExample : EditorWindow
{
    private GameObject[] _lastSelection = System.Array.Empty<GameObject>();

    [MenuItem("Window/Examples/Hierarchy Hook Example")]
    private static void Open() => GetWindow<HierarchyHookExample>("Hierarchy Hook");

    private void OnEnable()
    {
        HierarchyHook.OnSelectionChanged += OnSelected;
    }

    private void OnDisable()
    {
        HierarchyHook.OnSelectionChanged -= OnSelected;
    }

    private void OnSelected(GameObject[] selection)
    {
        _lastSelection = selection;
        // Debug.Log($"Selected {selection.Length}: {string.Join(", ", selection.Select(go => go.name))}");
        Repaint();
    }

    private void OnGUI()
    {
        EditorGUILayout.LabelField("Current Selection", EditorStyles.boldLabel);
        if (_lastSelection.Length == 0)
        {
            EditorGUILayout.HelpBox("Select one or more GameObjects in the Hierarchy.", MessageType.Info);
        }
        else
        {
            foreach (var go in _lastSelection)
                EditorGUILayout.LabelField($"• {go.name}");
        }

        GUILayout.Space(8);

        using (new EditorGUILayout.HorizontalScope())
        {
            if (GUILayout.Button("Refresh From API"))
                _lastSelection = HierarchyHook.GetSelectedGameObjects();

            if (GUILayout.Button("Select Children"))
            {
                var selected = HierarchyHook.GetSelectedGameObjects();
                var list = new System.Collections.Generic.List<GameObject>();
                foreach (var s in selected)
                    foreach (Transform t in s.transform)
                        list.Add(t.gameObject);
                HierarchyHook.SetSelectedGameObjects(list.ToArray());
            }
        }
    }
}
```

### Notes on the event
- Empty selection is ignored: the event is not fired when the selection becomes empty
- If you need to react to selection clearing, also listen to Unity's `Selection.selectionChanged` and check for `Selection.gameObjects.Length == 0`
- Invocations occur on the main thread during editor selection changes

## Notes and Limitations
- Editor-only: not included in player builds
- Scope: only GameObject selections are forwarded by this helper
- Multiple scenes/prefab stages: forwarded objects may span contexts; guard your logic accordingly
- Lifetime: unsubscribe from the static event in `OnDisable` to avoid stale callbacks

## Files in This Package
- `Editor/HierarchyHook.cs` – Core hook and helpers (`OnSelectionChanged`, get/set selection)
- `package.json` – Package manifest metadata

## Tags
unity, unity-editor, hierarchy, selection, hook, event, editor-utility
