# BidUtilityHelper - Complete Documentation

## 📋 Overview

**File Location:** `/Helpers/BidUtilityHelper.cs`
**Class Type:** `public static class`
**Namespace:** `Nafis.Services.Implementation.Helpers`
**Lines of Code:** 144 lines
**Methods:** 12 utility methods

---

## 🎯 Class Purpose

### What It Does
BidUtilityHelper provides utility methods for common bid operations including:
- Site map updates
- Bid status checks
- Bid visibility/type checks
- Display name formatting
- Attachment validation
- Reference number formatting

### When to Use
Use BidUtilityHelper when you need to:
- Check bid status (draft, published, closed)
- Check bid visibility (public, private, habilitation)
- Validate invitation attachments
- Format bid reference numbers
- Get localized display names
- Update site map modification dates

### When NOT to Use
Do NOT use BidUtilityHelper for:
- Complex validation logic (use BidValidationHelper)
- Financial calculations (use BidCalculationHelper)
- Query building (use BidQueryHelper)
- Database operations (keep in BidServiceCore)

---

## 📚 Methods Documentation

### 1. UpdateSiteMapLastModificationDateIfSpecificDataChanged

#### Purpose
Updates the site map's last modification date if key bid information has changed. This is used for SEO and search engine crawling optimization.

#### Method Signature
```csharp
public static void UpdateSiteMapLastModificationDateIfSpecificDataChanged(
    Bid bid,
    AddBidModelNew requestModel)
```

#### Parameters

| Parameter | Type | Description | Required |
|-----------|------|-------------|----------|
| bid | Bid | The existing bid entity from database | Yes |
| requestModel | AddBidModelNew | The incoming request model with updated data | Yes |

#### Return Value
- **Type:** `void`
- **Description:** No return value. Updates bid entity directly if changes detected.

#### What It Does
1. Validates both parameters are not null
2. Compares 5 key fields between existing bid and request:
   - BidName
   - BidDescription
   - LastDateInReceivingEnquiries
   - LastDateInOffersSubmission
   - OffersOpeningDate
3. If ANY field changed, updates `bid.SiteMapLastModificationDate` to current UTC time
4. If no changes, does nothing (preserves existing date)

#### When to Use
- Before saving bid updates to database
- When you need to track SEO-relevant changes
- When bid information changes that affect public listings
- During bid update operations

#### Where to Replace in BidServiceCore
**Original Location:** Lines 3450-3465 in BidServiceCore.cs

**Before:**
```csharp
private void UpdateSiteMapLastModificationDateIfSpecificDataChanged(Bid bid, AddBidModelNew requestModel)
{
    if (bid is null || requestModel is null)
        return;

    bool hasChanged = bid.BidName != requestModel.BidName ||
                     bid.BidDescription != requestModel.BidDescription ||
                     bid.LastDateInReceivingEnquiries != requestModel.LastDateInReceivingEnquiries ||
                     bid.LastDateInOffersSubmission != requestModel.LastDateInOffersSubmission ||
                     bid.OffersOpeningDate != requestModel.OffersOpeningDate;

    if (hasChanged)
    {
        bid.SiteMapLastModificationDate = DateTime.UtcNow;
    }
}
```

**After:**
```csharp
// ✅ Using BidUtilityHelper instead of private method
BidUtilityHelper.UpdateSiteMapLastModificationDateIfSpecificDataChanged(bid, requestModel);
```

#### Usage Example
```csharp
// Before updating bid in database
var existingBid = await _bidRepository.GetByIdAsync(bidId);
var updateModel = new AddBidModelNew
{
    BidName = "Updated Bid Name",
    BidDescription = "New description",
    // ... other fields
};

// Update site map date if SEO-relevant fields changed
BidUtilityHelper.UpdateSiteMapLastModificationDateIfSpecificDataChanged(existingBid, updateModel);

// Now save to database
await _bidRepository.UpdateAsync(existingBid);
```

#### Real-World Scenarios

**Scenario 1: Bid Name Changed**
```csharp
var bid = new Bid
{
    BidName = "Original Name",
    SiteMapLastModificationDate = new DateTime(2024, 1, 1)
};

var model = new AddBidModelNew
{
    BidName = "New Name",
    BidDescription = bid.BidDescription // Same
};

BidUtilityHelper.UpdateSiteMapLastModificationDateIfSpecificDataChanged(bid, model);

// Result: bid.SiteMapLastModificationDate = DateTime.UtcNow (today)
```

