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
        GameObject target;
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

            
        }


        public override void OnServerDisconnect(NetworkConnection conn)
        {
            // destroy ball
            if (target != null)
                NetworkServer.Destroy(target);

            // call base functionality (actually destroys the player)
            base.OnServerDisconnect(conn);
        }
    }
}
