using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Infrastructure.Enums
{
    public enum AudioTheme
    {
        None = 0,
        [LongName("Assets/Audio/Music/MainMusic.ogg")]
        MainMenu,
        [LongName("Assets/Audio/Music/LevelMusic.ogg")]
        Level
    }
}
