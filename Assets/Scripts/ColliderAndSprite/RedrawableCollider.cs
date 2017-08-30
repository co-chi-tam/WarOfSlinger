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

	public Action<Vector2, GameObject> OnEventColliderObject;

	#endregion

	#region Implmentation MonoBehaviour

    protected virtual void Awake() {
		this.m_PolygonCollider	= this.GetComponent<PolygonCollider2D>();
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

	public virtual void OnCollisionExit2D(Collision2D col) {

	}

	public virtual void OnTriggerEnter2D(Collider2D col) {
	
	}

	public virtual void OnTriggerStay2D(Collider2D col) {

	}

	public virtual void OnTriggerExit2D(Collider2D col) {

	}

	#endregion

	#region Main methods

	public virtual void RedrawCollider() {
		Destroy(this.m_PolygonCollider);
		this.m_PolygonCollider = this.gameObject.AddComponent<PolygonCollider2D> ();

    }

    public virtual void DisableCollider() {
        if (this.m_PolygonCollider != null) {
            this.m_PolygonCollider.enabled = false;
        }
    }

	#endregion

}

