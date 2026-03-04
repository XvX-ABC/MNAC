using UnityEngine;

namespace MNAC.Interaction
{
    public class GameObjTarget_New : MonoBehaviour, IGameObjTarget_New
    {
        ITargetsCatcher_New<IGameObjTarget_New> _catcher;
        public GameObject Obj => this.gameObject;

        public ITargetsCatcher_New<IGameObjTarget_New> Catcher
        {
            get => _catcher;
            set
            {
                if (_catcher != null)
                    _catcher.RemoveItem(this);
                if (value != null)
                    value.AddItem(this);
                _catcher = value;
            }
        }
        void OnEnable()
        {
            _catcher?.AddItem(this);
        }
        void OnDisable()
        {
            _catcher.RemoveItem(this);
        }
        private void OnDestroy()
        {
            _catcher.RemoveItem(this);
        }

    }

}
