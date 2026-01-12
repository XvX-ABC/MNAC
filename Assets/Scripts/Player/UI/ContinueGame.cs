using UnityEngine;

namespace Tests.Players.UI
{
    internal class ContinueGame : MonoBehaviour
    {
        MainPlane _panel => MainPlane.Instance;
        public void Execute()
        {
            _panel.Hide = true;
        }
    }
}
