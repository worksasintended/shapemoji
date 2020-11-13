using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

namespace Spawner
{
    /**
    * Class, which manages the available stones on the playground
    */
    public class StoneSpawner : MonoBehaviour
    {
        [FormerlySerializedAs("MaxStones")] public int maxStones;

        public StoneFactory factory;
        public List<GameObject> spawnZones;
        
        private readonly List<GameObject> _spawnPlaces;

        public StoneSpawner()
        {
            _spawnPlaces = new List<GameObject>();
        }

        private void Start()
        {
            StartGeneration();
            InvokeRepeating(nameof(CreateRandomStone), 1f, 1f);
        }

        /**
         * Generates predefined number of random stones and places them onto the playground 
         */
        public void StartGeneration()
        {
            Random.InitState((int) System.DateTime.Now.Ticks); //TODO should be moved to a different class
            
            foreach (var child in spawnZones.SelectMany(zone => zone.transform.Cast<Transform>()))
            {
                _spawnPlaces.Add(child.gameObject);
            }
            
            for (var i = 0; i < maxStones; i++)
            {
                CreateRandomStone();
            }
        }
        
        /**
        * deletes a stone at given position
         *
         * @param stone delete reference of given stone to free position
        */
        public void DeleteStone(GameObject stone)
        {
            var places = _spawnPlaces
                .Where(ContainsStone)
                .Select(x => x.GetComponent<SpawnPlace>())
                .Where(x => x.stone.Equals(stone)).ToList();
            places.ForEach(x => x.stone = null);
        }

        /**
         * checks, if Spawner is at maximum capacity
         *
         * @returns true, if spawner cannot spawn any new stones
         */
        public bool IsFull()
        {
            var places = _spawnPlaces.Where(ContainsStone).Select(x => 1).Sum();
            return places == maxStones;
        }
        
        
        /**
         * creates a stone at a random location
         */
        public void CreateRandomStone()
        {
            if (!IsFull())
            {
                var places = _spawnPlaces.Where(plc => !ContainsStone(plc)).ToList();
                if (places.Count > 0)
                {
                    var random = Random.Range(0, places.Count);
                    var place = places[random];
                    var spawn = place.GetComponent<SpawnPlace>();

                    var spawnPosition = place.transform.position;
                    var x = spawnPosition.x;
                    var y = spawnPosition.y;
            
                    var stone = factory.CreateStone(x, y);
                    spawn.stone = stone;    
                }
            
            }
        }
    
        /**
         * determines, if the chosen GameObject already contains a stone
         *
         * @param place chosen spawn place for a stone
         * @returns true, if the gameObject contains a SpawnPlace and it contains a stone
         */
        private static bool ContainsStone(GameObject place)
        {
            var spawn = place.GetComponent<SpawnPlace>();
            return spawn != null && spawn.stone != null;
        }
    }
}