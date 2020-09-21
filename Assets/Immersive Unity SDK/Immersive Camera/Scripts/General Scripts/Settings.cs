/* Copyright (C) Immersive Interactive, Ltd - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by Luke Bissell <luke@immersive.co.uk>, July 2019
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;
namespace Com.Immersive.Cameras
{
    public enum ScaleMode
    {
        NoScale,
        ScaleAndCrop,
        Fill,
        Zoom
    }

    public class Settings
    {
        private string filePath = "";
        public string FilePath
        {
            get { return filePath; }
            set
            {
                filePath = value;
            }
        }
        private Rect position;
        public Rect Position
        {
            get { return position; }
            set { position = value; }
        }
        private List<Rect> masks;
        public List<Rect> Masks
        {
            get { return masks; }
            set { masks = value; }
        }
        private bool stretchToFit = true;
        public bool StretchToFit
        {
            get { return stretchToFit; }
            set { stretchToFit = value; }
        }
        private string server = "127.0.0.1";
        public string Server
        {
            get { return server; }
            set { server = value; }
        }
        private int port = 3030;
        public int Port
        {
            get { return port; }
            set { port = value; }
        }
        private bool debug = false;
        public bool Debug
        {
            get { return debug; }
            set { debug = value; }
        }
        private bool screenshot = false;
        public bool Screenshot
        {
            get { return screenshot; }
            set { screenshot = value; }
        }
        private string outDir = "";
        public string OutDir
        {
            get { return outDir; }
            set { outDir = value; }
        }
        private string url = "";
        public string Url
        {
            get { return url; }
            set { url = value; }
        }
        public bool DVD
        {
            get;
            set;
        }
        private string dvdDrive = "D";
        public string DvdDrive
        {
            get { return dvdDrive; }
            set { dvdDrive = value; }
        }
        private string miLightIP = "";
        public string MiLightIP
        {
            get { return miLightIP; }
            set { miLightIP = value; }
        }
        private int miLightPort = 8899;
        public int MiLightPort
        {
            get { return miLightPort; }
            set { miLightPort = value; }
        }
        private int appPort = 8080;
        public int AppPort
        {
            get { return appPort; }
            set { appPort = value; }
        }
        private int responseAppPort = 8081;
        public int ResponseAppPort
        {
            get { return responseAppPort; }
            set { responseAppPort = value; }
        }

        public List<Rect> Surfaces { get; set; }

        public static Rect ConvertToRect(string v)
        {
            var parts = v.Split(',');

            return new Rect(Convert.ToInt32(parts[0]), Convert.ToInt32(parts[1]), Convert.ToInt32(parts[2]), Convert.ToInt32(parts[3]));
        }
        public static List<Rect> ConvertToRectList(string v)
        {
            var ret = new List<Rect>();
            if (String.IsNullOrEmpty(v))
            {
                return ret;
            }
            var rects = v.Split(new[] { '[', ']' }, StringSplitOptions.RemoveEmptyEntries);

            //var rects = v.Split('|');
            foreach (var r in rects)
            {
                ret.Add(ConvertToRect(r));
            }
            return ret;
        }
        public bool SceneFilePaths { get; set; }
        public int Volume { get; set; }
        public bool AutoMatchColour { get; set; }
        public int DmxStartChannel { get; set; }
        public int startTime = -1;
        public int StartTime
        {
            get
            {
                return startTime;
            }
            set
            {
                startTime = value;
            }
        }
        public int stopTime = -1;
        public int StopTime
        {
            get
            {
                return stopTime;
            }
            set
            {
                stopTime = value;
            }
        }
        public int remotingPort = 9960;
        public int RemotingPort
        {
            get
            {
                return remotingPort;
            }
            set
            {
                remotingPort = value;
            }
        }
        public string language = "";
        public string Language
        {
            get
            {
                return language;
            }
            set
            {
                language = value;
            }
        }
        public int DmxDevice { get; set; }
        private string label = "";
        public string Label
        {
            get { return label; }
            set
            {
                label = value;
                if (label.ToLower() == "center")
                {
                    label = "Centre";
                }
            }
        }
        private bool youTube = false;
        public bool YouTube
        {
            get
            {
                return youTube;
            }
            set
            {
                youTube = value;
            }
        }
        public int SceneID { get; set; }
        public int SceneListID { get; set; }

        public int SceneItemID { get; set; }
        private bool loop = true;
        public bool Loop
        {
            get { return loop; }
            set { loop = value; }
        }
        private string appURL = "LOCALHOST";
        public string AppURL
        {
            get { return appURL; }
            set { appURL = value; }
        }
        private ScaleMode scaleMode = ScaleMode.Fill;
        public ScaleMode ScaleMode
        {
            get { return scaleMode; }
            set { scaleMode = value; }
        }

        private string creditsText;
        public string CreditsText
        {
            get { return creditsText; }
            set { creditsText = value; }
        }
        private SurfacePosition layout = SurfacePosition.Left | SurfacePosition.Center | SurfacePosition.Right | SurfacePosition.Floor;
        public SurfacePosition Layout
        {
            get
            {
                return layout;
            }
            set
            {
                layout = value;
            }
        }


        private float _yaw = 0;
        public float Yaw
        {
            get { return _yaw; }
            set { _yaw = value; }
        }

        private float _verticalOffset = 0;
        public float VerticalOffset
        {
            get { return _verticalOffset; }
            set { _verticalOffset = value; }
        }

        private float _verticalScale = 1;
        public float VerticalScale
        {
            get { return _verticalScale; }
            set { _verticalScale = value; }
        }

        public enum VideoFormat360 { Equirectangular, ImmersiveCube };
        public VideoFormat360 VideoFormat { set; get; } = VideoFormat360.Equirectangular;
        public void SetVideoFormat(string formatStr)
        {
            switch (formatStr)
            {
                case "Equirectangular":
                    VideoFormat = VideoFormat360.Equirectangular;
                    break;
                case "ImmersiveCube":
                    VideoFormat = VideoFormat360.ImmersiveCube;
                    break;
                default:
                    break;
            }
        }

        public bool VirtualRoom { get; set; } = false;
        public bool PanningPreview { get; set; } = false;

    }
}