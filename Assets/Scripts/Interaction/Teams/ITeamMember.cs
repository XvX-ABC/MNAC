using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests.Interaction
{
    public interface ITeamMember
    {
        public bool CheckFriendlyBy(TeamMask mask)
        {
            return TeamMask.Contains(mask.Value);
        }
        TeamMask TeamMask { get; set; }
    }
}
