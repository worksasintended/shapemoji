﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Harpoon;
using NUnit.Framework;
using Spawner;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using Assert = NUnit.Framework.Assert;

namespace Tests.PlayMode
{
    /**
     * Test class for wind in and inventory functionality
     * @author Andrei Dziubenka
     * @date 2020.11.26
     */
    public class InventoryTest
    {
        /*
         * _harpoon, _projectile, _inventory, _field are GameObjects selected by class method LoadPlayer
         */
        private GameObject _harpoon, _projectile, _inventory, _field, _field2, _spawner, _wheel;

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
        public IEnumerator InventoryTestPlayer1()
        {
            yield return TestPlayer(1);
        }

        /**
         * Test for Player 2
         */
        [UnityTest]
        public IEnumerator InventoryTestPlayer2()
        {
            yield return TestPlayer(2);
        }

        /**
         * Test for Player 3
         */
        [UnityTest]
        public IEnumerator InventoryTestPlayer3()
        {
            yield return TestPlayer(3);
        }

        /**
         * Test for Player 4
         */
        [UnityTest]
        public IEnumerator InventoryTestPlayer4()
        {
            yield return TestPlayer(4);
        }

        /**
         * Run test for player
         * @param player Player number
         */
        private IEnumerator TestPlayer(int player)
        {
            LoadPlayer(player);
            Object.FindObjectOfType<StartCountdownTime>().OnCountdownEnd();
            yield return new WaitForSeconds(0.1f);
            GameObject stoneCollided = null;

            _projectile.GetComponent<ProjectileCollision>().CollisionEvent +=
                delegate(object sender, Collider2D collider2D)
                {
                    //if it is a stone
                    if (collider2D.gameObject.CompareTag("Stone"))
                    {
                        //if no stone collision happened in the past
                        // ReSharper disable once AccessToModifiedClosure
                        if (stoneCollided == null)
                        {
                            stoneCollided = collider2D.gameObject;
                        }
                    }
                };

            for (int i = 0; i < _inventory.GetComponent<Inventory>().slots.Length + 1; i++)
            {
                GameObject stoneToAim = FindStone();
                Assert.IsNotNull(stoneToAim, $"Player {player}: not enough stones on the playing field");

                // stoneCollided can be changed by Collision Event Handler
                // stoneCollided is used to determine which stone was catched by harpoon
                stoneCollided = null;

                Vector3 stonePos = stoneToAim.transform.position;
                yield return AimAtPoint(stonePos.x, stonePos.y);
                WindIn();
                yield return new WaitForSeconds(0.1f);
                Assert.IsNotNull(stoneCollided, $"Player {player}: could not find collided stone");
                var pos1 = stoneCollided.transform.position;
                yield return new WaitForSeconds(0.1f);
                // get new stone position, pos2
                // ReSharper disable once Unity.InefficientPropertyAccess
                var pos2 = stoneCollided.transform.position;

                yield return new WaitForSeconds(5.0f);

                // check if stone is dragged away
                Assert.AreNotEqual(pos1, pos2, $"Player {player}: Stone isn't dragged away during wind in");

                // last stone should be deleted, because inventory is full
                if (i == 4)
                {
                    UnityEngine.Assertions.Assert.IsNull(stoneCollided,
                        $"Player {player}: Stone isn't deleted, despite full inventory");
                }
                // first 4 stones are expected to get into inventory
                else
                {
                    Stone s = (Stone) stoneCollided.GetComponent<HookableObject>();
                    var position = stoneCollided.transform.position;
                    Assert.AreEqual(position.x, _inventory.GetComponent<Inventory>().GetPositionOfStoneChild(s).x, 0.1f,
                        $"Player {player}: Stone wasn't placed into inventory (x-Axis)");
                    Assert.AreEqual(position.y, _inventory.GetComponent<Inventory>().GetPositionOfStoneChild(s).y, 0.1f,
                        $"Player {player}: Stone wasn't placed into inventory (y-Axis)");
                }
            }
        }

        /**
         * Wind in harpoon method
         */
        private void WindIn()
        {
            for (var i = 0; i < 400; i++)
            {
                _wheel.GetComponent<CrankController>().RotateCrank(90);
            }
        }

        /**
         * Find stone for the current player to aim at
         * @return Stone from a spawn place or null, if no stones found
         */
        private GameObject FindStone()
        {
            List<GameObject> spawnZones = new List<GameObject>();
            List<GameObject> spawnPlaces = new List<GameObject>();

            spawnZones.Add(_field);
            spawnZones.Add(_field2);

            foreach (var child in spawnZones.SelectMany(zone => zone.transform.Cast<Transform>()))
            {
                spawnPlaces.Add(child.gameObject);
            }

            var stones = spawnPlaces.Where(ContainsStone).ToList();

            if (stones.Any())
            {
                var stoneToAim = stones.First().GetComponent<SpawnPlace>().hookableObject.gameObject;
                return stoneToAim;
            }

            // return null, if no stones are found
            return null;
        }

        /**
         * Rotates Harpoon to aim at point
         * @param harpoon Harpoon Object to aim
         * @param pointX x parameter of point to aim in Screen Coordinates
         * @param pointY y parameter of point to aim in Screen Coordinates
         */
        private IEnumerator AimAtPoint(float pointX, float pointY)
        {
            Vector3 pointWorldCoord = new Vector3(pointX, pointY, 0);
            Vector3 harpoonPosition = _harpoon.transform.position;
            Vector3 path = pointWorldCoord - harpoonPosition;
            float angle = Vector3.SignedAngle(Vector3.up, path, Vector3.forward);
            _harpoon.GetComponent<HarpoonController>().RotateHarpoon(angle);
            _harpoon.GetComponent<HarpoonController>().ShootProjectile();
            yield return new WaitForSeconds(10.0f);
        }

        /**
         * Source: StoneSpawner.cs
         * Author: Robert Meier
         * determines, if the chosen GameObject already contains a stone
         *
         * @param place chosen spawn place for a stone
         * @returns true, if the gameObject contains a SpawnPlace and it contains a stone
         */
        private bool ContainsStone(GameObject place)
        {
            var spawn = place.GetComponent<SpawnPlace>();
            return spawn != null && spawn.hookableObject != null;
        }

        /**
         * Method to load player Game Objects
         * @param player Player number
         */
        private void LoadPlayer(int player)
        {
            var team = (player + 1) / 2;
            _harpoon = GameObject.Find("Team_" + team + "/Player_" + player + "/Base/HarpoonBase/Harpoon");
            _projectile =
                GameObject.Find("Team_" + team + "/Player_" + player +
                                "/Base/HarpoonBase/Harpoon/HarpoonCannon/HarpoonProjectile");
            _inventory =
                GameObject.Find("Team_" + team + "/Player_" + player +
                                "/Base/Inventory");

            _wheel =
                GameObject.Find("Team_" + team + "/Player_" + player +
                                "/Base/Wheel");

            switch (player)
            {
                case 1:
                    _field = GameObject.Find("SpawnArea_SouthWest");
                    _field2 = GameObject.Find("SpawnArea_SouthEast");
                    break;
                case 2:
                    _field = GameObject.Find("SpawnArea_NorthWest");
                    _field2 = GameObject.Find("SpawnArea_NorthEast");
                    break;
                case 3:
                    _field = GameObject.Find("SpawnArea_SouthEast");
                    _field2 = GameObject.Find("SpawnArea_SouthWest");
                    break;
                case 4:
                    _field = GameObject.Find("SpawnArea_NorthEast");
                    _field2 = GameObject.Find("SpawnArea_NorthWest");
                    break;
            }
        }
    }
}