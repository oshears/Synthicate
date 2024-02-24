using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Synthicate
{
    public class AudioController : MonoBehaviour
    {

        public AudioManagerSO audioManager;
        public GameManagerSO gameManager;

        public AudioSource mainThemeSource;
        public AudioSource sfxSource;

        // Start is called before the first frame update
        void Start()
        {
            // play main theme
            AudioSource[] sources = GetComponentsInChildren<AudioSource>();
            Debug.Assert(sources.Length > 1,"Error: Did not find at least 2 audio sources!");
            sfxSource = sources[0];
            mainThemeSource = sources[1];

            mainThemeSource.clip = audioManager.mainTheme;
            mainThemeSource.loop = true;
            mainThemeSource.Play();

            // event responders
            gameManager.nextTurnEvent.AddListener((uint diceValue) =>
            {

                sfxSource.clip = audioManager.nextTurnSound;
                sfxSource.Play();
            });

            gameManager.hackerRollEvent.AddListener(() =>
            {
                sfxSource.clip = audioManager.hackerSound;
                sfxSource.Play();
            });
            gameManager.playerHackEvent.AddListener(() =>
            {
                sfxSource.clip = audioManager.hackerSound;
                sfxSource.Play();
            });
            gameManager.playerCardEvent.AddListener((CardType cardType) =>
            {
                sfxSource.clip = audioManager.influencePointSound;
                sfxSource.Play();
            });
            gameManager.playerBuildEvent.AddListener(() =>
            {
                sfxSource.clip = audioManager.buildSound;
                sfxSource.Play();
            });
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}

