using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class MeshCombiner : MonoBehaviour
{
    [ContextMenu("Combine LOD Meshes")]
    public void CombineLODMeshes()
    {
        // 모든 Ash 4 오브젝트들 수집
        GameObject[] ashTrees = GameObject.FindObjectsOfType<GameObject>()
            .Where(go => go.name.Contains("Ash 4") && go.GetComponent<LODGroup>() != null)
            .ToArray();

        Debug.Log($"Found {ashTrees.Length} trees with LOD Groups");

        CombineTreesByLODLevel(ashTrees);
    }

    void CombineTreesByLODLevel(GameObject[] trees)
    {
        if (trees.Length == 0) return;

        // 첫 번째 나무의 LOD 구조를 참조로 사용
        LODGroup referenceLOD = trees[0].GetComponent<LODGroup>();
        LOD[] referenceLODs = referenceLOD.GetLODs();

        GameObject combinedParent = new GameObject("CombinedAshTrees_LOD");
        LODGroup combinedLODGroup = combinedParent.AddComponent<LODGroup>();

        List<LOD> newLODs = new List<LOD>();

        // 각 LOD 레벨별로 처리
        for (int lodLevel = 0; lodLevel < referenceLODs.Length; lodLevel++)
        {
            List<CombineInstance> combineInstances = new List<CombineInstance>();
            Material sharedMaterial = null;

            // 모든 나무의 같은 LOD 레벨 메쉬 수집
            foreach (GameObject tree in trees)
            {
                LODGroup lodGroup = tree.GetComponent<LODGroup>();
                LOD[] lods = lodGroup.GetLODs();

                if (lodLevel < lods.Length && lods[lodLevel].renderers.Length > 0)
                {
                    foreach (Renderer renderer in lods[lodLevel].renderers)
                    {
                        MeshFilter meshFilter = renderer.GetComponent<MeshFilter>();
                        if (meshFilter != null && meshFilter.sharedMesh != null)
                        {
                            CombineInstance combine = new CombineInstance();
                            combine.mesh = meshFilter.sharedMesh;
                            combine.transform = renderer.transform.localToWorldMatrix;
                            combineInstances.Add(combine);

                            if (sharedMaterial == null)
                                sharedMaterial = renderer.sharedMaterial;
                        }
                    }
                }
            }

            // LOD 레벨별 결합된 메쉬 생성
            if (combineInstances.Count > 0)
            {
                GameObject lodObject = new GameObject($"LOD_{lodLevel}");
                lodObject.transform.SetParent(combinedParent.transform);

                MeshFilter meshFilter = lodObject.AddComponent<MeshFilter>();
                MeshRenderer meshRenderer = lodObject.AddComponent<MeshRenderer>();

                Mesh combinedMesh = new Mesh();
                combinedMesh.CombineMeshes(combineInstances.ToArray());
                meshFilter.sharedMesh = combinedMesh;
                meshRenderer.material = sharedMaterial;

                // LOD 설정
                LOD newLOD = new LOD();
                newLOD.renderers = new Renderer[] { meshRenderer };
                newLOD.screenRelativeTransitionHeight = referenceLODs[lodLevel].screenRelativeTransitionHeight;
                newLODs.Add(newLOD);
            }
        }

        // 새로운 LOD Group 설정
        combinedLODGroup.SetLODs(newLODs.ToArray());
        combinedLODGroup.RecalculateBounds();

        // 원본 나무들 비활성화
        foreach (GameObject tree in trees)
        {
            tree.SetActive(false);
        }

        Debug.Log($"Combined {trees.Length} trees into LOD group with {newLODs.Count} LOD levels");
    }
}
