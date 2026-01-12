using Minimalist.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tests.Utilities.Composable;

namespace Tests.Characters
{
    internal class CharacterComponent : ComponentBase_MonoComponent
    {
    }
    internal class CharacterComponentSingleton<T> : Singleton<T> where T : CharacterComponent
    {

    }
}
