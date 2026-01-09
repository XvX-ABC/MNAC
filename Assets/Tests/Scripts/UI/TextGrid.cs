using Tests.Utilities.Blackboards;
using UnityEngine;

namespace Tests.UI
{
    public class TextGrid : UIComponent
    {
        [SerializeField]
        TextBox _textBox_0;
        [SerializeField]
        TextBox _textBox_1;
        [SerializeField]
        TextBox _textBox_2;
        [SerializeField]
        TextBox _textBox_3;

        public TextBox TextBox_0
        {
            get
            {
                _textBox_0.gameObject.SetActive(true);
                return _textBox_0;
            }
        }
        public TextBox TextBox_1
        {
            get
            {
                _textBox_1.gameObject.SetActive(true);
                return _textBox_1;
            }
        }
        public TextBox TextBox_2
        {
            get
            {
                _textBox_2.gameObject.SetActive(true);
                return _textBox_2;
            }
        }
        public TextBox TextBox_3
        {
            get
            {
                _textBox_3.gameObject.SetActive(true);
                return _textBox_3;
            }
        }
        
        protected override void Awake()
        {
            base.Awake();
            _textBox_0.gameObject.SetActive(false);
            _textBox_1.gameObject.SetActive(false);
            _textBox_2.gameObject.SetActive(false);
            _textBox_3.gameObject.SetActive(false);
        }
        public override void Initialize(Blackboard blackboard)
        {
            base.Initialize(blackboard);
            blackboard.TryRegisterFieldOrWriteValue(UIBlackboardFields.Weapons_Text_Grid, this);
        }
        public override void Dispose()
        {
            blackboard.TryUnregisterField(UIBlackboardFields.Weapons_Text_Grid);
            base.Dispose();
        }
    }
}
