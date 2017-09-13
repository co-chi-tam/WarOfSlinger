using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WarOfSlinger {
	public class CDamageableObjectController : CObjectController {

		#region Fields

		[Header("Damageable Object")]
		[SerializeField]    protected RedrawableCollider m_RedrawCollider;
		[SerializeField]    protected RedrawableSprite m_RedrawSprite;

		[Header("Damage timer")]
		[SerializeField]    protected float m_ActiveDamageAfterTime = 0.1f;

		protected float m_CountDownActive = 0.1f;
		protected bool m_DamageActive = true;

		#endregion

		#region Implementation Monobehaviour

		protected override void Awake() {
			base.Awake();
			// REGISTER EVENT
			this.m_RedrawCollider.OnEventEnterColliderObject -= OnObjectColliderWith;
			this.m_RedrawCollider.OnEventEnterColliderObject += OnObjectColliderWith;
		}

		protected override void Update() {
			base.Update();
			if (this.m_Inited == false)
				return;
			// COUNTDOWN TIMER
			if (this.m_CountDownActive > 0f) {
				this.m_CountDownActive -= Time.deltaTime;
			}
			// UPDATE BUILDING ACTIVE
			this.m_DamageActive = this.m_CountDownActive < 0f;
		}

		#endregion

		#region Main methods

		protected virtual void OnObjectColliderWith(Vector2 point, GameObject obj) {
			// BUILDING ACTIVED
			if (this.m_DamageActive == false)
				return;
			// CIRCLE COLLIDER
//			var circleCollider = obj.GetComponent<Collider2D> () as CircleCollider2D;
//			var radius = circleCollider != null ? (int)(circleCollider.radius * 100) : 30;
//			this.OnDamageObject (point, radius);
			this.m_ColliderPoint = point;
		}

		public override void OnDamageObject(Vector2 point, CObjectController target, int damage) {
			base.OnDamageObject (point, target, damage);
			this.m_RedrawSprite.Draw(point.x, point.y, Mathf.Clamp (damage, 30, 100));
			this.m_CountDownActive = this.m_ActiveDamageAfterTime;
		}

		#endregion

		#region Getter && Setter

		public override void SetCurrentHealth(int value) {
			
		}

		public override int GetCurrentHealth() {
			return base.GetCurrentHealth();
		}

		public override int GetMaxHealth() {
			return base.GetMaxHealth();
		}

		#endregion

	}
}
