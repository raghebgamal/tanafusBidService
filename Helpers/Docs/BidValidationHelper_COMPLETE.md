# BidValidationHelper - Complete Documentation

## 📋 Table of Contents
1. [Overview](#overview)
2. [Class Purpose](#class-purpose)
3. [When to Use This Helper](#when-to-use-this-helper)
4. [Methods Documentation](#methods-documentation)
5. [Quick Reference Table](#quick-reference-table)
6. [Common Usage Patterns](#common-usage-patterns)

---

## Overview

**File:** `Helpers/BidValidationHelper.cs`
**Type:** Static Helper Class
**Namespace:** `Nafis.Services.Implementation.Helpers`
**Purpose:** Centralizes all bid validation logic including date validation, financial validation, and business rule validation.

---

## Class Purpose

The `BidValidationHelper` class extracts and centralizes all validation logic related to bids. This includes:
- ✅ Date and time validation
- ✅ Financial value validation based on bid type
- ✅ Required field validation
- ✅ Business rule validation
- ✅ Date consistency checks

**Why use this helper?**
- Reduces BidServiceCore complexity
- Makes validation logic reusable across services
- Easier to test validation rules independently
- Clear separation between validation and business logic
- Single source of truth for validation rules

---

## When to Use This Helper

Use `BidValidationHelper` when you need to:
- ✅ Validate bid dates (enquiry dates, submission dates, opening dates)
- ✅ Check if required data is provided for non-draft bids
- ✅ Adjust date/time values to end of day
- ✅ Validate financial values based on bid type
- ✅ Ensure date consistency across bid lifecycle

**Don't use this helper for:**
- ❌ Database operations (use repositories instead)
- ❌ Business logic execution (keep in service methods)
- ❌ Calculations (use BidCalculationHelper instead)

---

## Methods Documentation

### 1. ValidateBidFinancialValueWithBidType

#### **Purpose**
Validates and adjusts financial insurance requirements based on bid type. Only Public and Private bids require financial insurance.

#### **Method Signature**
```csharp
public static void ValidateBidFinancialValueWithBidType(AddBidModelNew model)
```

#### **Parameters**
| Parameter | Type | Description |
|-----------|------|-------------|
| `model` | `AddBidModelNew` | The bid model to validate and potentially modify |

#### **Return Value**
`void` - Modifies the model in place

#### **What It Does**
1. Checks if bid type is NOT Public or Private
2. If true, sets `IsFinancialInsuranceRequired` to `false`
3. Clears `BidFinancialInsuranceValue` to `null`

#### **Business Rule**
Financial insurance is ONLY required for:
- ✅ Public bids (`BidTypes.Public`)
- ✅ Private bids (`BidTypes.Private`)

NOT required for:
- ❌ Habilitation bids
- ❌ Other bid types

#### **When to Use**
Call this method when:
- Creating a new bid (`AddBidNew`)
- Updating bid financial details
- Changing bid type

#### **Where to Replace in BidServiceCore**
**Original Location:** Line 806-813 in BidServiceCore.cs

**Original Code:**
```csharp
private void ValidateBidFinancialValueWithBidType(AddBidModelNew model)
{
    if (model.BidTypeId != (int)BidTypes.Public && model.BidTypeId != (int)BidTypes.Private)
    {
        model.IsFinancialInsuranceRequired = false;
        model.BidFinancialInsuranceValue = null;
    }
}
```

**Replacement:**
```csharp
// ✅ Using BidValidationHelper instead of private method
BidValidationHelper.ValidateBidFinancialValueWithBidType(model);
```

#### **Usage Example**
```csharp
// In AddBidNew method
public async Task<OperationResult<AddBidResponse>> AddBidNew(AddBidModelNew model)
{
    // ... authentication checks ...

    // Validate financial insurance based on bid type
    BidValidationHelper.ValidateBidFinancialValueWithBidType(model);

    // Now model.IsFinancialInsuranceRequired and model.BidFinancialInsuranceValue
    // are correctly set based on bid type

    // ... continue with bid creation ...
}
```

#### **Real-World Scenario**
```csharp
// Scenario: User creates a Habilitation bid with financial insurance
var model = new AddBidModelNew
{
    BidTypeId = (int)BidTypes.Habilitation,
    IsFinancialInsuranceRequired = true,
    BidFinancialInsuranceValue = 50000
};

// Apply validation
BidValidationHelper.ValidateBidFinancialValueWithBidType(model);

// Result: Financial insurance is cleared for Habilitation bids
// model.IsFinancialInsuranceRequired = false
// model.BidFinancialInsuranceValue = null
```

---

### 2. IsRequiredDataForNotSaveAsDraftAdded

#### **Purpose**
Checks if all required fields are filled when saving a bid as non-draft. Ensures data completeness before publishing.

#### **Method Signature**
```csharp
public static bool IsRequiredDataForNotSaveAsDraftAdded(AddBidModelNew model)
```

#### **Parameters**
| Parameter | Type | Description |
|-----------|------|-------------|
| `model` | `AddBidModelNew` | The bid model to validate |

#### **Return Value**
| Type | Value | Meaning |
|------|-------|---------|
| `bool` | `true` | Required data is MISSING (validation FAILED) |
| `bool` | `false` | All required data is present (validation PASSED) |

**⚠️ IMPORTANT:** This method returns `true` when validation **FAILS**, not when it passes!

#### **What It Does**
Validates that when `IsDraft = false`, the following are provided:
1. ✅ `LastDateInReceivingEnquiries` has value
2. ✅ `LastDateInOffersSubmission` has value
3. ✅ `OffersOpeningDate` has value
4. ✅ `RegionsId` list is not null and not empty

#### **Business Rule**
For non-draft bids (ready to publish), ALL of these are mandatory:
- Enquiry deadline date
- Submission deadline date
- Opening date
- At least one region

#### **When to Use**
Call this method when:
- Saving bid as non-draft
- Before publishing a bid
- Validating bid completeness

#### **Where to Replace in BidServiceCore**
**Original Location:** Line 815-820 in BidServiceCore.cs

**Original Code:**
```csharp
private static bool IsRequiredDataForNotSaveAsDraftAdded(AddBidModelNew model)
{
    var isAllRequiredDatesAdded = model.LastDateInReceivingEnquiries.HasValue &&
         model.LastDateInOffersSubmission.HasValue && model.OffersOpeningDate.HasValue;
    return !model.IsDraft && ((!isAllRequiredDatesAdded) || (model.RegionsId is null || model.RegionsId.Count == 0));
}
```

**Replacement:**
```csharp
// ✅ Using BidValidationHelper instead of private method
if (BidValidationHelper.IsRequiredDataForNotSaveAsDraftAdded(model))
    return OperationResult<AddBidResponse>.Fail(HttpErrorCode.InvalidInput, CommonErrorCodes.INVALID_INPUT);
```

#### **Usage Example**
```csharp
// In AddBidNew method
public async Task<OperationResult<AddBidResponse>> AddBidNew(AddBidModelNew model)
{
    // ... authentication checks ...

    // Check if required data is missing for non-draft bids
    if (BidValidationHelper.IsRequiredDataForNotSaveAsDraftAdded(model))
    {
        // Required data is MISSING - return error
        return OperationResult<AddBidResponse>.Fail(
            HttpErrorCode.InvalidInput,
            CommonErrorCodes.INVALID_INPUT
        );
    }

    // Required data is present - continue
    // ... rest of bid creation ...
}
```

#### **Real-World Scenarios**

**Scenario 1: Draft Bid (validation passes)**
```csharp
var model = new AddBidModelNew
{
    IsDraft = true,
    // Dates can be null for drafts
    LastDateInReceivingEnquiries = null,
    RegionsId = null
};

bool hasError = BidValidationHelper.IsRequiredDataForNotSaveAsDraftAdded(model);
// Result: false (no error - drafts don't need complete data)
```

**Scenario 2: Non-Draft Bid with Missing Data (validation fails)**
```csharp
var model = new AddBidModelNew
{
    IsDraft = false,
    LastDateInReceivingEnquiries = DateTime.Now,
    LastDateInOffersSubmission = null, // ❌ MISSING
    OffersOpeningDate = DateTime.Now,
    RegionsId = new List<int> { 1 }
};

bool hasError = BidValidationHelper.IsRequiredDataForNotSaveAsDraftAdded(model);
// Result: true (ERROR - submission date is missing)
```

**Scenario 3: Non-Draft Bid with Complete Data (validation passes)**
```csharp
var model = new AddBidModelNew
{
    IsDraft = false,
    LastDateInReceivingEnquiries = DateTime.Now.AddDays(10),
    LastDateInOffersSubmission = DateTime.Now.AddDays(20),
    OffersOpeningDate = DateTime.Now.AddDays(21),
    RegionsId = new List<int> { 1, 2 }
};

bool hasError = BidValidationHelper.IsRequiredDataForNotSaveAsDraftAdded(model);
// Result: false (no error - all required data present)
```

---

### 3. AdjustRequestBidAddressesToTheEndOfTheDay

#### **Purpose**
Adjusts date/time values to ensure consistent time boundaries. Sets dates to end of day (23:59:59) or start of day (00:00:00) as appropriate.

#### **Method Signature**
```csharp
public static OperationResult<bool> AdjustRequestBidAddressesToTheEndOfTheDay<T>(T model)
    where T : BidAddressesModelRequest
```

#### **Parameters**
| Parameter | Type | Description |
|-----------|------|-------------|
| `model` | `T : BidAddressesModelRequest` | Generic bid model containing date fields |

#### **Return Value**
| Type | Value | Meaning |
|------|-------|---------|
| `OperationResult<bool>` | Success(true) | Dates adjusted successfully |
| `OperationResult<bool>` | Fail | Model is null (invalid input) |

#### **What It Does**
Adjusts these dates in the model:
1. **LastDateInReceivingEnquiries** → **23:59:59** (end of day)
2. **LastDateInOffersSubmission** → **23:59:59** (end of day)
3. **OffersOpeningDate** → **00:00:00** (start of day)
4. **ExpectedAnchoringDate** → **00:00:00** (start of day)

#### **Business Rule**
- Deadlines (enquiries, submissions) should be at END of day (give users full day)
- Opening/anchoring dates should be at START of day (clear beginning)

#### **When to Use**
Call this method when:
- Creating new bid
- Updating bid dates
- Receiving dates from user input
- Before saving bid dates to database

#### **Where to Replace in BidServiceCore**
**Original Location:** Line 822-833 in BidServiceCore.cs

**Original Code:**
```csharp
private OperationResult<bool> AdjustRequestBidAddressesToTheEndOfTheDay<T>(T model)
    where T : BidAddressesModelRequest
{
    if (model is null)
        return OperationResult<bool>.Fail(HttpErrorCode.InvalidInput, CommonErrorCodes.INVALID_INPUT);

    model.LastDateInReceivingEnquiries = model.LastDateInReceivingEnquiries is null
        ? model.LastDateInReceivingEnquiries
        : new DateTime(model.LastDateInReceivingEnquiries.Value.Year,
                      model.LastDateInReceivingEnquiries.Value.Month,
                      model.LastDateInReceivingEnquiries.Value.Day, 23, 59, 59);
    // ... similar for other dates
    return OperationResult<bool>.Success(true);
}
```

**Replacement:**
```csharp
// ✅ Using BidValidationHelper instead of private method
var adjustResult = BidValidationHelper.AdjustRequestBidAddressesToTheEndOfTheDay(model);
if (!adjustResult.IsSucceeded)
    return OperationResult<AddBidResponse>.Fail(adjustResult.HttpErrorCode, adjustResult.Code);
```

#### **Usage Example**
```csharp
// In AddBidNew method
public async Task<OperationResult<AddBidResponse>> AddBidNew(AddBidModelNew model)
{
    // Adjust dates to proper time boundaries
    var adjustResult = BidValidationHelper.AdjustRequestBidAddressesToTheEndOfTheDay(model);
    if (!adjustResult.IsSucceeded)
    {
        return OperationResult<AddBidResponse>.Fail(
            adjustResult.HttpErrorCode,
            adjustResult.Code
        );
    }

    // Now dates are properly adjusted
    // model.LastDateInReceivingEnquiries = "2024-01-15 23:59:59"
    // model.OffersOpeningDate = "2024-01-20 00:00:00"

    // ... continue with bid creation ...
}
```

#### **Real-World Scenario**
```csharp
// User submits dates with random times
var model = new AddBidModelNew
{
    LastDateInReceivingEnquiries = new DateTime(2024, 1, 15, 14, 30, 45),  // 2:30 PM
    LastDateInOffersSubmission = new DateTime(2024, 1, 20, 9, 15, 0),       // 9:15 AM
    OffersOpeningDate = new DateTime(2024, 1, 21, 16, 45, 30),              // 4:45 PM
    ExpectedAnchoringDate = new DateTime(2024, 2, 1, 11, 0, 0)              // 11:00 AM
};

// Adjust to proper boundaries
var result = BidValidationHelper.AdjustRequestBidAddressesToTheEndOfTheDay(model);

// After adjustment:
// LastDateInReceivingEnquiries = 2024-01-15 23:59:59 (end of day)
// LastDateInOffersSubmission  = 2024-01-20 23:59:59 (end of day)
// OffersOpeningDate           = 2024-01-21 00:00:00 (start of day)
// ExpectedAnchoringDate       = 2024-02-01 00:00:00 (start of day)
```

---

### 4. ValidateBidDates

#### **Purpose**
Validates the logical consistency of bid dates and ensures they follow business rules for date ordering.

#### **Method Signature**
```csharp
public static OperationResult<AddBidResponse> ValidateBidDates(
    AddBidModelNew model,
    Bid bid,
    ReadOnlyAppGeneralSettings generalSettings,
    Func<AddBidModelNew, Bid, bool> checkLastReceivingEnqiryDate)
```

#### **Parameters**
| Parameter | Type | Description |
|-----------|------|-------------|
| `model` | `AddBidModelNew` | New bid data to validate |
| `bid` | `Bid` | Existing bid (can be null for new bids) |
| `generalSettings` | `ReadOnlyAppGeneralSettings` | System settings (stopping period) |
| `checkLastReceivingEnqiryDate` | `Func<AddBidModelNew, Bid, bool>` | Function to check enquiry date validity |

#### **Return Value**
| Type | Value | Meaning |
|------|-------|---------|
| `OperationResult<AddBidResponse>` | Success(null) | All dates are valid |
| `OperationResult<AddBidResponse>` | Fail | Date validation failed with specific error code |

#### **What It Does**
Validates these date rules:
1. ✅ Enquiry date must not be before today (for existing bids)
2. ✅ Submission date > Enquiry date
3. ✅ Opening date > Submission date
4. ✅ Anchoring date > Opening date + Stopping Period

#### **Business Rules**

**Rule 1: Enquiry Date (for updates)**
```
LastDateInReceivingEnquiries >= Today (if date is being changed)
```

**Rule 2: Date Sequence**
```
LastDateInReceivingEnquiries < LastDateInOffersSubmission < OffersOpeningDate
```

**Rule 3: Anchoring Date**
```
ExpectedAnchoringDate > OffersOpeningDate + StoppingPeriodDays
```

#### **When to Use**
Call this method when:
- Creating new bid with dates
- Updating bid dates
- Before saving bid to database
- During bid approval process

#### **Where to Replace in BidServiceCore**
**Original Location:** Line 835-851 in BidServiceCore.cs

**Original Code:**
```csharp
private OperationResult<AddBidResponse> ValidateBidDates(AddBidModelNew model, Bid bid, ReadOnlyAppGeneralSettings generalSettings)
{
    if (bid is not null && checkLastReceivingEnqiryDate(model, bid))
        return OperationResult<AddBidResponse>.Fail(HttpErrorCode.InvalidInput,
            BidErrorCodes.LAST_DATE_IN_RECEIVING_ENQUIRIES_MUST_NOT_BE_BEFORE_TODAY_DATE);

    else if (model.LastDateInReceivingEnquiries > model.LastDateInOffersSubmission)
        return OperationResult<AddBidResponse>.Fail(HttpErrorCode.InvalidInput,
            BidErrorCodes.LAST_DATE_IN_OFFERS_SUBMISSION_MUST_BE_GREATER_THAN_LAST_DATE_IN_RECEIVING_ENQUIRIES);

    // ... more validations

    return OperationResult<AddBidResponse>.Success(null);
}
```

**Replacement:**
```csharp
// ✅ Using BidValidationHelper instead of private method
var validateDatesResult = BidValidationHelper.ValidateBidDates(
    model,
    bid,
    generalSettings,
    checkLastReceivingEnqiryDate
);

if (!validateDatesResult.IsSucceeded)
    return OperationResult<AddBidResponse>.Fail(
        validateDatesResult.HttpErrorCode,
        validateDatesResult.Code
    );
```

#### **Usage Example**
```csharp
// In AddBidNew method
public async Task<OperationResult<AddBidResponse>> AddBidNew(AddBidModelNew model)
{
    // ... get general settings ...
    var generalSettings = await _appGeneralSettingService.GetAppGeneralSettings();

    // ... get existing bid (if update) ...
    Bid existingBid = model.Id != 0
        ? await _bidRepository.FindOneAsync(x => x.Id == model.Id)
        : null;

    // Validate date consistency
    var dateValidation = BidValidationHelper.ValidateBidDates(
        model,
        existingBid,
        generalSettings.Data,
        checkLastReceivingEnqiryDate  // Method reference
    );

    if (!dateValidation.IsSucceeded)
    {
        return OperationResult<AddBidResponse>.Fail(
            dateValidation.HttpErrorCode,
            dateValidation.Code,
            dateValidation.ErrorMessage
        );
    }

    // Dates are valid - continue
    // ... rest of bid creation ...
}
```

#### **Real-World Scenarios**

**Scenario 1: Valid Date Sequence**
```csharp
var model = new AddBidModelNew
{
    LastDateInReceivingEnquiries = new DateTime(2024, 1, 15),
    LastDateInOffersSubmission = new DateTime(2024, 1, 25),
    OffersOpeningDate = new DateTime(2024, 1, 26),
    ExpectedAnchoringDate = new DateTime(2024, 2, 5)
};

var settings = new ReadOnlyAppGeneralSettings { StoppingPeriodDays = 7 };

var result = BidValidationHelper.ValidateBidDates(model, null, settings, checkFunc);
// Result: Success - all dates follow rules
```

**Scenario 2: Submission Before Enquiry (INVALID)**
```csharp
var model = new AddBidModelNew
{
    LastDateInReceivingEnquiries = new DateTime(2024, 1, 25),  // ❌ After submission
    LastDateInOffersSubmission = new DateTime(2024, 1, 20),     // ❌ Before enquiry
    OffersOpeningDate = new DateTime(2024, 1, 26)
};

var result = BidValidationHelper.ValidateBidDates(model, null, settings, checkFunc);
// Result: Fail with error code:
// LAST_DATE_IN_OFFERS_SUBMISSION_MUST_BE_GREATER_THAN_LAST_DATE_IN_RECEIVING_ENQUIRIES
```

**Scenario 3: Opening Before Submission (INVALID)**
```csharp
var model = new AddBidModelNew
{
    LastDateInReceivingEnquiries = new DateTime(2024, 1, 15),
    LastDateInOffersSubmission = new DateTime(2024, 1, 25),
    OffersOpeningDate = new DateTime(2024, 1, 20)  // ❌ Before submission
};

var result = BidValidationHelper.ValidateBidDates(model, null, settings, checkFunc);
// Result: Fail with error code:
// OFFERS_OPENING_DATE_MUST_BE_GREATER_THAN_LAST_DATE_IN_OFFERS_SUBMISSION
```

**Scenario 4: Anchoring Too Soon (INVALID)**
```csharp
var model = new AddBidModelNew
{
    LastDateInReceivingEnquiries = new DateTime(2024, 1, 15),
    LastDateInOffersSubmission = new DateTime(2024, 1, 25),
    OffersOpeningDate = new DateTime(2024, 1, 26),
    ExpectedAnchoringDate = new DateTime(2024, 1, 28)  // ❌ Only 2 days after opening
};

var settings = new ReadOnlyAppGeneralSettings { StoppingPeriodDays = 7 };

var result = BidValidationHelper.ValidateBidDates(model, null, settings, checkFunc);
// Result: Fail with error code:
// EXPECTED_ANCHORING_DATE_MUST_BE_GREATER_THAN_OFFERS_OPENING_DATE_PLUS_STOPPING_PERIOD
```

---

### 5. ValidateBidDatesWhileApproving

#### **Purpose**
Similar to `ValidateBidDates` but specifically for bid approval workflow. Validates dates when admin is approving a bid.

#### **Method Signature**
```csharp
public static OperationResult<AddBidResponse> ValidateBidDatesWhileApproving(
    Bid bid,
    ReadOnlyAppGeneralSettings generalSettings)
```

#### **Parameters**
| Parameter | Type | Description |
|-----------|------|-------------|
| `bid` | `Bid` | The bid being approved |
| `generalSettings` | `ReadOnlyAppGeneralSettings` | System settings |

#### **Return Value**
Same as `ValidateBidDates`

#### **What It Does**
Validates dates from the existing bid entity (not from model) during approval process.

#### **When to Use**
Call this method when:
- Admin is approving a bid
- Publishing a bid
- Moving bid from draft to active

#### **Where to Replace in BidServiceCore**
**Original Location:** Line 2015-2031 in BidServiceCore.cs

**Replacement:**
```csharp
// ✅ Using BidValidationHelper instead of private method
var dateValidation = BidValidationHelper.ValidateBidDatesWhileApproving(bid, generalSettings);
if (!dateValidation.IsSucceeded)
    return dateValidation;
```

---

### 6. CheckLastReceivingEnqiryDate

#### **Purpose**
Checks if the enquiry date is being changed to a past date (which is not allowed for published bids).

#### **Method Signature**
```csharp
public static bool CheckLastReceivingEnqiryDate(AddBidModelNew model, Bid bid)
```

#### **Parameters**
| Parameter | Type | Description |
|-----------|------|-------------|
| `model` | `AddBidModelNew` | New bid data with updated dates |
| `bid` | `Bid` | Existing bid in database |

#### **Return Value**
| Type | Value | Meaning |
|------|-------|---------|
| `bool` | `true` | Date is invalid (in the past and changed) |
| `bool` | `false` | Date is valid |

#### **What It Does**
Returns `true` if:
1. New enquiry date has a value
2. Existing bid has enquiry date
3. New date is before today (UTC)
4. Date is being changed (not the same as existing)

#### **When to Use**
- Used as parameter in `ValidateBidDates` method
- Don't call directly - pass as function reference

#### **Where to Replace in BidServiceCore**
**Original Location:** Line 916-922 in BidServiceCore.cs

**Usage:**
```csharp
// Pass as parameter to ValidateBidDates
var result = BidValidationHelper.ValidateBidDates(
    model,
    bid,
    settings,
    BidValidationHelper.CheckLastReceivingEnqiryDate  // Pass as function
);
```

---

## Quick Reference Table

| Method | When to Use | Returns | Primary Use Case |
|--------|-------------|---------|------------------|
| `ValidateBidFinancialValueWithBidType` | Creating/updating bid | `void` | Clear financial insurance for non-public/private bids |
| `IsRequiredDataForNotSaveAsDraftAdded` | Before publishing | `bool` | Check if all required fields are filled |
| `AdjustRequestBidAddressesToTheEndOfTheDay` | Before saving dates | `OperationResult<bool>` | Adjust dates to proper time boundaries |
| `ValidateBidDates` | Creating/updating bid | `OperationResult<AddBidResponse>` | Validate date sequence and consistency |
| `ValidateBidDatesWhileApproving` | During approval | `OperationResult<AddBidResponse>` | Validate dates when approving |
| `CheckLastReceivingEnqiryDate` | Helper for ValidateBidDates | `bool` | Check if enquiry date is valid |

---

## Common Usage Patterns

### Pattern 1: New Bid Creation
```csharp
public async Task<OperationResult<AddBidResponse>> AddBidNew(AddBidModelNew model)
{
    // Step 1: Adjust dates to proper boundaries
    var adjustResult = BidValidationHelper.AdjustRequestBidAddressesToTheEndOfTheDay(model);
    if (!adjustResult.IsSucceeded)
        return OperationResult<AddBidResponse>.Fail(adjustResult.HttpErrorCode, adjustResult.Code);

    // Step 2: Check required data
    if (BidValidationHelper.IsRequiredDataForNotSaveAsDraftAdded(model))
        return OperationResult<AddBidResponse>.Fail(HttpErrorCode.InvalidInput, CommonErrorCodes.INVALID_INPUT);

    // Step 3: Validate financial values
    BidValidationHelper.ValidateBidFinancialValueWithBidType(model);

    // Step 4: Validate dates
    var dateValidation = BidValidationHelper.ValidateBidDates(model, null, settings, checkLastReceivingEnqiryDate);
    if (!dateValidation.IsSucceeded)
        return dateValidation;

    // All validations passed - create bid
    // ... rest of creation logic ...
}
```

### Pattern 2: Bid Update
```csharp
public async Task<OperationResult<AddBidResponse>> UpdateBid(AddBidModelNew model)
{
    // Get existing bid
    var existingBid = await _bidRepository.FindOneAsync(x => x.Id == model.Id);

    // Step 1: Adjust dates
    var adjustResult = BidValidationHelper.AdjustRequestBidAddressesToTheEndOfTheDay(model);
    if (!adjustResult.IsSucceeded)
        return OperationResult<AddBidResponse>.Fail(adjustResult.HttpErrorCode, adjustResult.Code);

    // Step 2: Validate dates (with existing bid)
    var dateValidation = BidValidationHelper.ValidateBidDates(
        model,
        existingBid,  // Pass existing bid for enquiry date check
        settings,
        BidValidationHelper.CheckLastReceivingEnqiryDate
    );
    if (!dateValidation.IsSucceeded)
        return dateValidation;

    // All validations passed - update bid
    // ... rest of update logic ...
}
```

### Pattern 3: Bid Approval
```csharp
public async Task<OperationResult<bool>> ApproveBid(long bidId)
{
    var bid = await _bidRepository.FindOneAsync(x => x.Id == bidId);
    var settings = await _appGeneralSettingService.GetAppGeneralSettings();

    // Validate dates before approving
    var dateValidation = BidValidationHelper.ValidateBidDatesWhileApproving(
        bid,
        settings.Data
    );

    if (!dateValidation.IsSucceeded)
        return OperationResult<bool>.Fail(dateValidation.HttpErrorCode, dateValidation.Code);

    // Dates are valid - approve bid
    // ... approval logic ...
}
```

---

## Error Codes Reference

| Error Code | Method | Meaning |
|------------|--------|---------|
| `INVALID_INPUT` | `IsRequiredDataForNotSaveAsDraftAdded` | Required fields missing for non-draft bid |
| `LAST_DATE_IN_RECEIVING_ENQUIRIES_MUST_NOT_BE_BEFORE_TODAY_DATE` | `ValidateBidDates` | Enquiry date in past |
| `LAST_DATE_IN_OFFERS_SUBMISSION_MUST_BE_GREATER_THAN_LAST_DATE_IN_RECEIVING_ENQUIRIES` | `ValidateBidDates` | Submission before enquiry |
| `OFFERS_OPENING_DATE_MUST_BE_GREATER_THAN_LAST_DATE_IN_OFFERS_SUBMISSION` | `ValidateBidDates` | Opening before submission |
| `EXPECTED_ANCHORING_DATE_MUST_BE_GREATER_THAN_OFFERS_OPENING_DATE_PLUS_STOPPING_PERIOD` | `ValidateBidDates` | Anchoring too soon |

---

## Summary

**BidValidationHelper** centralizes all bid validation logic:
- ✅ **6 methods** for comprehensive validation
- ✅ **Zero dependencies** - pure static helper
- ✅ **Reduces BidServiceCore** by ~70 lines
- ✅ **Reusable** across all services
- ✅ **Easy to test** independently

**Use this helper whenever you need to validate bid data!**
