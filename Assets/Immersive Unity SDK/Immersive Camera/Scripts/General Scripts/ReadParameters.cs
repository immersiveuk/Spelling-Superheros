 /* Copyright (C) Immersive Interactive, Ltd - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by Luke Bissell <luke@immersive.co.uk>, July 2019
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mono.Options;
using System;
using System.Text.RegularExpressions;

namespace Com.Immersive.Cameras
{
    public class ReadParameters : MonoBehaviour
    {

        private static Settings _settings;
        public static Settings Settings { get { return _settings; } }

        // Use this for initialization
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        public static void Init()
        {
            if (Application.isEditor) return;

            string[] _params;
#if UNITY_WEBGL
            _params = GetParamsWebGl();
#else
            _params = GetParams();
#endif
            _settings = new Settings();
           
            var p = new OptionSet()
            {
                //File being split up
                { "file=",      v => _settings.FilePath = RemoveQuotationMarks(v) },
                { "position=",  v => { _settings.Position = Settings.ConvertToRect(RemoveQuotationMarks(v)); Debug.LogError(_settings.Position); } },
                { "masks=",   v => { _settings.Masks = Settings.ConvertToRectList(RemoveQuotationMarks(v)); } },
                { "stretch=",   v => { _settings.StretchToFit = Convert.ToBoolean(RemoveQuotationMarks(v)); } },
                { "server=",   v => { _settings.Server = RemoveQuotationMarks(v); } },
                { "port=",   v => { _settings.Port = Convert.ToInt32(RemoveQuotationMarks(v)); } },
                { "debug=",   v => { _settings.Debug = Convert.ToBoolean(RemoveQuotationMarks(v)); } },
                { "screenshot=", v =>  {_settings.Screenshot = Convert.ToBoolean(RemoveQuotationMarks(v)); } },
                { "outdir=", v => { _settings.OutDir = Convert.ToString(RemoveQuotationMarks(v)); } },
                { "url=", v => { _settings.Url = Convert.ToString(RemoveQuotationMarks(v)); } },
                { "dvd=", v =>  {_settings.DVD = Convert.ToBoolean(RemoveQuotationMarks(v)); } },
                { "dvdDrive=", v =>  {_settings.DvdDrive = Convert.ToString(RemoveQuotationMarks(v)); } },
                { "miLightIP=", v => {_settings.MiLightIP = Convert.ToString(RemoveQuotationMarks(v)); } },
                { "miPort=", v => {_settings.MiLightPort = Convert.ToInt32(RemoveQuotationMarks(v)); } },
                { "appPort=", v => { _settings.AppPort = Convert.ToInt32(RemoveQuotationMarks(v)); } },
                { "responseAppPort=", v => { _settings.ResponseAppPort = Convert.ToInt32(RemoveQuotationMarks(v)); } },
                { "volume=", v => { _settings.Volume = Convert.ToInt32(RemoveQuotationMarks(v)); } },
                { "autoColour=", v => { _settings.AutoMatchColour = Convert.ToBoolean(RemoveQuotationMarks(v)); }},
                { "dmxStartChannel=", v => { _settings.DmxStartChannel =  Convert.ToInt32(RemoveQuotationMarks(v)); }},
                { "startTime=", v => { _settings.startTime =  Convert.ToInt32(RemoveQuotationMarks(v)); }},
                { "stopTime=", v => { _settings.stopTime=  Convert.ToInt32(RemoveQuotationMarks(v)); }},
                { "remotingPort=", v => { _settings.remotingPort = Convert.ToInt32(RemoveQuotationMarks(v)); }},
                { "language=", v => { _settings.language = RemoveQuotationMarks(v).ToLower(); }},
                { "dmxDevice=", v => { _settings.DmxDevice = Convert.ToInt32(RemoveQuotationMarks(v)); }},
                { "label=", v => { _settings.Label = Convert.ToString(RemoveQuotationMarks(v)); }},
                { "sceneID=", v => { _settings.SceneID = Convert.ToInt32(RemoveQuotationMarks(v)); }},
                { "sceneListID=", v => { _settings.SceneListID = Convert.ToInt32(RemoveQuotationMarks(v)); }},
                { "sceneItemID=", v => { _settings.SceneItemID = Convert.ToInt32(RemoveQuotationMarks(v)); }},
                { "appURL=", v => { _settings.AppURL = Convert.ToString(RemoveQuotationMarks(v)); }},
                { "loop=", v => { _settings.Loop = Convert.ToBoolean(RemoveQuotationMarks(v)); }},
                { "scaleMode=", v => { _settings.ScaleMode = string.IsNullOrEmpty(RemoveQuotationMarks(v)) ?
                   ScaleMode.Fill : (ScaleMode)Enum.Parse(typeof(ScaleMode), RemoveQuotationMarks(v));}},
                { "layout=", v => {Debug.LogError(v); _settings.Layout =  (SurfacePosition)Enum.Parse(typeof(SurfacePosition), RemoveQuotationMarks(v));} },
                { "videoFormat=", v => { Settings.SetVideoFormat(RemoveQuotationMarks(v)); } },
                { "multidisplay", v => {Debug.Log("Multidisplay"); } },
                { "surfaces=", v => { _settings.Surfaces = Settings.ConvertToRectList(RemoveQuotationMarks(v)); } },
                { "yaw=", v => { _settings.Yaw = Convert.ToSingle(RemoveQuotationMarks(v)); } },
                { "verticalOffset=", v => { _settings.VerticalOffset = Convert.ToSingle(RemoveQuotationMarks(v)); } },
                { "verticalScale=", v => { _settings.VerticalScale = Convert.ToSingle(RemoveQuotationMarks(v)); } },
                { "virtualRoom=", v => { _settings.VirtualRoom = Convert.ToBoolean(RemoveQuotationMarks(v)); } }
            };

            List<string> extra = p.Parse(_params);
       }


        //Gets an array of command line arguments.
        //Cannot use the default System.Environment.GetCommandLineArgs()
        //because it splits by white space and that can split URLs
        private static String[] GetParams()
        {
            return SplitParams(System.Environment.CommandLine);
        }

        public static String[] SplitParams(string paramsStr)
        {

            Regex regex = new Regex(@"-[A-Za-z]+=((""[^""]+"")|(\S+))");
            MatchCollection matches = regex.Matches(paramsStr);
            string[] args = new string[matches.Count];
            var index = 0;
            foreach (Match match in matches)
            {
                args[index] = match.Value;
                index++;
            }
            return args;
        }

        private static String[] GetParamsWebGl()
        {
            int pos = Application.absoluteURL.IndexOf("?");
            if (pos != -1)
            {
                string unsplitArgs = Application.absoluteURL.Split("?"[0])[1];
                string[] args = unsplitArgs.Split('&');
                for (var i = 1; i < args.Length; i++) { args[i] = "-" + args[i]; }
                return args;
            }
            return new string[0];
        }

        public static String RemoveQuotationMarks(String initialString)
        {
            var newString = initialString.Replace("\"", string.Empty).Trim();
            return newString;
        }

    }
}