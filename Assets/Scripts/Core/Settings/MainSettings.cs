using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Core.Settings
{
    [Serializable]
    public class MainSettings
    {
        #region Resolution

        public int Height;
        public int Width;

        #endregion

        public int DistanceForRed;
    }
}
