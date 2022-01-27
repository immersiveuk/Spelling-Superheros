using System;
using System.Collections.Generic;
using Com.Immersive.Hotspots;
using Immersive.Enumerations;
using Immersive.UserEditable.Enumerations;
using Immersive.UserEditable.Properties;
using UnityEngine;

namespace Immersive.UserEditable
{
#if INTRO_END_SEQUENCE
    [UserEditable(typeof(IntroSequence))]
    public class UserEditableIntroSequence : UserEditableComponent
    {
        private IntroSequence introSequence;

        [SerializeField] private UserEditableFontData fontData;

        // StartUp Panel

        [HideInInspector, SerializeField]
        private UserEditableImageProperty backgroundImageProperty = new UserEditableImageProperty("Startup Background Image");

        [HideInInspector, SerializeField]
        private UserEditableImageProperty hotspotImageProperty = new UserEditableImageProperty("Startup Hotspot Image");

        [HideInInspector, SerializeField]
        private UserEditableColorProperty backgroundColourProperty = new UserEditableColorProperty("Startup Background Color");

        [HideInInspector, SerializeField]
        private UserEditableStringProperty startupBodyStringProperty = new UserEditableStringProperty("Startup Body Text");

        [HideInInspector, SerializeField]
        private UserEditableStringProperty startupTitleStringProperty = new UserEditableStringProperty("Startup Title Text");

        [HideInInspector, SerializeField]
        private UserEditableIntProperty startupBodyFontSizeProperty = new UserEditableIntProperty("Startup Body Font Size");

        [HideInInspector, SerializeField]
        private UserEditableIntProperty startupTitleFontSizeProperty = new UserEditableIntProperty("Startup Title Font Size");

        [HideInInspector, SerializeField]
        private UserEditableColorProperty startupBodyFontColourProperty = new UserEditableColorProperty("Startup Body Font Color");

        [HideInInspector, SerializeField]
        private UserEditableColorProperty startupTitleFontColourProperty = new UserEditableColorProperty("Startup Title Font Color");

        [HideInInspector, SerializeField]
        private UserEditableEnumProperty startupBodyAlignmentProperty = new UserEditableEnumProperty("Startup Body Alignment");

        [HideInInspector, SerializeField]
        private UserEditableEnumProperty startupTitleAlignmentProperty = new UserEditableEnumProperty("Startup Title Alignment");


        // Intro Panel

        [HideInInspector, SerializeField]
        private UserEditableImageProperty introBackgroundImageProperty = new UserEditableImageProperty("Intro Background Image");

        [HideInInspector, SerializeField]
        private UserEditableImageProperty introHotspotImageProperty = new UserEditableImageProperty("Intro Hotspot Image");

        [HideInInspector, SerializeField]
        private UserEditableStringProperty introBodyStringProperty = new UserEditableStringProperty("Intro Body Text");

        [HideInInspector, SerializeField]
        private UserEditableStringProperty introTitleStringProperty = new UserEditableStringProperty("Intro Title Text");

        [HideInInspector, SerializeField]
        private UserEditableIntProperty introBodyFontSizeProperty = new UserEditableIntProperty("Intro Body Font Size");

        [HideInInspector, SerializeField]
        private UserEditableIntProperty introTitleFontSizeProperty = new UserEditableIntProperty("Intro Title Font Size");

        [HideInInspector, SerializeField]
        private UserEditableColorProperty introBodyFontColourProperty = new UserEditableColorProperty("Intro Body Font Color");

        [HideInInspector, SerializeField]
        private UserEditableColorProperty introTitleFontColourProperty = new UserEditableColorProperty("Intro Title Font Color");

        [HideInInspector, SerializeField]
        private UserEditableEnumProperty introBodyAlignmentProperty = new UserEditableEnumProperty("Intro Body Alignment");

        [HideInInspector, SerializeField]
        private UserEditableEnumProperty introTitleAlignmentProperty = new UserEditableEnumProperty("Intro Title Alignment");


