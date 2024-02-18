#nullable enable
#if YUZE_USE_UNITY_NETCODE
using System.Collections.Generic;
using UnityEngine;

namespace YuzeToolkit.Network
{
    public interface ISessionPlayerData
    {
        bool IsConnected { get; set; }
        ulong ClientID { get; set; }
        void Reinitialize();
    }

    /// <summary>
    /// This class uses a unique player ID to bind a player to a session. Once that player connects to a host, the host
    /// associates the current ClientID to the player's unique ID. If the player disconnects and reconnects to the same
    /// host, the session is preserved.
    /// </summary>
    /// <remarks>
    /// Using a client-generated player ID and sending it directly could be problematic, as a malicious user could
    /// intercept it and reuse it to impersonate the original user. We are currently investigating this to offer a
    /// solution that handles security better.
    /// </remarks>
    /// <typeparam name="T"></typeparam>
    public class SessionManager<T> where T : struct, ISessionPlayerData
    {
        private SessionManager()
        {
        }

        public static SessionManager<T> S_Instance => _s_instance ??= new SessionManager<T>();

        private static SessionManager<T>? _s_instance;

        /// <summary>
        /// Maps a given client player id to the data for a given client player.
        /// </summary>
        private readonly Dictionary<string, T> _clientData = new();

        /// <summary>
        /// Map to allow us to cheaply map from player id to player data.
        /// </summary>
        private readonly Dictionary<ulong, string> _clientIdToPlayerId = new();

        private bool _isInit;

        /// <summary>
        /// Handles client disconnect."
        /// </summary>
        public void DisconnectClient(ulong clientId)
        {
            if (_isInit)
            {
                // Mark client as disconnected, but keep their data so they can reconnect.
                if (_clientIdToPlayerId.TryGetValue(clientId, out var playerId))
                {
                    if (GetPlayerData(playerId)?.ClientID == clientId)
                    {
                        var clientData = _clientData[playerId];
                        clientData.IsConnected = false;
                        _clientData[playerId] = clientData;
                    }
                }
            }
            else
            {
                // Session has not started, no need to keep their data
                if (!_clientIdToPlayerId.Remove(clientId, out var playerId)) return;

                if (GetPlayerData(playerId)?.ClientID == clientId)
                    _clientData.Remove(playerId);
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="playerId">This is the playerId that is unique to this client and persists across multiple logins from the same client</param>
        /// <returns>True if a player with this ID is already connected.</returns>
        public bool IsDuplicateConnection(string playerId)
        {
            return _clientData.ContainsKey(playerId) && _clientData[playerId].IsConnected;
        }

        /// <summary>
        /// Adds a connecting player's session data if it is a new connection, or updates their session data in case of a reconnection.
        /// </summary>
        /// <param name="clientId">This is the clientId that Netcode assigned us on login. It does not persist across multiple logins from the same client. </param>
        /// <param name="playerId">This is the playerId that is unique to this client and persists across multiple logins from the same client</param>
        /// <param name="sessionPlayerData">The player's initial data</param>
        public void SetupConnectingPlayerSessionData(ulong clientId, string playerId, T sessionPlayerData)
        {
            var isReconnecting = false;

            // Test for duplicate connection
            if (IsDuplicateConnection(playerId))
            {
                Debug.LogError(
                    $"Player ID {playerId} already exists. This is a duplicate connection. Rejecting this session data.");
                return;
            }

            // If another client exists with the same playerId
            if (_clientData.TryGetValue(playerId, out var playerData))
            {
                if (!playerData.IsConnected)
                {
                    // If this connecting client has the same player Id as a disconnected client, this is a reconnection.
                    isReconnecting = true;
                }
            }

            // Reconnecting. Give data from old player to new player
            if (isReconnecting)
            {
                // Update player session data
                sessionPlayerData = _clientData[playerId];
                sessionPlayerData.ClientID = clientId;
                sessionPlayerData.IsConnected = true;
            }

            //Populate our dictionaries with the SessionPlayerData
            _clientIdToPlayerId[clientId] = playerId;
            _clientData[playerId] = sessionPlayerData;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="clientId"> id of the client whose data is requested</param>
        /// <returns>The Player ID matching the given client ID</returns>
        public string? GetPlayerId(ulong clientId)
        {
            if (_clientIdToPlayerId.TryGetValue(clientId, out var playerId))
            {
                return playerId;
            }

            Debug.Log($"No client player ID found mapped to the given client ID: {clientId}");
            return null;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="clientId"> id of the client whose data is requested</param>
        /// <returns>Player data struct matching the given ID</returns>
        public T? GetPlayerData(ulong clientId)
        {
            //First see if we have a playerId matching the clientID given.
            var playerId = GetPlayerId(clientId);
            if (playerId != null)
            {
                return GetPlayerData(playerId);
            }

            Debug.Log($"No client player ID found mapped to the given client ID: {clientId}");
            return null;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="playerId"> Player ID of the client whose data is requested</param>
        /// <returns>Player data struct matching the given ID</returns>
        public T? GetPlayerData(string playerId)
        {
            if (_clientData.TryGetValue(playerId, out var data))
            {
                return data;
            }

            Debug.Log($"No PlayerData of matching player ID found: {playerId}");
            return null;
        }

        /// <summary>
        /// Updates player data
        /// </summary>
        /// <param name="clientId"> id of the client whose data will be updated </param>
        /// <param name="sessionPlayerData"> new data to overwrite the old </param>
        public void SetPlayerData(ulong clientId, T sessionPlayerData)
        {
            if (_clientIdToPlayerId.TryGetValue(clientId, out var playerId))
            {
                _clientData[playerId] = sessionPlayerData;
            }
            else
            {
                Debug.LogError($"No client player ID found mapped to the given client ID: {clientId}");
            }
        }

        /// <summary>
        /// Marks the current session as started, so from now on we keep the data of disconnected players.
        /// </summary>
        public void OnSessionStarted()
        {
            _isInit = true;
        }

        /// <summary>
        /// Reinitializes session data from connected players, and clears data from disconnected players, so that if they reconnect in the next game, they will be treated as new players
        /// </summary>
        public void OnSessionEnded()
        {
            ClearDisconnectedPlayersData();
            ReinitializePlayersData();
            _isInit = false;
        }

        /// <summary>
        /// Resets all our runtime state, so it is ready to be reinitialized when starting a new server
        /// </summary>
        public void OnServerEnded()
        {
            _clientData.Clear();
            _clientIdToPlayerId.Clear();
            _isInit = false;
        }

        private void ReinitializePlayersData()
        {
            foreach (var id in _clientIdToPlayerId.Keys)
            {
                var playerId = _clientIdToPlayerId[id];
                var sessionPlayerData = _clientData[playerId];
                sessionPlayerData.Reinitialize();
                _clientData[playerId] = sessionPlayerData;
            }
        }

        private void ClearDisconnectedPlayersData()
        {
            var idsToClear = new List<ulong>();
            foreach (var id in _clientIdToPlayerId.Keys)
            {
                var data = GetPlayerData(id);
                if (data is { IsConnected: false })
                {
                    idsToClear.Add(id);
                }
            }

            foreach (var id in idsToClear)
            {
                var playerId = _clientIdToPlayerId[id];
                if (GetPlayerData(playerId)?.ClientID == id)
                {
                    _clientData.Remove(playerId);
                }

                _clientIdToPlayerId.Remove(id);
            }
        }
    }
}
#endif