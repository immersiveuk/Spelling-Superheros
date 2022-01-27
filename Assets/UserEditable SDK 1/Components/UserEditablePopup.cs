using System;
using Com.Immersive.Hotspots;

namespace Immersive.UserEditable
{
    public abstract class UserEditableHotspot : UserEditableComponent
    {

        private HotspotScript _hotspotScript = null;
        protected HotspotScript hotspotScript
        {
            get
            {
                if (_hotspotScript == null)
                    _hotspotScript = GetComponent<HotspotScript>();
                return _hotspotScript;
            }
        }

        private void OnEnable()
        {
            Enable();
        }

        protected override void SetDefaultPropertyValues()
        {
            SetDefaults();
        }

        protected abstract void Enable();
        protected abstract void SetDefaults();
    }
}