**Scenario 2: No Changes**
```csharp
var bid = new Bid
{
    BidName = "Same Name",
    BidDescription = "Same Description",
    SiteMapLastModificationDate = new DateTime(2024, 1, 1)
};

var model = new AddBidModelNew
{
    BidName = "Same Name",
    BidDescription = "Same Description"
};

BidUtilityHelper.UpdateSiteMapLastModificationDateIfSpecificDataChanged(bid, model);

// Result: bid.SiteMapLastModificationDate = 2024-01-01 (unchanged)
```

**Scenario 3: Date Changed**
```csharp
var bid = new Bid
{
    LastDateInOffersSubmission = new DateTime(2024, 12, 31),
    SiteMapLastModificationDate = new DateTime(2024, 1, 1)
};

var model = new AddBidModelNew
{
    LastDateInOffersSubmission = new DateTime(2025, 1, 15) // Extended deadline
};

BidUtilityHelper.UpdateSiteMapLastModificationDateIfSpecificDataChanged(bid, model);

// Result: bid.SiteMapLastModificationDate = DateTime.UtcNow (updated)
```

---

### 2. ValidateBidInvitationAttachmentsNew

#### Purpose
Validates that required invitation attachments are present for habilitation bids when attachments are marked as required.

#### Method Signature
```csharp
public static bool ValidateBidInvitationAttachmentsNew(AddBidModelNew model)
```

#### Parameters

| Parameter | Type | Description | Required |
|-----------|------|-------------|----------|
| model | AddBidModelNew | The bid request model to validate | Yes |

#### Return Value
- **Type:** `bool`
- **Returns:** `true` if validation FAILS (attachments missing), `false` if valid

#### What It Does
1. Checks if bid is habilitation type (`BidVisibility == BidTypes.Habilitation`)
2. Checks if invitation attachments are marked as required (`IsInvitationNeedAttachments == true`)
3. Checks if attachments collection is null or empty
4. Returns `true` if all conditions met (validation failed - attachments required but missing)

#### When to Use
- Before creating or updating habilitation bids
- When validating bid submission forms
- During bid draft to published transition
- To ensure habilitation bids have required invitation documents

#### Where to Replace in BidServiceCore
**Original Location:** Lines 3467-3472 in BidServiceCore.cs

**Before:**
```csharp
private static bool ValidateBidInvitationAttachmentsNew(AddBidModelNew model)
{
    return model.BidVisibility == BidTypes.Habilitation &&
        (model.IsInvitationNeedAttachments.HasValue ? model.IsInvitationNeedAttachments.Value : false)
        && (model.BidInvitationsAttachments is null || model.BidInvitationsAttachments.Count == 0);
}
```

**After:**
```csharp
// ✅ Using BidUtilityHelper instead of private method
if (BidUtilityHelper.ValidateBidInvitationAttachmentsNew(model))
{
    return OperationResult<AddBidResponse>.Fail(
        HttpErrorCode.InvalidInput,
        BidErrorCodes.ADDING_INVITATION_ATTACHMENTS_REQUIRED);
}
```

#### Usage Example
```csharp
// Validate invitation attachments before creating bid
var model = new AddBidModelNew
{
    BidVisibility = BidTypes.Habilitation,
    IsInvitationNeedAttachments = true,
    BidInvitationsAttachments = null // Or empty list
};

if (BidUtilityHelper.ValidateBidInvitationAttachmentsNew(model))
{
    // Validation failed - attachments required but missing
    return OperationResult<AddBidResponse>.Fail(
        HttpErrorCode.InvalidInput,
        BidErrorCodes.ADDING_INVITATION_ATTACHMENTS_REQUIRED);
}

// Continue with bid creation...
```

#### Real-World Scenarios

**Scenario 1: Habilitation Bid Missing Required Attachments (INVALID)**
```csharp
var model = new AddBidModelNew
{
    BidVisibility = BidTypes.Habilitation,
    IsInvitationNeedAttachments = true,
    BidInvitationsAttachments = null
};

bool validationFailed = BidUtilityHelper.ValidateBidInvitationAttachmentsNew(model);
// Result: true (validation FAILED - attachments required but missing)
```

