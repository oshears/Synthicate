using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Synthicate
{
    [CreateAssetMenu(fileName = "AudioManagerSO", menuName = "ScriptableObjects/Audio Manager")]
    public class AudioManagerSO : ScriptableObject
    {

        public AudioClip mainTheme;
        public AudioClip menuSelectionSound;
        public AudioClip hackerSound;
        public AudioClip nextTurnSound;
        public AudioClip buildSound;
        public AudioClip influencePointSound;

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}

