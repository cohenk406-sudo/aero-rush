using System;
using UnityEngine;

namespace AeroRush
{
    /// <summary>
    /// Lobby flow and mode selection. Attach to a scene object and connect the
    /// events to menu buttons. A transport implementation is required for online play.
    /// </summary>
    public sealed class LobbyManager : MonoBehaviour
    {
        private const string CodeCharacters = "ABCDEFGHJKLMNPQRSTUVWXYZ23456789";
        private ILobbyTransport transport;

        public LobbySettings CurrentLobby { get; private set; }
        public event Action<LobbySettings> LobbyCreated;
        public event Action<string> StatusChanged;

        public void SetTransport(ILobbyTransport lobbyTransport)
        {
            transport = lobbyTransport;
        }

        public void CreateNormalMatch(bool privateLobby = false, int maxPlayers = 4)
        {
            Create(MatchMode.NormalMatch, privateLobby, maxPlayers);
        }

        public void CreatePrivateFreeForAll(int maxPlayers = 4)
        {
            Create(MatchMode.PrivateFreeForAll, true, maxPlayers);
        }

        public void Create(MatchMode mode, bool privateLobby, int maxPlayers)
        {
            if (transport == null)
            {
                StatusChanged?.Invoke("Connect an online lobby transport to create a room.");
                return;
            }

            var settings = new LobbySettings(
                mode,
                privateLobby ? LobbyVisibility.Private : LobbyVisibility.Public,
                Mathf.Clamp(maxPlayers, 2, 8),
                GenerateJoinCode());

            transport.CreateLobby(settings, (success, message) =>
            {
                if (!success)
                {
                    StatusChanged?.Invoke(message ?? "Could not create lobby.");
                    return;
                }

                CurrentLobby = settings;
                LobbyCreated?.Invoke(settings);
                StatusChanged?.Invoke($"Lobby ready. Share code {settings.JoinCode} with friends.");
            });
        }

        public void JoinByCode(string code)
        {
            string normalized = (code ?? string.Empty).Trim().ToUpperInvariant();
            if (normalized.Length != 6)
            {
                StatusChanged?.Invoke("Enter a six-character lobby code.");
                return;
            }
            if (transport == null)
            {
                StatusChanged?.Invoke("Connect an online lobby transport to join friends.");
                return;
            }

            transport.JoinLobby(normalized, (success, message) =>
            {
                StatusChanged?.Invoke(success ? "Joined lobby." : (message ?? "Could not join lobby."));
            });
        }

        public void Leave()
        {
            if (transport == null)
            {
                CurrentLobby = null;
                return;
            }
            transport.LeaveLobby(() =>
            {
                CurrentLobby = null;
                StatusChanged?.Invoke("Left lobby.");
            });
        }

        private static string GenerateJoinCode()
        {
            var chars = new char[6];
            for (int i = 0; i < chars.Length; i++)
                chars[i] = CodeCharacters[UnityEngine.Random.Range(0, CodeCharacters.Length)];
            return new string(chars);
        }
    }
}