        [SerializeField] private ImageFlags imageFlags;
        [SerializeField] private TextFlags textFlags;

        private void OnEnable()
        {
            introSequence = GetComponent<IntroSequence>();

            if (fontData != null)
            {
                FontLoaded();
                fontData.FontLoaded += FontLoaded;
            }

            // Startup Panel

            backgroundImageProperty.ValueSet = OnBackgroundImageChanged;

            hotspotImageProperty.ValueSet = OnHotspotImageChanged;

            backgroundColourProperty.ValueSet = OnBackgroundColorChanged;

            startupBodyStringProperty.ValueSet = OnStartupBodyTextChanged;

            startupTitleStringProperty.ValueSet = OnStartupTitleTextChanged;

            startupBodyFontSizeProperty.ValueSet = OnStartupBodyFontSizeChanged;

            startupTitleFontSizeProperty.ValueSet = OnStartupTitleFontSizeChanged;

            startupBodyFontColourProperty.ValueSet = OnStartupBodyFontColourChanged;

            startupTitleFontColourProperty.ValueSet = OnStartupTitleFontColourChanged;

            startupBodyAlignmentProperty.ValueSet = OnStartupBodyAlignmentChanged;

            startupTitleAlignmentProperty.ValueSet = OnStartupTitleAlignmentChanged;

            // Intro Panel

            introBackgroundImageProperty.ValueSet = OnIntroBackgroundImageChanged;

            introHotspotImageProperty.ValueSet = OnIntroHotspotImageChanged;

            introBodyStringProperty.ValueSet = OnIntroBodyTextChanged;

            introTitleStringProperty.ValueSet = OnIntroTitleTextChanged;

            introBodyFontSizeProperty.ValueSet = OnIntroBodyFontSizeChanged;

            introTitleFontSizeProperty.ValueSet = OnIntroTitleFontSizeChanged;

            introBodyFontColourProperty.ValueSet = OnIntroBodyFontColourChanged;

            introTitleFontColourProperty.ValueSet = OnIntroTitleFontColourChanged;

            introBodyAlignmentProperty.ValueSet = OnIntroBodyAlignmentChanged;

            introTitleAlignmentProperty.ValueSet = OnIntroTitleAlignmentChanged;

            introSequence.UpdateSpritesAndAudio();
            introSequence.UpdateText();
        }
        private void OnDestroy()
        {
            if (fontData != null)
            {
                fontData.FontLoaded -= FontLoaded;
            }
        }

        #region Property Changed Callbacks

        private void OnIntroTitleAlignmentChanged()
        {
            if (!textFlags.HasFlag(TextFlags.Align)) return;

            introSequence.introTitle.Alignment = (HorizontalAlignment)introTitleAlignmentProperty.Value;
            introSequence.UpdateText();
        }

        private void OnIntroBodyAlignmentChanged()
        {
            if (!textFlags.HasFlag(TextFlags.Align)) return;

            introSequence.introText.Alignment = (HorizontalAlignment)introBodyAlignmentProperty.Value;
            introSequence.UpdateText();
        }

        private void OnStartupTitleAlignmentChanged()
        {
            if (!textFlags.HasFlag(TextFlags.Align)) return;

            introSequence.title.Alignment = (HorizontalAlignment)startupTitleAlignmentProperty.Value;
            introSequence.UpdateText();
        }

        private void OnStartupBodyAlignmentChanged()
        {
            if (!textFlags.HasFlag(TextFlags.Align)) return;

            introSequence.text.Alignment = (HorizontalAlignment)startupBodyAlignmentProperty.Value;
            introSequence.UpdateText();
        }

        private void OnIntroTitleFontColourChanged()
        {
            if (!textFlags.HasFlag(TextFlags.Color)) return;

            introSequence.introTitle.Color = introTitleFontColourProperty.Value;
            introSequence.UpdateText();
        }

