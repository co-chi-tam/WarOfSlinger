using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollower : MonoBehaviour {

    #region Fields

    [SerializeField]    protected Transform m_FollowTransform;
    [SerializeField]    protected float m_Damping = 0.1f;
    [SerializeField]    protected bool m_IsFollowing = false;

    protected Transform m_Transform;

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
        if (this.m_IsFollowing == false)
            return;
        var targetPosition  = this.m_FollowTransform.position;
        var newPosition     = Vector3.Lerp (this.m_Transform.position, targetPosition, this.m_Damping);
        newPosition.z       = this.m_Transform.position.z;
        this.m_Transform.position = newPosition;
    }

    #endregion

}
