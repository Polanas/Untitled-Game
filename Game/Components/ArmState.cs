using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game;

struct ArmsState : IEcsAutoReset<ArmsState>
{
    public Vector2[] armEndOffsets;

    public ArmsMode armsMode;

    public void AutoReset(ref ArmsState c)
    {
        c.armEndOffsets = new Vector2[2];
        c.armsMode = ArmsMode.Regular;
    }
}
