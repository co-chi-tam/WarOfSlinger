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
	[SerializeField]	protected float m_SpritePercent = 1f;

	[Header ("Events")]
	public UnityEvent OnRedrawed;

    protected Sprite m_CurrentSprite;
    protected SpriteRenderer m_SpriteRenderer;
    protected Texture2D m_CurrentTexture;
	protected Transform m_Transform;
	// CALCULATE PERCENT CONTAIN
	protected float m_PixelUpdate = 1f;

    #endregion

    #region Implementation Monobehaviour

    protected virtual void Awake() {
        // CURRENT SPRITE RENDERER
        this.m_SpriteRenderer   = this.GetComponent<SpriteRenderer>();
        // SAVE SPRITE
        this.m_CurrentSprite    = this.m_SpriteRenderer.sprite;
        // SAVE TEXTURE
        this.m_CurrentTexture   = this.m_CurrentSprite.texture;
        // LOAD NEW TEXTURE
        var usedRect            = this.m_CurrentSprite.textureRect;
        this.m_ReTexture        = new Texture2D((int)usedRect.width, (int)usedRect.height);
        this.m_ReTexture.SetPixels(this.m_CurrentTexture.GetPixels((int) usedRect.x, 
                                                                    (int)usedRect.y, 
                                                                    (int)usedRect.width, 
                                                                    (int)usedRect.height));
        this.m_ReTexture.Apply();
        // LOAD NEW SPRITE
        this.m_ReSprite         = Sprite.Create (this.m_ReTexture, 
                                                                    new Rect(0f, 0f, usedRect.width, usedRect.height), 
                                                                    new Vector2(0.5f, 0.5f), 
                                                                    this.m_CurrentSprite.pixelsPerUnit);
        this.m_SpriteRenderer.sprite = this.m_ReSprite;
		// TRANSFORM
		this.m_Transform = this.transform;
    }

    #endregion

    #region Main methods

	// CONVERT WORLD POINT TO TEXTURE COORDINATES
	public virtual void Draw(float wX, float wY, int radius) {
		var usedRect	= this.m_CurrentSprite.textureRect;
		var extendRect 	= this.m_SpriteRenderer.bounds.extents;
		var parentPosition = this.m_Root.position;
		// X in world space
		var minX 		= parentPosition.x - extendRect.x + this.m_Transform.localPosition.x;	
		var maxX 		= parentPosition.x + extendRect.x + this.m_Transform.localPosition.x;
		// Y in world space
		var minY 		= parentPosition.y - extendRect.y + this.m_Transform.localPosition.y;			
		var maxY 		= parentPosition.y + extendRect.y + this.m_Transform.localPosition.y;	
		// SAMPLE X && Y
//		wX 				= wX > maxX ? maxX : wX;
//		wY 				= wY > maxY ? maxY : wY;
		var worldX 		= maxX - minX;
		var tmpX 		= wX - minX;
		var worldY 		= maxY - minY;
		var tmpY 		= wY - minY;
		var texturePointX = (tmpX / worldX) * usedRect.width;
		var texturePointY = (tmpY / worldY) * usedRect.height; 
		this.TransparentCircle (this.m_ReTexture, (int)texturePointX, (int)texturePointY, radius);
	}

	// SET ALPHA
	public virtual void TransparentCircle(Texture2D tex, int cx, int cy, int r)
    {
		// PARAMS
        int x, y, px, nx, py, ny, d;
		// GET PIXELS
        var tempArray = tex.GetPixels32();
		// COUNT PIXEL UPDATE
		var countPixelUpdate = 0;
		// UPDATE TEXTURE
        for (x = 0; x <= r; x++)
        {
            d = (int)Mathf.Ceil(Mathf.Sqrt(r * r - x * x));
            for (y = 0; y <= d; y++)
            {
                px = cx + x;
                nx = cx - x;
                py = cy + y;
                ny = cy - y;
				// UPDATE COLOR ARRAY
				var temp1 = py * tex.width + px;
				if (temp1 < tempArray.Length && temp1 > 0) {
					tempArray [py * tex.width + px].a = 0;
				}
				var temp2 = py * tex.width + nx;
				if (temp2 < tempArray.Length && temp2 > 0) {
					tempArray[py * tex.width + nx].a = 0;
				}
				var temp3 = ny * tex.width + px;
				if (temp3 < tempArray.Length && temp3 > 0) {
					tempArray[ny * tex.width + px].a = 0;
				}
				var temp4 = ny * tex.width + nx;
				if (temp4 < tempArray.Length && temp4 > 0) {
					tempArray[ny * tex.width + nx].a = 0;
				}
            }
        }
		// APPLY TEXTURE
        tex.SetPixels32(tempArray);
        tex.Apply();
		// UPDATE PERCENT
		var currentPixelUpdated = (float)countPixelUpdate / tempArray.Length;
		this.m_SpritePercent -= currentPixelUpdated;
		// INVOKE EVENTS
		if (this.OnRedrawed != null) {
			this.OnRedrawed.Invoke ();
		}
    }

    #endregion

}
