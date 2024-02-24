using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace Synthicate
{
    public class HexTextController : MonoBehaviour
    {

        //public string textValue;

        // Start is called before the first frame update
        void Start()
        {
            //gameObject.GetComponent<TextMeshPro>().text = textValue;
        }

        // Update is called once per frame
        void Update()
        {
            GameObject mainCamera = GameObject.FindGameObjectWithTag("MainCamera");

            if (mainCamera != null) transform.rotation = Quaternion.LookRotation( (transform.position - mainCamera.transform.position) * Time.deltaTime);
        }
    }
}