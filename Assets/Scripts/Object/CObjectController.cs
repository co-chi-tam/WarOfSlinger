using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleComponent;

namespace WarOfSlinger {
	public class CObjectController : MonoBehaviour, IJobOwner {

        #region Fields

		[Header("UI")]
		[SerializeField]	protected GameObject m_UIJobPoint;

		[Header("Animator")]
		[SerializeField]	protected Animator m_Animator;

		[Header("Collider")]
		[SerializeField]	protected RedrawableCollider m_Collider2D;

        protected List<CComponent> m_Components = new List<CComponent>();

		protected bool m_Active;
        protected CEventComponent m_EventComponent;
		protected Transform m_Transform;

		protected Vector2 m_ColliderPoint;
		public Vector2 colliderPoint {
			get { return this.m_ColliderPoint; }
			protected set { this.m_ColliderPoint = value; }
		}

        #endregion

		#region Properties

		public virtual bool IsObjectActive {
			get { return this.m_Active; }
			set { this.m_Active = value; }
		}

		public virtual Vector3 objectPosition {
			get { return this.m_Transform.position; }
			set { 
				value.y = 0;
				value.z = 0;
				this.m_Transform.position = value; 
			}
		}

		public float objectSide {
			get { return this.m_Transform.localScale.x; }
			set { this.m_Transform.localScale = new Vector3(value, 1f, 1f); }
		}

		public virtual Vector3 targetPosition {
			get { return Vector3.zero; }
			set {  }
		}

		public virtual Transform uiJobPoint{
			get { return this.m_UIJobPoint.transform; }
		}

		#endregion

        #region Implementation Moonobehaviour

        public virtual void Init() {
            this.m_EventComponent = new CEventComponent();
            this.RegisterComponent(this.m_EventComponent);
        }

        protected virtual void Awake() {
			this.m_Active = true;  
			this.m_Transform = this.transform;
        }

        protected virtual void Start() {
            for (int i = 0; i < this.m_Components.Count; i++) {
                this.m_Components[i].StartComponent();
            }
        }

        protected virtual void Update() {
            for (int i = 0; i < this.m_Components.Count; i++) {
                this.m_Components[i].UpdateComponent(Time.deltaTime);
            }
        }

        //protected virtual void OnDestroy() {
        //    for (int i = 0; i < this.m_Components.Count; i++) {
        //        this.m_Components[i].EndComponent();
        //    }
        //}

        #endregion

		#region JobOwner

		public virtual void ExcuteJobOwner(string jobName) {
			
		}

		public virtual void ClearJobOwner(string name) {
			
		}

		#endregion

        #region Components

        // REGISTER COMPONENT
        public virtual void RegisterComponent(CComponent value) {
            if (this.m_Components.Contains(value))
                return;
            this.m_Components.Add(value);
        }

        #endregion

        #region Main methods

        public virtual void InvokeAction(string name) {
            this.m_EventComponent.TriggerCallback(name);
        }

        public virtual void InvokeAction(string name, params object[] prams) {
            this.m_EventComponent.TriggerCallback(name, prams);
        }

        public virtual void AddAction(string name, System.Action callback) {
            this.m_EventComponent.AddCallback(name, (objs) => {
                if (callback != null) {
                    callback();
                }
            });
        }

        public virtual void AddAction(string name, System.Action<object[]> callbacks) {
            this.m_EventComponent.AddCallback(name, callbacks);
        }

		public virtual void OnDamageObject(Vector2 point, int radius) {

		}

		public virtual void Talk(string value) {
			// REGISTER UI
			CGameManager.Instance.ShowTalk (this.m_UIJobPoint.transform, value);
		}

        #endregion

        #region Getter && Setter

		public virtual void SetAnimation (string name, object param = null)
		{
			if (this.m_Animator == null)
				return;
			if (param is int) {
				this.m_Animator.SetInteger (name, (int)param);
			} else if (param is bool) {
				this.m_Animator.SetBool (name, (bool)param);
			} else if (param is float) {
				this.m_Animator.SetFloat (name, (float)param);
			} else if (param == null) {
				this.m_Animator.SetTrigger (name);
			}
		}

        public virtual void SetData(CObjectData value) {

        }

        public virtual CObjectData GetData() {
            return null;
        }

		public virtual void SetActive(bool value) {
			this.m_Active = value;
		}

		public virtual bool GetActive() {
			return this.m_Active;
		}

		public virtual void SetPosition(Vector3 value) {
			value.y = 0f;
			value.z = 0f;
			this.m_Transform.position = value;	
		}

		public virtual Vector3 GetPosition() {
			return this.m_Transform.position;
		}

		public virtual void SetTargetPosition(Vector3 value) {
			
		}

		public virtual Vector3 GetTargetPosition() {
			return this.m_Transform.position;
		}

		public virtual CObjectController GetTargetController() {
			return null;
		}

		public virtual void SetTargetController(CObjectController value) {
		
		}

		public virtual Collider2D GetCollider() {
			return this.m_Collider2D.collider;
		}

		public virtual Vector3 GetClosestPoint(Vector3 point) {
			if (this.m_Collider2D.collider == null)
				return this.m_Transform.position;
			return this.m_Collider2D.GetClosestPoint (point);
		}

		public virtual Vector3 GetUIPointPosition() {
			return this.m_UIJobPoint.transform.position;
		}

		public virtual CObjectController GetController() {
			return this;
		}

        #endregion

    }
}
