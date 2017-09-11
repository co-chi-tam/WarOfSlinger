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

	public Action<Vector2, GameObject> OnEventEnterColliderObject;
	public Action<Vector2, GameObject> OnEventTriggerAllObject;

	public Collider2D collider2D {
		get { return this.m_PolygonCollider; }
	}

	public bool enabledCollider {
		get { return this.m_PolygonCollider.enabled; }
		set { this.m_PolygonCollider.enabled = value; }
	}

	public bool enabledTrigger {
		get { return this.m_PolygonCollider.isTrigger; }
		set { this.m_PolygonCollider.isTrigger = value; }
	}

	#endregion

	#region Implmentation MonoBehaviour

    protected virtual void Awake() {
		this.m_PolygonCollider	= this.GetComponent<PolygonCollider2D>();
		this.m_Transform = this.transform;
    }

	public virtual void OnCollisionEnter2D(Collision2D col) {
		this.OnCollisionObject2D (col);
	}

	public virtual void OnCollisionStay2D(Collision2D col) {
		this.OnCollisionObject2D (col);
	}

	public virtual void OnTriggerEnter2D(Collider2D col) {
		this.OnColliderObject2D (col);
	}

	public virtual void OnTriggerStay2D(Collider2D col) {
		this.OnColliderObject2D (col);
		if (this.OnEventTriggerAllObject != null) {
			var nearestPoint = this.GetClosestPoint (col.transform.position);
			this.OnEventTriggerAllObject (nearestPoint, col.gameObject);
		}
	}

	#endregion

	#region Main methods

	private void OnCollisionObject2D(Collision2D col) {
		var contactPoints = col.contacts;
		var i = 0;
		var pointContact = contactPoints [i];
		var layer = contactPoints [i].collider.gameObject.layer;
		if (this.m_LayerTrigger == (this.m_LayerTrigger | (1 << layer))) {
			// CALL BACK TRIGGER
			if (this.OnEventColliderEnter != null) {
				this.OnEventColliderEnter.Invoke();
			}
			if (this.OnEventEnterColliderObject != null) {
				this.OnEventEnterColliderObject (pointContact.point, pointContact.collider.gameObject);
			}
		}
	}

	private void OnColliderObject2D(Collider2D col) {
		var layer = col.gameObject.layer;
		var nearestPoint = this.GetClosestPoint (col.transform.position);
		if (this.m_LayerTrigger == (this.m_LayerTrigger | (1 << layer))) {
			// CALL BACK TRIGGER
			if (this.OnEventColliderEnter != null) {
				this.OnEventColliderEnter.Invoke();
			}
			if (this.OnEventEnterColliderObject != null) {
				this.OnEventEnterColliderObject (nearestPoint, col.gameObject);
			}
		}
	}

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

