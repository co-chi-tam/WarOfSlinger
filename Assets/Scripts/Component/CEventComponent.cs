using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleComponent;

namespace WarOfSlinger {
	public class CEventComponent : CComponent {

        #region Fields

        protected Dictionary<string, EVCallback> m_Events;

        #endregion

        #region Internal class

        public class EVCallback {
            public Action<object[]> callback;
        }

        #endregion

        #region Constructor

        public CEventComponent() : base() {
            this.m_Events = new Dictionary<string, EVCallback>();
        }

        #endregion

        #region Main methods

        public virtual void AddCallback(string name, Action<object[]> callback) {
			if (this.m_Events.ContainsKey (name))
				return;
			var callbackEvent = new EVCallback ();
			callbackEvent.callback = callback;
			this.m_Events.Add (name, callbackEvent);
		}

		public virtual void RemoveCallback(string name) {
			if (this.m_Events.ContainsKey (name) == false)
				return;
			this.m_Events.Remove (name);
		}

		public virtual void TriggerCallback(string name) {
			if (this.m_Events.ContainsKey (name) == false)
				return;
			if (this.m_Events [name].callback != null) {
				this.m_Events [name].callback (null);
			}
		}

		public virtual void TriggerCallback(string name, params object[] prams) {
			if (this.m_Events.ContainsKey (name) == false)
				return;
			if (this.m_Events [name].callback != null) {
				this.m_Events [name].callback (prams);
			}
		}

        #endregion

    }
}
