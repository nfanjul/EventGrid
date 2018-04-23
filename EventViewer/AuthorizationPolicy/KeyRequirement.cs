using Microsoft.AspNetCore.Authorization;

namespace EventViewer.AuthorizationPolicy
{
    public class KeyRequirement : IAuthorizationRequirement
    {
        public string Key { get; private set; }

        public KeyRequirement(string key)
        {
            Key = key;
        }
    }
}