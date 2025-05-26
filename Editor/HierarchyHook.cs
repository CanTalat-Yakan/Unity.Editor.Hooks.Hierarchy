#if UNITY_EDITOR
using System;
using UnityEditor;
using UnityEngine;

namespace UnityEssentials
{
    [InitializeOnLoad]
    public static class HierarchyHook
    {
        public static event Action<GameObject[]> OnSelectionChanged;

        static HierarchyHook() =>
            Selection.selectionChanged += HandleSelectionChanged;

        private static void HandleSelectionChanged()
        {
            if(Selection.gameObjects.Length == 0)
                return;

            OnSelectionChanged?.Invoke(Selection.gameObjects);
        }

        /// <summary>
        /// Returns the currently selected GameObjects in the Hierarchy.
        /// </summary>
        public static GameObject[] GetSelectedGameObjects() =>
            Selection.gameObjects;

        /// <summary>
        /// Sets the selection in the Hierarchy to the specified GameObjects.
        /// </summary>
        public static void SetSelectedGameObjects(GameObject[] gameObjects) =>
            Selection.objects = gameObjects;
    }
}
#endif