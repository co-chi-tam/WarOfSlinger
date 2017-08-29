using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CBuildingController : MonoBehaviour {

	#region Fields

	[Header("Object Control")]
	[SerializeField]	protected RedrawableCollider m_RedrawCollider;
	[SerializeField]	protected RedrawableSprite m_RedrawSprite;
    [SerializeField]    protected GameObject m_PrefabBrick;

    [Header("Build timer")]
    [SerializeField]    protected float m_DelayTime = 0.1f;
    [SerializeField]    protected float m_CountDownActiveTime = 1f;

    protected bool m_BuildingActive = false;
    protected float m_Delay = 0.1f;
    protected float m_CountDownActive = 1f;

    #endregion

    #region Implementation Moonobehaviour

    protected virtual void Awake() {
		// REGISTER EVENT
		this.m_RedrawCollider.OnEventColliderObject -= OnBuildingCollider;
		this.m_RedrawCollider.OnEventColliderObject += OnBuildingCollider;
	}

    protected virtual void Update() {
        if (Input.GetMouseButtonDown(0)) {
            var simpleBrick = Instantiate (this.m_PrefabBrick);
            var positionBrick = this.transform.position;
            positionBrick.x += Random.RandomRange(-1f, 1f);
            simpleBrick.transform.position = new Vector3(positionBrick.x, positionBrick.y + 1.5f, 0f);
        }

        // DELAY TIMER
        if (this.m_Delay > 0f) {
            this.m_Delay -= Time.deltaTime;
        }
        // COUNTDOWN TIMER
        if (this.m_CountDownActive > 0f) {
            this.m_CountDownActive -= Time.deltaTime;
        }
        // UPDATE BUILDING ACTIVE
        this.m_BuildingActive = this.m_Delay < 0f && this.m_CountDownActive < 0f;
    }

	#endregion

	#region Main methods

	protected virtual void OnBuildingCollider(Vector2 point, GameObject obj) {
        // BUILDING ACTIVED
        if (this.m_BuildingActive == false)
            return;
		// TEST
		this.m_RedrawSprite.Draw (point.x, point.y, 30);
        this.m_CountDownActive = this.m_CountDownActiveTime;

    }

	#endregion

}
