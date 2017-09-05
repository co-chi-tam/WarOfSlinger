using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraSlide : MonoBehaviour {

	#region Fields

    [Header ("Slide Info")]
    [SerializeField]    protected float m_Speed = 1f;
    [SerializeField]    protected float m_Damping = 0.5f;
	[SerializeField]	protected LayerMask m_DetectLayerMask;

    protected Camera m_Camera;
    protected Transform m_Transform;
    protected Vector3 m_LastMousePos;
    protected Vector3 m_LastTransfromPos;
	protected bool m_IsSlide = false;

	#endregion

	#region Implmentation Monobehaviour

    protected virtual void Awake()
    {
        this.m_Camera = this.GetComponent<Camera>();
        this.m_Transform = this.transform;
    }

    protected virtual void Update() {
#if UNITY_EDITOR || UNITY_STANDALONE
        this.MoveCameraStandalone();
#else
		this.MoveCameraMobile();
#endif
    }

	#endregion

	#region Main methods

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
		if (Input.GetMouseButtonUp (0)) {
			this.EndPoint ();
		}
    }

	private void MoveCameraMobile() {
		if (Input.touchCount != 1)
			return;
		var touchPhase = Input.GetTouch (0);
		switch (touchPhase.phase) {
		case TouchPhase.Began:
			this.SavePoint ();
			break;
		case TouchPhase.Moved:
			this.MovedPoint ();
			break;
		case TouchPhase.Canceled:
		case TouchPhase.Ended:
			this.EndPoint ();
			break;
		}
	}

    private void SavePoint() {
        this.m_LastMousePos = this.m_Camera.ScreenToViewportPoint(Input.mousePosition);
        this.m_LastTransfromPos = this.m_Transform.position;
		var mouseWorldPoint = this.m_Camera.ScreenToWorldPoint (Input.mousePosition);
		var rayHit2Ds = Physics2D.RaycastAll (mouseWorldPoint, Vector2.zero, Mathf.Infinity);
		if (rayHit2Ds.Length > 0) {
			var isCollide = true;
			for (int i = 0; i < rayHit2Ds.Length; i++) {
				isCollide &= this.m_DetectLayerMask == (this.m_DetectLayerMask | (1 << rayHit2Ds[i].collider.gameObject.layer));
			}
			this.m_IsSlide = isCollide;
		}
    }

    private void MovedPoint() {
		if (this.m_IsSlide == false)
			return;
        var direction = this.m_Camera.ScreenToViewportPoint(Input.mousePosition) - this.m_LastMousePos;
        var movePos = direction.normalized;
        movePos.x = direction.x;
        movePos.y = direction.y;
        movePos.z = 0f;
        this.m_Transform.position = Vector3.Lerp(this.m_Transform.position,
            this.m_LastTransfromPos + -movePos * this.m_Speed,
            this.m_Damping);
    }

	private void EndPoint() {
		this.m_IsSlide = false;
	}

	#endregion

}
