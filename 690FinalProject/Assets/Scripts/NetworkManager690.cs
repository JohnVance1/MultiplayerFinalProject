using UnityEngine;

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

        public override void OnServerAddPlayer(NetworkConnection conn)
        {
            // add player at correct spawn position
            Transform start = numPlayers == 0 ? playerLeftSpawn : playerRightSpawn;
            GameObject player = Instantiate(playerPrefab, start.position, start.rotation);
            NetworkServer.AddPlayerForConnection(conn, player);

            // spawn target if two players
            if (numPlayers == 2)
            {
                //target = Instantiate(spawnPrefabs.Find(prefab => prefab.name == "Target"));
                //NetworkServer.Spawn(target);
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
