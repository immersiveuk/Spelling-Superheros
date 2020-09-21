/* Copyright (C) Immersive Interactive, Ltd - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by Luke Bissell <luke@immersive.co.uk>, Dec 2019
 */

using Com.Immersive.Hotspots;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// This script will change the sprite on the provided SpriteRender when all Q&A popups, with the same ID, are answered correctly.
/// </summary>
public class ChangeSpriteOnAllQAndAsAnsweredCorrectly : MonoBehaviour, IQuestionAnsweredHandler, IHotspotActionCompleteHandler
{
    [Tooltip("All components of this type with the same ID will be treated as a single system.")]
    public int id = 0;

    public SpriteRenderer targetRenderer;
    public Sprite newSprite;

    public static Dictionary<int, int> numberOfQandAsInSystem;
    private static bool _countCalculated = false;

    private bool previouslyAnsweredCorrectly = false;

    // Start is called before the first frame update
    void Start()
    {
        if (!_countCalculated)
        {
            _countCalculated = true;
            var objects = Resources.FindObjectsOfTypeAll<ChangeSpriteOnAllQAndAsAnsweredCorrectly>();
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
            targetRenderer.sprite = newSprite;
        }
    }

    private void ResetOnSceneChange(Scene arg0, LoadSceneMode arg1)
    {
        _countCalculated = false;
    }

}
