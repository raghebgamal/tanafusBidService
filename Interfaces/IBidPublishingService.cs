using Nafes.CrossCutting.Common.OperationResponse;
using Nafes.CrossCutting.Model.Entities;
using Nafes.CrossCutting.Model.Enums;
using Tanafos.Main.Services.DTO.Bid;
using System.Threading.Tasks;

namespace Nafis.Services.Contracts
{
    /// <summary>
    /// Service interface for bid publishing, approval workflows, and notifications
    /// </summary>
    public interface IBidPublishingService
    {
        /// <summary>
        /// Admin takes action on bid publishing (approve/reject)
        /// </summary>
        Task<OperationResult<bool>> TakeActionOnPublishingBidByAdmin(PublishBidDto request);

        /// <summary>
        /// Executes post-publishing logic after bid is published
        /// </summary>
        Task ExecutePostPublishingLogic(Bid bid, ApplicationUser usr, TenderStatus oldStatusOfBid);

        /// <summary>
        /// Donor takes action on bid (approve/reject)
        /// </summary>
        Task<OperationResult<bool>> TakeActionOnBidByDonor(long bidDonorId, DonorResponse donorResponse);

        /// <summary>
        /// Supervising entity takes action on bid submission
        /// </summary>
        Task<OperationResult<bool>> TakeActionOnBidSubmissionBySupervisingBid(BidSupervisingActionRequest req);

        /// <summary>
        /// Sends email and notification to donor about bid
        /// </summary>
        Task SendEmailAndNotifyDonor(Bid bid);

        /// <summary>
        /// Sends updated bid email to creator and providers
        /// </summary>
        Task SendUpdatedBidEmailToCreatorAndProvidersOfThisBid(Bid bid);
    }
}
