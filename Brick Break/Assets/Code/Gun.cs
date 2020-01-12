using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Gun : MonoBehaviour
{
    [Header("Gun")]
    [SerializeField] float m_shootPerSeconds = 0.3f;

    [Header("Ball")]
    [SerializeField] int m_numberBall;
    [SerializeField] float m_ballRadius = 0.5f;
    [SerializeField] Ball m_ballPrefab;

    [Header("Guide Line")]
    [SerializeField] LayerMask m_layerMaskGuideLine;
    [SerializeField] LineRenderer m_guideLine;


    Camera m_camera;

    List<Ball> m_listBall = new List<Ball>();
    int m_numberBallFinishedFly = 0;

    Vector3 m_mousePosition;
    Vector3 m_directShoot;
    //RaycastHit2D m_raycastHitGuideLine;
    void Start()
    {
        m_camera = Camera.main;
        InitBall();
        
    }

    

    void InitBall()
    {
        for (int i = 0; i < m_numberBall; i++)
        {
            Ball ball = Instantiate(m_ballPrefab, transform.position, Quaternion.identity);
            m_listBall.Add(ball);
            ball.SetGun(this);
        }
        m_numberBallFinishedFly = m_numberBall;
    }

    
    void Update()
    {
        bool isPointerOverGameObject = EventSystem.current.IsPointerOverGameObject();
        if (Input.GetMouseButtonDown(0) && !isPointerOverGameObject)
        {
            m_guideLine.enabled = true;
        }

        if (Input.GetMouseButton(0) && m_numberBallFinishedFly == m_numberBall && !isPointerOverGameObject)
        {
            UpdateDirectShoot();
            DrawGuideLine();
        }

        if (Input.GetMouseButtonUp(0) && !isPointerOverGameObject)
        {
            m_guideLine.enabled = false;
            Shoot();
        }
    }

    void UpdateDirectShoot()
    {
        m_mousePosition = Input.mousePosition;
        m_mousePosition.z = -m_camera.transform.position.z;
        m_mousePosition = m_camera.ScreenToWorldPoint(m_mousePosition);
        m_directShoot = m_mousePosition - transform.position;
        transform.up = m_directShoot;
    }
    void DrawGuideLine()
    {
        //Điểm 0
        m_guideLine.SetPosition(0, transform.position);


        //Điểm 1
        Vector3 direction = m_directShoot;
        RaycastHit2D raycastHit =  Physics2D.CircleCast(transform.position, m_ballRadius, direction, 100, m_layerMaskGuideLine);
        Vector3 origin = raycastHit.point + raycastHit.normal * m_ballRadius;
        m_guideLine.SetPosition(1, origin);

        //Điểm 2
        direction = Vector3.Reflect(direction, raycastHit.normal);
        raycastHit = Physics2D.CircleCast(origin, m_ballRadius, direction, 100, m_layerMaskGuideLine);
        origin = raycastHit.point + raycastHit.normal * m_ballRadius;
        m_guideLine.SetPosition(2, origin);
    }
    void Shoot()
    {
        if(m_numberBallFinishedFly != m_numberBall)
        {
            Debug.Log("Chưa bắn xong!");
            return;
        }
        StartCoroutine(Timer());
        IEnumerator Timer()
        {
            m_numberBallFinishedFly = m_numberBall;
            int length = m_listBall.Count;
            for (int i = 0; i < length; i++)
            {
                m_numberBallFinishedFly--;
                m_listBall[i].FlyInDirection(m_directShoot);
                yield return new WaitForSeconds(m_shootPerSeconds);
            }
        }
    }

    public void NotifyBallFinishedFly()
    {
        m_numberBallFinishedFly++;
    }

    public void RevokeAllBall()
    {
        StopAllCoroutines();
        foreach (var ball in m_listBall)
        {
            ball.RevokeBall();
        }
    }
}
