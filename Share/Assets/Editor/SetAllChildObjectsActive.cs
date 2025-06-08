using UnityEngine;
using UnityEditor;

public class SetAllChildObjectsActive
{
    [MenuItem("GameObject/Set All Child Objects Active")]
    static void SetChildObjectsActive()
    {
        Selection.activeGameObject.SetActiveRecursively(true);
    }
    [MenuItem("GameObject/Set All Child Objects Deactive")]
    static void SetChildObjectsDeactive()
    {
        Selection.activeGameObject.SetActiveRecursively(false);
    }
}