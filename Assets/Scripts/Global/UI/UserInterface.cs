using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Synthicate
{
	public class UserInterface
	{
		public static int s_screenBlockWidth = Screen.width / 16;
		public static int s_screenBlockHeight = Screen.height / 16;
		// Entire Screen Area
		public static Rect s_screenArea = new Rect(0, 0, Screen.width, Screen.height);
		// Game Menu Area for Player Actions
		public static Rect s_gameMenuArea = new Rect(0, 14 * s_screenBlockHeight, 2 * s_screenBlockWidth, 2 * s_screenBlockHeight);
		// Expanded Game Menu Area for Player Actions
		public static Rect s_expandedGameMenu = new Rect(0, 11 * s_screenBlockHeight, 2 * s_screenBlockWidth, 5 * s_screenBlockHeight);

		// Expanded Game Menu Area for Player Actions
		public static Rect s_instructionArea = new Rect(0, 11 * s_screenBlockHeight, 4 * s_screenBlockWidth, 5 * s_screenBlockHeight);

		// Status Area Displaying Player Resources
		public static Rect s_playerResourcesArea = new Rect(13 * s_screenBlockWidth, 0, 3 * s_screenBlockWidth, 5 * s_screenBlockHeight);

		// Status Area Displaying Players' Influnece Points
		public static Rect s_playerStatusArea = new Rect(0,0,3 * s_screenBlockWidth,5 * s_screenBlockHeight);

		// Pop up area for event cards and general events
		public static Rect s_tradeEventArea = new Rect(6 * s_screenBlockWidth, 6 * s_screenBlockHeight, 4 * s_screenBlockWidth, 4 * s_screenBlockHeight);
		public static Rect s_playerEventArea = new Rect(11 * s_screenBlockWidth, 14 * s_screenBlockHeight, 5 * s_screenBlockWidth, s_screenBlockHeight);

		// Trade Menu Area
		public static Rect s_tradeMenuArea = new Rect(6 * s_screenBlockWidth, 3 * s_screenBlockHeight, 4 * s_screenBlockWidth, 7 * s_screenBlockHeight);
		// DEBUG AREA
		public static Rect s_debugArea = new Rect(6*s_screenBlockWidth,15 * s_screenBlockHeight, 3 * s_screenBlockWidth, s_screenBlockHeight);

	}
}