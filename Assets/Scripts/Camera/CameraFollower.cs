using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollower : MonoBehaviour {

    #region Fields

    [SerializeField]    protected Transform m_FollowTransform;
    [SerializeField]    protected float m_Damping = 0.1f;
    [SerializeField]    protected bool m_IsFollowing = false;

    protected Transform m_Transform;
	protected float m_FollowTimer = -1f;

	public Transform FollowTransform {
		get { return this.m_FollowTransform; }
		set { this.m_FollowTransform = value; }
	}

    public bool IsFollowing {
        get { return this.m_IsFollowing; }
        set { this.m_IsFollowing = value; }
    }

    #endregion

    #region Implementation Monobehaviour

    protected virtual void Awake() {
        this.m_Transform = this.transform;
    }

    protected virtual void Update() {
		if (this.m_IsFollowing == false || this.m_FollowTransform == null)
            return;
        var targetPosition  = this.m_FollowTransform.position;
        var newPosition     = Vector3.Lerp (this.m_Transform.position, targetPosition, this.m_Damping);
        newPosition.z       = this.m_Transform.position.z;
        this.m_Transform.position = newPosition;
		if (this.m_FollowTimer > 0f) {
			this.m_FollowTimer -= Time.deltaTime;
			if (this.m_FollowTimer <= 0f) {
				this.m_IsFollowing = false;
				this.m_FollowTransform = null;
			}
		}
    }

    #endregion

	#region Main methods

	public void FollowUntil (Transform value, float time) {
		this.m_FollowTransform = value;
		this.m_FollowTimer = time;
		this.IsFollowing = true;
	}

	#endregion

}
