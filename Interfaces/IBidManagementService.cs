using Nafes.CrossCutting.Common.OperationResponse;
using Nafes.CrossCutting.Model.Entities;
using Tanafos.Main.Services.DTO.Bid;
using Tanafos.Main.Services.DTO.BidAddresses;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Nafis.Services.Contracts
{
    /// <summary>
    /// Service interface for bid management operations (CRUD, status, details)
    /// </summary>
    public interface IBidManagementService
    {
        // Basic bid retrieval methods
        Task<OperationResult<ReadOnlyBidModel>> GetBidDetails(long id);
        Task<OperationResult<ReadOnlyBidMainDataModel>> GetBidMainData(long id);
        Task<OperationResult<ReadOnlyBidResponse>> GetDetailsForBidByIdAsync(long bidId);
        Task<OperationResult<ReadOnlyPublicBidModel>> GetPublicBidDetails(long id);
        Task<OperationResult<GetBidDetailsForShare>> GetBidDetailsForShare(long bidId);

        // Bid components retrieval
        Task<OperationResult<ReadOnlyBidAddressesTimeModel>> GetBidAddressesTime(long bidId);
        Task<OperationResult<ReadOnlyBidAttachmentRequest>> GetBidAttachment(long bidId);
        Task<OperationResult<IReadOnlyList<ReadOnlyBidNewsModel>>> GetBidNews(long bidId);
        Task<OperationResult<List<ReadOnlyQuantitiesTableModel>>> GetBidQuantitiesTable(long bidId);
        Task<PagedResponse<List<ReadOnlyQuantitiesTableModel>>> GetBidQuantitiesTableNew(long bidId, int pageSize = 5, int pageNumber = 1);
        Task<(OperationResult<byte[]>, string)> GetZipFileForBidAttachmentAsBinary(long bidId);

        // Status and timeline methods
        Task<OperationResult<ReadOnlyBidStatusDetailsModel>> GetBidStatusDetails(long bidId);
        Task<OperationResult<List<BidStatusResponse>>> GetBidStatusWithDates(long bidId);
        Task<OperationResult<List<BidStatusResponse>>> GetBidStatusWithDatesForBid(Bid bidInDb);
        Task<OperationResult<List<BidStatusResponse>>> GetBidStatusWithDatesForInstantBids(Bid bidInDb);
        Task<List<BidStatusResponse>> OrderTimelineByIndexIfIgnoreTimelineIsTrue(List<BidStatusResponse> model);

        // Bid state management
        Task<OperationResult<bool>> UpdateReadProviderRead(long id);
        Task<OperationResult<bool>> IsBidInEvaluation(long bidId);
        Task<OperationResult<bool>> ToggelsAbleToSubscribeToBid(long bidId);
        Task<OperationResult<bool>> RevealBidInCaseNotSubscribe(long bidId);

        // RFI and Requests
        Task<OperationResult<long>> AddRFIandRequests(AddRFIRequestModel model);
        Task<OperationResult<IReadOnlyList<ReadOnlyBidRFiRequestModel>>> GetBidRFiRequests(long bidId, int typeId);

        // Helper/utility methods
        Task<OperationResult<GetUserRoleResponse>> GetUserRole();
        Task<OperationResult<int>> GetStoppingPeriod();
        Task<OperationResult<QuantityStableSettings>> QuantityStableSettings();
        Task<OperationResult<GetEntityContactDetailsResponse>> GetEntityContactDetails(GetEntityContactDetailsRequest request);

        // Bid creator information
        Task<string> GetBidCreatorName(Bid bid);
        Task<string> GetBidCreatorEmailToReceiveEmails(Bid bid);
        Task<(string, string)> GetBidCreatorImage(Bid bid);
    }
}