**Scenario 2: Habilitation Bid with Attachments (VALID)**
```csharp
var model = new AddBidModelNew
{
    BidVisibility = BidTypes.Habilitation,
    IsInvitationNeedAttachments = true,
    BidInvitationsAttachments = new List<BidInvitationAttachment>
    {
        new BidInvitationAttachment { FileName = "invite.pdf" }
    }
};

bool validationFailed = BidUtilityHelper.ValidateBidInvitationAttachmentsNew(model);
// Result: false (validation passed - attachments present)
```

**Scenario 3: Public Bid (No Validation Needed)**
```csharp
var model = new AddBidModelNew
{
    BidVisibility = BidTypes.Public, // Not habilitation
    IsInvitationNeedAttachments = true,
    BidInvitationsAttachments = null
};

bool validationFailed = BidUtilityHelper.ValidateBidInvitationAttachmentsNew(model);
// Result: false (no validation - not habilitation type)
```

**Scenario 4: Habilitation Bid Without Attachment Requirement**
```csharp
var model = new AddBidModelNew
{
    BidVisibility = BidTypes.Habilitation,
    IsInvitationNeedAttachments = false, // Attachments not required
    BidInvitationsAttachments = null
};

bool validationFailed = BidUtilityHelper.ValidateBidInvitationAttachmentsNew(model);
// Result: false (no validation - attachments not required)
```

---

### 3. CheckIfWeNeedAddAttachmentNew

#### Purpose
Determines if invitation attachments need to be added to the database for a habilitation bid.

#### Method Signature
```csharp
public static bool CheckIfWeNeedAddAttachmentNew(AddBidModelNew model)
```

#### Parameters

| Parameter | Type | Description | Required |
|-----------|------|-------------|----------|
| model | AddBidModelNew | The bid request model to check | Yes |

#### Return Value
- **Type:** `bool`
- **Returns:** `true` if attachments should be added, `false` otherwise

#### What It Does
1. Checks if bid is habilitation type
2. Verifies `IsInvitationNeedAttachments` flag is set and true
3. Checks if attachments collection exists and has items
4. Returns `true` only if all conditions met (ready to add attachments)

#### When to Use
- Before inserting invitation attachments to database
- During bid creation process
- To avoid unnecessary database operations
- When processing habilitation bid submissions

#### Where to Replace in BidServiceCore
**Original Location:** Lines 3474-3480 in BidServiceCore.cs

**Before:**
```csharp
private static bool CheckIfWeNeedAddAttachmentNew(AddBidModelNew model)
{
    return model.BidVisibility == BidTypes.Habilitation &&
           model.IsInvitationNeedAttachments.HasValue &&
           model.IsInvitationNeedAttachments.Value &&
           model.BidInvitationsAttachments != null &&
           model.BidInvitationsAttachments.Any();
}
```

**After:**
```csharp
// ✅ Using BidUtilityHelper instead of private method
if (BidUtilityHelper.CheckIfWeNeedAddAttachmentNew(model))
{
    await _bidInvitationAttachmentRepository.AddRangeAsync(model.BidInvitationsAttachments);
}
```

#### Usage Example
```csharp
// After creating bid, check if we need to add attachments
var bid = await _bidRepository.AddAsync(bidEntity);

if (BidUtilityHelper.CheckIfWeNeedAddAttachmentNew(model))
{
    // Map bid ID to attachments
    foreach (var attachment in model.BidInvitationsAttachments)
    {
        attachment.BidId = bid.Id;
    }

    // Add to database
    await _bidInvitationAttachmentRepository.AddRangeAsync(
        model.BidInvitationsAttachments);
}
```

#### Real-World Scenarios

**Scenario 1: Habilitation Bid with Attachments to Add**
```csharp
var model = new AddBidModelNew
{
    BidVisibility = BidTypes.Habilitation,
    IsInvitationNeedAttachments = true,
    BidInvitationsAttachments = new List<BidInvitationAttachment>
    {
        new BidInvitationAttachment { FileName = "requirements.pdf" },
        new BidInvitationAttachment { FileName = "invitation.docx" }
    }
};

bool needsAdd = BidUtilityHelper.CheckIfWeNeedAddAttachmentNew(model);
// Result: true (should add attachments to database)
```

