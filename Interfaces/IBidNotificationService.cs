using Nafes.CrossCutting.Common.OperationResponse;
using Nafes.CrossCutting.Model.Entities;
using Tanafos.Main.Services.DTO.Bid;
using Tanafos.Main.Services.DTO.ReviewedSystemRequestLog;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Nafis.Services.Contracts
{
    /// <summary>
    /// Service interface for bid notifications and provider invitations
    /// </summary>
    public interface IBidNotificationService
    {
        /// <summary>
        /// Invites providers with matching commercial sectors to a bid
        /// </summary>
        Task<OperationResult<bool>> InviteProvidersWithSameCommercialSectors(long bidId, bool isAutomatically = false);

        /// <summary>
        /// Gets all invited companies for a specific bid
        /// </summary>
        Task<OperationResult<List<InvitedCompanyResponseDto>>> GetAllInvitedCompaniesForBidAsync(GetAllInvitedCompaniesRequestModel request);

        /// <summary>
        /// Gets provider invitation logs for a bid
        /// </summary>
        Task<OperationResult<List<GetReviewedSystemRequestLogResponse>>> GetProviderInvitationLogs(long bidId);

        /// <summary>
        /// Gets provider user IDs who bought terms policy for notification purposes
        /// </summary>
        Task<(List<NotificationReceiverUser> ActualReceivers, List<NotificationReceiverUser> RealtimeReceivers)> GetProvidersUserIdsWhoBoughtTermsPolicyForNotification(Bid bid);
    }
}
