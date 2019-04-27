using LD44.World;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LD44.InputModes
{
    public class SelectionMode : InputMode
    {
        public SelectionMode(Galaxy map) : base(map)
        {
        }

        public override InputPreviewData GetInputPreviewData()
        {
            return null;   
        }

        public override void Select()
        {
            
        }

        public override void Unselect()
        {
            
        }

        public override void Update(double dt)
        {
            
        }
    }
}
