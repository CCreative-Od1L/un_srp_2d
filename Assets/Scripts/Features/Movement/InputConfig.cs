using UnSrp2d.Core.Contracts;
using UnityEngine;

namespace UnSrp2d.Features.Movement
{
    [CreateAssetMenu(fileName = "InputConfig", menuName = "UnSrp2d/Movement/InputConfig")]
    public class InputConfig : ScriptableObject, IInputConfig
    {
        public float Deadzone = 0.1f;

        float IInputConfig.Deadzone => Deadzone;
    }
}
