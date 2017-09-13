using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using FSM;

namespace WarOfSlinger {
	public class CDragable2DComponent : MonoBehaviour {

		[SerializeField]	protected GameObject m_DragObject;
		[SerializeField]	protected Vector2 m_DragOffsetPosition = new Vector2 (0f, -0.2f);

		public GameObject DragObject {
			get { return this.m_DragObject; }
			protected set { this.m_DragObject = value; }
		}

		public virtual void OnBeginDrag2D(Vector2 position) {
			this.m_DragObject.transform.position = position;
		}

		public virtual void OnDrag2D(Vector2 position) {
			var newPosition = position + this.m_DragOffsetPosition;
			this.m_DragObject.transform.position = newPosition;
		}

		public virtual void OnEndDrag2D(Vector2 position) {
			var newPositon = position;
			newPositon.y = 0f;
			this.m_DragObject.transform.position = newPositon;
		}
		
	}
}
