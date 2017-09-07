using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using SimpleSingleton;

namespace WarOfSlinger {
    public class CGameManager : CMonoSingleton<CGameManager> {

		[Header ("UITalk")]
		[SerializeField]	protected CUITalk m_UITalk;

		[Header("Game object detected")]
		[SerializeField]	protected Camera m_Camera;
		[SerializeField]	protected LayerMask m_TouchDetectLayerMask;
		[SerializeField]	protected CUIJob m_UIJob;

		protected bool m_OnTouched = false;

		protected virtual void Update() {
			this.OnMouseDetectStandalone ();
		}

		public void OnMouseDetectStandalone() {
			if (Input.GetMouseButtonDown (0) && !EventSystem.current.IsPointerOverGameObject()) {
				GameObject detectedGo = null;
				if (this.OnDectedGameObject (ref detectedGo)) {
					var parentRoot = detectedGo.transform.root;
					var objController = parentRoot.GetComponent<CObjectController> ();
					var objData = objController.GetData ();
					if (objData != null) {
						var uiPos = objController.uiJobPoint;
						this.m_UIJob.ShowJobs (uiPos, (currentJob) => {
							// EXCUTE
							objController.ExcuteJobOwner (currentJob.jobExcute);
						}, (currentJob) => {
							// CLEAR
							objController.ClearJobOwner (currentJob.jobExcute);
						}, objData.objectJobs);
					}
				}
			}
		}

		protected virtual bool OnDectedGameObject(ref GameObject detectGo) {
			var mouseWorldPoint = this.m_Camera.ScreenToWorldPoint (Input.mousePosition);
			var rayHit2Ds = Physics2D.RaycastAll (mouseWorldPoint, Vector2.zero, Mathf.Infinity);
			if (rayHit2Ds.Length > 0) {
				for (int i = 0; i < rayHit2Ds.Length; i++) {
					var objectLayer = rayHit2Ds [i].collider.gameObject.layer;
					var isDetected = this.m_TouchDetectLayerMask == (this.m_TouchDetectLayerMask | (1 << objectLayer));
					if (isDetected == true) {
						detectGo = rayHit2Ds [i].collider.gameObject;
						return true;
					}
				}
			}
			return false;
		}

		public virtual void ShowTalk(Transform parent, string value) {
			if (this.m_UITalk == null)
				return;
			this.m_UITalk.ShowTalkItem (parent, value);
		}
		
	}
}
