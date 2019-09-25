﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tweetinvi.Core.Controllers;
using Tweetinvi.Core.Extensions;
using Tweetinvi.Core.Factories;
using Tweetinvi.Core.Injectinvi;
using Tweetinvi.Core.Iterators;
using Tweetinvi.Core.Web;
using Tweetinvi.Models;
using Tweetinvi.Models.DTO;
using Tweetinvi.Models.DTO.QueryDTO;
using Tweetinvi.Parameters;

namespace Tweetinvi.Controllers.Friendship
{
    public class FriendshipController : IFriendshipController
    {
        private readonly IFriendshipQueryExecutor _friendshipQueryExecutor;
        private readonly IUserFactory _userFactory;
        private readonly IFriendshipFactory _friendshipFactory;
        private readonly IFactory<IRelationshipDetails> _relationshipFactory;
        private readonly IFactory<IRelationshipState> _relationshipStateFactory;
        private readonly IFactory<IFriendshipAuthorizations> _friendshipAuthorizationsFactory;

        public FriendshipController(
            IFriendshipQueryExecutor friendshipQueryExecutor,
            IUserFactory userFactory,
            IFriendshipFactory friendshipFactory,
            IFactory<IRelationshipDetails> relationshipFactory,
            IFactory<IRelationshipState> relationshipStateFactory,
            IFactory<IFriendshipAuthorizations> friendshipAuthorizationsFactory)
        {
            _friendshipQueryExecutor = friendshipQueryExecutor;
            _userFactory = userFactory;
            _friendshipFactory = friendshipFactory;
            _relationshipFactory = relationshipFactory;
            _relationshipStateFactory = relationshipStateFactory;
            _friendshipAuthorizationsFactory = friendshipAuthorizationsFactory;
        }
        
        // Get Users You requested to follow
        public Task<IEnumerable<long>> GetUserIdsYouRequestedToFollow(int maximumUsersToRetrieve = TweetinviConsts.FRIENDSHIPS_OUTGOING_IDS_MAX_PER_REQ)
        {
            return _friendshipQueryExecutor.GetUserIdsYouRequestedToFollow(maximumUsersToRetrieve);
        }

        public async Task<IEnumerable<IUser>> GetUsersYouRequestedToFollow(int maximumUsersToRetrieve = TweetinviConsts.FRIENDSHIPS_OUTGOING_USERS_MAX_PER_REQ)
        {
            var userIds = await GetUserIdsYouRequestedToFollow(maximumUsersToRetrieve);
            return await _userFactory.GetUsersFromIds(userIds);
        }

        // Get Users not authorized to retweet
        public Task<long[]> GetUserIdsWhoseRetweetsAreMuted()
        {
            return _friendshipQueryExecutor.GetUserIdsWhoseRetweetsAreMuted();
        }

        public async Task<IEnumerable<IUser>> GetUsersWhoseRetweetsAreMuted()
        {
            var userIds = await GetUserIdsWhoseRetweetsAreMuted();
            return await _userFactory.GetUsersFromIds(userIds);
        }

        // Update Friendship Authorizations
        public Task<bool> UpdateRelationshipAuthorizationsWith(IUserIdentifier user, bool retweetsEnabled, bool deviceNotificationEnabled)
        {
            var friendshipAuthorizations = _friendshipFactory.GenerateFriendshipAuthorizations(retweetsEnabled, deviceNotificationEnabled);
            return _friendshipQueryExecutor.UpdateRelationshipAuthorizationsWith(user, friendshipAuthorizations);
        }

        public Task<bool> UpdateRelationshipAuthorizationsWith(long userId, bool retweetsEnabled, bool deviceNotificationEnabled)
        {
            var friendshipAuthorizations = _friendshipFactory.GenerateFriendshipAuthorizations(retweetsEnabled, deviceNotificationEnabled);
            return _friendshipQueryExecutor.UpdateRelationshipAuthorizationsWith(new UserIdentifier(userId), friendshipAuthorizations);
        }

