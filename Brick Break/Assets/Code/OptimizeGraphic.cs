using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OptimizeGraphic : MonoBehaviour
{
    [Range(0.1f, 1f)]
    [SerializeField] float m_scaleResolution = 0.75f;
    [SerializeField] int m_targetFrameRate = 60;
    [SerializeField] int m_antiAliasing = 2;
    void Start()
    {
        Optimize();
    }

    void Optimize()
    {
        Application.targetFrameRate = m_targetFrameRate;
        // int width = (int)(Screen.width * m_scaleResolution);
        // int height = (int)(Screen.height * m_scaleResolution);
        // Screen.SetResolution(width, height, true);
        //
        // QualitySettings.antiAliasing = m_antiAliasing;
    }
}
