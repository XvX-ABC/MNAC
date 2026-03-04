using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace MNAC.Interaction
{
    public class InteractionHelper
    {
        public static bool CheckFriendly(TeamMask mask, GameObject obj)
        {
            if (obj.TryGetComponent<ITeamMember>(out var memeber))
                return memeber.CheckFriendlyBy(mask);
            return false;
        }
    }
}