        public Task<bool> UpdateRelationshipAuthorizationsWith(string userScreenName, bool retweetsEnabled, bool deviceNotificationEnabled)
        {
            var friendshipAuthorizations = _friendshipFactory.GenerateFriendshipAuthorizations(retweetsEnabled, deviceNotificationEnabled);
            return _friendshipQueryExecutor.UpdateRelationshipAuthorizationsWith(new UserIdentifier(userScreenName), friendshipAuthorizations);
        }

        // Get Relationship (get between 2 users as there is no id for a relationship)
        public async Task<IRelationshipDetails> GetRelationshipBetween(IUserIdentifier sourceUserIdentifier, IUserIdentifier targetUserIdentifier)
        {
            var relationshipDTO = await _friendshipQueryExecutor.GetRelationshipBetween(sourceUserIdentifier, targetUserIdentifier);
            return GenerateRelationshipFromRelationshipDTO(relationshipDTO);
        }

        public Task<IRelationshipDetails> GetRelationshipBetween(IUserIdentifier sourceUserIdentifier, long? targetUserId)
        {
            return InternalGetRelationshipBetween(sourceUserIdentifier, targetUserId);
        }

        public Task<IRelationshipDetails> GetRelationshipBetween(IUserIdentifier sourceUserIdentifier, string targetUserScreenName)
        {
            return InternalGetRelationshipBetween(sourceUserIdentifier, targetUserScreenName);
        }

        public Task<IRelationshipDetails> GetRelationshipBetween(long? sourceUserId,
            IUserIdentifier targetUserIdentifier)
        {
            return InternalGetRelationshipBetween(sourceUserId, targetUserIdentifier);
        }

        public Task<IRelationshipDetails> GetRelationshipBetween(string sourceUserScreenName, IUserIdentifier targetUserIdentifier)
        {
            return InternalGetRelationshipBetween(sourceUserScreenName, targetUserIdentifier);
        }

        public Task<IRelationshipDetails> GetRelationshipBetween(long? sourceUserId, long? targetUserId)
        {
            return InternalGetRelationshipBetween(sourceUserId, targetUserId);
        }

        public Task<IRelationshipDetails> GetRelationshipBetween(long? sourceUserId, string targetUserScreenName)
        {
            return InternalGetRelationshipBetween(sourceUserId, targetUserScreenName);
        }

        public Task<IRelationshipDetails> GetRelationshipBetween(string sourceUserScreenName, long? targetUserId)
        {
            return InternalGetRelationshipBetween(sourceUserScreenName, targetUserId);
        }

        public Task<IRelationshipDetails> GetRelationshipBetween(string sourceUserScreenName, string targetUserScreenName)
        {
            return InternalGetRelationshipBetween(sourceUserScreenName, targetUserScreenName);
        }

        private Task<IRelationshipDetails> InternalGetRelationshipBetween(object sourceIdentifier, object targetIdentifier)
        {
            IUserIdentifier sourceUserIdentifier = null;
            IUserIdentifier targetUserIdentifier = null;

            if (sourceIdentifier is long)
            {
                sourceUserIdentifier = _userFactory.GenerateUserIdentifierFromId((long) sourceIdentifier);
            }
            else
            {
                var screenName = sourceIdentifier as string;
                if (screenName != null)
                {
                    sourceUserIdentifier = _userFactory.GenerateUserIdentifierFromScreenName(screenName);
                }
                else
                {
                    sourceUserIdentifier = sourceIdentifier as IUserIdentifier;
                }
            }

            if (targetIdentifier is long)
            {
                targetUserIdentifier = _userFactory.GenerateUserIdentifierFromId((long) targetIdentifier);
            }
            else
            {
                var screenName = targetIdentifier as string;
                if (screenName != null)
                {
                    targetUserIdentifier = _userFactory.GenerateUserIdentifierFromScreenName(screenName);
                }
                else
                {
                    targetUserIdentifier = targetIdentifier as IUserIdentifier;
                }
            }

            return GetRelationshipBetween(sourceUserIdentifier, targetUserIdentifier);
        }

