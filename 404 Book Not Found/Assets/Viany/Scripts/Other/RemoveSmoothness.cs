using UnityEngine;

public class RemoveSmoothness : MonoBehaviour
{
    void Start()
    {
        Renderer renderer = GetComponent<Renderer>();
        if (renderer != null && renderer.material != null)
        {
            if (renderer.material.HasProperty("_Smoothness"))
                renderer.material.SetFloat("_Smoothness", 0f);

            if (renderer.material.HasProperty("_Glossiness"))
                renderer.material.SetFloat("_Glossiness", 0f);
        }
    }
}
