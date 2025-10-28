# Refactoring Example: AddBidNew Method

This document shows a **real example** of how to refactor `BidServiceCore` using the helper classes.

---

## 🎯 Goal

Refactor the `AddBidNew` method in `BidServiceCore.cs` (line 453) to use helper classes, reducing complexity and improving readability.

---

## 📝 Before Refactoring

### Current Code (BidServiceCore.cs - lines 453-805)

```csharp
public async Task<OperationResult<AddBidResponse>> AddBidNew(AddBidModelNew model)
{
    try
    {
        var usr = _currentUserService.CurrentUser;
        if (usr is null)
            return OperationResult<AddBidResponse>.Fail(HttpErrorCode.NotAuthenticated);

        // Line 466: Call to private method
        var adjustBidAddressesToTheEndOfDayResult = AdjustRequestBidAddressesToTheEndOfTheDay(model);
        if (!adjustBidAddressesToTheEndOfDayResult.IsSucceeded)
            return OperationResult<AddBidResponse>.Fail(adjustBidAddressesToTheEndOfDayResult.HttpErrorCode, adjustBidAddressesToTheEndOfDayResult.Code);

        // Line 470: Call to private method
        if (IsRequiredDataForNotSaveAsDraftAdded(model))
            return OperationResult<AddBidResponse>.Fail(HttpErrorCode.InvalidInput, CommonErrorCodes.INVALID_INPUT);

        // Line 500: Call to private method
        ValidateBidFinancialValueWithBidType(model);

        // ... more logic ...

        // Line 835: Call to private method
        var validateDatesResult = ValidateBidDates(model, bid, generalSettings);
        if (!validateDatesResult.IsSucceeded)
            return validateDatesResult;

        // Line 853: Call to private method
        UpdateSiteMapLastModificationDateIfSpecificDataChanged(bid, model);

        // Line 864: Call to private method
        var calculatePricesResult = CalculateAndUpdateBidPrices(association_Fees, generalSettings, bid);
        if (!calculatePricesResult.IsSucceeded)
            return OperationResult<AddBidResponse>.Fail(calculatePricesResult.HttpErrorCode, calculatePricesResult.Code);

        // ... more logic ...
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Error in AddBidNew");
        return OperationResult<AddBidResponse>.Fail(HttpErrorCode.InternalServerError, CommonErrorCodes.INTERNAL_SERVER_ERROR);
    }
}
```

---

## ✅ After Refactoring

### Refactored Code (BidServiceCore.cs)

```csharp
public async Task<OperationResult<AddBidResponse>> AddBidNew(AddBidModelNew model)
{
    try
    {
        var usr = _currentUserService.CurrentUser;
        if (usr is null)
            return OperationResult<AddBidResponse>.Fail(HttpErrorCode.NotAuthenticated);

        // ✅ Use BidValidationHelper instead of private method
        var adjustBidAddressesToTheEndOfDayResult = BidValidationHelper.AdjustRequestBidAddressesToTheEndOfTheDay(model);
        if (!adjustBidAddressesToTheEndOfDayResult.IsSucceeded)
            return OperationResult<AddBidResponse>.Fail(adjustBidAddressesToTheEndOfDayResult.HttpErrorCode, adjustBidAddressesToTheEndOfDayResult.Code);

        // ✅ Use BidValidationHelper instead of private method
        if (BidValidationHelper.IsRequiredDataForNotSaveAsDraftAdded(model))
            return OperationResult<AddBidResponse>.Fail(HttpErrorCode.InvalidInput, CommonErrorCodes.INVALID_INPUT);

        // ✅ Use BidValidationHelper instead of private method
        BidValidationHelper.ValidateBidFinancialValueWithBidType(model);

        // ... more logic ...

        // ✅ Use BidValidationHelper instead of private method
        var validateDatesResult = BidValidationHelper.ValidateBidDates(model, bid, generalSettings, checkLastReceivingEnqiryDate);
        if (!validateDatesResult.IsSucceeded)
            return validateDatesResult;

        // ✅ Use BidUtilityHelper instead of private method
        BidUtilityHelper.UpdateSiteMapLastModificationDateIfSpecificDataChanged(bid, model);

        // ✅ Use BidCalculationHelper instead of private method
        var calculatePricesResult = BidCalculationHelper.CalculateAndUpdateBidPrices(association_Fees, generalSettings, bid);
        if (!calculatePricesResult.IsSucceeded)
            return OperationResult<AddBidResponse>.Fail(calculatePricesResult.HttpErrorCode, calculatePricesResult.Code);

        // ... more logic ...
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Error in AddBidNew");
        return OperationResult<AddBidResponse>.Fail(HttpErrorCode.InternalServerError, CommonErrorCodes.INTERNAL_SERVER_ERROR);
    }
}
```

---

## 🗑️ Delete These Private Methods (After Refactoring)

Once you've replaced all calls, **delete these private methods** from BidServiceCore.cs:

```csharp
// ❌ DELETE - Line 806-813 (8 lines)
private void ValidateBidFinancialValueWithBidType(AddBidModelNew model) { ... }

// ❌ DELETE - Line 815-820 (6 lines)
private static bool IsRequiredDataForNotSaveAsDraftAdded(AddBidModelNew model) { ... }

// ❌ DELETE - Line 822-833 (12 lines)
private OperationResult<bool> AdjustRequestBidAddressesToTheEndOfTheDay<T>(T model) { ... }

// ❌ DELETE - Line 835-851 (17 lines)
private OperationResult<AddBidResponse> ValidateBidDates(...) { ... }

// ❌ DELETE - Line 853-862 (10 lines)
private void UpdateSiteMapLastModificationDateIfSpecificDataChanged(Bid bid, AddBidModelNew requestModel) { ... }

// ❌ DELETE - Line 864-885 (22 lines)
private OperationResult<bool> CalculateAndUpdateBidPrices(...) { ... }

// ❌ DELETE - Line 916-922 (7 lines)
private bool checkLastReceivingEnqiryDate(AddBidModelNew model, Bid bid) { ... }
```