**Scenario 2: Public Bid (No Attachments Needed)**
```csharp
var model = new AddBidModelNew
{
    BidVisibility = BidTypes.Public,
    IsInvitationNeedAttachments = true,
    BidInvitationsAttachments = new List<BidInvitationAttachment> { ... }
};

bool needsAdd = BidUtilityHelper.CheckIfWeNeedAddAttachmentNew(model);
// Result: false (public bids don't use invitation attachments)
```

**Scenario 3: Habilitation Bid with Empty Attachments**
```csharp
var model = new AddBidModelNew
{
    BidVisibility = BidTypes.Habilitation,
    IsInvitationNeedAttachments = true,
    BidInvitationsAttachments = new List<BidInvitationAttachment>() // Empty
};

bool needsAdd = BidUtilityHelper.CheckIfWeNeedAddAttachmentNew(model);
// Result: false (no attachments to add)
```

---

### 4. FormatBidRefNumber

#### Purpose
Formats a complete bid reference number by combining the prefix and random parts.

#### Method Signature
```csharp
public static string FormatBidRefNumber(string firstPart, string randomPart)
```

#### Parameters

| Parameter | Type | Description | Required |
|-----------|------|-------------|----------|
| firstPart | string | The prefix/first part of reference number | Yes |
| randomPart | string | The random/unique part of reference number | Yes |

#### Return Value
- **Type:** `string`
- **Returns:** Formatted reference number as `{firstPart}{randomPart}`

#### When to Use
- When generating new bid reference numbers
- During bid creation
- When formatting reference numbers for display

#### Usage Example
```csharp
string prefix = "BID-2024-";
string random = "A7X2K9";

string refNumber = BidUtilityHelper.FormatBidRefNumber(prefix, random);
// Result: "BID-2024-A7X2K9"
```

#### Real-World Scenarios

**Scenario 1: Standard Reference Number**
```csharp
string formatted = BidUtilityHelper.FormatBidRefNumber("TND", "123456");
// Result: "TND123456"
```

**Scenario 2: Year-Based Reference**
```csharp
string year = DateTime.Now.Year.ToString();
string random = Guid.NewGuid().ToString("N").Substring(0, 8).ToUpper();

string formatted = BidUtilityHelper.FormatBidRefNumber($"BID{year}-", random);
// Result: "BID2024-A1B2C3D4"
```

---

### 5. IsBidDraft

#### Purpose
Checks if a bid is in draft status.

#### Method Signature
```csharp
public static bool IsBidDraft(Bid bid)
```

#### Parameters

| Parameter | Type | Description | Required |
|-----------|------|-------------|----------|
| bid | Bid | The bid entity to check | Yes |

#### Return Value
- **Type:** `bool`
- **Returns:** `true` if bid is draft, `false` otherwise

#### When to Use
- Before allowing bid edits
- To show/hide draft-specific UI elements
- When determining allowed operations on a bid
- During bid status transitions

#### Usage Example
```csharp
var bid = await _bidRepository.GetByIdAsync(bidId);

if (BidUtilityHelper.IsBidDraft(bid))
{
    // Allow full editing
    await UpdateBidAsync(bid, model);
}
else
{
    // Restrict editing - bid is published or closed
    return OperationResult.Fail(HttpErrorCode.Conflict, "Cannot edit published bid");
}
```

#### Real-World Scenarios

**Scenario 1: Draft Bid (Editable)**
```csharp
var bid = new Bid { TenderStatusId = (int)TenderStatus.Draft };

bool isDraft = BidUtilityHelper.IsBidDraft(bid);
// Result: true (can be edited)
```

**Scenario 2: Published Bid (Not Editable)**
```csharp
var bid = new Bid { TenderStatusId = (int)TenderStatus.Published };

bool isDraft = BidUtilityHelper.IsBidDraft(bid);
// Result: false (restricted editing)
```

---

### 6. IsBidPublished

#### Purpose
Checks if a bid is published and visible to users.

#### Method Signature
```csharp
public static bool IsBidPublished(Bid bid)
```

#### Parameters

| Parameter | Type | Description | Required |
|-----------|------|-------------|----------|
| bid | Bid | The bid entity to check | Yes |

#### Return Value
- **Type:** `bool`
- **Returns:** `true` if bid is published, `false` otherwise

#### When to Use
- To determine if bid is publicly visible
- Before allowing bid applications
- When checking if notifications should be sent
- During search/listing operations

