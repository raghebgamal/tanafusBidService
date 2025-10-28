# Bid Service Helper Classes

## Overview

These helper classes extract common logic from `BidServiceCore` to reduce its size and improve maintainability. All helpers are **static classes** with no dependencies, making them easy to test and reuse.

---

## 📁 Available Helpers

### 1. **BidValidationHelper** - Validation Logic

Handles all bid validation rules and business constraints.

**Methods:**
- `ValidateBidFinancialValueWithBidType(model)` - Validates and adjusts financial values based on bid type
- `IsRequiredDataForNotSaveAsDraftAdded(model)` - Checks if required fields are filled for non-draft bids
- `AdjustRequestBidAddressesToTheEndOfTheDay(model)` - Adjusts date/time values to end of day
- `ValidateBidDates(model, bid, settings, checkFunc)` - Validates date consistency and business rules
- `ValidateBidDatesWhileApproving(bid, settings)` - Validates dates during bid approval
- `CheckLastReceivingEnqiryDate(model, bid)` - Validates enquiry date constraints

**Usage Example:**
```csharp
// Before (in BidServiceCore private method):
private void ValidateBidFinancialValueWithBidType(AddBidModelNew model)
{
    if (model.BidTypeId != (int)BidTypes.Public && model.BidTypeId != (int)BidTypes.Private)
    {
        model.IsFinancialInsuranceRequired = false;
        model.BidFinancialInsuranceValue = null;
    }
}

// After (using helper):
BidValidationHelper.ValidateBidFinancialValueWithBidType(model);
```

---

### 2. **BidCalculationHelper** - Price & Fee Calculations

Handles all financial calculations including fees, taxes, and pricing.

**Methods:**
- `CalculateAndUpdateBidPrices(associationFees, settings, bid)` - Calculates all bid prices and updates bid entity
- `CalculateTanafos Fees(associationFees, percentage, minFees)` - Calculates Tanafos platform fees
- `CalculateVAT(amount, vatPercentage)` - Calculates VAT/tax amount
- `CalculateTotalBidDocumentPrice(associationFees, settings)` - Calculates total bid document price

**Usage Example:**
```csharp
// Before (in BidServiceCore private method):
private OperationResult<bool> CalculateAndUpdateBidPrices(double association_Fees, ReadOnlyAppGeneralSettings settings, Bid bid)
{
    double tanafosMoneyWithoutTax = Math.Round((association_Fees * ((double)settings.TanfasPercentage / 100)), 8);
    if (tanafosMoneyWithoutTax < settings.MinTanfasOfBidDocumentPrice)
        tanafosMoneyWithoutTax = settings.MinTanfasOfBidDocumentPrice;
    // ... more calculation logic
}

// After (using helper):
var result = BidCalculationHelper.CalculateAndUpdateBidPrices(association_Fees, settings, bid);
```

---

### 3. **BidUtilityHelper** - Utility Methods

Provides common utility functions for bid operations.

**Methods:**
- `UpdateSiteMapLastModificationDateIfSpecificDataChanged(bid, model)` - Updates sitemap date if relevant fields changed
- `ValidateBidInvitationAttachmentsNew(model)` - Validates invitation attachments
- `CheckIfWeNeedAddAttachmentNew(model)` - Checks if attachments are needed
- `FormatBidRefNumber(firstPart, randomPart)` - Formats bid reference numbers
- `IsBidDraft(bid)` - Checks if bid is in draft status
- `IsBidPublished(bid)` - Checks if bid is published
- `IsBidClosed(bid)` - Checks if bid is closed
- `IsPrivateBid(bid)` / `IsPublicBid(bid)` / `IsHabilitationBid(bid)` - Bid type checks
- `GetBidTypeDisplayName(bidType)` - Gets display name in Arabic
- `GetBidStatusDisplayName(status)` - Gets status display name in Arabic

**Usage Example:**
```csharp
// Before (scattered throughout BidServiceCore):
if (bid.TenderStatusId == (int)TenderStatus.Published)
{
    // logic
}

// After (using helper):
if (BidUtilityHelper.IsBidPublished(bid))
{
    // logic
}
```

---

### 4. **BidQueryHelper** - Query Builders

Provides reusable LINQ expressions for common bid queries.

**Methods:**
- `GetPublishedBidsFilter()` - Filter for published bids
- `GetDraftBidsFilter()` - Filter for draft bids
- `GetBidsByAssociationFilter(associationId)` - Filter by association
- `GetBidsByRegionFilter(regionId)` - Filter by region
- `GetPublicBidsFilter()` / `GetPrivateBidsFilter()` - Filter by visibility
- `GetActiveBidsFilter(currentDate)` - Filter for active bids
- `GetExpiredBidsFilter(currentDate)` - Filter for expired bids
- `GetBidsByDateRangeFilter(start, end)` - Filter by date range
- `GetBidsByIndustryFilter(industryId)` - Filter by industry
- `CombineFiltersWithAnd(filters)` - Combines multiple filters
- `GetDefaultBidOrdering()` - Default ordering (by created date desc)
- `GetBidOrderingByDeadline()` - Order by submission deadline

