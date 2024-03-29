using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.PlayerLoop;
using UnityEngine.UI;


namespace Synthicate
{
	public class UiPlayerViewManager : UiUpdatableElementMonoBehavior
	{
		[SerializeField] GameObject[] playerViews;
		
		[SerializeField] GameObject[] playerTurnIndicators;
		
		[Header("Event Channels")]
		
		[SerializeField] EventChannelSO m_UpdateUiEventChannel;
		
		override protected void Awake() {
			base.Awake();
			
			m_UpdateUiEventChannel.OnEventRaised += UpdateUserInterfaceEventHandler;
		}
		
		void Start()
		{
			// Disable all Ui player views
			// for (int i = 0; i < playerViews.Length; i++)
			// {
			// 	playerViews[i].SetActive(false);
			// }
		}
		
		override protected void InitilizeUserInterfaceEventHandler()
		{
			for (int i = 0; i < playerViews.Length; i++)
			{
				playerViews[i].SetActive(false);
				playerTurnIndicators[i].SetActive(false);
			}
			for (int i = 0; i < gameManagerSO.playerList.Count; i++)
			{
				playerViews[i].SetActive(true);
				playerViews[i].GetComponent<UiPlayerViewController>().SetPlayerName(gameManagerSO.playerList[i].GetName());
				playerViews[i].GetComponent<UiPlayerViewController>().SetPlayerColor(i == 0 ? Color.blue : i == 1 ? Color.green : i == 2 ? Color.yellow : Color.red);
				playerViews[i].GetComponent<UiPlayerViewController>().SetHackerCount(0);
			}
		}
		
		override protected void UpdateUserInterfaceEventHandler()
		{
			for (int i = 0; i < gameManagerSO.playerList.Count; i++)
			{
				playerViews[i].GetComponent<UiPlayerViewController>().SetInfluenceCount(gameManagerSO.playerList[i].GetNumHackersUsed());
				
				playerTurnIndicators[i].SetActive(false);
				
				if (gameManagerSO.currentPlayerTurn == gameManagerSO.playerList[i].GetId())
				{
					playerTurnIndicators[i].SetActive(true);
				}
			}
		}
	}
}