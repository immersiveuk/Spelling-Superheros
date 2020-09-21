/* Copyright (C) Immersive Interactive, Ltd - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by Luke Bissell <luke@immersive.co.uk>, July 2019
 */

using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Threading;
using UnityEngine;

namespace Com.Immersive.Cameras
{
    /// <summary>
    /// Processes a touch blob.
    /// Small touches will be treated as a single touch point.
    /// Large touches will be treated as a geometry which can interact with particles.
    /// </summary>
    public class ProcessTouches : MonoBehaviour
    {
        //==============================================================
        // PUBLIC VARIABLES
        //==============================================================

        public bool createGeometry = true;

        [HideInInspector]
        public Camera cam;
        public AbstractImmersiveCamera immersiveCamera;

        [HideInInspector]
        public string blobsDataStr = "";
        private int blobsHash = 0;
        private int _blobsHash = 0;

        [HideInInspector]
        public int port = 3030;

        [HideInInspector]
        public int surfaceIndex;

        public bool isFloor;

        // Blobs below a certain size are treated as a touch.
        private List<BlobInfo> smallBlobs = new List<BlobInfo>();
        // Blobs above a certain size are treated as a geometry to interact with particles.
        private List<BlobInfo> largeBlobs = new List<BlobInfo>();

        // All the small blobs from the previous frame.
        private List<BlobInfo> previousFrameSmallBlobs = new List<BlobInfo>();
        // Maximum scale of touch.
        //private readonly float maxBlobPixelSize = 200;

        // Start is called before the first frame update
        void Start()
        {
            ConnectToTcpServer();
        }

        private void OnDestroy()
        {
            StopListeningToTouchServer();
        }

        private bool InteractionOn => (!isFloor && immersiveCamera.interactionOn) ||                            //Not Floor
            (isFloor && !immersiveCamera.customFloorInteractionSettings && immersiveCamera.interactionOn) ||    //Not using custom floor settings
            (isFloor && immersiveCamera.customFloorInteractionSettings && immersiveCamera.floorInteractionOn);  //Custom Floor Settings and Floor Interaction On.

        private bool PointTouchesOn => (!isFloor && immersiveCamera.pointTouchesOn) ||                           //Not Floor
            (isFloor && !immersiveCamera.customFloorInteractionSettings && immersiveCamera.pointTouchesOn) ||    //Not using custom floor settings
            (isFloor && immersiveCamera.customFloorInteractionSettings && immersiveCamera.floorPointTouchesOn);  //Custom Floor Settings and Floor Interaction On.

        private bool AreaTouchesOn => (!isFloor && immersiveCamera.areaTouchesOn) ||                            //Not Floor
            (isFloor && !immersiveCamera.customFloorInteractionSettings && immersiveCamera.areaTouchesOn) ||    //Not using custom floor settings
            (isFloor && immersiveCamera.customFloorInteractionSettings && immersiveCamera.floorAreaTouchesOn);  //Custom Floor Settings and Floor Interaction On.

        private bool DisplayTouchPoints => (!isFloor && immersiveCamera.displayTouchPoints) ||                            //Not Floor
            (isFloor && !immersiveCamera.customFloorInteractionSettings && immersiveCamera.displayTouchPoints) ||    //Not using custom floor settings
            (isFloor && immersiveCamera.customFloorInteractionSettings && immersiveCamera.floorDisplayTouchPoints);  //Custom Floor Settings and Floor Interaction On.

        private bool DisplayAreaTouches => (!isFloor && immersiveCamera.displayAreaTouches) ||                            //Not Floor
            (isFloor && !immersiveCamera.customFloorInteractionSettings && immersiveCamera.displayAreaTouches) ||    //Not using custom floor settings
            (isFloor && immersiveCamera.customFloorInteractionSettings && immersiveCamera.floorDisplayAreaTouches);  //Custom Floor Settings and Floor Interaction On.