#### Usage Example
```csharp
var bid = await _bidRepository.GetByIdAsync(bidId);

if (BidUtilityHelper.IsBidPublished(bid))
{
    // Allow suppliers to submit offers
    await SubmitOfferAsync(bid, offer);
}
else
{
    return OperationResult.Fail(HttpErrorCode.NotFound, "Bid not available");
}
```

---

### 7. IsBidClosed

#### Purpose
Checks if a bid is closed and no longer accepting submissions.

#### Method Signature
```csharp
public static bool IsBidClosed(Bid bid)
```

#### Parameters

| Parameter | Type | Description | Required |
|-----------|------|-------------|----------|
| bid | Bid | The bid entity to check | Yes |

#### Return Value
- **Type:** `bool`
- **Returns:** `true` if bid is closed, `false` otherwise

#### When to Use
- Before accepting new submissions
- When determining if evaluation can begin
- During bid lifecycle checks
- To show appropriate messages to users

#### Usage Example
```csharp
var bid = await _bidRepository.GetByIdAsync(bidId);

if (BidUtilityHelper.IsBidClosed(bid))
{
    // Start evaluation process
    await BeginEvaluationAsync(bid);
}
else
{
    return OperationResult.Fail("Bid still accepting submissions");
}
```

---

### 8. IsPrivateBid

#### Purpose
Checks if a bid is private (invitation-only).

#### Method Signature
```csharp
public static bool IsPrivateBid(Bid bid)
```

#### Parameters

| Parameter | Type | Description | Required |
|-----------|------|-------------|----------|
| bid | Bid | The bid entity to check | Yes |

#### Return Value
- **Type:** `bool`
- **Returns:** `true` if bid is private, `false` otherwise

#### When to Use
- To enforce access control
- Before showing bid in public listings
- When determining who can view/submit
- During authorization checks

#### Usage Example
```csharp
var bid = await _bidRepository.GetByIdAsync(bidId);

if (BidUtilityHelper.IsPrivateBid(bid))
{
    // Check if user is invited
    bool isInvited = await _invitationRepository.IsUserInvitedAsync(bid.Id, userId);
    if (!isInvited)
    {
        return OperationResult.Fail(HttpErrorCode.Forbidden, "Not invited to this bid");
    }
}
```

---

### 9. IsPublicBid

#### Purpose
Checks if a bid is public (open to all suppliers).

#### Method Signature
```csharp
public static bool IsPublicBid(Bid bid)
```

#### Parameters

| Parameter | Type | Description | Required |
|-----------|------|-------------|----------|
| bid | Bid | The bid entity to check | Yes |

#### Return Value
- **Type:** `bool`
- **Returns:** `true` if bid is public, `false` otherwise

#### When to Use
- To include in public search results
- When no authorization is required
- During bid discovery features
- To skip invitation checks

#### Usage Example
```csharp
var bid = await _bidRepository.GetByIdAsync(bidId);

if (BidUtilityHelper.IsPublicBid(bid))
{
    // Allow any registered supplier to view and submit
    return await GetBidDetailsAsync(bid);
}
```

---

### 10. IsHabilitationBid

#### Purpose
Checks if a bid is a habilitation/qualification bid.

#### Method Signature
```csharp
public static bool IsHabilitationBid(Bid bid)
```

#### Parameters

| Parameter | Type | Description | Required |
|-----------|------|-------------|----------|
| bid | Bid | The bid entity to check | Yes |

#### Return Value
- **Type:** `bool`
- **Returns:** `true` if bid is habilitation, `false` otherwise

#### When to Use
- To determine if invitation attachments are needed
- When applying habilitation-specific rules
- During qualification process workflows
- To show habilitation-specific UI elements

#### Usage Example
```csharp
var bid = await _bidRepository.GetByIdAsync(bidId);

if (BidUtilityHelper.IsHabilitationBid(bid))
{
    // Require additional qualification documents
    await ValidateQualificationDocumentsAsync(bid, submission);
}
```

---

### 11. GetBidTypeDisplayName

#### Purpose
Returns the Arabic display name for a bid visibility type.

#### Method Signature
```csharp
public static string GetBidTypeDisplayName(BidTypes bidType)
```

#### Parameters

| Parameter | Type | Description | Required |
|-----------|------|-------------|----------|
| bidType | BidTypes | The bid type enum value | Yes |

#### Return Value
- **Type:** `string`
- **Returns:** Arabic display name for the bid type

