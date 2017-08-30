using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleComponent;

namespace WarOfSlinger {
    public class CObjectController : MonoBehaviour {

        #region Fields

        protected List<CComponent> m_Components = new List<CComponent>();

        protected CEventComponent m_EventComponent;

        #endregion

        #region Implementation Moonobehaviour

        public virtual void Init() {
            this.m_EventComponent = new CEventComponent();
            this.RegisterComponent(this.m_EventComponent);
        }

        protected virtual void Awake() {
           
        }

        protected virtual void Start() {
            for (int i = 0; i < this.m_Components.Count; i++) {
                this.m_Components[i].StartComponent();
            }
        }

        protected virtual void Update() {
            for (int i = 0; i < this.m_Components.Count; i++) {
                this.m_Components[i].UpdateComponent(Time.deltaTime);
            }
        }

        //protected virtual void OnDestroy() {
        //    for (int i = 0; i < this.m_Components.Count; i++) {
        //        this.m_Components[i].EndComponent();
        //    }
        //}

        #endregion

        #region Components

        // REGISTER COMPONENT
        public virtual void RegisterComponent(CComponent value) {
            if (this.m_Components.Contains(value))
                return;
            this.m_Components.Add(value);
        }

        #endregion

        #region Main methods

        public virtual void InvokeAction(string name) {
            this.m_EventComponent.TriggerCallback(name);
        }

        public virtual void InvokeAction(string name, params object[] prams) {
            this.m_EventComponent.TriggerCallback(name, prams);
        }

        public virtual void AddAction(string name, System.Action callback) {
            this.m_EventComponent.AddCallback(name, (objs) => {
                if (callback != null) {
                    callback();
                }
            });
        }

        public virtual void AddAction(string name, System.Action<object[]> callbacks) {
            this.m_EventComponent.AddCallback(name, callbacks);
        }

        #endregion

        #region Getter && Setter

        public virtual void SetData(CObjectData value) {

        }

        public virtual CObjectData GetData() {
            return null;
        }

        #endregion

    }
}
