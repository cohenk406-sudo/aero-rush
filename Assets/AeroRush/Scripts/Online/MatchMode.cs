using System;

namespace AeroRush
{
    /// <summary>Playable queue types supported by the lobby prototype.</summary>
    public enum MatchMode
    {
        NormalMatch,
        PrivateFreeForAll
    }

    public enum LobbyVisibility
    {
        Public,
        Private
    }

    [Serializable]
    public sealed class LobbySettings
    {
        public MatchMode Mode;
        public LobbyVisibility Visibility;
        public int MaxPlayers;
        public string JoinCode;

        public LobbySettings(MatchMode mode, LobbyVisibility visibility, int maxPlayers, string joinCode)
        {
            Mode = mode;
            Visibility = visibility;
            MaxPlayers = maxPlayers;
            JoinCode = joinCode;
        }
    }

    /// <summary>
    /// Replace this boundary with Unity Gaming Services Lobby/Relay or another
    /// networking provider to make rooms joinable across devices.
    /// </summary>
    public interface ILobbyTransport
    {
        void CreateLobby(LobbySettings settings, Action<bool, string> completed);
        void JoinLobby(string joinCode, Action<bool, string> completed);
        void LeaveLobby(Action completed);
    }
}
