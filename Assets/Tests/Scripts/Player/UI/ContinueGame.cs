using UnityEngine;

namespace Tests.Players.UI
{
    internal class ContinueGame : MonoBehaviour
    {
        PlayerPanel _panel => PlayerPanel.instance;
        public void Execute()
        {
            _panel.Hide = true;
        }
    }
}