| Input | Output |
|-------|--------|
| BidTypes.Public | "عامة" (Public) |
| BidTypes.Private | "خاصة" (Private) |
| BidTypes.Habilitation | "تأهيل" (Habilitation) |
| Other | "غير محدد" (Unspecified) |

#### When to Use
- When displaying bid type to users
- In reports and exports
- For UI labels
- In notification messages

#### Usage Example
```csharp
var bid = new Bid { BidVisibility = BidTypes.Public };

string displayName = BidUtilityHelper.GetBidTypeDisplayName(bid.BidVisibility);
Console.WriteLine($"نوع المنافسة: {displayName}");
// Output: نوع المنافسة: عامة
```

#### Real-World Scenarios

**Scenario 1: Display in List**
```csharp
var bids = await _bidRepository.GetAllAsync();

foreach (var bid in bids)
{
    string typeName = BidUtilityHelper.GetBidTypeDisplayName(bid.BidVisibility);
    Console.WriteLine($"{bid.BidName} - {typeName}");
}
// Output:
// "Construction Project - عامة"
// "IT Services - خاصة"
// "Supplier Qualification - تأهيل"
```

---

### 12. GetBidStatusDisplayName

#### Purpose
Returns the Arabic display name for a bid status.

#### Method Signature
```csharp
public static string GetBidStatusDisplayName(TenderStatus status)
```

#### Parameters

| Parameter | Type | Description | Required |
|-----------|------|-------------|----------|
| status | TenderStatus | The bid status enum value | Yes |

#### Return Value
- **Type:** `string`
- **Returns:** Arabic display name for the status

| Input | Output |
|-------|--------|
| TenderStatus.Draft | "مسودة" (Draft) |
| TenderStatus.PendingApproval | "في انتظار الموافقة" (Pending Approval) |
| TenderStatus.Published | "منشورة" (Published) |
| TenderStatus.Closed | "مغلقة" (Closed) |
| TenderStatus.Cancelled | "ملغاة" (Cancelled) |
| Other | "غير محدد" (Unspecified) |

#### When to Use
- When displaying status to users
- In status badges/chips
- For audit logs
- In notification messages

#### Usage Example
```csharp
var bid = new Bid { TenderStatusId = (int)TenderStatus.Published };
var status = (TenderStatus)bid.TenderStatusId;

string displayName = BidUtilityHelper.GetBidStatusDisplayName(status);
Console.WriteLine($"الحالة: {displayName}");
// Output: الحالة: منشورة
```

#### Real-World Scenarios

**Scenario 1: Status Timeline Display**
```csharp
var statusHistory = new[]
{
    TenderStatus.Draft,
    TenderStatus.PendingApproval,
    TenderStatus.Published
};

foreach (var status in statusHistory)
{
    string displayName = BidUtilityHelper.GetBidStatusDisplayName(status);
    Console.WriteLine($"- {displayName}");
}
// Output:
// - مسودة
// - في انتظار الموافقة
// - منشورة
```

---

## 📊 Quick Reference Table

| Method | Purpose | Returns | Common Use |
|--------|---------|---------|------------|
| UpdateSiteMapLastModificationDateIfSpecificDataChanged | Update SEO timestamp | void | Before saving bid updates |
| ValidateBidInvitationAttachmentsNew | Check if attachments missing | bool | Habilitation bid validation |
| CheckIfWeNeedAddAttachmentNew | Check if should add attachments | bool | Before inserting attachments |
| FormatBidRefNumber | Format reference number | string | Bid creation |
| IsBidDraft | Check if draft | bool | Authorization checks |
| IsBidPublished | Check if published | bool | Display/submission logic |
| IsBidClosed | Check if closed | bool | Submission cutoff checks |
| IsPrivateBid | Check if private | bool | Access control |
| IsPublicBid | Check if public | bool | Public listing inclusion |
| IsHabilitationBid | Check if habilitation | bool | Qualification workflows |
| GetBidTypeDisplayName | Get type display name | string | UI display |
| GetBidStatusDisplayName | Get status display name | string | UI display |

---

## 🔄 Common Usage Patterns

