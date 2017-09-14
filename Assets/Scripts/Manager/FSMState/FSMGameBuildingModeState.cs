using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FSM;

namespace WarOfSlinger {
	public class FSMGameBuildingModeState : FSMBaseState {

		protected CGameManager m_GameManager;
		protected string[] m_ApplyObjectType;
		protected CDragable2DComponent m_CurrentDraggedObject;
		protected CObjectController m_CurrentSelectObject;

		public FSMGameBuildingModeState (IContext context): base(context)
		{
			this.m_GameManager = context as CGameManager;
			this.m_ApplyObjectType = new string[] { "building", "environment" };
		}

		public override void StartState ()
		{
			base.StartState ();
			this.m_GameManager.OnEventTouchedGameObject = this.OnTouchedObject;
			var villageObjects = this.m_GameManager.villageObjects;
			foreach (var item in villageObjects) {
				var listObjs = item.Value;
				if (Array.IndexOf (this.m_ApplyObjectType, item.Key) == -1) {
					for (int i = 0; i < listObjs.Count; i++) {
						listObjs [i].gameObject.SetActive (false);
					}
				} else {
					for (int i = 0; i < listObjs.Count; i++) {
						if (listObjs [i] == null)
							continue;
						listObjs [i].SetEnabledPhysic (false);
						listObjs [i].gameObject.SetActive (true);
					}
				}
			}
		}

		public override void UpdateState (float dt)
		{
			base.UpdateState (dt);
#if UNITY_EDITOR || UNITY_STANDALONE
			this.m_GameManager.OnMouseDetectStandalone();
#else
			this.m_GameManager.OnMouseDetectMobile();
#endif
		}

		public override void ExitState ()
		{
			base.ExitState ();
			var villageObjects = this.m_GameManager.villageObjects;
			foreach (var item in villageObjects) {
				var listObjs = item.Value;
				if (Array.IndexOf (this.m_ApplyObjectType, item.Key) == -1) {
					for (int i = 0; i < listObjs.Count; i++) {
						listObjs [i].gameObject.SetActive (true);
					}
				} else {
					for (int i = 0; i < listObjs.Count; i++) {
						listObjs [i].SetEnabledPhysic (true);
						listObjs [i].gameObject.SetActive (true);
					}
				}
			}
		}

		protected virtual void OnTouchedObject(GameObject detectedGo) {
			var dragableComponent = detectedGo.GetComponent<CDragable2DComponent> ();
			if (dragableComponent != null && this.m_CurrentDraggedObject == null) {
				this.m_CurrentDraggedObject 			= dragableComponent;
				this.m_CurrentSelectObject 				= dragableComponent.DragObject.GetComponent<CObjectController>();
				this.m_GameManager.OnEventBeginTouch 	= dragableComponent.OnBeginDrag2D;
				this.m_GameManager.OnEventTouched 		= dragableComponent.OnDrag2D;
				this.m_GameManager.OnEventEndTouch 		= ResetTouch;
				this.m_GameManager.FollowObject (dragableComponent.transform);
			}
		}

		protected virtual void ResetTouch(Vector2 position) {
			if (this.m_CurrentSelectObject != null) {
				this.m_GameManager.canChangeMode = this.m_CurrentSelectObject.IsObjectWorking;
			} else {
				this.m_GameManager.canChangeMode = true;
			}

			this.m_GameManager.OnEventBeginTouch 	= null;
			this.m_GameManager.OnEventTouched 		= null;
			this.m_GameManager.OnEventEndTouch 		= null;
			this.m_GameManager.FollowObject (null);

			this.m_CurrentDraggedObject.OnEndDrag2D (position);
			this.m_CurrentDraggedObject 			= null;
			this.m_CurrentSelectObject 				= null;
		}

	}
}
