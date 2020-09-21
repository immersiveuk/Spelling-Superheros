/* Copyright (C) Immersive Interactive, Ltd - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by Luke Bissell <luke@immersive.co.uk>, Nov 2019
 */

using Com.Immersive.Hotspots;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Script which will activate and disable GameObjects when all hotspots with Q&A Popups and this script applied are answered correctly.
/// </summary>
public class ActivateAndDisableObjectOnAllQandAPopupsAnsweredCorrectly : AbstractActivateAndDisable, IQuestionAnsweredHandler, IHotspotActionCompleteHandler
{
    [Tooltip("All components of this type with the same ID will be treated as a single system.")]
    public int id = 0;


    public static Dictionary<int, int> numberOfQandAsInSystem;
    private static bool _countCalculated = false;

    private bool previouslyAnsweredCorrectly = false;

    // Start is called before the first frame update
    void Start()
    {
        if (!_countCalculated)
        {
            _countCalculated = true;
            var objects = Resources.FindObjectsOfTypeAll<ActivateAndDisableObjectOnAllQandAPopupsAnsweredCorrectly>();
            numberOfQandAsInSystem = new Dictionary<int, int>();

            foreach (var obj in objects)
            {
                if (numberOfQandAsInSystem.ContainsKey(obj.id)) numberOfQandAsInSystem[obj.id]++;
                else numberOfQandAsInSystem.Add(obj.id, 1);
            }

            //Listen for scene change.
            SceneManager.sceneLoaded += ResetOnSceneChange;
        }
    }

    public void QuestionAnswered(bool isAnswerCorrect)
    {
        if (isAnswerCorrect && !previouslyAnsweredCorrectly)
        {
            previouslyAnsweredCorrectly = true;
            numberOfQandAsInSystem[id]--;
        }
    }

    public void HotspotActionComplete()
    {
        if (numberOfQandAsInSystem[id] == 0)
        {
            ActivateAndDisable();
        }
    }
  
    private void ResetOnSceneChange(Scene arg0, LoadSceneMode arg1)
    {
        _countCalculated = false;
    }

    private void Update()
    {
        //Shortcut to skip Q&As
        //Press Ctrl + E
        if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl))
        {
            if (Input.GetKeyUp(KeyCode.E))
            {
                numberOfQandAsInSystem[id] = 0;
                foreach (var obj in objectsToActivate)
                {
                    obj.SetActive(true);
                }

                foreach (var obj in objectsToDisable)
                {
                    obj.SetActive(false);
                }
            }
        }
    }
}