        void Update()
        {
            if (InteractionOn)
            {
                //Blob has changed
                if (blobsHash != _blobsHash)
                {
                    //Create new touch points
                    if (blobsDataStr != "-")
                    {
                        // Parse blob json in BlobInfo objects
                        BlobInfo[] blobs = ParseTouchesJson(blobsDataStr);

                        smallBlobs = new List<BlobInfo>();
                        largeBlobs = new List<BlobInfo>();

                        // Split blobs into large and small blobs.
                        if (PointTouchesOn && AreaTouchesOn)
                        {
                            SeperateLargeAndSmallBlobs(blobs);
                        }
                        else if (PointTouchesOn)
                        {
                            foreach (var blob in blobs)
                            {
                                smallBlobs.Add(blob);
                            }
                        }
                        else if (AreaTouchesOn)
                        {
                            foreach (var blob in blobs)
                            {
                                largeBlobs.Add(blob);
                            }
                        }

                        largeBlobs = ConvertBlobsToPixelCoords(largeBlobs);
                        smallBlobs = ConvertBlobsToPixelCoords(smallBlobs);

                        if (AreaTouchesOn) CreateBlobGeometry(largeBlobs);
                        if (PointTouchesOn) HandleTouches(smallBlobs);
                    }

                    // No touches in blobs. Removes existing touch information.
                    else
                    {
                        if (AreaTouchesOn)
                        {
                            for (int i = 0; i < transform.childCount; i++)
                            {
                                Destroy(transform.GetChild(i).gameObject);
                            }
                        }
                        if (PointTouchesOn) HandleReleasedTouches();
                        previousFrameSmallBlobs = new List<BlobInfo>();
                    }
                }
            }

        }


        /// <summary>
        /// Creates two lists of blobs. Those below a certain size are treated as
        /// individual touches and those above a certain size are treated as 
        /// geometry to interact with particles.
        /// </summary>
        private void SeperateLargeAndSmallBlobs(BlobInfo[] blobs)
        {
            foreach (var blob in blobs)
            {
                var box = blob.boundingBox;
                var blobWidth = box.width * AbstractImmersiveCamera.CurrentImmersiveCamera.surfaces[surfaceIndex].rect.width;
                var blobHeight = box.height * AbstractImmersiveCamera.CurrentImmersiveCamera.surfaces[surfaceIndex].rect.width;

                var distance = blobWidth * blobWidth + blobHeight * blobHeight;

                //Small Blob
                /*if (distance < maxBlobPixelSize * maxBlobPixelSize) */smallBlobs.Add(blob);

                //Large Blob
                /*else */largeBlobs.Add(blob);
            }
        }

        /// <summary>
        /// Converts blobs from a position on the surface between 0 and 1, to the pixels coordinates.
        /// </summary>
        private List<BlobInfo> ConvertBlobsToPixelCoords(List<BlobInfo> originalBlobs)
        {
            var newBlobs = new List<BlobInfo>();
            foreach (var oldBlob in originalBlobs)
            {
                var newBlob = new BlobInfo();
                newBlob.id = oldBlob.id;
                newBlob.centroid = ConvertPointToPixelCoord(oldBlob.centroid);
                newBlob.boundingBox = ConvertRectToPixelCoord(oldBlob.boundingBox);

                var newPoints = new Vector2[oldBlob.points.Length];
                for (int i = 0; i < oldBlob.points.Length; i++)
                {
                    newPoints[i] = ConvertPointToPixelCoord(oldBlob.points[i]);
                }
                newBlob.points = newPoints;
                newBlobs.Add(newBlob);
            }
            return newBlobs;
        }

        /// <summary>
        /// Converts a single point from a blob into pixel coordinates. 
        /// Should ONLY be used by ConvertBlobsToPixelCoords.
        /// </summary>
        private Vector2 ConvertPointToPixelCoord(Vector2 originalPoint)
        {
            var xOffset = cam.rect.x * (cam.pixelWidth / cam.rect.width);                       //This will be 0 unless in a spanning mode
            var yOffset = ((1 - cam.rect.height) * cam.pixelHeight) / (2 * cam.rect.height);    //This will be 0 unless the image is letterboxed.

            return new Vector2(cam.pixelWidth * originalPoint.x + xOffset, cam.pixelHeight * (1 - originalPoint.y) + yOffset);
        }

        /// <summary>
        /// Converts a rect from a blob into pixel coordingates.
        /// Should ONLY be used by ConvertBlobsToPixelCoords.
        /// </summary>
        private Rect ConvertRectToPixelCoord(Rect originalRect)
        {
            var newRect = new Rect();
            newRect.min = ConvertPointToPixelCoord(originalRect.min);
            newRect.max = ConvertPointToPixelCoord(originalRect.max);
            return newRect;
        }


