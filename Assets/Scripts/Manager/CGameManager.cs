using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SimpleSingleton;

namespace WarOfSlinger {
    public class CGameManager : CMonoSingleton<CGameManager> {

		[Header("Game object detected")]
		[SerializeField]	protected Camera m_Camera;
		[SerializeField]	protected LayerMask m_TouchDetectLayerMask;
		[SerializeField]	protected CUIJob m_UIJob;

		protected virtual void Update() {
			if (Input.GetMouseButtonDown (0)) {
				GameObject detectedGo = null;
				if (this.OnDectedGameObject (ref detectedGo)) {
					var parentRoot = detectedGo.transform.root;
					var objController = parentRoot.GetComponent<CObjectController> ();
					var objData = objController.GetData ();
					if (objData != null) {
						var uiPos = objController.uiJobPoint;
						this.m_UIJob.ShowJobs (uiPos, (currentJob) => {
							objController.ExcuteJob (currentJob.jobExcute);
						}, objData.objectJobs);
					}
				}
			}
		}

		public virtual bool OnDectedGameObject(ref GameObject detectGo) {
			var mouseWorldPoint = this.m_Camera.ScreenToWorldPoint (Input.mousePosition);
			var rayHit2Ds = Physics2D.RaycastAll (mouseWorldPoint, Vector2.zero, Mathf.Infinity);
			if (rayHit2Ds.Length > 0) {
				for (int i = 0; i < rayHit2Ds.Length; i++) {
					var isDetected = this.m_TouchDetectLayerMask == (this.m_TouchDetectLayerMask | (1 << rayHit2Ds [i].collider.gameObject.layer));
					if (isDetected == true) {
						detectGo = rayHit2Ds [i].collider.gameObject;
						return true;
					}
				}
			}
			return false;
		}
		
	}
}
