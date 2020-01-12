using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using System;
using UnityEngine.Events;

public class Ball : MonoBehaviour
{
    [SerializeField] int m_damage;
    [SerializeField] float m_flySpeed = 2f;
    [SerializeField] float m_radius = 0.5f;
    [SerializeField] LayerMask m_layerMaskVaCham;

    Gun m_gun;
    RaycastHit2D m_raycastHit;
    Vector3 m_directFly;
    TweenerCore<Vector3, Vector3, VectorOptions> m_tweener;

    BallState m_ballState = BallState.Idle;

    enum BallState
    {
        Idle, Flying, Revoking
    }

    void Start()
    {

    }
    public void SetGun(Gun gun)
    {
        m_gun = gun;
    }

    public void FlyInDirection(Vector3 directFly)
    {
        m_directFly = directFly;
        m_raycastHit = Physics2D.CircleCast(transform.position, m_radius, m_directFly, 100f, m_layerMaskVaCham);

        Vector3 destination = m_raycastHit.point + m_raycastHit.normal * m_radius;  //điểm cuối cách 1 khoảng bằng radius, để bóng ko đi xuyên qua tường hoặc khối
        m_tweener = transform.DOMove(destination, m_raycastHit.distance / m_flySpeed);
        m_tweener.SetEase(Ease.Linear);
        m_tweener.OnComplete(() => OnDoMoveComplete());
        m_ballState = BallState.Flying;
    }

    void OnDoMoveComplete()
    {
        if(m_raycastHit.transform == null) //Nếu khối đã vỡ
        {
            FlyInDirection(m_directFly); //bay tiếp theo hướng cũ
        }
        else
        {
            if (m_raycastHit.transform.tag == "Wall Bottom") //Nếu đã chạm xuống sàn
            {
                FinishFly();
            }
            else
            {
                if (m_raycastHit.transform.tag == "Block Target")
                {
                    BlockTarget blockTarget = m_raycastHit.transform.GetComponent<BlockTarget>();
                    blockTarget.TakeDamage(m_damage);
                }
                FlyInDirection(Vector3.Reflect(m_directFly, m_raycastHit.normal));
            }
        }
    }

    void FinishFly()
    {
        m_tweener = transform.DOMove(m_gun.transform.position, 0.5f).SetEase(Ease.Linear).OnComplete(() => 
        {
            m_gun.NotifyBallFinishedFly();
            m_ballState = BallState.Idle;
        });
    }

    public void RevokeBall()
    {
        if(m_ballState == BallState.Flying)
        {
            m_ballState = BallState.Revoking;
            m_tweener.Kill();

            Vector3 pos = m_gun.transform.position;
            pos.x = transform.position.x;
            float distance = Vector3.Distance(transform.position, pos);
            m_tweener = transform.DOMove(pos, distance / m_flySpeed); //bay thẳng xuống cái đã
            m_tweener.SetEase(Ease.Linear);
            m_tweener.OnComplete(() =>
            {
                FinishFly();
            });
        }
    }
}