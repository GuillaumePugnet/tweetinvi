﻿using Tweetinvi.Core.Credentials;
using Tweetinvi.Core.Injectinvi;
using Tweetinvi.Core.RateLimit;
using Tweetinvi.Core.Web;
using Tweetinvi.Credentials.QueryDTO;
using Tweetinvi.Credentials.RateLimit;
using Tweetinvi.Models.DTO.QueryDTO;

namespace Tweetinvi.Credentials
{
    public class TweetinviCredentialsModule : ITweetinviModule
    {
        public void Initialize(ITweetinviContainer container)
        {
            container.RegisterType<ITwitterAccessor, TwitterAccessor>(RegistrationLifetime.InstancePerApplication);
            container.RegisterType<ICredentialsAccessor, CredentialsAccessor>(RegistrationLifetime.InstancePerApplication);

            container.RegisterType<ICursorQueryHelper, CursorQueryHelper>();

            RegisterRateLimitHandler(container);
        }

        private void RegisterRateLimitHandler(ITweetinviContainer container)
        {
            container.RegisterType<IRateLimitAwaiter, RateLimitAwaiter>(RegistrationLifetime.InstancePerThread);
            container.RegisterType<IRateLimitCache, RateLimitCache>(RegistrationLifetime.InstancePerApplication);
            container.RegisterType<IRateLimitCacheManager, RateLimitCacheManager>(RegistrationLifetime.InstancePerThread);
            container.RegisterType<IRateLimitHelper, RateLimitHelper>(RegistrationLifetime.InstancePerApplication);
            container.RegisterType<IRateLimitUpdater, RateLimitUpdater>(RegistrationLifetime.InstancePerThread);
        }
    }
}