        // Get multiple relationships
        public async Task<Dictionary<IUser, IRelationshipState>> GetRelationshipStatesAssociatedWith(IEnumerable<IUser> targetUsers)
        {
            if (targetUsers == null)
            {
                throw new ArgumentNullException("Target users cannot be null.");
            }

            if (targetUsers.IsEmpty())
            {
                throw new ArgumentNullException("Target users cannot be empty.");
            }

            var relationshipStates = await GetMultipleRelationships(targetUsers.Select(x => x.UserDTO).ToList());
            var userRelationshipState = new Dictionary<IUser, IRelationshipState>();

            foreach (var targetUser in targetUsers)
            {
                var userRelationship = relationshipStates.FirstOrDefault(x => x.TargetId == targetUser.Id ||
                                                                              x.TargetScreenName == targetUser.ScreenName);
                userRelationshipState.Add(targetUser, userRelationship);
            }

            return userRelationshipState;
        }

        public async Task<IEnumerable<IRelationshipState>> GetMultipleRelationships(IEnumerable<IUserIdentifier> targetUserIdentifiers)
        {
            var relationshipDTO = await _friendshipQueryExecutor.GetMultipleRelationshipsQuery(targetUserIdentifiers);
            return GenerateRelationshipStatesFromRelationshipStatesDTO(relationshipDTO);
        }

        public async Task<IEnumerable<IRelationshipState>> GetMultipleRelationships(IEnumerable<long> targetUsersId)
        {
            var relationshipDTO = await _friendshipQueryExecutor.GetMultipleRelationshipsQuery(targetUsersId);
            return GenerateRelationshipStatesFromRelationshipStatesDTO(relationshipDTO);
        }

        public async Task<IEnumerable<IRelationshipState>> GetMultipleRelationships(IEnumerable<string> targetUsersScreenName)
        {
            var relationshipDTO = await _friendshipQueryExecutor.GetMultipleRelationshipsQuery(targetUsersScreenName);
            return GenerateRelationshipStatesFromRelationshipStatesDTO(relationshipDTO);
        }

        // Generate From DTO
        private IRelationshipDetails GenerateRelationshipFromRelationshipDTO(IRelationshipDetailsDTO relationshipDetailsDTO)
        {
            if (relationshipDetailsDTO == null)
            {
                return null;
            }

            var relationshipParameter = _relationshipFactory.GenerateParameterOverrideWrapper("relationshipDetailsDTO", relationshipDetailsDTO);
            return _relationshipFactory.Create(relationshipParameter);
        }

        // Generate Relationship state from DTO
        private IRelationshipState GenerateRelationshipStateFromRelationshipStateDTO(IRelationshipStateDTO relationshipStateDTO)
        {
            if (relationshipStateDTO == null)
            {
                return null;
            }

            var relationshipStateParameter = _relationshipFactory.GenerateParameterOverrideWrapper("relationshipStateDTO", relationshipStateDTO);
            return _relationshipStateFactory.Create(relationshipStateParameter);
        }

        private List<IRelationshipState> GenerateRelationshipStatesFromRelationshipStatesDTO(IEnumerable<IRelationshipStateDTO> relationshipStateDTOs)
        {
            if (relationshipStateDTOs == null)
            {
                return null;
            }

            return relationshipStateDTOs.Select(GenerateRelationshipStateFromRelationshipStateDTO).ToList();
        }

        // Generate RelationshipAuthorizations
        public IFriendshipAuthorizations GenerateFriendshipAuthorizations(bool retweetsEnabled, bool deviceNotificationEnabled)
        {
            var friendshipAuthorization = _friendshipAuthorizationsFactory.Create();

            friendshipAuthorization.RetweetsEnabled = retweetsEnabled;
            friendshipAuthorization.DeviceNotificationEnabled = deviceNotificationEnabled;

            return friendshipAuthorization;
        }
    }
}