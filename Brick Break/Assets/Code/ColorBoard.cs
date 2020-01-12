using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ColorBoard : MonoBehaviour
{
    [System.Serializable]
    class HPColor
    {
        public int hp;
        public Color color;
    }


    [SerializeField] List<HPColor> m_listColor;


    bool m_inited = false;
    Gradient m_gradient = new Gradient();
    float m_hpMin;
    float m_hpMax;


    static ColorBoard m_instance;
    public static ColorBoard Instance
    {
        get { return m_instance; }
    }

    private void Awake()
    {
        m_instance = this;
    }
    void Start()
    {
        if (m_inited == false)
            Init();
    }

    void Init()
    {
        m_inited = true;
        m_listColor = m_listColor.OrderBy(item => item.hp).ToList();

        GradientAlphaKey[] gaks = new GradientAlphaKey[2];
        gaks[0] = new GradientAlphaKey(1, 0);
        gaks[1] = new GradientAlphaKey(1, 1);

        List<GradientColorKey> listGck = new List<GradientColorKey>();

        m_hpMin = m_listColor[0].hp;
        m_hpMax = m_listColor[m_listColor.Count-1].hp;

        foreach (var hpColor in m_listColor)
        {
            GradientColorKey gck = new GradientColorKey();
            gck.color = hpColor.color;
            gck.time = GetDelta(m_hpMin, m_hpMax, hpColor.hp);
            listGck.Add(gck);
        }
        m_gradient.SetKeys(listGck.ToArray(), gaks);
    }

    float GetDelta(float min, float max, float value)
    {
        return (value - min) / (max - min);
    }
    public Color GetColor(int hp)
    {
        if (m_inited == false)
            Init();
        if (hp > m_hpMax)
            return Color.black;
        return m_gradient.Evaluate(GetDelta(m_hpMin, m_hpMax, hp));
    }
}
