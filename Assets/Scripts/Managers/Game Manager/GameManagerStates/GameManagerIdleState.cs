using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Synthicate
{
	public class GameManagerIdleState : GameManagerAbstractState
	{
		
		public enum MenuState
		{
			Default,
			Expanded,
			Build,
			Hack,	
		}
		
		MenuState _menuState;
		
		public GameManagerIdleState(GameManager owner) : base(owner) 
		{
			_menuState = MenuState.Default;
		}

		public override void Enter()
		{
			
		}
		
		public override void Execute()
		{

		}

		public override void Exit()
		{
			
		}

		public override void OnGUI()
		{
			if (_menuState == MenuState.Default)
			{
				GUI.Box(UserInterface.s_gameMenuArea, "");
				// game menu box
				GUILayout.BeginArea(UserInterface.s_gameMenuArea);
				//GUILayout.Label("Current Turn: Player " + gameManagerSO.getGUITurn());
				if (GUILayout.Button("Menu")){
					_menuState = MenuState.Expanded;
				}
				if(GUILayout.Button("Next Turn"))
				{
					_gameManagerSO.OnStartNextTurn();
					changeState(new GameManagerDiceState(_owner));
				}
				GUILayout.EndArea();
			}
			else if (_menuState == MenuState.Expanded)
			{
				GUI.Box(UserInterface.s_expandedGameMenu, "");

				// game menu box
				GUILayout.BeginArea(UserInterface.s_expandedGameMenu);
				//GUILayout.Label("Current Turn: Player " + gameManagerSO.getGUITurn());
				if (GUILayout.Button("Build"))
				{
					// _menuState = MenuState.BuildMode;

					// gameManagerSO.beginClientBuildMode();
					changeState(new GameManagerBuildingState(_owner));
				}
				if (GUILayout.Button("Trade"))
				{
					// tradeRequestSelection = 0;
					// tradeRequestSlider = 1f;
					// tradeOfferSelection = 0;
					// tradeOfferSlider = 1f;
					// _menuState = MenuState.TradeRequest;
				}
				if(GUILayout.Button("Event Card"))
				{
					// _menuState = MenuState.Active;
					// gameManagerSO.clientRequestEventCard();
				}
				if (GUILayout.Button("Hack"))
				{
					// _menuState = MenuState.HackMode;
					// gameManagerSO.beginClientHackMode();
				}
				if (GUILayout.Button("Close"))
				{
					_menuState = MenuState.Default;
				}
				if(GUILayout.Button("Next Turn"))
				{
					_gameManagerSO.OnStartNextTurn();
					changeState(new GameManagerDiceState(_owner));
				}
				// if(GUILayout.Button("Next Turn")) gameManagerSO.nextTurn();
				GUILayout.EndArea();
			}
			

			// resource box
		}
		
		

	}
}