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
    [UserEditable(typeof(EndSequence))]
    public class UserEditableEndSequence : UserEditableComponent
    {
        private EndSequence endSequence;

        [SerializeField] private UserEditableFontData fontData;

        [HideInInspector, SerializeField]
        private UserEditableImageProperty backgroundImageProperty = new UserEditableImageProperty("Background Image");

        [HideInInspector, SerializeField]
        private UserEditableColorProperty backgroundColourProperty = new UserEditableColorProperty("Background Image Color");

        [HideInInspector, SerializeField]
        private UserEditableStringProperty subTitleStringProperty = new UserEditableStringProperty("Sub-Title Text");

        [HideInInspector, SerializeField]
        private UserEditableStringProperty titleStringProperty = new UserEditableStringProperty("Title Text");

        [HideInInspector, SerializeField]
        private UserEditableIntProperty subTitleFontSizeProperty = new UserEditableIntProperty("Sub-Title Font Size");

        [HideInInspector, SerializeField]
        private UserEditableIntProperty titleFontSizeProperty = new UserEditableIntProperty("Title Font Size");

        [HideInInspector, SerializeField]
        private UserEditableColorProperty subTitleFontColourProperty = new UserEditableColorProperty("Sub-Title Font Color");

        [HideInInspector, SerializeField]
        private UserEditableColorProperty titleFontColourProperty = new UserEditableColorProperty("Title Font Color");

        [HideInInspector, SerializeField]
        private UserEditableEnumProperty subTitleAlignmentProperty = new UserEditableEnumProperty("Sub-Title Alignment");

        [HideInInspector, SerializeField]
        private UserEditableEnumProperty titleAlignmentProperty = new UserEditableEnumProperty("Title Alignment");

        [HideInInspector, SerializeField]
        private UserEditableImageProperty titleImageProperty = new UserEditableImageProperty("Title Image");

        [HideInInspector, SerializeField]
        private UserEditableAudioProperty audioProperty = new UserEditableAudioProperty("Audio");

        [HideInInspector, SerializeField]
        private UserEditableImageProperty hotspotImageProperty = new UserEditableImageProperty("Hotspot Image");

        [HideInInspector, SerializeField]
        private UserEditableBoolProperty onlyShowAchievedStarsProperty = new UserEditableBoolProperty("Only Show Achieved Stars");

        [HideInInspector, SerializeField]
        private UserEditableBoolProperty removeAnimationIfUnachievedProperty = new UserEditableBoolProperty("Remove Animation if Unachieved");

        [HideInInspector, SerializeField]
        private UserEditableFloatProperty oneStarThresholdProperty = new UserEditableFloatProperty("One Star Threshold");

        [HideInInspector, SerializeField]
        private UserEditableFloatProperty twoStarThresholdProperty = new UserEditableFloatProperty("Two Star Threshold");

        [HideInInspector, SerializeField]
        private UserEditableFloatProperty threeStarThresholdProperty = new UserEditableFloatProperty("Three Star Threshold");

        [HideInInspector, SerializeField]
        private UserEditableImageProperty starOneImageProperty = new UserEditableImageProperty("Star One Image");

        [HideInInspector, SerializeField]
        private UserEditableImageProperty starOneUnachievedImageProperty = new UserEditableImageProperty("Star One Unachieved Image");

        [HideInInspector, SerializeField]
        private UserEditableImageProperty starTwoImageProperty = new UserEditableImageProperty("Star Two Image");

        [HideInInspector, SerializeField]
        private UserEditableImageProperty starTwoUnachievedImageProperty = new UserEditableImageProperty("Star Two Unachieved Image");

        [HideInInspector, SerializeField]
        private UserEditableImageProperty starThreeImageProperty = new UserEditableImageProperty("Star Three Image");

        [HideInInspector, SerializeField]
        private UserEditableImageProperty starThreeUnachievedImageProperty = new UserEditableImageProperty("Star Three Unachieved Image");

        [HideInInspector, SerializeField]
        private UserEditableFloatProperty spacingProperty = new UserEditableFloatProperty("Star Spacing");

        [SerializeField] private ImageFlags imageFlags;
        [SerializeField] private TextFlags textFlags;

        private void OnEnable()
        {
            endSequence = GetComponent<EndSequence>();

            if (fontData != null)
            {
                FontLoaded();
                fontData.FontLoaded += FontLoaded;
            }

            backgroundImageProperty.ValueSet = OnBackgroundImageChanged;

            backgroundColourProperty.ValueSet = OnBackgroundColorChanged;

            subTitleStringProperty.ValueSet = OnSubTitleTextChanged;

            titleStringProperty.ValueSet = OnTitleTextChanged;

            subTitleFontSizeProperty.ValueSet = OnSubTitleFontSizeChanged;

            titleFontSizeProperty.ValueSet = OnTitleFontSizeChanged;

            subTitleFontColourProperty.ValueSet = OnSubTitleFontColourChanged;

            titleFontColourProperty.ValueSet = OnTitleFontColourChanged;

            subTitleAlignmentProperty.ValueSet = OnSubTitleAlignmentChanged;

            titleAlignmentProperty.ValueSet = OnTitleAlignmentChanged;
            
            titleImageProperty.ValueSet = OnTitleImageChanged;

            audioProperty.ValueSet = OnAudioChanged;

            hotspotImageProperty.ValueSet = OnHotspotImageChanged;

            onlyShowAchievedStarsProperty.ValueSet = OnShowStarsChanged;

            removeAnimationIfUnachievedProperty.ValueSet = OnRemoveAnimationChanged;

            oneStarThresholdProperty.ValueSet = OnOneStarThresholdChanged;

            twoStarThresholdProperty.ValueSet = OnTwoStarThresholdChanged;

            threeStarThresholdProperty.ValueSet = OnThreeStarThresholdChanged;

            starOneImageProperty.ValueSet = OnStarOneImageChanged;

            starOneUnachievedImageProperty.ValueSet = OnStarOneUnachievedImageChanged;

            starTwoImageProperty.ValueSet = OnStarTwoImageChanged;

            starTwoUnachievedImageProperty.ValueSet = OnStarTwoUnachievedImageChanged;

            starThreeImageProperty.ValueSet = OnStarThreeImageChanged;

            starThreeUnachievedImageProperty.ValueSet = OnStarThreeUnachievedImageChanged;

            spacingProperty.ValueSet = OnSpacingChanged;

            endSequence.UpdateSpritesAndAudio();
            endSequence.UpdateText();
            endSequence.DisplaySettings();
        }
        private void OnDestroy()
        {
            if (fontData != null)
            {
                fontData.FontLoaded -= FontLoaded;
            }
        }

        #region Property Changed Callbacks
        private void OnSpacingChanged()
        {
            endSequence.starSpacing = spacingProperty.Value;
        }

        private void OnStarThreeUnachievedImageChanged()
        {
            if (!imageFlags.HasFlag(ImageFlags.Image)) return;

            endSequence.starThreeInactiveSprite = starThreeUnachievedImageProperty.Value;
            endSequence.UpdateSpritesAndAudio();
        }

        private void OnStarThreeImageChanged()
        {
            if (!imageFlags.HasFlag(ImageFlags.Image)) return;

            endSequence.starThreeSprite = starThreeImageProperty.Value;
            endSequence.UpdateSpritesAndAudio();
        }

        private void OnStarTwoUnachievedImageChanged()
        {
            if (!imageFlags.HasFlag(ImageFlags.Image)) return;

            endSequence.starTwoInactiveSprite = starTwoUnachievedImageProperty.Value;
            endSequence.UpdateSpritesAndAudio();
        }

        private void OnStarTwoImageChanged()
        {
            if (!imageFlags.HasFlag(ImageFlags.Image)) return;

            endSequence.starTwoSprite = starTwoImageProperty.Value;
            endSequence.UpdateSpritesAndAudio();
        }

        private void OnStarOneUnachievedImageChanged()
        {
            if (!imageFlags.HasFlag(ImageFlags.Image)) return;

            endSequence.starOneInactiveSprite = starOneUnachievedImageProperty.Value;
            endSequence.UpdateSpritesAndAudio();
        }

        private void OnStarOneImageChanged()
        {
            if (!imageFlags.HasFlag(ImageFlags.Image)) return;

            endSequence.starOneSprite = starOneImageProperty.Value;
            endSequence.UpdateSpritesAndAudio();
        }

        private void OnThreeStarThresholdChanged()
        {
            endSequence.threeStarThreshold = threeStarThresholdProperty.Value;
            endSequence.CalculateStars();
        }

        private void OnTwoStarThresholdChanged()
        {
            endSequence.twoStarThreshold = twoStarThresholdProperty.Value;
            endSequence.CalculateStars();
        }

        private void OnOneStarThresholdChanged()
        {
            endSequence.oneStarThreshold = oneStarThresholdProperty.Value;
            endSequence.CalculateStars();
        }

        private void OnRemoveAnimationChanged()
        {
            endSequence.removeAnimationFromInactiveStars = removeAnimationIfUnachievedProperty.Value;
            endSequence.CalculateStars();
        }

        private void OnShowStarsChanged()
        {
            endSequence.onlyShowAchievedStars = onlyShowAchievedStarsProperty.Value;
            endSequence.CalculateStars();
        }

        private void OnHotspotImageChanged()
        {
            if (!imageFlags.HasFlag(ImageFlags.Image)) return;

            endSequence.hotspotSprite = hotspotImageProperty.Value;
            endSequence.UpdateSpritesAndAudio();
        }

        private void OnAudioChanged()
        {
            endSequence.endAudio = audioProperty.Value;
        }

        private void OnTitleImageChanged()
        {
            if (!imageFlags.HasFlag(ImageFlags.Image)) return;

            endSequence.titleImageSprite = titleImageProperty.Value;
            endSequence.UpdateSpritesAndAudio();
        }

        private void OnTitleAlignmentChanged()
        {
            if (!textFlags.HasFlag(TextFlags.Align)) return;

            endSequence.titleText.Alignment = (HorizontalAlignment)titleAlignmentProperty.Value;
            endSequence.UpdateText();
        }

        private void OnSubTitleAlignmentChanged()
        {
            if (!textFlags.HasFlag(TextFlags.Align)) return;

            endSequence.subTitleText.Alignment = (HorizontalAlignment)subTitleAlignmentProperty.Value;
            endSequence.UpdateText();
        }

        private void OnTitleFontColourChanged()
        {
            if (!textFlags.HasFlag(TextFlags.Color)) return;

            endSequence.titleText.Color = titleFontColourProperty.Value;
            endSequence.UpdateText();
        }

        private void OnSubTitleFontColourChanged()
        {
            if (!textFlags.HasFlag(TextFlags.Color)) return;

            endSequence.subTitleText.Color = subTitleFontColourProperty.Value;
            endSequence.UpdateText();
        }

        private void OnTitleFontSizeChanged()
        {
            if (!textFlags.HasFlag(TextFlags.Size)) return;

            endSequence.titleText.FontSize = titleFontSizeProperty.Value;
            endSequence.UpdateText();
        }

        private void OnSubTitleFontSizeChanged()
        {
            if (!textFlags.HasFlag(TextFlags.Size)) return;

            endSequence.subTitleText.FontSize = subTitleFontSizeProperty.Value;
            endSequence.UpdateText();
        }

        private void OnBackgroundImageChanged()
        {
            if (!imageFlags.HasFlag(ImageFlags.Image)) return;

            endSequence.background.sprite = backgroundImageProperty.Value;
            endSequence.UpdateSpritesAndAudio();
        }

        private void OnBackgroundColorChanged()
        {
            if (!imageFlags.HasFlag(ImageFlags.Color)) return;

            endSequence.background.color = backgroundColourProperty.Value;
            endSequence.UpdateSpritesAndAudio();
        }

        private void OnTitleTextChanged()
        {
            if (!textFlags.HasFlag(TextFlags.Text)) return;

            endSequence.titleText.Text = titleStringProperty.Value;
            endSequence.UpdateText();
        }

        private void OnSubTitleTextChanged()
        {
            if (!textFlags.HasFlag(TextFlags.Text)) return;

            endSequence.subTitleText.Text = subTitleStringProperty.Value;
            endSequence.UpdateText();
        }

        #endregion

        protected override void SetDefaultPropertyValues()
        {
            if (endSequence == null)
                endSequence = GetComponent<EndSequence>();

            backgroundColourProperty.SetDefaultValue(endSequence.background.color);

            titleStringProperty.SetDefaultValue(endSequence.titleText.Text);

            subTitleStringProperty.SetDefaultValue(endSequence.subTitleText.Text);

            subTitleFontSizeProperty.SetDefaultValue(endSequence.subTitleText.FontSize);

            titleFontSizeProperty.SetDefaultValue(endSequence.titleText.FontSize);

            subTitleFontColourProperty.SetDefaultValue(endSequence.subTitleText.Color);

            titleFontColourProperty.SetDefaultValue(endSequence.titleText.Color);

            titleAlignmentProperty.SetDefaultValue(endSequence.titleText.Alignment);

            subTitleAlignmentProperty.SetDefaultValue(endSequence.subTitleText.Alignment);

            onlyShowAchievedStarsProperty.SetDefaultValue(endSequence.onlyShowAchievedStars);

            removeAnimationIfUnachievedProperty.SetDefaultValue(endSequence.removeAnimationFromInactiveStars);

            oneStarThresholdProperty.SetDefaultValues(endSequence.oneStarThreshold, 0f, 1f);

            twoStarThresholdProperty.SetDefaultValues(endSequence.twoStarThreshold);

            threeStarThresholdProperty.SetDefaultValues(endSequence.threeStarThreshold, 0f, 1f);

            spacingProperty.SetDefaultValues(endSequence.starSpacing, 0f, 350f);
        }

        public override List<UserEditableProperty> UserEditableProperties
        {
            get
            {
                if (endSequence == null)
                    endSequence = GetComponent<EndSequence>();

                List<UserEditableProperty> properties = new List<UserEditableProperty>();

                if (imageFlags.HasFlag(ImageFlags.Image))
                {
                    properties.Add(backgroundImageProperty);

                    properties.Add(titleImageProperty);

                    properties.Add(hotspotImageProperty);

                    properties.Add(starOneImageProperty);

                    properties.Add(starOneUnachievedImageProperty);

                    properties.Add(starTwoImageProperty);

                    properties.Add(starTwoUnachievedImageProperty);

                    properties.Add(starThreeImageProperty);

                    properties.Add(starThreeUnachievedImageProperty);

                }

                if (textFlags.HasFlag(TextFlags.Text))
                {
                    properties.Add(titleStringProperty);

                    properties.Add(subTitleStringProperty);
                }

                if (textFlags.HasFlag(TextFlags.Color))
                {
                    properties.Add(subTitleFontColourProperty);

                    properties.Add(titleFontColourProperty);
                }

                if (textFlags.HasFlag(TextFlags.Size))
                {
                    properties.Add(subTitleFontSizeProperty);

                    properties.Add(titleFontSizeProperty);
                }

                if (textFlags.HasFlag(TextFlags.Align))
                {

                    properties.Add(titleAlignmentProperty);

                    properties.Add(subTitleAlignmentProperty);
                }

                properties.Add(backgroundColourProperty);

                properties.Add(audioProperty);

                properties.Add(onlyShowAchievedStarsProperty);

                properties.Add(removeAnimationIfUnachievedProperty);

                properties.Add(oneStarThresholdProperty);

                properties.Add(twoStarThresholdProperty);

                properties.Add(threeStarThresholdProperty);

                properties.Add(spacingProperty);

                return properties;
            }
        }

        private void FontLoaded()
        {
            if (fontData != null && fontData.LoadedFont == null) return;

            endSequence.isRightToLeftText = fontData.UseRTL;
            endSequence.titleText.Font = fontData.LoadedFont;
            endSequence.subTitleText.Font = fontData.LoadedFont;
            endSequence.scoreTitleText.Font = fontData.LoadedFont;
            endSequence.scoreText.Font = fontData.LoadedFont;
            endSequence.knowledgeTitleText.Font = fontData.LoadedFont;

            endSequence.UpdateText();
        }
    }
#endif
}