        private void OnIntroBodyFontColourChanged()
        {
            if (!textFlags.HasFlag(TextFlags.Color)) return;

            introSequence.introText.Color = introBodyFontColourProperty.Value;
            introSequence.UpdateText();
        }

        private void OnIntroTitleFontSizeChanged()
        {
            if (!textFlags.HasFlag(TextFlags.Size)) return;

            introSequence.introTitle.FontSize = introTitleFontSizeProperty.Value;
            introSequence.UpdateText();
        }

        private void OnIntroBodyFontSizeChanged()
        {
            if (!textFlags.HasFlag(TextFlags.Size)) return;

            introSequence.introText.FontSize = introBodyFontSizeProperty.Value;
            introSequence.UpdateText();
        }

        private void OnStartupTitleFontColourChanged()
        {
            if (!textFlags.HasFlag(TextFlags.Color)) return;

            introSequence.title.Color = startupTitleFontColourProperty.Value;
            introSequence.UpdateText();
        }

        private void OnStartupBodyFontColourChanged()
        {
            if (!textFlags.HasFlag(TextFlags.Color)) return;

            introSequence.text.Color = startupBodyFontColourProperty.Value;
            introSequence.UpdateText();
        }

        private void OnStartupTitleFontSizeChanged()
        {
            if (!textFlags.HasFlag(TextFlags.Size)) return;

            introSequence.title.FontSize = startupTitleFontSizeProperty.Value;
            introSequence.UpdateText();
        }

        private void OnStartupBodyFontSizeChanged()
        {
            if (!textFlags.HasFlag(TextFlags.Size)) return;

            introSequence.text.FontSize = startupBodyFontSizeProperty.Value;
            introSequence.UpdateText();
        }

        private void OnBackgroundImageChanged()
        {
            if (!imageFlags.HasFlag(ImageFlags.Image)) return;

            introSequence.background.sprite = backgroundImageProperty.Value;
            introSequence.UpdateSpritesAndAudio();
        }

        private void OnIntroHotspotImageChanged()
        {
            if (!imageFlags.HasFlag(ImageFlags.Image)) return;

            introSequence.introHotspotSprite = introHotspotImageProperty.Value;
            introSequence.UpdateSpritesAndAudio();
        }

        private void OnHotspotImageChanged()
        {
            if (!imageFlags.HasFlag(ImageFlags.Image)) return;

            introSequence.startUpHotspotSprite = hotspotImageProperty.Value;
            introSequence.UpdateSpritesAndAudio();
        }

        private void OnBackgroundColorChanged()
        {
            if (!imageFlags.HasFlag(ImageFlags.Color)) return;

            introSequence.background.color = backgroundColourProperty.Value;
            introSequence.UpdateSpritesAndAudio();
        }

        private void OnStartupTitleTextChanged()
        {
            if (!textFlags.HasFlag(TextFlags.Text)) return;

            introSequence.title.Text = startupTitleStringProperty.Value;
            introSequence.UpdateText();
        }

        private void OnStartupBodyTextChanged()
        {
            if (!textFlags.HasFlag(TextFlags.Text)) return;

            introSequence.text.Text = startupBodyStringProperty.Value;
            introSequence.UpdateText();
        }


        private void OnIntroTitleTextChanged()
        {
            if (!textFlags.HasFlag(TextFlags.Text)) return;

            introSequence.introTitle.Text = introTitleStringProperty.Value;
            introSequence.UpdateText();
        }

        private void OnIntroBodyTextChanged()
        {
            if (!textFlags.HasFlag(TextFlags.Text)) return;

            introSequence.introText.Text = introBodyStringProperty.Value;
            introSequence.UpdateText();
        }

        private void OnIntroBackgroundImageChanged()
        {
            if (!imageFlags.HasFlag(ImageFlags.Image)) return;

            introSequence.introBackground.sprite = backgroundImageProperty.Value;
            introSequence.UpdateSpritesAndAudio();
        }

        #endregion

