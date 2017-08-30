using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WarOfSlinger {
    public class CCharacterController : CObjectController {

        #region Fields

        [Header("Character Data")]
        [SerializeField]    protected TextAsset m_TextAsset;
        [SerializeField]    protected CCharacterData m_CharacterData;

        #endregion

        #region Implementation Moonobehaviour

        public override void Init() {
            base.Init();
            // DATA
            this.m_CharacterData = TinyJSON.JSON.Load(this.m_TextAsset.text).Make<CCharacterData>();
        }

        protected override void Awake() {
            // TEST
            this.Init();

            base.Awake();
        }

        #endregion

    }
}