        //==============================================================
        // HANDLE TOUCHES
        // Blobs below a certain size are treated as a discrete touch.
        //==============================================================

        /// <summary>
        /// Processes touches from small blobs.
        /// These are handled treated as discrete touches
        /// </summary>
        private void HandleTouches(List<BlobInfo> blobs)
        {
            foreach (BlobInfo blob in blobs)
            {
                ///TODO: Need to not call Display.RelativeToMousePosition in Immersive camera to work on side walls.
                // Calculate center of touch in correct coordinate space.
                var blobCenter = new Vector3(blob.centroid.x, blob.centroid.y, surfaceIndex);

                // Check if touch already exists.
                // List previousFrameSmallBlobs will contain all released touches at the end of the .
                // The total list of blobs will be relatively small so its ok to iterate through them all.
                var isNewTouch = true;
                foreach (var prevBlob in previousFrameSmallBlobs)
                {
                    if (prevBlob.id == blob.id)
                    {
                        previousFrameSmallBlobs.Remove(prevBlob);
                        HandleTouch(blobCenter, cam, TouchPhase.Moved, blob.id);

                        isNewTouch = false;
                        break;
                    }
                }
                if (isNewTouch)
                {
                    HandleTouch(blobCenter, cam, TouchPhase.Began, blob.id);
                }
            }
            // Sends touch ended signal to last position of released touch.
            HandleReleasedTouches();

            //Reset previous frame blobs list.
            previousFrameSmallBlobs = blobs;
        }


        private void HandleTouch(Vector3 blobCenter, Camera cam, TouchPhase phase, int blobId)
        {
            var hitUIObject = immersiveCamera.RaycastOnClickEventsToUI(cam, blobCenter, phase);
            if (!hitUIObject) immersiveCamera.HandleTouchOrClick(blobCenter, phase, blobId);
        }

        /// <summary>
        /// Processes touches which have just been released.
        /// </summary>
        private void HandleReleasedTouches()
        {
            foreach (var blob in previousFrameSmallBlobs)
            {
                var blobCenter = new Vector3(blob.centroid.x, blob.centroid.y, surfaceIndex);

                HandleTouch(blobCenter, cam, TouchPhase.Ended, blob.id);
            }
        }

        //==============================================================
        // Handle Geometry Interactions
        // Blobs above a certain size create geometry which can interact with particles.
        //==============================================================

        // Simply a list used to store all child objects.
        private List<GameObject> childList = new List<GameObject>();

