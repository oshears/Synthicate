using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Synthicate
{
	public class PlayerController : MonoBehaviour
	{
		float zoom_speed = 10000.0f;             // zoom speed
		float rotate_speed_x = 180.0f;              // Rotation speed
													//public float rotate_speed_x = 2.0f;              // Rotation speed

		[SerializeField]
		GameObject board;

		void Start()
		{
			// board = GameObject.Find("Board");

		}

		void Update()
		{


			//float xInput = Input.GetAxis("Horizontal");
			//float zInput = Input.GetAxis("Vertical");

			////Next, we want to get the vector direction to move in and apply it to our position.
			//Vector3 dir = transform.forward * zInput + transform.right * xInput;
			//transform.position += dir * 1f * Time.deltaTime;
			//float xMin = -80f;
			//float xMax = 80f;
			//float yMin = 30f;
			//float yMax = 80f;
			//float zMin = -90f;
			//float zMax = 90f;

			//Vector3 originalPos = transform.position;
			//Vector3 newPos = transform.position;

			//if (Input.mousePosition.x < Screen.width / 8)
			//    newPos -= transform.right * 20f * Time.deltaTime;
			//else if (Input.mousePosition.x > 7 * Screen.width / 8)
			//    newPos += transform.right * 20f * Time.deltaTime;

			//if (Input.mousePosition.y < Screen.height / 8)
			//    newPos -= transform.up * 20f * Time.deltaTime;
			//else if (Input.mousePosition.y > 7 * Screen.height / 8)
			//    newPos += transform.up * 20f * Time.deltaTime;

			//if (Input.mousePosition.x < Screen.width / 8)
			//{
			//    Vector3 xMovement = transform.right * 20f * Time.deltaTime;
			//    newPos = transform.position - xMovement;
				
			//}
			//else if (Input.mousePosition.x > 7 * Screen.width / 8)
			//{
			//    Vector3 xMovement = transform.right * 20f * Time.deltaTime;
			//    newPos = transform.position + xMovement;
			//}

			//if (
			//    newPos.x > xMin && newPos.x < xMax &&
			//    newPos.y > yMin && newPos.y < yMax &&
			//    newPos.z > zMin && newPos.z < zMax)
			//{
			//    transform.position = newPos;
			//}
			//else
			//{
			//    Debug.Log();
			//}




			float scrollwheel = Input.GetAxis("Mouse ScrollWheel");

			//zoom with scroll wheel; forward to zoom in, backward to scroll out.
			Vector3 zoomAmount = transform.forward * zoom_speed * scrollwheel * Time.deltaTime;
			if (transform.position.y + zoomAmount.y > 40f && transform.position.y + zoomAmount.y < 100f)
			{
				transform.Translate(zoomAmount, Space.World);
			}

			//Debug.Log(scrollwheel);

			// Orbit function using right mouse button pressed.
			if (Input.GetMouseButton(1))
			{
				float rotation_y = Input.GetAxis("Mouse X") * rotate_speed_x * Time.deltaTime;
				//transform.localEulerAngles = new Vector3(0, rotation_y, 0);
				transform.RotateAround(board.transform.position, Vector3.up, rotation_y);
			}
		}
	}
}