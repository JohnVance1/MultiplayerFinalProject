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

            // spawn target if two players
            if (numPlayers == 2)
            {
                if (NetworkServer.connections.TryGetValue(players[0], out NetworkConnectionToClient ni))
                {
                    if (NetworkServer.connections.TryGetValue(players[1], out NetworkConnectionToClient ni2))
                    {
                        ni.identity.gameObject.GetComponent<Player>().SetPlayer(
                            ni2.identity.gameObject.GetComponent<Player>());
                        ni2.identity.gameObject.GetComponent<Player>().SetPlayer(
                            ni.identity.gameObject.GetComponent<Player>());

                    }
                }
                //NetworkServer.connections[1].identity.gameObject.GetComponent<Player>().CmdSetOtherPlayer(
                    //NetworkServer.connections[0].identity.gameObject.GetComponent<Player>());

                //foreach (KeyValuePair<int, NetworkConnectionToClient> ni in NetworkServer.connections)
                //{
                //    if (ni.Value.identity.gameObject.GetComponent<Player>() != this)
                //    {

                //    }
                //    Debug.Log(ni.Value.identity);

                //}

            }
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
