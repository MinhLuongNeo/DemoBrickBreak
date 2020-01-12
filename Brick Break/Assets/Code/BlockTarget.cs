using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class BlockTarget : MonoBehaviour
{
    [SerializeField] int m_maxHP;
    [SerializeField] TextMeshProUGUI m_textHP;
    [SerializeField] Animator m_animator;
    [SerializeField] GameObject m_effectBreak;
    [SerializeField] AudioSource m_sfx;


    MaterialPropertyBlock m_materialBlock;
    Renderer m_render;
    int m_currentHP;

    void Start()
    {
        m_materialBlock = new MaterialPropertyBlock();
        m_render = GetComponent<Renderer>();
        m_render.GetPropertyBlock(m_materialBlock);
        SetCurrentHP(m_maxHP);
    }

    public void SetCurrentHP(int hp)
    {
        m_currentHP = hp;
        m_textHP.text = m_currentHP.ToString();
        m_materialBlock.SetColor("_Color", ColorBoard.Instance.GetColor(m_currentHP));
        m_render.SetPropertyBlock(m_materialBlock);
    }

    public int GetCurrentHP()
    {
        return m_currentHP;
    }

    public void TakeDamage(int value)
    {
        SetCurrentHP(m_currentHP - value);

        if (m_currentHP > 0)
        {
            m_textHP.text = m_currentHP.ToString();
            m_animator.SetTrigger("Take Damage");
            m_sfx.Play();
            //m_sfx.PlayOneShot();
        }
        else
            Break();
    }
    void Break()
    {
        m_effectBreak.transform.parent = null;
        m_effectBreak.SetActive(true);
        Destroy(m_effectBreak, 4f);
        Destroy(gameObject);
    }
}
