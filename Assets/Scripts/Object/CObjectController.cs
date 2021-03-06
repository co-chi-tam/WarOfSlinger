﻿using System;
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

		[Header("Renderer")]
		[SerializeField]	protected SpriteRenderer m_SpriteRender;

		[Header("Physic Collider")]
		[SerializeField]	protected RedrawableCollider m_Collider2D;
		[SerializeField]	protected Collider2D m_CharacterCollider;
		[SerializeField]	protected Rigidbody2D m_Rigidbody2D;

        protected List<CComponent> m_Components = new List<CComponent>();

		protected bool m_Active;
		protected bool m_Inited;
		protected Transform m_Transform;
		protected float m_Timer = 0f;
		// COMPONENTS
		protected CEventComponent m_EventComponent;
		protected CJobComponent m_JobComponent;

		protected Vector2 m_ColliderPoint;
		public Vector2 colliderPoint {
			get { return this.m_ColliderPoint; }
			protected set { this.m_ColliderPoint = value; }
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

		[SerializeField]	protected bool m_IsObjectWorking = false;
		public bool IsObjectWorking {
			get { return this.m_IsObjectWorking; }
			set { this.m_IsObjectWorking = value; }
		}

		#endregion

        #region Implementation Moonobehaviour

        public virtual void Init() {
			// INITED
			this.m_Inited = true;
			this.m_Active = true; 
			this.m_IsObjectWorking = true;
			// REGISTER COMPONENT
			this.m_EventComponent = new CEventComponent();
			this.m_JobComponent = new CJobComponent(this);
			this.RegisterComponent(this.m_EventComponent);
			this.RegisterComponent(this.m_JobComponent);
        }

		protected virtual void OnEnable() {
		
		}

		protected virtual void OnDisable() {
		
		}

        protected virtual void Awake() {
			this.m_Transform = this.transform;
        }

        protected virtual void Start() {
            for (int i = 0; i < this.m_Components.Count; i++) {
                this.m_Components[i].StartComponent();
            }
        }

		protected virtual void Update() {
			if (this.m_Inited == false || this.m_Active == false || this.m_IsObjectWorking == false)
				return;
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

		#region FSM

		public virtual bool IsActive ()
		{
			return this.GetActive();
		}

		public virtual bool After30Second() {
			this.m_Timer += Time.deltaTime;
			return this.m_Timer >= 30f;
		}

		public virtual bool After60Second() {
			this.m_Timer += Time.deltaTime;
			return this.m_Timer >= 60f;
		}

		public virtual bool After90Second() {
			this.m_Timer += Time.deltaTime;
			return this.m_Timer >= 90f;
		}

		#endregion

		#region JobOwner

		public virtual void ExcuteJobOwner(string jobName) {
			this.m_JobComponent.ExcuteActiveJob (this, jobName);
		}

		public virtual void ClearJobOwner(string name) {
			this.m_JobComponent.ClearJob (this, name);
		}

		public virtual void ClearJobLabor() {
			
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

		public virtual void OnDamageObject(Vector2 point, CObjectController target, int damage) {

		}

		public virtual void Talk(string value) {
			// REGISTER UI
			CGameManager.Instance.ShowTalk (this.m_UIJobPoint.transform, value);
		}

		public virtual void ResetTimer() {
			this.m_Timer = 0f;
		}

		public virtual void ResetOject() {
			
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

		public virtual CJobObjectData[] GetJobDatas() {
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
			if (this.m_Collider2D != null) 
				return this.m_Collider2D.collider2D;
			if (this.m_CharacterCollider != null)
				return this.m_CharacterCollider;
			return null;	
		}

		public virtual Vector3 GetClosestPoint(Vector3 point) {
			if (this.m_Collider2D != null) {
				return this.m_Collider2D.GetClosestPoint (point);
			} else if (this.m_CharacterCollider != null) {
				return this.m_CharacterCollider.bounds.ClosestPoint (point);
			}
			return this.m_Transform.position;
		}

		public virtual Vector3 GetUIPointPosition() {
			return this.m_UIJobPoint.transform.position;
		}

		public virtual CObjectController GetController() {
			return this;
		}

		public virtual string GetObjectType() {
			return string.Empty;
		}

		public virtual int GetCurrentHealth() {
			return 0;
		}

		public virtual void SetCurrentHealth(int value) {
			
		}

		public virtual int GetMaxHealth() {
			return 0;
		}

		public virtual void SetEnabledPhysic(bool value) {
			if (this.m_Rigidbody2D != null)
				this.m_Rigidbody2D.bodyType = value ? RigidbodyType2D.Dynamic : RigidbodyType2D.Static;
			if (this.m_Collider2D != null)
				this.m_Collider2D.enabledTrigger = !value;
			if (this.m_CharacterCollider != null)
				this.m_CharacterCollider.isTrigger = !value;
		}

		public virtual void SetColor(Color value) {
			if (this.m_SpriteRender != null) {
				this.m_SpriteRender.color = value;
			}
		}

		public virtual float GetActionSpeed() {
			return 0f;
		}

        #endregion

    }
}