        private void CreateBlobGeometry(List<BlobInfo> blobs)
        {
            // 1. Updates list with all child objects.
            childList = new List<GameObject>();
            for (var i = 0; i < transform.childCount; i++)
            {
                childList.Add(transform.GetChild(i).gameObject);
            }

            // 2. Create Geometry for each blob.
            foreach (BlobInfo blob in blobs)
            {

                var name = "Touch " + blob.id;

                GameObject child = null;
                LineRenderer lineRend = null;
                MeshFilter meshFilter = null;
                MeshCollider meshCollider = null;
                // 2.1 If touch already exists update existing object
                foreach (var childObj in childList)
                {
                    if (childObj.name == name)
                    {
                        child = childObj;
                        if (DisplayAreaTouches) lineRend = child.GetComponent<LineRenderer>();
                        meshFilter = child.GetComponent<MeshFilter>();
                        meshCollider = child.GetComponent<MeshCollider>();

                        childList.Remove(child);
                        break;
                    }
                }
                // 2.2 If touch is new. New touch is created.
                if (child == null)
                {
                    child = new GameObject();
                    child.transform.parent = transform;
                    child.name = name;

                    if (DisplayAreaTouches)
                    {
                        lineRend = child.AddComponent(typeof(LineRenderer)) as LineRenderer;
                        lineRend.startWidth = 0.005f;
                        lineRend.endWidth = 0.005f;
                    }

                    meshFilter = child.AddComponent(typeof(MeshFilter)) as MeshFilter;
                    meshCollider = child.AddComponent(typeof(MeshCollider)) as MeshCollider;

                    if (Application.isEditor)
                    {
                        child.AddComponent(typeof(MeshRenderer));
                    }
                }

                var points = blob.points;
                var mesh = meshFilter.mesh;


                // 2.3. Calculate the points of the blob on the camera near field and far field.
                //      The first hald of combinePositions is the near field points and the second half
                //      is the far field points. This is used to create geometry.
                Vector3[] nearPositions = new Vector3[points.Length];
                Vector3[] combinedPositions = new Vector3[points.Length * 2];

                for (int i = 0; i < points.Length; i++)
                {
                    var nearPos = cam.ScreenToWorldPoint(new Vector3(points[i].x, points[i].y, cam.nearClipPlane));
                    var farPos = cam.ScreenToWorldPoint(new Vector3(points[i].x, points[i].y, cam.farClipPlane));

                    nearPositions[i] = nearPos;

                    combinedPositions[i] = nearPos;
                    combinedPositions[i + points.Length] = farPos;
                }

                //The line renderer shows the blob to the user.
                if (DisplayAreaTouches)
                {
                    lineRend.positionCount = nearPositions.Length;
                    lineRend.SetPositions(nearPositions);
                }

                // 2.4. Create the triangles
                int[] newTris = new int[points.Length * 2 * 3];
                for (int i = 0; i < points.Length - 1; i++)
                {
                    var a = i * 3;
                    var b = i * 3 + points.Length;
                    var c = i * 3 + 1;

                    var a2 = i * 3 + 1;
                    var b2 = i * 3 + points.Length;
                    var c2 = i * 3 + 1 + points.Length;

                    newTris[i * 6] = i + 1;
                    newTris[i * 6 + 1] = i + points.Length;
                    newTris[i * 6 + 2] = i;

                    newTris[i * 6 + 3] = i + 1 + points.Length;
                    newTris[i * 6 + 4] = i + points.Length;
                    newTris[i * 6 + 5] = i + 1;
                }

                //Final triangle
                var j = points.Length - 1;
                newTris[j * 6] = 0;
                newTris[j * 6 + 1] = j + points.Length;
                newTris[j * 6 + 2] = j;

                newTris[j * 6 + 3] = points.Length;
                newTris[j * 6 + 4] = j + points.Length;
                newTris[j * 6 + 5] = 0;


                // 2.5. Pass the triangles and vertices to the mesh
                mesh.triangles = null;
                mesh.vertices = combinedPositions;
                mesh.triangles = newTris;

                meshCollider.sharedMesh = mesh;
            }

            // 3. Destroy child objects which are no longer valid. 
            foreach (var child in childList)
            {
                Destroy(child);
            }
        }


        //==============================================================
        // Process Touch Blob Strings
        //==============================================================

        /// <summary>
        /// Splits all the touch data, in the form of a string, into its component blobs, in the form of a BlobInfo struct.
        /// </summary>
        /// <param name="">A String containing the touch information for a single frame.</param>
        /// <returns>Touch information split into an array of BlobInfo structs.</returns>
        private BlobInfo[] ParseTouchesJson(string touchesStr)
        {
            //Split string into individual touches
            string[] splitTouches = touchesStr.Split(new[] { '{', '}' }, System.StringSplitOptions.RemoveEmptyEntries);

            BlobInfo[] blobs = new BlobInfo[splitTouches.Length];
            for (int i = 0; i < splitTouches.Length; i++)
            {
                BlobInfo blobInfo = SplitIndividualBlob(splitTouches[i]);
                blobs[i] = blobInfo;
            }
            return blobs;
        }



        /// <summary>
        /// Splits a string containing blob information from a single touch into a BlobInfo Struct.
        /// </summary>
        /// <param name="blobStr">String representing a single touch blob.</param>
        /// <returns>The touch info converted into a BlobInfo struct</returns>
        private BlobInfo SplitIndividualBlob(string blobStr)
        {
            var segments = blobStr.Split('|');
            if (segments.Length != 4) throw new MalformedTouchInfoException();

            int id = 0;
            if (!int.TryParse(segments[0], out id)) throw new MalformedTouchInfoException();
            var centroid = SplitCentroid(segments[1]);
            var boundingBox = SplitBoundingBox(segments[2]);
            var points = SplitPoints(segments[3]);

            return new BlobInfo(id, centroid, boundingBox, points);
        }

