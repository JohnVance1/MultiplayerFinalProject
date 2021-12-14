using UnityEngine;
using System.Collections.Generic;

namespace Mirror
{
    // Custom NetworkManager that simply assigns the correct racket positions when
    // spawning players. The built in RoundRobin spawn method wouldn't work after
    // someone reconnects (both players would be on the same side).
    [AddComponentMenu("")]
    public class NetworkManager690 : NetworkManager
    {
        public Transform playerLeftSpawn;
        public Transform playerRightSpawn;
        public Transform weaponLeftSpawn;
        public Transform weaponRightSpawn;
        public GameObject weapon1;
        public GameObject weapon2;
        private int i = 0;
        public List<int> players;

        public override void OnServerAddPlayer(NetworkConnection conn)
        {
            // add player at correct spawn position
            Transform start = numPlayers == 0 ? playerLeftSpawn : playerRightSpawn;
            GameObject player = Instantiate(playerPrefab, start.position, start.rotation);
            player.GetComponent<Player>().playerName = "Player " + i++;
            NetworkServer.AddPlayerForConnection(conn, player);
            players.Add(conn.connectionId);

            if(numPlayers == 2)
            {
                weapon1 = Instantiate(spawnPrefabs.Find(prefab => prefab.name == "Weapon"), weaponLeftSpawn.position, Quaternion.identity);
                NetworkServer.Spawn(weapon1);
                weapon2 = Instantiate(spawnPrefabs.Find(prefab => prefab.name == "Weapon"), weaponRightSpawn.position, Quaternion.identity);
                NetworkServer.Spawn(weapon2);
            }

            
        }


        public override void OnServerDisconnect(NetworkConnection conn)
        {
            // destroy weapon
            if (weapon1 != null)
                NetworkServer.Destroy(weapon1);
            if (weapon2 != null)
                NetworkServer.Destroy(weapon2);

            // call base functionality (actually destroys the player)
            base.OnServerDisconnect(conn);
        }
    }
}