**Total Lines Removed:** ~82 lines just from this one section!

---

## 📊 Impact Summary

| Metric | Before | After | Improvement |
|--------|--------|-------|-------------|
| Lines in AddBidNew | ~350 lines | ~350 lines | Same (logic stays) |
| Private Helper Methods | 7 methods (~82 lines) | 0 methods | -82 lines |
| BidServiceCore Total Lines | 10,429 | 10,347 | -82 lines |
| Code Reusability | Low | High | ✅ |
| Testability | Difficult | Easy | ✅ |

---

## 🔄 Step-by-Step Refactoring Process

### Step 1: Find the First Private Method Call
```csharp
var adjustBidAddressesToTheEndOfDayResult = AdjustRequestBidAddressesToTheEndOfTheDay(model);
```

### Step 2: Replace with Helper Call
```csharp
var adjustBidAddressesToTheEndOfDayResult = BidValidationHelper.AdjustRequestBidAddressesToTheEndOfTheDay(model);
```

### Step 3: Search for All Usages
Use your IDE's "Find All References" for `AdjustRequestBidAddressesToTheEndOfTheDay` to find all usages.

### Step 4: Replace All Usages
Replace all calls throughout BidServiceCore.cs with the helper version.

### Step 5: Delete Private Method
Once all usages are replaced, delete the private method:
```csharp
// DELETE THIS:
private OperationResult<bool> AdjustRequestBidAddressesToTheEndOfTheDay<T>(T model) where T : BidAddressesModelRequest
{
    // ... implementation
}
```

### Step 6: Test
Build and test to ensure everything still works.

### Step 7: Repeat for Next Method
Move on to the next private method and repeat the process.

---

## 🎯 Priority Order for Refactoring

Refactor methods in this order for maximum impact:

### **High Priority** (Used frequently):
1. ✅ `ValidateBidFinancialValueWithBidType` → `BidValidationHelper`
2. ✅ `CalculateAndUpdateBidPrices` → `BidCalculationHelper`
3. ✅ `AdjustRequestBidAddressesToTheEndOfTheDay` → `BidValidationHelper`
4. ✅ `ValidateBidDates` → `BidValidationHelper`

### **Medium Priority**:
5. `GenerateBidRefNumber` → `BidUtilityHelper` (but needs repository access - see note below)
6. `UpdateSiteMapLastModificationDateIfSpecificDataChanged` → `BidUtilityHelper`
7. `IsTermsBookBoughtBeforeInBid` → Keep in Core (needs repository)
8. Query patterns → `BidQueryHelper`

### **Low Priority**:
9. Business logic methods (keep in Core for now)
10. Methods with complex dependencies (refactor last)

---

## ⚠️ Important Notes

### Methods That Need Repository Access
Some private methods access repositories (like `_bidRepository`, `_providerBidRepository`). These should either:

**Option A:** Stay in BidServiceCore (if they're complex business logic)
```csharp
private async Task<bool> IsTermsBookBoughtBeforeInBid(long bidId)
{
    var isBoughtBefore = await _providerBidRepository.Any(x => x.BidId == bidId && x.IsPaymentConfirmed);
    return isBoughtBefore;
}
// ✅ Keep this in BidServiceCore
```

**Option B:** Create a new helper that accepts repository as parameter
```csharp
// In BidQueryHelper.cs
public static async Task<bool> IsTermsBookBoughtBeforeInBid(
    ICrossCuttingRepository<ProviderBid, long> providerBidRepository,
    long bidId)
{
    var isBoughtBefore = await providerBidRepository.Any(x => x.BidId == bidId && x.IsPaymentConfirmed);
    return isBoughtBefore;
}

// In BidServiceCore.cs
var isBought = await BidQueryHelper.IsTermsBookBoughtBeforeInBid(_providerBidRepository, bidId);
```

**Recommendation:** Start with pure logic helpers (validation, calculation, utility) first. Leave data access methods in Core for now.

---

## 📈 Expected Results

After refactoring all validation and calculation methods:

- **BidServiceCore:** ~9,000-9,500 lines (down from 10,429)
- **Helper Classes:** ~600-800 lines (extracted logic)
- **Net Reduction:** 400-800 lines removed (duplicates and redundant code)
- **Maintainability:** ⬆️ Significantly Improved
- **Testability:** ⬆️ Much Better
- **Code Reuse:** ⬆️ Across multiple services

---

## ✅ Verification Checklist

After refactoring, verify:

- [ ] All private method calls replaced with helper calls
- [ ] All tests still pass (if you have tests)
- [ ] Application builds successfully
- [ ] Existing services (BidCreationService, etc.) still work
- [ ] No logic changes - behavior is identical
- [ ] Private methods deleted from BidServiceCore
- [ ] Helper classes have XML documentation

---

## 🚀 Next Steps

1. Start with validation helpers (easiest)
2. Move to calculation helpers
3. Then utility helpers
4. Finally query helpers
5. Consider extracting email/notification logic later
6. Add unit tests for helpers as you go

---

## Need Help?

- See `Helpers/README.md` for full documentation
- Each helper class has XML comments explaining usage
- Helper methods are static - no dependency injection needed
- No breaking changes to existing refactored services
