namespace HR_System.Application.Services
{
    public static class BlackListTokenService
    {
        private static Dictionary<string, string> _blackListTokens = new Dictionary<string, string>();

        public static bool isBlackListed(string tokenId, string expDate)
        {
            var isExist = _blackListTokens.TryGetValue(tokenId, out var result);
            if (isExist)
                return true;
            return false;

        }

        public static void InsertInBlackList(string tokenId, string expDate)
        {
            if (isBlackListed(tokenId, expDate))
                return;
            _blackListTokens[tokenId] = expDate;
        }

        public static void RemoveAllExpired()
        {
            foreach (var kvp in _blackListTokens)
            {
                var expUnix = long.Parse(kvp.Value);
                var expirationDate = DateTimeOffset.FromUnixTimeSeconds(expUnix).UtcDateTime;

                if (expirationDate <= DateTime.UtcNow)
                    _blackListTokens.Remove(kvp.Key);
            }
        }
    }
}
