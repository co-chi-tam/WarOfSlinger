using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraBound : MonoBehaviour {

    #region Fields

    // MAP X & Y
    [Header("Bound info")]
    [SerializeField]    protected SpriteRenderer m_MapSprite;

    // CAMERA X
    [SerializeField] protected float m_MinX;
    [SerializeField] protected float m_MaxX;

    // CAMERA Y
    [SerializeField] protected float m_MinY;
    [SerializeField] protected float m_MaxY;

    protected Transform m_Transform;
    protected float m_MapX = 100f;
    protected float m_MapY = 100f;

    #endregion

    #region Implementation Monobehaviour

    protected virtual void Awake() {
        this.m_Transform = this.transform;
    }

	protected virtual void Start () {
        // BOUNDS
        var bounds = this.m_MapSprite.bounds;
        this.m_MapX = bounds.extents.x * 2f;
        this.m_MapY = bounds.extents.y * 2f ;
        // HORIZAL && VERTICLE
        var vertExtent = Camera.main.orthographicSize;
        var horzExtent = vertExtent * Screen.width / Screen.height;
        // X
        this.m_MinX = horzExtent - this.m_MapX / 2f + this.m_MapSprite.transform.position.x;
        this.m_MaxX = this.m_MapX / 2f - horzExtent + this.m_MapSprite.transform.position.x;
        // Y
        this.m_MinY = vertExtent - this.m_MapY / 2f + this.m_MapSprite.transform.position.y;
        this.m_MaxY = this.m_MapY / 2f - vertExtent + this.m_MapSprite.transform.position.y;
	}
	
	protected virtual void LateUpdate () {
        var v3 = this.m_Transform.position;
        v3.x = Mathf.Clamp(v3.x, this.m_MinX, m_MaxX);
        v3.y = Mathf.Clamp(v3.y, this.m_MinY, m_MaxY);
        this.m_Transform.position = v3;
	}

    #endregion

}
