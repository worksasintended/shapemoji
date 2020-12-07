﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace ScoreArea
{
    public class ScoreArea : CanHoldHookableObject
    {
        private List<Stone> _stones;
        private BoxCollider2D _collider;
        private int _teamScore;

        // UI
        private Text teamScoreText;
        private Text emojiScoreText;
        private Toggle _button1;
        private Toggle _button2;
        [SerializeField] private GameObject emojiScoreUI;
        [SerializeField] private GameObject teamScoreUI;
        [SerializeField] private GameObject button1;
        [SerializeField] private GameObject button2;


        // Start is called before the first frame update
        private void Start()
        {
            _stones = new List<Stone>();
            _collider = gameObject.GetComponent<BoxCollider2D>();
            _player = gameObject.GetComponentInParent<Player>();
            _team = gameObject.GetComponentInParent<Team>();

            // UI
            teamScoreText = teamScoreUI.GetComponent<Text>();
            emojiScoreText = emojiScoreUI.GetComponent<Text>();
            emojiScoreUI.SetActive(false);

            _button1 = button1.GetComponent<Toggle>();
            // change the button color to red at the start
            var button1Colors = _button1.colors;
            button1Colors.normalColor = Color.red;
            button1Colors.highlightedColor = Color.red;
            _button1.colors = button1Colors;
            _button1.onValueChanged.AddListener(delegate { TurnInEmoji(); });
            _button1.onValueChanged.AddListener(delegate { ChangeColorOfButton(_button1); });

            _button2 = button2.GetComponent<Toggle>();
            // change the button color to red at the start
            var button2Colors = _button2.colors;
            button2Colors.normalColor = Color.red;
            button2Colors.highlightedColor = Color.red;
            _button2.colors = button2Colors;
            _button2.onValueChanged.AddListener(delegate { TurnInEmoji(); });
            _button2.onValueChanged.AddListener(delegate { ChangeColorOfButton(_button2); });
        }

        /**
         * implementation of method required by parent class, calls StoneToScoreArea at HookableObjectController
         * 
         * @param stone Stone to add
         * 
         * TODO: do not call Method from HookableGameObject, this has historic reasons and is not necessary with refactored code structure
         */
        public override bool StoneToCanHoldHookableObject(Stone stone)
        {
            HookableObjectController.StoneToScoreArea(stone, this);

            return true;
        }

        /**
     * true if stone is part of _stones
     *
     * @param stone Stone to check
     */
        public override bool IsStoneInCanHoldHookableObject(Stone stone)
        {
            return _stones.Contains(stone);
        }

        /**
     * true if stone is part of _stones
     *
     * @param stone Stone to check
     *
     * TODO: IsStoneInCanHoldHookableObject shoud be used instead
     */
        public bool ContainsStone(Stone stone)
        {
            return _stones.Contains(stone);
        }

        /**
     * adds stone to _stones and disables draggable on old _stones
     *
     * @param stone Stone to add
     *
     * TODO: should not be needed but part of StoneToCanHoldHookableObject
     */
        public void AddStone(Stone stone)
        {
            _stones.Add(stone);
            //lock n-1th stone
            if (_stones.Count > 1) HookableObjectController.DisableStoneDraggable(_stones[_stones.Count - 2]);
        }

        /**
     * get snapbackposition of stone
     *
     * @param stone Stone to add
     *
     * TODO: maybe try to store information of where stone has been placed before on ScoreArea and return this for snapback when outside ScoreArea but ScoreArea is still parent
     */
        public override Vector3 GetPositionOfStoneChild(Stone stone)
        {
            var currentPosition = HookableObjectController.GetPositionOfHookableObject(stone);
            if (_stones.Contains(stone) && !_collider.bounds.Contains(currentPosition)
            ) // bound only works with rectengular colliders 
                return gameObject.transform.position;
            return currentPosition;
        }

        /**
     * remove stone from ScoreArea, make last stone in list _stones draggable again
     *
     * @param stone Stone to remove
     */
        public override void RemoveStone(Stone stone)
        {
            _stones.Remove(stone);
            // reenable draggable on last stone
            var stoneCount = _stones.Count;
            if (stoneCount > 0)
            {
                var newLastStone = _stones.Last();
                HookableObjectController.ReEnableStoneDraggable(newLastStone);
            }
        }

        /**
        * AddScore adds the calculated score to the team score
        *
        * @param score calculated Score
        */
        public void AddScore(int score)
        {
            _teamScore += score;
            StartCoroutine(DisplayScore(score));
        }

        /**
         * ChangeEmoji calls the EmojiSpriteManager to change the emoji
         */
        public void ChangeEmoji()
        {
            // EmojiSpriteManager.ChangeEmoji()
        }

        /**
         * DisplayScore updates the team score and displays the emoji score for one second
         *
         * @param score Score to be displayed
         */
        IEnumerator DisplayScore(int score)
        {
            teamScoreText.text = "" + _teamScore;

            emojiScoreUI.SetActive(true);
            emojiScoreText.text = "" + score;

            yield return new WaitForSeconds(1);

            emojiScoreUI.SetActive(false);
        }

        /**
         * TurnInEmoji calls the calculation in the ScoreCalculation script
         */
        private void TurnInEmoji()
        {
            if (_button1.isOn && _button2.isOn)
            {
                // ScoreCalculation.GetScore()
            }
        }

        /**
         * ChangeColorOfButton changes the color of the button on every click
         *
         * @param button Button to be changed
         */
        private void ChangeColorOfButton(Toggle button)
        {
            var buttonColors = button.colors;
            if (button.isOn)
            {
                buttonColors.normalColor = Color.green;
                buttonColors.selectedColor = Color.green;
                buttonColors.highlightedColor = Color.green;
            }
            else
            {
                buttonColors.normalColor = Color.red;
                buttonColors.selectedColor = Color.red;
                buttonColors.highlightedColor = Color.red;
            }

            button.colors = buttonColors;
        }
    }
}