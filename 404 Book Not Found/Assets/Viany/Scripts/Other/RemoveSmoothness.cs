using UnityEngine;

public class RemoveSmoothness : MonoBehaviour
{
    void Start()
    {
        Renderer renderer = GetComponent<Renderer>();
        if (renderer != null && renderer.materials != null)
        {
            foreach (Material mat in renderer.materials)
            {
                if (mat == null) continue;

                if (mat.HasProperty("_Smoothness"))
                    mat.SetFloat("_Smoothness", 0f);

                if (mat.HasProperty("_Glossiness"))
                    mat.SetFloat("_Glossiness", 0f);
            }
        }

        SkinnedMeshRenderer skinnedMeshRenderer = GetComponent<SkinnedMeshRenderer>();
        if (skinnedMeshRenderer != null && skinnedMeshRenderer.materials != null)
        {
            foreach (Material mat in skinnedMeshRenderer.materials)
            {
                if (mat == null) continue;

                if (mat.HasProperty("_Smoothness"))
                    mat.SetFloat("_Smoothness", 0f);

                if (mat.HasProperty("_Glossiness"))
                    mat.SetFloat("_Glossiness", 0f);
            }
        }
    }
}
