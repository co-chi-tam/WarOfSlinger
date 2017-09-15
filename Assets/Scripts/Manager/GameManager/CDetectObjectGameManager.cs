using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using SimpleSingleton;
using FSM;
using SceneTask;

namespace WarOfSlinger {
	public partial class CGameManager {

		#region Fields

		protected bool m_IsUITouched = false;

		#endregion

		#region Detect object method

		public void OnMouseDetectStandalone() {
			var worldPos = this.m_Camera.ScreenToWorldPoint(Input.mousePosition);
			if (Input.GetMouseButtonDown (0)) {
				if (!EventSystem.current.IsPointerOverGameObject()) {
					this.OnTouchedObject ();
				}
				if (this.OnEventBeginTouch != null) {
					this.OnEventBeginTouch (worldPos);
				}
			}
			if (Input.GetMouseButton (0)) {
				if (this.OnEventTouched != null) {
					this.OnEventTouched (worldPos);
				}
			}
			if (Input.GetMouseButtonUp (0)) {
				if (this.OnEventEndTouch != null) {
					this.OnEventEndTouch (worldPos);
				}
			}
		}

		public void OnMouseDetectMobile() {
			if (Input.touchCount != 1)
				return;
			var fingerTouch = Input.GetTouch (0);
			var worldPos = this.m_Camera.ScreenToWorldPoint(fingerTouch.position);
			switch (fingerTouch.phase) {
			case TouchPhase.Began:
				this.m_IsUITouched = EventSystem.current.IsPointerOverGameObject (fingerTouch.fingerId);
				if (this.OnEventBeginTouch != null) {
					this.OnEventBeginTouch (worldPos);
				}
				break;
			case TouchPhase.Moved:
				if (this.OnEventTouched != null) {
					this.OnEventTouched (worldPos);
				}
				break;
			case TouchPhase.Canceled:
			case TouchPhase.Ended:
				if (this.OnEventEndTouch != null) {
					this.OnEventEndTouch (worldPos);
				}
				break;
			}
			if (this.m_IsUITouched == false) {
				this.OnTouchedObject ();
			}
		}

		protected virtual void OnTouchedObject() {
			GameObject detectedGo = null;
			if (this.OnDectedGameObject (ref detectedGo)) {
				if (this.OnEventTouchedGameObject != null) {
					this.OnEventTouchedGameObject (detectedGo);
				}
			}
		}

		public virtual bool OnDectedGameObject(ref GameObject detectGo) {
			var mouseWorldPoint = this.m_Camera.ScreenToWorldPoint (Input.mousePosition);
			var rayHit2Ds = Physics2D.RaycastAll (mouseWorldPoint, Vector2.zero, Mathf.Infinity);
			if (rayHit2Ds.Length > 0) {
				for (int i = 0; i < rayHit2Ds.Length; i++) {
					var objectLayer = rayHit2Ds [i].collider.gameObject.layer;
					var isDetected = this.m_TouchDetectLayerMask == (this.m_TouchDetectLayerMask | (1 << objectLayer));
					if (isDetected == true) {
						var rayhitObject = rayHit2Ds [i].collider;
						detectGo = rayHit2Ds [i].collider.gameObject;
						return true;
					}
				}
			}
			return false;
		}

		#endregion
		
	}
}