        protected override void SetDefaultPropertyValues()
        {
            if (introSequence == null)
                introSequence = GetComponent<IntroSequence>();

            backgroundColourProperty.SetDefaultValue(introSequence.background.color);

            startupTitleStringProperty.SetDefaultValue(introSequence.title.Text);

            startupBodyStringProperty.SetDefaultValue(introSequence.text.Text);

            introTitleStringProperty.SetDefaultValue(introSequence.introTitle.Text);

            introBodyStringProperty.SetDefaultValue(introSequence.introText.Text);

            startupBodyFontSizeProperty.SetDefaultValue(introSequence.text.FontSize);

            startupTitleFontSizeProperty.SetDefaultValue(introSequence.title.FontSize);

            startupBodyFontColourProperty.SetDefaultValue(introSequence.text.Color);

            startupTitleFontColourProperty.SetDefaultValue(introSequence.title.Color);

            introBodyFontSizeProperty.SetDefaultValue(introSequence.introText.FontSize);

            introTitleFontSizeProperty.SetDefaultValue(introSequence.introTitle.FontSize);

            introBodyFontColourProperty.SetDefaultValue(introSequence.introText.Color);

            introTitleFontColourProperty.SetDefaultValue(introSequence.introTitle.Color);

            introTitleAlignmentProperty.SetDefaultValue(introSequence.introTitle.Alignment);

            introBodyAlignmentProperty.SetDefaultValue(introSequence.introText.Alignment);

            startupTitleAlignmentProperty.SetDefaultValue(introSequence.title.Alignment);

            startupBodyAlignmentProperty.SetDefaultValue(introSequence.text.Alignment);
        }

        public override List<UserEditableProperty> UserEditableProperties
        {
            get
            {
                if (introSequence == null)
                    introSequence = GetComponent<IntroSequence>();

                List<UserEditableProperty> properties = new List<UserEditableProperty>();

                if (imageFlags.HasFlag(ImageFlags.Image))
                {
                    properties.Add(backgroundImageProperty);
                    properties.Add(introBackgroundImageProperty);
                    properties.Add(hotspotImageProperty);
                    properties.Add(introHotspotImageProperty);
                }

                if (imageFlags.HasFlag(ImageFlags.Color))
                {
                    properties.Add(backgroundColourProperty);
                }

                if (textFlags.HasFlag(TextFlags.Text))
                {
                    properties.Add(startupTitleStringProperty);
                    properties.Add(startupBodyStringProperty);
                    properties.Add(introTitleStringProperty);
                    properties.Add(introBodyStringProperty);
                }

                if (textFlags.HasFlag(TextFlags.Color))
                {
                    properties.Add(startupBodyFontColourProperty);
                    properties.Add(startupTitleFontColourProperty);
                    properties.Add(introBodyFontColourProperty);
                    properties.Add(introTitleFontColourProperty);
                }

                if (textFlags.HasFlag(TextFlags.Size))
                {
                    properties.Add(startupBodyFontSizeProperty);
                    properties.Add(startupTitleFontSizeProperty);
                    properties.Add(introBodyFontSizeProperty);
                    properties.Add(introTitleFontSizeProperty);
                }

                if (textFlags.HasFlag(TextFlags.Align))
                {
                    properties.Add(introTitleAlignmentProperty);
                    properties.Add(introBodyAlignmentProperty);
                    properties.Add(startupTitleAlignmentProperty);
                    properties.Add(startupBodyAlignmentProperty);
                }

                return properties;
            }
        }
        private void FontLoaded()
        {
            if (fontData != null && fontData.LoadedFont == null) return;

            introSequence.isRightToLeftText = fontData.UseRTL;
            introSequence.text.Font = fontData.LoadedFont;
            introSequence.title.Font = fontData.LoadedFont;
            introSequence.introText.Font = fontData.LoadedFont;
            introSequence.introTitle.Font = fontData.LoadedFont;

            introSequence.UpdateText();
        }
    }
#endif
}