using System;
using System.Collections.Generic;
using Com.Immersive.Hotspots;
using Immersive.Enumerations;
using Immersive.UserEditable.Enumerations;
using Immersive.UserEditable.Properties;
using UnityEngine;

namespace Immersive.UserEditable
{
    [UserEditable(typeof(HotspotScript))]
    public class UserEditableTextPopup : UserEditableHotspot
    {
        [SerializeField] private UserEditableFontData fontData;

        [HideInInspector, SerializeField] private UserEditableStringProperty titleTextProperty = new UserEditableStringProperty("Title");
        [HideInInspector, SerializeField] private UserEditableStringProperty bodyTextProperty = new UserEditableStringProperty("Body");
        [HideInInspector, SerializeField] private UserEditableFloatProperty titleSizeProperty = new UserEditableFloatProperty("Title Size");
        [HideInInspector, SerializeField] private UserEditableFloatProperty bodySizeProperty = new UserEditableFloatProperty("Body Size");
        [HideInInspector, SerializeField] private UserEditableColorProperty titleColorProperty = new UserEditableColorProperty("Title Color");
        [HideInInspector, SerializeField] private UserEditableColorProperty bodyColorProperty = new UserEditableColorProperty("Body Color");
        [HideInInspector, SerializeField] private UserEditableEnumProperty<HorizontalAlignment> titleAlignProperty = new UserEditableEnumProperty<HorizontalAlignment>("Title Alignment");
        [HideInInspector, SerializeField] private UserEditableEnumProperty<HorizontalAlignment> bodyAlignProperty = new UserEditableEnumProperty<HorizontalAlignment>("Body Alignment");

        [SerializeField] private TextFlags titleFlags;
        [SerializeField] private TextFlags bodyFlags;

        protected override void Enable()
        {
            if (fontData != null)
            {
                FontLoaded();
                fontData.FontLoaded += FontLoaded;
            }

            titleTextProperty.ValueSet = OnTitleTextChanged;
            titleSizeProperty.ValueSet = OnTitleSizeChanged;
            titleColorProperty.ValueSet = OnTitleColorChanged;
            titleAlignProperty.OnValueSet = OnTitleAlignChanged;

            bodyTextProperty.ValueSet = OnBodyTextChanged;
            bodySizeProperty.ValueSet = OnBodySizeChanged;
            bodyColorProperty.ValueSet = OnBodyColorChanged;
            bodyAlignProperty.ValueSet = OnBodyAlignChanged;
        }

        private void OnDestroy()
        {
            if (fontData != null)
            {
                fontData.FontLoaded -= FontLoaded;
            }
        }

        public override List<UserEditableProperty> UserEditableProperties
        {
            get
            {
                List<UserEditableProperty> properties = new List<UserEditableProperty>();

                if(hotspotScript.textPopUpDataModel.popUpSetting.includeTitle || titleFlags.HasFlag(TextFlags.Text))
                    properties.Add(titleTextProperty);

                if (titleFlags.HasFlag(TextFlags.Size))
                {
                    properties.Add(titleSizeProperty);
                }

                if (titleFlags.HasFlag(TextFlags.Color))
                {
                    properties.Add(titleColorProperty);
                }

                if (titleFlags.HasFlag(TextFlags.Align))
                {
                    properties.Add(titleAlignProperty);
                }

                properties.Add(bodyTextProperty);

                if (bodyFlags.HasFlag(TextFlags.Size))
                {
                    properties.Add(bodySizeProperty);
                }

                if (bodyFlags.HasFlag(TextFlags.Color))
                {
                    properties.Add(bodyColorProperty);
                }

                if (bodyFlags.HasFlag(TextFlags.Align))
                {
                    properties.Add(bodyAlignProperty);
                }

                return properties;
            }
        }

        protected override void SetDefaults()
        {
            titleTextProperty.SetDefaultValue(hotspotScript.textPopUpDataModel.popUpSetting.title.Text);
            titleSizeProperty.SetDefaultValues(hotspotScript.textPopUpDataModel.popUpSetting.title.FontSize);
            titleColorProperty.SetDefaultValue(hotspotScript.textPopUpDataModel.popUpSetting.title.Color);


            bodyTextProperty.SetDefaultValue(hotspotScript.textPopUpDataModel.popUpSetting.body.Text);
            bodySizeProperty.SetDefaultValues(hotspotScript.textPopUpDataModel.popUpSetting.body.FontSize);
            bodyColorProperty.SetDefaultValue(hotspotScript.textPopUpDataModel.popUpSetting.body.Color);

            titleAlignProperty.SetDefaultValue(HorizontalAlignment.Left);
            bodyAlignProperty.SetDefaultValue(HorizontalAlignment.Left);
        }

        private void OnTitleTextChanged() => hotspotScript.textPopUpDataModel.popUpSetting.title.Text = titleTextProperty.Value;
        private void OnTitleSizeChanged() => hotspotScript.textPopUpDataModel.popUpSetting.title.FontSize = (int) titleSizeProperty.Value;
        private void OnTitleColorChanged() => hotspotScript.textPopUpDataModel.popUpSetting.title.Color = titleColorProperty.Value;
        private void OnTitleAlignChanged(HorizontalAlignment newValue) => hotspotScript.textPopUpDataModel.popUpSetting.title.Alignment = newValue;
        private void OnBodyTextChanged() => hotspotScript.textPopUpDataModel.popUpSetting.body.Text = bodyTextProperty.Value;
        private void OnBodySizeChanged() => hotspotScript.textPopUpDataModel.popUpSetting.body.FontSize = (int) bodySizeProperty.Value;
        private void OnBodyColorChanged() => hotspotScript.textPopUpDataModel.popUpSetting.body.Color = bodyColorProperty.Value;
        private void OnBodyAlignChanged() => hotspotScript.textPopUpDataModel.popUpSetting.body.Alignment = (HorizontalAlignment) bodyAlignProperty.Value;

        private void FontLoaded()
        {
            if (fontData != null && fontData.LoadedFont == null) return;

            hotspotScript.textPopUpDataModel.popUpSetting.isRightToLeftText = fontData.UseRTL;
            hotspotScript.textPopUpDataModel.popUpSetting.body.Font = fontData.LoadedFont;
            hotspotScript.textPopUpDataModel.popUpSetting.title.Font = fontData.LoadedFont;
        }
    }
}
