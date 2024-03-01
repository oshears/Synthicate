using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;

namespace Synthicate
{
	public class GameManagerInitState : GameManagerAbstractState
	{
		public GameManagerInitState(GameManager owner) : base(owner) 
		{
			
		}

		public override void Enter()
		{
			changeState(new GameManagerMainMenuState(_owner));
		}
		
		public override void Execute()
		{

		}

		public override void Exit()
		{
			
		}
	}
}