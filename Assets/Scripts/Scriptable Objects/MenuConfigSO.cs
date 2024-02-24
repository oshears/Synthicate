using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Synthicate {


    [CreateAssetMenu(fileName = "MenuConfigSO", menuName = "ScriptableObjects/MenuConfigSO")]
    public class MenuConfigSO : ScriptableObject
    {
        string playerName;
        int sfxVolume;
        int musicVolume;

    }

}