using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WarOfSlinger {
	public class CDamageableObjectController : CObjectController {

		[Header("Damageable Object")]
		[SerializeField]    protected RedrawableCollider m_RedrawCollider;
		[SerializeField]    protected RedrawableSprite m_RedrawSprite;

		[Header("Damage timer")]
		[SerializeField]    protected float m_DelayTime = 0.1f;
		[SerializeField]    protected float m_CountDownActiveTime = 1f;

		protected float m_Delay = 0.1f;
		protected float m_CountDownActive = 1f;
		protected bool m_DamageActive = true;

		protected override void Awake() {
			base.Awake();
			// REGISTER EVENT
			this.m_RedrawCollider.OnEventColliderObject -= OnBuildingCollider;
			this.m_RedrawCollider.OnEventColliderObject += OnBuildingCollider;
		}

		protected override void Update() {
			base.Update();
			// DELAY TIMER
			if (this.m_Delay > 0f) {
				this.m_Delay -= Time.deltaTime;
			}
			// COUNTDOWN TIMER
			if (this.m_CountDownActive > 0f) {
				this.m_CountDownActive -= Time.deltaTime;
			}
			// UPDATE BUILDING ACTIVE
			this.m_DamageActive = this.m_Delay < 0f && this.m_CountDownActive < 0f;
		}

		protected virtual void OnBuildingCollider(Vector2 point, GameObject obj) {
			// BUILDING ACTIVED
			if (this.m_DamageActive == false)
				return;
			// CIRCLE COLLIDER
			var circleCollider = obj.GetComponent<Collider2D> () as CircleCollider2D;
			var radius = circleCollider != null ? (int)(circleCollider.radius * 100) : 30;
			this.m_RedrawSprite.Draw(point.x, point.y, Mathf.Clamp (radius, 20, 100));
			this.m_CountDownActive = this.m_CountDownActiveTime;

		}

	}
}
