using System;

namespace Immersive.UserEditable.Enumerations
{
    [Serializable, Flags]
    public enum AudioFlags
    {
        Thumbnail = 1 << 0
    }
}