**Usage Example:**
```csharp
// Before (repeated query patterns):
var publishedBids = await _bidRepository.FindAsync(b => b.TenderStatusId == (int)TenderStatus.Published);

// After (using helper):
var filter = BidQueryHelper.GetPublishedBidsFilter();
var publishedBids = await _bidRepository.FindAsync(filter);

// Combining multiple filters:
var combinedFilter = BidQueryHelper.CombineFiltersWithAnd(
    BidQueryHelper.GetPublishedBidsFilter(),
    BidQueryHelper.GetBidsByAssociationFilter(associationId),
    BidQueryHelper.GetActiveBidsFilter(DateTime.UtcNow)
);
var result = await _bidRepository.FindAsync(combinedFilter);
```

---

## 🔧 How to Refactor BidServiceCore

### Step 1: Identify Private Methods to Extract

Look for private methods in BidServiceCore that fit these categories:
- **Validation** → Move to `BidValidationHelper`
- **Calculations** → Move to `BidCalculationHelper`
- **Utilities** → Move to `BidUtilityHelper`
- **Query patterns** → Move to `BidQueryHelper`

### Step 2: Replace Method Calls

**Before:**
```csharp
public class BidServiceCore
{
    public async Task<OperationResult<AddBidResponse>> AddBidNew(AddBidModelNew model)
    {
        ValidateBidFinancialValueWithBidType(model);
        var result = CalculateAndUpdateBidPrices(fees, settings, bid);
        // ...
    }

    private void ValidateBidFinancialValueWithBidType(AddBidModelNew model)
    {
        // validation logic
    }

    private OperationResult<bool> CalculateAndUpdateBidPrices(double fees, ReadOnlyAppGeneralSettings settings, Bid bid)
    {
        // calculation logic
    }
}
```

**After:**
```csharp
public class BidServiceCore
{
    public async Task<OperationResult<AddBidResponse>> AddBidNew(AddBidModelNew model)
    {
        BidValidationHelper.ValidateBidFinancialValueWithBidType(model);
        var result = BidCalculationHelper.CalculateAndUpdateBidPrices(fees, settings, bid);
        // ...
    }

    // Private methods removed - now in helper classes
}
```

### Step 3: Delete Old Private Methods

Once you've replaced all calls to a private method with helper calls, **delete the private method** from BidServiceCore.

---

## 📊 Impact on BidServiceCore Size

| Metric | Before | After Extraction | Reduction |
|--------|--------|------------------|-----------|
| Total Lines | 10,429 | ~8,500-9,000 | ~15-20% |
| Private Methods | 109 | ~70-80 | ~30 methods |
| Validation Logic | Mixed in | Centralized | ✅ |
| Calculation Logic | Mixed in | Centralized | ✅ |
| Query Patterns | Repeated | Reusable | ✅ |

---

## ✅ Benefits

1. **Reduced Complexity** - BidServiceCore becomes smaller and more focused
2. **Better Testability** - Helper methods can be tested independently
3. **Code Reusability** - Helpers can be used across multiple services
4. **Easier Maintenance** - Logic grouped by responsibility
5. **No Breaking Changes** - Only internal refactoring, public API unchanged
6. **Improved Readability** - Helper names clearly describe what they do

---

## 🚀 Next Steps (Optional)

1. **Extract more private methods** to helpers as you find patterns
2. **Add unit tests** for each helper class
3. **Create notification helper** for email/notification logic
4. **Create mapping helper** for DTO mappings
5. **Document complex business rules** in helper method XML comments

---

## 📝 Important Notes

- **No logic changes** - Helpers contain exact same logic as before
- **No database/entity changes** - Only code organization
- **Backward compatible** - Existing services work without modification
- **Static helpers** - No dependencies, easy to use
- **Well documented** - XML comments on all methods

---

## Example Migration

Here's a complete example of migrating validation logic:

### Before:
```csharp
// In BidServiceCore.cs (Line 806-851)
private void ValidateBidFinancialValueWithBidType(AddBidModelNew model) { /* logic */ }
private OperationResult<bool> AdjustRequestBidAddressesToTheEndOfTheDay<T>(T model) { /* logic */ }
private OperationResult<AddBidResponse> ValidateBidDates(AddBidModelNew model, Bid bid, ReadOnlyAppGeneralSettings generalSettings) { /* logic */ }

// Usage in public method:
public async Task<OperationResult<AddBidResponse>> AddBidNew(AddBidModelNew model)
{
    ValidateBidFinancialValueWithBidType(model);
    var adjustResult = AdjustRequestBidAddressesToTheEndOfTheDay(model);
    var dateResult = ValidateBidDates(model, bid, settings);
}
```

### After:
```csharp
// Private methods REMOVED from BidServiceCore.cs
// Moved to Helpers/BidValidationHelper.cs

// Usage in public method:
public async Task<OperationResult<AddBidResponse>> AddBidNew(AddBidModelNew model)
{
    BidValidationHelper.ValidateBidFinancialValueWithBidType(model);
    var adjustResult = BidValidationHelper.AdjustRequestBidAddressesToTheEndOfTheDay(model);
    var dateResult = BidValidationHelper.ValidateBidDates(model, bid, settings, checkLastReceivingEnqiryDate);
}
```

**Lines Saved:** ~50 lines removed from BidServiceCore per extracted method!

---

## Questions?

These helpers are designed to work alongside your existing refactored services. They don't affect:
- ✅ BidCreationService
- ✅ BidManagementService
- ✅ BidSearchService
- ✅ BidPublishingService
- ✅ BidPaymentService
- ✅ BidNotificationService
- ✅ BidStatisticsService

All services continue working as before, but now BidServiceCore can use these helpers to reduce its complexity.
