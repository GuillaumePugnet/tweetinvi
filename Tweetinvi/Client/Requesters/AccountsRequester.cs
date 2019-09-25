using Tweetinvi.Core.Controllers;
using Tweetinvi.Core.Iterators;
using Tweetinvi.Core.Web;
using Tweetinvi.Credentials.QueryJsonConverters;
using Tweetinvi.Models.DTO.QueryDTO;
using Tweetinvi.Parameters;

namespace Tweetinvi.Client.Requesters
{
    public interface IInternalAccountsRequester : IAccountsRequester, IBaseRequester
    {
    }
    
    /// <summary>
    /// A client providing all the methods related with account management.
    /// The results from this client contain additional metadata.
    /// </summary>
    public interface IAccountsRequester
    {
        /// <summary>
        /// Get the pending follower requests for protected accounts.
        /// <para>Read more : https://developer.twitter.com/en/docs/accounts-and-users/follow-search-get-users/api-reference/get-friendships-incoming</para>
        /// <para>Protected accounts : https://help.twitter.com/en/safety-and-security/public-and-protected-tweets</para>
        /// </summary>
        /// <returns>Collection of user ids for every user who has a pending request to follow the clients' authenticated user</returns>
        ITwitterPageIterator<ITwitterResult<IIdsCursorQueryResultDTO>> GetUserIdsRequestingFriendship(IGetUserIdsRequestingFriendshipParameters parameters);
    }
    
    public class AccountsRequester : BaseRequester, IInternalAccountsRequester
    {
        private readonly IAccountController _accountController;

        public AccountsRequester(IAccountController accountController)
        {
            _accountController = accountController;
        }
        
        public ITwitterPageIterator<ITwitterResult<IIdsCursorQueryResultDTO>> GetUserIdsRequestingFriendship(IGetUserIdsRequestingFriendshipParameters parameters)
        {
            var request = _twitterClient.CreateRequest();
            request.ExecutionContext.Converters = JsonQueryConverterRepository.Converters;
            return _accountController.GetUserIdsRequestingFriendship(parameters, request);
        }
    }
}