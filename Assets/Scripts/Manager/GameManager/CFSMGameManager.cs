using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using SimpleSingleton;
using FSM;
using SceneTask;

namespace WarOfSlinger {
	public partial class CGameManager {

		#region Fields

		protected FSMManager m_FSMManager;

		#endregion

		#region Main methods

		public virtual void RegisterFSM() {
			// LOAD FSM
			this.m_FSMManager = new FSMManager ();
			this.m_FSMManager.LoadFSM (this.m_GameModeTextAsset.text);
			// FSM STATE
			this.m_FSMManager.RegisterState ("GameFreeModeState", 		new FSMGameFreeModeState(this));
			this.m_FSMManager.RegisterState ("GameLoadingModeState", 	new FSMGameLoadingModeState(this));
			this.m_FSMManager.RegisterState ("GamePlayingModeState",	new FSMGamePlayingModeState(this));
			this.m_FSMManager.RegisterState ("GameBuildingModeState",	new FSMGameBuildingModeState(this));
			this.m_FSMManager.RegisterState ("GamePVPModeState", 		new FSMGamePVPModeState(this));
			this.m_FSMManager.RegisterState ("GamePVEModeState", 		new FSMGamePVEModeState(this));
			// FSM CONDITION
			this.m_FSMManager.RegisterCondition ("IsFreeMode", 			this.IsFreeMode);
			this.m_FSMManager.RegisterCondition ("IsLoadingMode",		this.IsLoadingMode);
			this.m_FSMManager.RegisterCondition ("IsPlayingMode",		this.IsPlayingMode);
			this.m_FSMManager.RegisterCondition ("IsBuildingMode",		this.IsBuildingMode);
			this.m_FSMManager.RegisterCondition ("IsPVPMode", 			this.IsPVPMode);
			this.m_FSMManager.RegisterCondition ("IsPVEMode", 			this.IsPVEMode);
		}

		#endregion

		#region FSM 

		public virtual bool IsFreeMode() {
			return this.m_GameMode == EGameMode.FREE;
		}

		public virtual bool IsLoadingMode() {
			return this.m_GameMode == EGameMode.LOADING;
		}

		public virtual bool IsPlayingMode() {
			return this.m_GameMode == EGameMode.PLAYING;
		}

		public virtual bool IsBuildingMode() {
			return this.m_GameMode == EGameMode.BUILDING;
		}

		public virtual bool IsPVEMode() {
			return this.m_GameMode == EGameMode.PVE;
		}

		public virtual bool IsPVPMode() {
			return this.m_GameMode == EGameMode.PVP;
		}

		#endregion
		
	}
}