### Pattern 1: Bid Type Checks with Actions
```csharp
var bid = await _bidRepository.GetByIdAsync(bidId);

if (BidUtilityHelper.IsPublicBid(bid))
{
    // No invitation needed
    return await ProcessPublicBidAsync(bid);
}
else if (BidUtilityHelper.IsPrivateBid(bid))
{
    // Check invitation
    await ValidateInvitationAsync(bid, userId);
    return await ProcessPrivateBidAsync(bid);
}
else if (BidUtilityHelper.IsHabilitationBid(bid))
{
    // Require qualification documents
    await ValidateQualificationAsync(bid, userId);
    return await ProcessHabilitationBidAsync(bid);
}
```

### Pattern 2: Status-Based Workflow
```csharp
var bid = await _bidRepository.GetByIdAsync(bidId);

if (BidUtilityHelper.IsBidDraft(bid))
{
    // Full editing allowed
    await UpdateBidAsync(bid, model);
}
else if (BidUtilityHelper.IsBidPublished(bid))
{
    // Restricted editing
    await UpdatePublishedBidAsync(bid, limitedModel);
}
else if (BidUtilityHelper.IsBidClosed(bid))
{
    // No editing, only viewing
    return OperationResult.Fail("Bid is closed");
}
```

### Pattern 3: Complete Bid Creation Flow
```csharp
// Create bid entity
var bid = MapToBidEntity(model);
bid.BidRefNumber = BidUtilityHelper.FormatBidRefNumber("TND", randomCode);
await _bidRepository.AddAsync(bid);

// Update site map
BidUtilityHelper.UpdateSiteMapLastModificationDateIfSpecificDataChanged(bid, model);

// Handle habilitation attachments
if (BidUtilityHelper.IsHabilitationBid(bid))
{
    // Validate attachments
    if (BidUtilityHelper.ValidateBidInvitationAttachmentsNew(model))
    {
        return OperationResult.Fail(BidErrorCodes.ADDING_INVITATION_ATTACHMENTS_REQUIRED);
    }

    // Add attachments if present
    if (BidUtilityHelper.CheckIfWeNeedAddAttachmentNew(model))
    {
        foreach (var attachment in model.BidInvitationsAttachments)
        {
            attachment.BidId = bid.Id;
        }
        await _attachmentRepository.AddRangeAsync(model.BidInvitationsAttachments);
    }
}

await _unitOfWork.CommitAsync();
```

### Pattern 4: Localized Display
```csharp
var bid = await _bidRepository.GetByIdAsync(bidId);

var bidDto = new BidDisplayDto
{
    Id = bid.Id,
    Name = bid.BidName,
    TypeName = BidUtilityHelper.GetBidTypeDisplayName(bid.BidVisibility),
    StatusName = BidUtilityHelper.GetBidStatusDisplayName((TenderStatus)bid.TenderStatusId),
    RefNumber = bid.BidRefNumber
};

// Display to user in Arabic
Console.WriteLine($"{bidDto.Name}");
Console.WriteLine($"النوع: {bidDto.TypeName}");
Console.WriteLine($"الحالة: {bidDto.StatusName}");
Console.WriteLine($"الرقم المرجعي: {bidDto.RefNumber}");
```

---

## 💡 Best Practices

### ✅ DO
- Use helper methods for all status/type checks
- Always validate habilitation attachments before saving
- Update site map dates for SEO-relevant changes
- Use display name methods for user-facing text
- Check bid status before allowing operations

### ❌ DON'T
- Don't hardcode status IDs - use helper methods
- Don't skip habilitation attachment validation
- Don't compare enum values directly - use helper methods
- Don't format reference numbers manually
- Don't hardcode Arabic text - use GetBidTypeDisplayName/GetBidStatusDisplayName

---

## 🎯 Lines Saved in BidServiceCore

By using BidUtilityHelper instead of private methods:
- **~40 lines** of code removed from BidServiceCore
- **12 methods** extracted to helper
- **Improved testability** - each method independently testable
- **Better organization** - utility logic separated from business logic

---

## 📝 Summary

BidUtilityHelper provides 12 essential utility methods for:
- **Status checks:** IsBidDraft, IsBidPublished, IsBidClosed
- **Type checks:** IsPublicBid, IsPrivateBid, IsHabilitationBid
- **Attachment validation:** ValidateBidInvitationAttachmentsNew, CheckIfWeNeedAddAttachmentNew
- **Formatting:** FormatBidRefNumber, GetBidTypeDisplayName, GetBidStatusDisplayName
- **SEO:** UpdateSiteMapLastModificationDateIfSpecificDataChanged

All methods are static, require no dependencies, and are easily testable!
