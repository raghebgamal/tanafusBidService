using Microsoft.AspNetCore.Http;
using Nafes.CrossCutting.Common.OperationResponse;
using Nafes.CrossCutting.Model.Entities;
using Nafis.Services.DTO.Bid;
using Tanafos.Main.Services.DTO.Bid;
using Tanafos.Main.Services.DTO.BidAddresses;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Nafis.Services.Contracts
{
    /// <summary>
    /// Service interface for bid creation, update, and deletion operations
    /// </summary>
    public interface IBidCreationService
    {
        /// <summary>
        /// Creates or updates a bid
        /// </summary>
        Task<OperationResult<AddBidResponse>> AddBidNew(AddBidModelNew model);

        /// <summary>
        /// Creates an instant bid
        /// </summary>
        Task<OperationResult<AddBidResponse>> AddInstantBid(AddInstantBid addInstantBidRequest);

        /// <summary>
        /// Adds or updates bid addresses and times
        /// </summary>
        Task<OperationResult<long>> AddBidAddressesTimes(AddBidAddressesTimesModel model);

        /// <summary>
        /// Adds bid quantities table
        /// </summary>
        Task<OperationResult<List<QuantitiesTable>>> AddBidQuantitiesTable(AddQuantitiesTableRequest model);

        /// <summary>
        /// Adds attachments to a bid
        /// </summary>
        Task<OperationResult<AddBidAttachmentsResponse>> AddBidAttachments(AddBidAttachmentRequest model);

        /// <summary>
        /// Adds attachments to an instant bid
        /// </summary>
        Task<OperationResult<AddInstantBidAttachmentResponse>> AddInstantBidAttachments(AddInstantBidsAttachments addInstantBidsAttachmentsRequest);

        /// <summary>
        /// Uploads bid attachments from form collection
        /// </summary>
        Task<OperationResult<List<UploadFileResponse>>> UploadBidAttachments(IFormCollection formCollection);

        /// <summary>
        /// Uploads bid news attachments from form collection
        /// </summary>
        Task<OperationResult<List<UploadFileResponse>>> UploadBidAttachmentsNewsFile(IFormCollection formCollection);

        /// <summary>
        /// Adds bid classification, area, and execution details
        /// </summary>
        Task<OperationResult<long>> AddBidClassificationAreaAndExecution(AddBidClassificationAreaAndExecutionModel model);

        /// <summary>
        /// Adds news to a bid
        /// </summary>
        Task<OperationResult<long>> AddBidNews(AddBidNewsModel model);

        /// <summary>
        /// Extends tender timeline
        /// </summary>
        Task<OperationResult<long>> TenderExtend(AddBidAddressesTimesTenderExtendModel model);

        /// <summary>
        /// Copies an existing bid
        /// </summary>
        Task<OperationResult<bool>> CopyBid(CopyBidRequest model);

        /// <summary>
        /// Deletes a draft bid
        /// </summary>
        Task<OperationResult<bool>> DeleteDraftBid(long bidId);
    }
}
