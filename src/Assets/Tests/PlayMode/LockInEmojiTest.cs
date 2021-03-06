﻿using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

namespace Tests.PlayMode
{
    /**
     * Test class for the integration of the turn in functionality
     * @author Robert Meier
     * @date 2020.12.08
     */
    public class LockInEmojiTest
    {
        private InteractionTestHelper helper;

        /**
         * Setup test environment
         */
        [SetUp]
        public void Setup()
        {
            SceneManager.LoadScene("Scenes/Scene_Playground_2vs2");
        }

        /**
         * Test for Player 1
         */
        [UnityTest]
        public IEnumerator LockInTestPlayer1()
        {
            yield return TestPlayer(1);
        }

        /**
         * Test for Player 2
         */
        [UnityTest]
        public IEnumerator LockInTestPlayer2()
        {
            yield return TestPlayer(2);
        }

        /**
         * Test for Player 3
         */
        [UnityTest]
        public IEnumerator LockInTestPlayer3()
        {
            yield return TestPlayer(3);
        }

        /**
         * Test for Player 4
         */
        [UnityTest]
        public IEnumerator LockInTestPlayer4()
        {
            yield return TestPlayer(4);
        }

        /**
         * Run integration Test for Player
         * 
         * @param player Player number
         */
        private IEnumerator TestPlayer(int player)
        {
            Object.FindObjectOfType<StartCountdownTime>().OnCountdownEnd();
            yield return new WaitForEndOfFrame();
            helper = new InteractionTestHelper(player);
            yield return helper.GetStone();
            helper.TestConditionsBeforeToWorkshop();
            yield return helper.MoveToWorkshopSlot();
            yield return helper.MoveToEmoji();
            yield return helper.TurnInEmoji();
        }
    }
}