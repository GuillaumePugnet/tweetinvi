﻿using System;
using Tweetinvi.Core.QueryValidators;
using Tweetinvi.Models;

namespace Tweetinvi.Controllers.User
{
    public class UserQueryValidator : IUserQueryValidator
    {
        public void ThrowIfUserCannotBeIdentified(IUserIdentifier user)
        {
            ThrowIfUserCannotBeIdentified(user, "user");
        }

        public void ThrowIfUserCannotBeIdentified(IUserIdentifier user, string parameterName)
        {
            if (user == null)
            {
                throw new ArgumentNullException($"{parameterName}", $"{parameterName} cannot be null");
            }

            if (!IsUserIdValid(user.Id) && string.IsNullOrEmpty(user.IdStr) && !IsScreenNameValid(user.ScreenName))
            {
                throw new ArgumentException($"{parameterName} is not valid.", parameterName);
            }
        }

        private bool IsScreenNameValid(string screenName)
        {
            return !string.IsNullOrEmpty(screenName);
        }

        private bool IsUserIdValid(long userId)
        {
            return userId > 0;
        }
    }
}