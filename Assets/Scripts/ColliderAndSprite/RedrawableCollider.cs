using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(PolygonCollider2D))]
public class RedrawableCollider : MonoBehaviour {

	#region Fields

	[Header("Events")]
	[SerializeField]	protected LayerMask m_LayerTrigger;
	public UnityEvent OnEventColliderEnter;

	protected PolygonCollider2D m_PolygonCollider;
	protected Transform m_Transform;

	public Action<Vector2, GameObject> OnEventColliderObject;

	public Collider2D collider {
		get { return this.m_PolygonCollider; }
	}

	#endregion

	#region Implmentation MonoBehaviour

    protected virtual void Awake() {
		this.m_PolygonCollider	= this.GetComponent<PolygonCollider2D>();
		this.m_Transform = this.transform;
    }

	public virtual void OnCollisionEnter2D(Collision2D col) {
		var contactPoints = col.contacts;
        var i = 0;
		var pointContact = contactPoints [i];
		var layer = contactPoints [i].collider.gameObject.layer;
		if (this.m_LayerTrigger == (this.m_LayerTrigger | (1 << layer))) {
			// CALL BACK TRIGGER
			if (this.OnEventColliderEnter != null) {
				this.OnEventColliderEnter.Invoke();
			}
			if (this.OnEventColliderObject != null) {
				this.OnEventColliderObject (pointContact.point, pointContact.collider.gameObject);
			}
		}
	}

	public virtual void OnCollisionStay2D(Collision2D col) {
		
	}

	public virtual void OnTriggerEnter2D(Collider2D col) {
		var i = 0;
		var layer = col.gameObject.layer;
		var nearestPoint = this.GetClosestPoint (col.transform.position);
		if (this.m_LayerTrigger == (this.m_LayerTrigger | (1 << layer))) {
			// CALL BACK TRIGGER
			if (this.OnEventColliderEnter != null) {
				this.OnEventColliderEnter.Invoke();
			}
			if (this.OnEventColliderObject != null) {
				this.OnEventColliderObject (nearestPoint, col.gameObject);
			}
		}
	}

	public virtual void OnTriggerStay2D(Collider2D col) {
		
	}

	#endregion

	#region Main methods

	public virtual void RedrawCollider() {
		Destroy(this.m_PolygonCollider);
		this.m_PolygonCollider = null;
		this.m_PolygonCollider = this.gameObject.AddComponent<PolygonCollider2D> ();
    }

    public virtual void DisableCollider() {
        if (this.m_PolygonCollider != null) {
            this.m_PolygonCollider.enabled = false;
        }
	}

	public Vector3 GetClosestPoint(Vector3 point) {
		var points = this.m_PolygonCollider.points;
		var nearestDistance = Mathf.Infinity;
		var neatestPosition = this.m_Transform.position;
		for (int i = 0; i < points.Length; i++) {
			var worldPoint = this.m_Transform.TransformPoint(points [i]);
			var distance = Vector3.Distance (worldPoint, point);
			if (distance < nearestDistance) {
				nearestDistance = distance;
				neatestPosition = worldPoint;
			}
		}
		return neatestPosition; 
	}

	#endregion

}

