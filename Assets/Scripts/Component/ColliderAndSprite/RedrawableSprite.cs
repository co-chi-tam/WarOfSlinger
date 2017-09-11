using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof (SpriteRenderer))]
public class RedrawableSprite : MonoBehaviour {

    #region Fields

	[Header("Parent")]
	[SerializeField]	protected Transform m_Root;

    [Header("Repaint texture")]
    [SerializeField]    protected Texture2D m_ReTexture;
    [SerializeField]    protected Sprite m_ReSprite;
    [SerializeField]    protected List<Vector2> m_RedrawList;
	[SerializeField]	protected bool m_DrawOnAwake = true;

	[Header ("Events")]
	public UnityEvent OnRedrawed;
    public UnityEvent OnAllBlank;

    protected Sprite m_CurrentSprite;
    protected SpriteRenderer m_SpriteRenderer;
    protected Texture2D m_CurrentTexture;
	protected Transform m_Transform;
	protected bool m_IsSpriteVisible = true;
	public bool IsSpriteVisible {
		get { return this.m_IsSpriteVisible; }
		set { this.m_IsSpriteVisible = value; }
	}

    #endregion

    #region Implementation Monobehaviour

    protected virtual void Awake() {
		if (this.m_DrawOnAwake) {
			// SETUP SPRITE
			this.SetupSprite ();
		}
        // SAVE LIST
        this.m_RedrawList = new List<Vector2>();
        // TRANSFORM
        this.m_Transform = this.transform;
		// VISIBLE SPRITE
		this.m_IsSpriteVisible = true;
    }

    #endregion

    #region Main methods

	public virtual void SetupSprite() {
		// CURRENT SPRITE RENDERER
		this.m_SpriteRenderer   = this.GetComponent<SpriteRenderer>();
		// SAVE SPRITE
		this.m_CurrentSprite    = this.m_SpriteRenderer.sprite;
		// SAVE TEXTURE
		this.m_CurrentTexture   = this.m_CurrentSprite.texture;
		// LOAD NEW TEXTURE
		var usedRect            = this.m_CurrentSprite.textureRect;
		this.m_ReTexture        = new Texture2D((int)usedRect.width, (int)usedRect.height, TextureFormat.ARGB32, false);
		this.m_ReTexture.SetPixels(this.m_CurrentTexture.GetPixels( (int)usedRect.x, 
																	(int)usedRect.y, 
																	this.m_ReTexture.width, 
																	this.m_ReTexture.height ));
		this.m_ReTexture.Apply();
		// LOAD NEW SPRITE
		this.m_ReSprite         = Sprite.Create (this.m_ReTexture,  new Rect(0f, 0f, this.m_ReTexture.width, this.m_ReTexture.height), 
																	new Vector2(0.5f, 0.5f), 
																	this.m_CurrentSprite.pixelsPerUnit);
		this.m_SpriteRenderer.sprite = this.m_ReSprite;
	}

	// CONVERT WORLD POINT TO TEXTURE COORDINATES
	public virtual void Draw(float wX, float wY, int radius) {
		var usedRect	= this.m_CurrentSprite.textureRect;
		var extendRect 	= this.m_SpriteRenderer.bounds.extents;
		var parentPosition = this.m_Root != null ? this.m_Root.position : Vector3.zero;
		// X in world space
		var minX 		= parentPosition.x - extendRect.x + this.m_Transform.localPosition.x;	
		var maxX 		= parentPosition.x + extendRect.x + this.m_Transform.localPosition.x;
		// Y in world space
		var minY 		= parentPosition.y - extendRect.y + this.m_Transform.localPosition.y;			
		var maxY 		= parentPosition.y + extendRect.y + this.m_Transform.localPosition.y;
		// SAMPLE X && Y
		var worldX 		= maxX - minX;
		var tmpX 		= wX - minX;
		var worldY 		= maxY - minY;
		var tmpY 		= wY - minY;
		var texturePointX = (tmpX / worldX) * usedRect.width;
		var texturePointY = (tmpY / worldY) * usedRect.height;
        // SAVE POINT
        var newPoint = new Vector2(texturePointX, texturePointY);
        if (this.m_RedrawList.Contains (newPoint) == false) {
            this.m_RedrawList.Add(newPoint);
        }
        this.TransparentCircle (this.m_ReTexture, (int)texturePointX, (int)texturePointY, radius);
	}

	// SET ALPHA
	public virtual void TransparentCircle(Texture2D tex, int cx, int cy, int rd)
    {
        var width = tex.width;
        var height = tex.height;
        var tempArray = tex.GetPixels();
        var minX = cx - rd < 0 ? 0 : cx - rd;
        var maxX = cx + rd > width ? width : cx + rd;
        var minY = cy - rd < 0 ? 0 : cy - rd;
        var maxY = cy + rd > height ? height : cy + rd;
        for (int x = minX; x < maxX; x++) {
            for (int y = minY; y < maxY; y++) {
                var dx = x - cx;
                var dy = y - cy;
                var dist = Mathf.Sqrt(dx * dx + dy * dy);
                if (dist < rd) {
                    var color = tex.GetPixel(x, y);
                    color.a = 0;
                    tex.SetPixel(x, y, color);
                }
            }
        }
        tex.Apply();
        // COUNT PIXEL UPDATE
		this.m_IsSpriteVisible = false;
        for (int i = 0; i < tempArray.Length; i++) {
			this.m_IsSpriteVisible |= tempArray[i].a != 0;
        }
        // INVOKE EVENTS
        if (this.OnRedrawed != null) {
			this.OnRedrawed.Invoke ();
        }
        // INVOKE EVENTS
		if (this.m_IsSpriteVisible == false) { 
            if (this.OnAllBlank != null) {
                this.OnAllBlank.Invoke();
            }
        }
    }

    #endregion

}
