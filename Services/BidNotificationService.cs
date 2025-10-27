using Nafes.CrossCutting.Common.OperationResponse;
using Nafes.CrossCutting.Model.Entities;
using Nafis.Services.Contracts;
using Tanafos.Main.Services.DTO.Bid;
using Tanafos.Main.Services.DTO.ReviewedSystemRequestLog;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Nafis.Services.Implementation
{
    /// <summary>
    /// Service for bid notifications and provider invitations
    /// </summary>
    public class BidNotificationService : IBidNotificationService
    {
        private readonly BidServiceCore _bidServiceCore;

        public BidNotificationService(BidServiceCore bidServiceCore)
        {
            _bidServiceCore = bidServiceCore;
        }

        public async Task<OperationResult<bool>> InviteProvidersWithSameCommercialSectors(long bidId, bool isAutomatically = false)
            => await _bidServiceCore.InviteProvidersWithSameCommercialSectors(bidId, isAutomatically);

        public async Task<OperationResult<List<InvitedCompanyResponseDto>>> GetAllInvitedCompaniesForBidAsync(GetAllInvitedCompaniesRequestModel request)
            => await _bidServiceCore.GetAllInvitedCompaniesForBidAsync(request);

        public async Task<OperationResult<List<GetReviewedSystemRequestLogResponse>>> GetProviderInvitationLogs(long bidId)
            => await _bidServiceCore.GetProviderInvitationLogs(bidId);

        public async Task<(List<NotificationReceiverUser> ActualReceivers, List<NotificationReceiverUser> RealtimeReceivers)> GetProvidersUserIdsWhoBoughtTermsPolicyForNotification(Bid bid)
            => await _bidServiceCore.GetProvidersUserIdsWhoBoughtTermsPolicyForNotification(bid);
    }
}
