using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SimpleSingleton;

namespace WarOfSlinger {
    public class CGameManager : CMonoSingleton<CGameManager> {

        [Header ("Test point")]
		[SerializeField]	protected CameraFollower m_Follower;
		[SerializeField]	protected Transform m_StartPosition;
		[SerializeField]	protected Dragable m_Drag;
		[SerializeField]	protected Rigidbody2D m_BulletPrefab;
		[Range(1f, 90f)]
		[SerializeField]	protected float m_Angle = 45f;
		[SerializeField]	protected Image m_UIAngle;
		[Range(1f, 20f)]
		[SerializeField]	protected float m_Force = 5f;
		[SerializeField]	protected Image m_UIForce;

        protected virtual void Start() {
			
        }

		protected virtual void Update() {


			if (Input.GetMouseButtonDown (0)) {
				this.m_Drag.angle = this.m_Angle;
				this.m_Drag.power = this.m_Force;
				var ball = this.m_Drag.createBall (this.m_BulletPrefab, this.m_StartPosition.position);
				this.m_Drag.throwBall ();
//				this.m_Follower.FollowTransform = ball.transform;
//				this.m_Follower.IsFollowing = true;
			}

		}
		
	}
}
