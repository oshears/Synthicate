using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;

namespace Synthicate
{
	public class GameManagerInitState : GameManagerAbstractState
	{

		public override void Enter()
		{
			changeState(_owner.mainMenuState);
		}
		
		public override void Execute()
		{

		}

		public override void Exit()
		{
			
		}
	}
}