        /// <summary>
        /// Converts a string representing the centroid of a touch into a Vector2.
        /// </summary>
        private Vector2 SplitCentroid(string centroidStr)
        {
            var centroidStrSplit = centroidStr.Split(',');
            if (centroidStrSplit.Length != 2) throw new MalformedTouchInfoException();

            //Try and convert to Vector2
            if (float.TryParse(centroidStrSplit[0], out float x) && float.TryParse(centroidStrSplit[1], out float y))
            {
                return new Vector2(x, y);
            }
            else
            {
                throw new MalformedTouchInfoException();
            }
        }

        /// <summary>
        /// Converts a string representing the bounding box of touch blob into a Rect.
        /// </summary>
        private Rect SplitBoundingBox(string boundingBoxStr)
        {
            var boundingBoxStrSplit = boundingBoxStr.Split(',');

            if (boundingBoxStrSplit.Length != 4) throw new MalformedTouchInfoException();

            if (float.TryParse(boundingBoxStrSplit[0], out float x) && float.TryParse(boundingBoxStrSplit[1], out float y) &&
                float.TryParse(boundingBoxStrSplit[2], out float w) && float.TryParse(boundingBoxStrSplit[3], out float h))
            {
                return new Rect(x, y, w, h);
            }
            else throw new MalformedTouchInfoException();
        }

        /// <summary>
        /// Converts a string representing a list of points from a touch blob into an array of Vector2.
        /// </summary>
        private Vector2[] SplitPoints(string pointsStr)
        {
            var pointsSplit = pointsStr.Split(',');
            if (pointsSplit.Length % 2 == 1) throw new MalformedTouchInfoException();

            Vector2[] points = new Vector2[pointsSplit.Length / 2];

            for (int i = 0; i < pointsSplit.Length; i += 2)
            {
                if (float.TryParse(pointsSplit[i], out float x) && float.TryParse(pointsSplit[i + 1], out float y))
                {
                    Vector2 point = new Vector2(x, y);
                    points[i / 2] = point;
                }
                else throw new MalformedTouchInfoException();
            }
            return points;
        }


        //==============================================================
        // TCP Client
        //==============================================================

        private readonly string IPAddress = "localhost";
        private bool clientRunning = true;

        /// <summary> 	
        /// Setup socket connection. 	
        /// </summary> 	
        public void ConnectToTcpServer()
        {
            try
            {
                Thread clientReceiveThread = new Thread(new ThreadStart(ListenForData));
                clientReceiveThread.IsBackground = true;
                clientReceiveThread.Start();
            }
            catch (Exception e)
            {
                print("On client connect exception " + e);
            }
        }

        /// <summary>
        /// Stop the TCP client listening.
        /// </summary>
        private void StopListeningToTouchServer()
        {
            clientRunning = false;
        }

        /// <summary> 	
        /// Runs in background clientReceiveThread; Listens for incoming data. 	
        /// </summary>     
        private void ListenForData()
        {
            try
            {
                using (TcpClient socketConnection = new TcpClient(IPAddress, port))
                using (NetworkStream stream = socketConnection.GetStream())
                using (StreamReader rd = new StreamReader(stream))
                {
                    while (clientRunning && stream.CanRead)
                    {
                        blobsDataStr = rd.ReadLine();
                        blobsHash = blobsDataStr.GetHashCode();
                    }
                }
            }
            catch (SocketException socketException)
            {
                print("Could Not Connect to Touch Sensor on Port " + port);
                print(socketException);
            }
        }




        //==============================================================
        // DATA STRUCTURES
        //==============================================================

        /// <summary>
        /// Struct representing a single touch blob
        /// </summary>
        private struct BlobInfo
        {
            public int id;
            public Vector2 centroid;
            public Rect boundingBox;
            public Vector2[] points;

            public BlobInfo(int id, Vector2 centroid, Rect boundingBox, Vector2[] points)
            {
                this.id = id;
                this.centroid = centroid;
                this.boundingBox = boundingBox;
                this.points = points;
            }
        }

        public class MalformedTouchInfoException : Exception { }
    }

}

