
namespace Silk.Auth
{
    public abstract class Auth
    {
        protected Auth() { }

        public abstract bool IsAuthenticated { get; }
        public abstract bool IsExpired { get; }
        public abstract bool IsRevoked { get; }
        public abstract bool IsBlocked { get; }
        public abstract bool IsBanned { get; }

        public bool IsAuth()
        {
            return IsAuthenticated && !IsExpired && !IsRevoked && !IsBlocked && !IsBanned;
        }
    }
}
