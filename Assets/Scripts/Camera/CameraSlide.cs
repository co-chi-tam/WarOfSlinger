using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraSlide : MonoBehaviour {

    [Header ("Slide Info")]
    [SerializeField]    protected float m_Speed = 1f;
    [SerializeField]    protected float m_Damping = 0.5f;

    protected Camera m_Camera;
    protected Transform m_Transform;
    protected Vector3 m_LastMousePos;
    protected Vector3 m_LastTransfromPos;

    protected virtual void Awake()
    {
        this.m_Camera = this.GetComponent<Camera>();
        this.m_Transform = this.transform;
    }

    protected virtual void Update() {
        this.MoveCameraStandalone();
    }

    private void MoveCameraStandalone()
    {
        if (Input.GetMouseButtonDown(0))
        {
            this.SavePoint ();
        }
        if (Input.GetMouseButton(0))
        {
            this.MovedPoint ();
        }
    }

    private void SavePoint() {
        this.m_LastMousePos = this.m_Camera.ScreenToViewportPoint(Input.mousePosition);
        this.m_LastTransfromPos = this.m_Transform.position;
    }

    private void MovedPoint() {
        var direction = this.m_Camera.ScreenToViewportPoint(Input.mousePosition) - this.m_LastMousePos;
        var movePos = direction.normalized;
        movePos.x = direction.x;
        movePos.y = direction.y;
        movePos.z = 0f;
        this.m_Transform.position = Vector3.Lerp(this.m_Transform.position,
            this.m_LastTransfromPos + -movePos * this.m_Speed,
            this.m_Damping);
    }

}
