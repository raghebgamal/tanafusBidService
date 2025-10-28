# BidQueryHelper - Complete Documentation

## 📋 Overview

**File Location:** `/Helpers/BidQueryHelper.cs`
**Class Type:** `public static class`
**Namespace:** `Nafis.Services.Implementation.Helpers`
**Lines of Code:** 180 lines
**Methods:** 14 query builder methods

---

## 🎯 Class Purpose

### What It Does
BidQueryHelper provides reusable query filters and expression builders for common bid queries including:
- Status-based filters (published, draft, active, expired)
- Visibility-based filters (public, private)
- Association and region filters
- Date range filters
- Industry filters
- Expression composition (AND logic)
- Query ordering helpers

### When to Use
Use BidQueryHelper when you need to:
- Build complex queries with multiple filters
- Filter bids by status, visibility, date range
- Combine multiple filters dynamically
- Apply consistent ordering to bid queries
- Create reusable query expressions
- Build search and filter features

### When NOT to Use
Do NOT use BidQueryHelper for:
- Simple single-property filters (write inline)
- Complex business validation (use BidValidationHelper)
- Financial calculations (use BidCalculationHelper)
- Utility checks (use BidUtilityHelper)
- Direct database operations (keep in repository/service)

---

## 📚 Methods Documentation

### 1. GetPublishedBidsFilter

#### Purpose
Creates a filter expression to get only published bids.

#### Method Signature
```csharp
public static Expression<Func<Bid, bool>> GetPublishedBidsFilter()
```

#### Parameters
None

#### Return Value
- **Type:** `Expression<Func<Bid, bool>>`
- **Returns:** Lambda expression filtering for published bids
- **Expression:** `bid => bid.TenderStatusId == (int)TenderStatus.Published`

#### When to Use
- Public bid listing pages
- Search results
- Active bid displays
- Supplier dashboards

#### Where to Use in BidServiceCore
**Common Pattern:**
```csharp
// Before:
var publishedBids = await _bidRepository.GetAsync(
    filter: bid => bid.TenderStatusId == (int)TenderStatus.Published);

// After:
var publishedBids = await _bidRepository.GetAsync(
    filter: BidQueryHelper.GetPublishedBidsFilter());
```

#### Usage Example
```csharp
// Get all published bids
var filter = BidQueryHelper.GetPublishedBidsFilter();
var publishedBids = await _bidRepository.GetAsync(filter);

// Use with additional filters
var publishedAndPublic = BidQueryHelper.CombineFiltersWithAnd(
    BidQueryHelper.GetPublishedBidsFilter(),
    BidQueryHelper.GetPublicBidsFilter()
);
```

#### Real-World Scenarios

**Scenario 1: Public Listing Page**
```csharp
// Show only published bids to public
var filter = BidQueryHelper.GetPublishedBidsFilter();
var bids = await _bidRepository.GetAsync(
    filter: filter,
    orderBy: BidQueryHelper.GetDefaultBidOrdering(),
    pageNumber: 1,
    pageSize: 20
);

// Result: Only bids with TenderStatusId = Published (e.g., 3)
```

**Scenario 2: Supplier Dashboard Active Opportunities**
```csharp
var activeFilter = BidQueryHelper.CombineFiltersWithAnd(
    BidQueryHelper.GetPublishedBidsFilter(),
    BidQueryHelper.GetActiveBidsFilter(DateTime.UtcNow)
);

var activeBids = await _bidRepository.GetAsync(filter: activeFilter);
// Result: Published bids that haven't expired yet
```

---

### 2. GetDraftBidsFilter

#### Purpose
Creates a filter expression to get only draft bids.

#### Method Signature
```csharp
public static Expression<Func<Bid, bool>> GetDraftBidsFilter()
```

#### Parameters
None

#### Return Value
- **Type:** `Expression<Func<Bid, bool>>`
- **Returns:** Lambda expression filtering for draft bids
- **Expression:** `bid => bid.TenderStatusId == (int)TenderStatus.Draft`

#### When to Use
- Association admin dashboard
- Draft bid management
- Unpublished bid lists
- Edit workflow features

#### Usage Example
```csharp
// Get all draft bids for an association
var draftFilter = BidQueryHelper.CombineFiltersWithAnd(
    BidQueryHelper.GetDraftBidsFilter(),
    BidQueryHelper.GetBidsByAssociationFilter(associationId)
);

var drafts = await _bidRepository.GetAsync(filter: draftFilter);
```

#### Real-World Scenarios

**Scenario 1: Association Admin Dashboard**
```csharp
long associationId = 123;

var draftBidsFilter = BidQueryHelper.CombineFiltersWithAnd(
    BidQueryHelper.GetDraftBidsFilter(),
    BidQueryHelper.GetBidsByAssociationFilter(associationId)
);

var drafts = await _bidRepository.GetAsync(filter: draftBidsFilter);
// Result: All draft bids for association 123
```

**Scenario 2: Incomplete Bids Notification**
```csharp
var oldDrafts = BidQueryHelper.CombineFiltersWithAnd(
    BidQueryHelper.GetDraftBidsFilter(),
    BidQueryHelper.GetBidsByDateRangeFilter(
        DateTime.UtcNow.AddDays(-30),
        DateTime.UtcNow.AddDays(-7)
    )
);

var oldDraftBids = await _bidRepository.GetAsync(filter: oldDrafts);
// Result: Drafts created 7-30 days ago (reminder candidates)
```

---

### 3. GetBidsByAssociationFilter

#### Purpose
Creates a filter expression to get bids for a specific association.

#### Method Signature
```csharp
public static Expression<Func<Bid, bool>> GetBidsByAssociationFilter(long associationId)
```

#### Parameters

| Parameter | Type | Description | Required |
|-----------|------|-------------|----------|
| associationId | long | The ID of the association | Yes |

#### Return Value
- **Type:** `Expression<Func<Bid, bool>>`
- **Returns:** Lambda expression filtering by association ID
- **Expression:** `bid => bid.AssociationId == associationId`

#### When to Use
- Association-specific bid listings
- Association admin panels
- Association statistics
- Multi-tenant data isolation

#### Usage Example
```csharp
long associationId = 456;
var filter = BidQueryHelper.GetBidsByAssociationFilter(associationId);
var associationBids = await _bidRepository.GetAsync(filter);
```

#### Real-World Scenarios

**Scenario 1: Association Admin View All Their Bids**
```csharp
long associationId = 789;

var filter = BidQueryHelper.GetBidsByAssociationFilter(associationId);
var allBids = await _bidRepository.GetAsync(
    filter: filter,
    orderBy: BidQueryHelper.GetDefaultBidOrdering()
);

// Result: All bids (draft, published, closed) for association 789
```

**Scenario 2: Association Published Bids Only**
```csharp
long associationId = 789;

var publishedFilter = BidQueryHelper.CombineFiltersWithAnd(
    BidQueryHelper.GetBidsByAssociationFilter(associationId),
    BidQueryHelper.GetPublishedBidsFilter()
);

var publishedBids = await _bidRepository.GetAsync(filter: publishedFilter);
// Result: Only published bids for association 789
```

---

### 4. GetBidsByRegionFilter

#### Purpose
Creates a filter expression to get bids for a specific region.

#### Method Signature
```csharp
public static Expression<Func<Bid, bool>> GetBidsByRegionFilter(int regionId)
```

#### Parameters

| Parameter | Type | Description | Required |
|-----------|------|-------------|----------|
| regionId | int | The ID of the region | Yes |

#### Return Value
- **Type:** `Expression<Func<Bid, bool>>`
- **Returns:** Lambda expression filtering by region
- **Expression:** `bid => bid.BidRegions.Any(br => br.RegionId == regionId)`

#### When to Use
- Regional bid searches
- Geographic filtering
- Location-based recommendations
- Regional statistics

#### Usage Example
```csharp
int riyadhRegionId = 1;
var filter = BidQueryHelper.GetBidsByRegionFilter(riyadhRegionId);
var regionalBids = await _bidRepository.GetAsync(filter);
```

#### Real-World Scenarios

**Scenario 1: Regional Search**
```csharp
int regionId = 5; // Eastern Region

var regionalFilter = BidQueryHelper.CombineFiltersWithAnd(
    BidQueryHelper.GetBidsByRegionFilter(regionId),
    BidQueryHelper.GetPublishedBidsFilter(),
    BidQueryHelper.GetActiveBidsFilter(DateTime.UtcNow)
);

var bids = await _bidRepository.GetAsync(filter: regionalFilter);
// Result: Active published bids in Eastern Region
```

**Scenario 2: Multi-Region Search**
```csharp
int[] regions = { 1, 2, 3 }; // Riyadh, Makkah, Madinah

var filters = regions.Select(r => BidQueryHelper.GetBidsByRegionFilter(r)).ToArray();

// Combine with OR logic (custom implementation needed)
// Or execute separately and combine results
var allBids = new List<Bid>();
foreach (var regionFilter in filters)
{
    var bids = await _bidRepository.GetAsync(filter: regionFilter);
    allBids.AddRange(bids);
}

// Result: All bids from specified regions
```

---

### 5. GetPublicBidsFilter

#### Purpose
Creates a filter expression to get only public bids.

#### Method Signature
```csharp
public static Expression<Func<Bid, bool>> GetPublicBidsFilter()
```

#### Parameters
None

#### Return Value
- **Type:** `Expression<Func<Bid, bool>>`
- **Returns:** Lambda expression filtering for public bids
- **Expression:** `bid => bid.BidVisibility == BidTypes.Public`

#### When to Use
- Public search results
- Anonymous user browsing
- Open bid listings
- No-authorization-required displays

#### Usage Example
```csharp
var filter = BidQueryHelper.GetPublicBidsFilter();
var publicBids = await _bidRepository.GetAsync(filter);
```

#### Real-World Scenarios

**Scenario 1: Homepage Featured Bids**
```csharp
var featuredFilter = BidQueryHelper.CombineFiltersWithAnd(
    BidQueryHelper.GetPublicBidsFilter(),
    BidQueryHelper.GetPublishedBidsFilter(),
    BidQueryHelper.GetActiveBidsFilter(DateTime.UtcNow)
);

var featuredBids = await _bidRepository.GetAsync(
    filter: featuredFilter,
    orderBy: query => query.OrderByDescending(b => b.CreatedDate),
    pageSize: 5
);

// Result: 5 latest active public bids for homepage
```

---

### 6. GetPrivateBidsFilter

#### Purpose
Creates a filter expression to get only private bids.

#### Method Signature
```csharp
public static Expression<Func<Bid, bool>> GetPrivateBidsFilter()
```

#### Parameters
None

#### Return Value
- **Type:** `Expression<Func<Bid, bool>>`
- **Returns:** Lambda expression filtering for private bids
- **Expression:** `bid => bid.BidVisibility == BidTypes.Private`

#### When to Use
- Invitation-based listings
- Private bid management
- Restricted access displays
- Authorization-required features

#### Usage Example
```csharp
var filter = BidQueryHelper.GetPrivateBidsFilter();
var privateBids = await _bidRepository.GetAsync(filter);
```

---

### 7. GetActiveBidsFilter

#### Purpose
Creates a filter expression to get active (not expired) published bids.

#### Method Signature
```csharp
public static Expression<Func<Bid, bool>> GetActiveBidsFilter(DateTime currentDate)
```

#### Parameters

| Parameter | Type | Description | Required |
|-----------|------|-------------|----------|
| currentDate | DateTime | The current date/time to compare against | Yes |

#### Return Value
- **Type:** `Expression<Func<Bid, bool>>`
- **Returns:** Lambda expression filtering for active bids
- **Expression:** `bid => bid.LastDateInOffersSubmission >= currentDate && bid.TenderStatusId == (int)TenderStatus.Published`

#### When to Use
- Active opportunities listing
- Supplier dashboards
- Real-time bid searches
- "Apply Now" features

#### Usage Example
```csharp
var filter = BidQueryHelper.GetActiveBidsFilter(DateTime.UtcNow);
var activeBids = await _bidRepository.GetAsync(filter);
```

#### Real-World Scenarios

**Scenario 1: Supplier Active Opportunities**
```csharp
var activeFilter = BidQueryHelper.CombineFiltersWithAnd(
    BidQueryHelper.GetActiveBidsFilter(DateTime.UtcNow),
    BidQueryHelper.GetPublicBidsFilter()
);

var opportunities = await _bidRepository.GetAsync(
    filter: activeFilter,
    orderBy: BidQueryHelper.GetBidOrderingByDeadline()
);

// Result: Public bids still accepting submissions, ordered by deadline
```

**Scenario 2: Expiring Soon Alert**
```csharp
DateTime now = DateTime.UtcNow;
DateTime threeDaysFromNow = now.AddDays(3);

var expiringSoonBids = await _bidRepository.GetAsync(
    filter: bid =>
        bid.LastDateInOffersSubmission >= now &&
        bid.LastDateInOffersSubmission <= threeDaysFromNow &&
        bid.TenderStatusId == (int)TenderStatus.Published
);

// Result: Bids expiring within 3 days
```

---

### 8. GetExpiredBidsFilter

#### Purpose
Creates a filter expression to get expired bids.

#### Method Signature
```csharp
public static Expression<Func<Bid, bool>> GetExpiredBidsFilter(DateTime currentDate)
```

#### Parameters

| Parameter | Type | Description | Required |
|-----------|------|-------------|----------|
| currentDate | DateTime | The current date/time to compare against | Yes |

#### Return Value
- **Type:** `Expression<Func<Bid, bool>>`
- **Returns:** Lambda expression filtering for expired bids
- **Expression:** `bid => bid.LastDateInOffersSubmission < currentDate`

#### When to Use
- Archive displays
- Historical data queries
- Auto-close processes
- Statistics and reporting

#### Usage Example
```csharp
var filter = BidQueryHelper.GetExpiredBidsFilter(DateTime.UtcNow);
var expiredBids = await _bidRepository.GetAsync(filter);
```

#### Real-World Scenarios

**Scenario 1: Auto-Close Published Bids**
```csharp
var expiredPublishedFilter = BidQueryHelper.CombineFiltersWithAnd(
    BidQueryHelper.GetExpiredBidsFilter(DateTime.UtcNow),
    BidQueryHelper.GetPublishedBidsFilter()
);

var bidsToClose = await _bidRepository.GetAsync(filter: expiredPublishedFilter);

foreach (var bid in bidsToClose)
{
    bid.TenderStatusId = (int)TenderStatus.Closed;
    await _bidRepository.UpdateAsync(bid);
}

// Result: All expired published bids now marked as closed
```

**Scenario 2: Historical Analysis**
```csharp
var lastMonthExpired = BidQueryHelper.CombineFiltersWithAnd(
    BidQueryHelper.GetExpiredBidsFilter(DateTime.UtcNow),
    BidQueryHelper.GetBidsByDateRangeFilter(
        DateTime.UtcNow.AddMonths(-1),
        DateTime.UtcNow
    )
);

var expiredLastMonth = await _bidRepository.GetAsync(filter: lastMonthExpired);
// Result: Bids that expired in the last month
```

---

### 9. GetBidsByDateRangeFilter

#### Purpose
Creates a filter expression to get bids created within a date range.

#### Method Signature
```csharp
public static Expression<Func<Bid, bool>> GetBidsByDateRangeFilter(
    DateTime startDate,
    DateTime endDate)
```

#### Parameters

| Parameter | Type | Description | Required |
|-----------|------|-------------|----------|
| startDate | DateTime | Start of date range (inclusive) | Yes |
| endDate | DateTime | End of date range (inclusive) | Yes |

#### Return Value
- **Type:** `Expression<Func<Bid, bool>>`
- **Returns:** Lambda expression filtering by creation date range
- **Expression:** `bid => bid.CreatedDate >= startDate && bid.CreatedDate <= endDate`

#### When to Use
- Date range searches
- Monthly/yearly reports
- Time-based analytics
- Historical queries

#### Usage Example
```csharp
var startDate = new DateTime(2024, 1, 1);
var endDate = new DateTime(2024, 12, 31);

var filter = BidQueryHelper.GetBidsByDateRangeFilter(startDate, endDate);
var bidsIn2024 = await _bidRepository.GetAsync(filter);
```

#### Real-World Scenarios

**Scenario 1: Monthly Report**
```csharp
var firstDay = new DateTime(2024, 10, 1);
var lastDay = new DateTime(2024, 10, 31, 23, 59, 59);

var monthlyFilter = BidQueryHelper.CombineFiltersWithAnd(
    BidQueryHelper.GetBidsByDateRangeFilter(firstDay, lastDay),
    BidQueryHelper.GetPublishedBidsFilter()
);

var octoberBids = await _bidRepository.GetAsync(filter: monthlyFilter);

Console.WriteLine($"Published bids in October: {octoberBids.Count}");
// Result: Count of published bids created in October 2024
```

**Scenario 2: Quarter Analysis**
```csharp
var q1Start = new DateTime(2024, 1, 1);
var q1End = new DateTime(2024, 3, 31, 23, 59, 59);

var q1Bids = await _bidRepository.GetAsync(
    filter: BidQueryHelper.GetBidsByDateRangeFilter(q1Start, q1End)
);

double avgValue = q1Bids.Average(b => b.Association_Fees);
Console.WriteLine($"Q1 Average Bid Value: {avgValue:C}");
// Result: Average bid value for Q1 2024
```

---

### 10. GetBidsByIndustryFilter

#### Purpose
Creates a filter expression to get bids for a specific industry.

#### Method Signature
```csharp
public static Expression<Func<Bid, bool>> GetBidsByIndustryFilter(long industryId)
```

#### Parameters

| Parameter | Type | Description | Required |
|-----------|------|-------------|----------|
| industryId | long | The ID of the industry | Yes |

#### Return Value
- **Type:** `Expression<Func<Bid, bool>>`
- **Returns:** Lambda expression filtering by industry
- **Expression:** `bid => bid.BidIndustries.Any(bi => bi.IndustryId == industryId)`

#### When to Use
- Industry-specific searches
- Supplier industry matching
- Industry statistics
- Targeted recommendations

#### Usage Example
```csharp
long constructionIndustryId = 5;
var filter = BidQueryHelper.GetBidsByIndustryFilter(constructionIndustryId);
var constructionBids = await _bidRepository.GetAsync(filter);
```

#### Real-World Scenarios

**Scenario 1: Supplier Matching**
```csharp
long supplierIndustryId = 12; // IT Services

var matchingBids = BidQueryHelper.CombineFiltersWithAnd(
    BidQueryHelper.GetBidsByIndustryFilter(supplierIndustryId),
    BidQueryHelper.GetActiveBidsFilter(DateTime.UtcNow),
    BidQueryHelper.GetPublicBidsFilter()
);

var recommendedBids = await _bidRepository.GetAsync(filter: matchingBids);
// Result: Active public bids in supplier's industry
```

**Scenario 2: Industry Performance Report**
```csharp
long industryId = 8;

var industryBids = BidQueryHelper.CombineFiltersWithAnd(
    BidQueryHelper.GetBidsByIndustryFilter(industryId),
    BidQueryHelper.GetBidsByDateRangeFilter(
        DateTime.UtcNow.AddMonths(-6),
        DateTime.UtcNow
    )
);

var bids = await _bidRepository.GetAsync(filter: industryBids);

Console.WriteLine($"Industry bids (6 months): {bids.Count}");
Console.WriteLine($"Total value: {bids.Sum(b => b.Association_Fees):C}");
// Result: Industry statistics for last 6 months
```

---

### 11. GetBidsWithTermsBookBoughtFilter

#### Purpose
Creates a filter expression to get bids where at least one provider bought the terms book.

#### Method Signature
```csharp
public static Expression<Func<Bid, bool>> GetBidsWithTermsBookBoughtFilter()
```

#### Parameters
None

#### Return Value
- **Type:** `Expression<Func<Bid, bool>>`
- **Returns:** Lambda expression filtering for bids with purchased terms
- **Expression:** `bid => bid.ProviderBids.Any(pb => pb.IsPaymentConfirmed)`

#### When to Use
- Revenue tracking
- Bid engagement metrics
- Popular bids identification
- Payment statistics

#### Usage Example
```csharp
var filter = BidQueryHelper.GetBidsWithTermsBookBoughtFilter();
var paidBids = await _bidRepository.GetAsync(filter);

double totalRevenue = paidBids.Sum(b => b.Bid_Documents_Price);
```

#### Real-World Scenarios

**Scenario 1: Revenue Report**
```csharp
var revenueFilter = BidQueryHelper.CombineFiltersWithAnd(
    BidQueryHelper.GetBidsWithTermsBookBoughtFilter(),
    BidQueryHelper.GetBidsByDateRangeFilter(
        new DateTime(2024, 1, 1),
        new DateTime(2024, 12, 31)
    )
);

var paidBids = await _bidRepository.GetAsync(filter: revenueFilter);

double totalRevenue = paidBids.Sum(b =>
    b.ProviderBids.Count(pb => pb.IsPaymentConfirmed) * b.Bid_Documents_Price
);

Console.WriteLine($"2024 Revenue: {totalRevenue:C}");
// Result: Total revenue from terms book sales in 2024
```

**Scenario 2: Popular Bids**
```csharp
var popularBids = await _bidRepository.GetAsync(
    filter: BidQueryHelper.GetBidsWithTermsBookBoughtFilter()
);

var rankedBids = popularBids
    .Select(b => new
    {
        Bid = b,
        PurchaseCount = b.ProviderBids.Count(pb => pb.IsPaymentConfirmed)
    })
    .OrderByDescending(x => x.PurchaseCount)
    .Take(10)
    .ToList();

// Result: Top 10 bids by terms book purchases
```

---

### 12. CombineFiltersWithAnd

#### Purpose
Combines multiple filter expressions using AND logic.

#### Method Signature
```csharp
public static Expression<Func<Bid, bool>> CombineFiltersWithAnd(
    params Expression<Func<Bid, bool>>[] filters)
```

#### Parameters

| Parameter | Type | Description | Required |
|-----------|------|-------------|----------|
| filters | Expression<Func<Bid, bool>>[] | Array of filter expressions to combine | Yes |

#### Return Value
- **Type:** `Expression<Func<Bid, bool>>`
- **Returns:** Combined expression with AND logic
- **Special Case:** If no filters provided, returns `bid => true` (all bids)

#### When to Use
- Building complex queries dynamically
- Combining multiple search criteria
- Creating advanced filters
- Multi-condition searches

#### Usage Example
```csharp
var combinedFilter = BidQueryHelper.CombineFiltersWithAnd(
    BidQueryHelper.GetPublishedBidsFilter(),
    BidQueryHelper.GetPublicBidsFilter(),
    BidQueryHelper.GetActiveBidsFilter(DateTime.UtcNow)
);

var bids = await _bidRepository.GetAsync(filter: combinedFilter);
// Result: Active public published bids
```

#### Real-World Scenarios

**Scenario 1: Advanced Search**
```csharp
long associationId = 123;
int regionId = 5;
long industryId = 8;

var searchFilter = BidQueryHelper.CombineFiltersWithAnd(
    BidQueryHelper.GetPublishedBidsFilter(),
    BidQueryHelper.GetActiveBidsFilter(DateTime.UtcNow),
    BidQueryHelper.GetBidsByAssociationFilter(associationId),
    BidQueryHelper.GetBidsByRegionFilter(regionId),
    BidQueryHelper.GetBidsByIndustryFilter(industryId)
);

var results = await _bidRepository.GetAsync(
    filter: searchFilter,
    orderBy: BidQueryHelper.GetBidOrderingByDeadline()
);

// Result: Active published bids for specific association, region, and industry
```

**Scenario 2: Dynamic Filter Builder**
```csharp
var filters = new List<Expression<Func<Bid, bool>>>();

// Always filter published
filters.Add(BidQueryHelper.GetPublishedBidsFilter());

// Add optional filters based on user input
if (searchModel.RegionId.HasValue)
{
    filters.Add(BidQueryHelper.GetBidsByRegionFilter(searchModel.RegionId.Value));
}

if (searchModel.IndustryId.HasValue)
{
    filters.Add(BidQueryHelper.GetBidsByIndustryFilter(searchModel.IndustryId.Value));
}

if (searchModel.ShowActiveOnly)
{
    filters.Add(BidQueryHelper.GetActiveBidsFilter(DateTime.UtcNow));
}

// Combine all filters
var finalFilter = BidQueryHelper.CombineFiltersWithAnd(filters.ToArray());
var bids = await _bidRepository.GetAsync(filter: finalFilter);

// Result: Dynamically built query based on user selections
```

**Scenario 3: Association Admin Complex Query**
```csharp
long associationId = 456;
DateTime startDate = new DateTime(2024, 1, 1);
DateTime endDate = DateTime.UtcNow;

var adminFilter = BidQueryHelper.CombineFiltersWithAnd(
    BidQueryHelper.GetBidsByAssociationFilter(associationId),
    BidQueryHelper.GetBidsByDateRangeFilter(startDate, endDate),
    BidQueryHelper.GetBidsWithTermsBookBoughtFilter()
);

var performingBids = await _bidRepository.GetAsync(filter: adminFilter);

// Statistics
int totalBids = performingBids.Count;
int totalPurchases = performingBids.Sum(b => b.ProviderBids.Count(pb => pb.IsPaymentConfirmed));
double totalRevenue = performingBids.Sum(b =>
    b.ProviderBids.Count(pb => pb.IsPaymentConfirmed) * b.Bid_Documents_Price
);

Console.WriteLine($"Association Performance Report:");
Console.WriteLine($"Bids with purchases: {totalBids}");
Console.WriteLine($"Total purchases: {totalPurchases}");
Console.WriteLine($"Total revenue: {totalRevenue:C}");

// Result: Performance metrics for association's paid bids in 2024
```

---

### 13. GetDefaultBidOrdering

#### Purpose
Returns a function to order bids by creation date descending (newest first).

#### Method Signature
```csharp
public static Func<IQueryable<Bid>, IOrderedQueryable<Bid>> GetDefaultBidOrdering()
```

#### Parameters
None

#### Return Value
- **Type:** `Func<IQueryable<Bid>, IOrderedQueryable<Bid>>`
- **Returns:** Ordering function
- **Order:** `query => query.OrderByDescending(b => b.CreatedDate)`

#### When to Use
- Default bid listings
- "What's New" displays
- General bid browsing
- When no specific ordering needed

#### Usage Example
```csharp
var bids = await _bidRepository.GetAsync(
    filter: BidQueryHelper.GetPublishedBidsFilter(),
    orderBy: BidQueryHelper.GetDefaultBidOrdering(),
    pageNumber: 1,
    pageSize: 20
);

// Result: Latest 20 published bids, newest first
```

#### Real-World Scenarios

**Scenario 1: Homepage Recent Bids**
```csharp
var recentBids = await _bidRepository.GetAsync(
    filter: BidQueryHelper.CombineFiltersWithAnd(
        BidQueryHelper.GetPublishedBidsFilter(),
        BidQueryHelper.GetPublicBidsFilter()
    ),
    orderBy: BidQueryHelper.GetDefaultBidOrdering(),
    pageSize: 5
);

// Result: 5 most recently created public published bids
```

**Scenario 2: "What's New This Week"**
```csharp
var weekAgo = DateTime.UtcNow.AddDays(-7);

var newThisWeek = await _bidRepository.GetAsync(
    filter: BidQueryHelper.CombineFiltersWithAnd(
        BidQueryHelper.GetPublishedBidsFilter(),
        BidQueryHelper.GetBidsByDateRangeFilter(weekAgo, DateTime.UtcNow)
    ),
    orderBy: BidQueryHelper.GetDefaultBidOrdering()
);

// Result: All bids published this week, newest first
```

---

### 14. GetBidOrderingByDeadline

#### Purpose
Returns a function to order bids by submission deadline ascending (soonest first).

#### Method Signature
```csharp
public static Func<IQueryable<Bid>, IOrderedQueryable<Bid>> GetBidOrderingByDeadline()
```

#### Parameters
None

#### Return Value
- **Type:** `Func<IQueryable<Bid>, IOrderedQueryable<Bid>>`
- **Returns:** Ordering function
- **Order:** `query => query.OrderBy(b => b.LastDateInOffersSubmission)`

#### When to Use
- "Expiring Soon" displays
- Urgency-based listings
- Deadline reminders
- Time-sensitive features

#### Usage Example
```csharp
var urgentBids = await _bidRepository.GetAsync(
    filter: BidQueryHelper.GetActiveBidsFilter(DateTime.UtcNow),
    orderBy: BidQueryHelper.GetBidOrderingByDeadline(),
    pageSize: 10
);

// Result: 10 active bids with soonest deadlines
```

#### Real-World Scenarios

**Scenario 1: "Closing Soon" Widget**
```csharp
var threeDaysFromNow = DateTime.UtcNow.AddDays(3);

var closingSoon = await _bidRepository.GetAsync(
    filter: bid =>
        bid.LastDateInOffersSubmission >= DateTime.UtcNow &&
        bid.LastDateInOffersSubmission <= threeDaysFromNow &&
        bid.TenderStatusId == (int)TenderStatus.Published,
    orderBy: BidQueryHelper.GetBidOrderingByDeadline()
);

foreach (var bid in closingSoon)
{
    TimeSpan timeLeft = bid.LastDateInOffersSubmission.Value - DateTime.UtcNow;
    Console.WriteLine($"{bid.BidName} - {timeLeft.Days} days, {timeLeft.Hours} hours left");
}

// Result: Bids closing within 3 days, ordered by urgency
```

**Scenario 2: Supplier Dashboard Priority List**
```csharp
var supplierIndustry = 5;

var priorityBids = await _bidRepository.GetAsync(
    filter: BidQueryHelper.CombineFiltersWithAnd(
        BidQueryHelper.GetActiveBidsFilter(DateTime.UtcNow),
        BidQueryHelper.GetBidsByIndustryFilter(supplierIndustry)
    ),
    orderBy: BidQueryHelper.GetBidOrderingByDeadline(),
    pageSize: 20
);

// Result: 20 active bids in supplier's industry, most urgent first
```

---

## 📊 Quick Reference Table

| Method | Purpose | Parameters | Returns | Common Use |
|--------|---------|------------|---------|------------|
| GetPublishedBidsFilter | Filter published bids | None | Expression | Public listings |
| GetDraftBidsFilter | Filter draft bids | None | Expression | Admin dashboards |
| GetBidsByAssociationFilter | Filter by association | associationId | Expression | Tenant isolation |
| GetBidsByRegionFilter | Filter by region | regionId | Expression | Geographic search |
| GetPublicBidsFilter | Filter public bids | None | Expression | Open access |
| GetPrivateBidsFilter | Filter private bids | None | Expression | Restricted access |
| GetActiveBidsFilter | Filter active bids | currentDate | Expression | Current opportunities |
| GetExpiredBidsFilter | Filter expired bids | currentDate | Expression | Archives |
| GetBidsByDateRangeFilter | Filter by date range | start, end | Expression | Reports |
| GetBidsByIndustryFilter | Filter by industry | industryId | Expression | Industry search |
| GetBidsWithTermsBookBoughtFilter | Filter paid bids | None | Expression | Revenue tracking |
| CombineFiltersWithAnd | Combine filters | filters[] | Expression | Complex queries |
| GetDefaultBidOrdering | Order by created date | None | Func | General browsing |
| GetBidOrderingByDeadline | Order by deadline | None | Func | Urgency display |

---

## 🔄 Common Usage Patterns

### Pattern 1: Complete Search Feature
```csharp
public async Task<List<Bid>> SearchBids(BidSearchModel searchModel)
{
    // Build filter list dynamically
    var filters = new List<Expression<Func<Bid, bool>>>();

    // Always show only published
    filters.Add(BidQueryHelper.GetPublishedBidsFilter());

    // Optional: Active only
    if (searchModel.ActiveOnly)
    {
        filters.Add(BidQueryHelper.GetActiveBidsFilter(DateTime.UtcNow));
    }

    // Optional: By visibility
    if (searchModel.BidType == BidTypes.Public)
    {
        filters.Add(BidQueryHelper.GetPublicBidsFilter());
    }
    else if (searchModel.BidType == BidTypes.Private)
    {
        filters.Add(BidQueryHelper.GetPrivateBidsFilter());
    }

    // Optional: By region
    if (searchModel.RegionId.HasValue)
    {
        filters.Add(BidQueryHelper.GetBidsByRegionFilter(searchModel.RegionId.Value));
    }

    // Optional: By industry
    if (searchModel.IndustryId.HasValue)
    {
        filters.Add(BidQueryHelper.GetBidsByIndustryFilter(searchModel.IndustryId.Value));
    }

    // Optional: By date range
    if (searchModel.StartDate.HasValue && searchModel.EndDate.HasValue)
    {
        filters.Add(BidQueryHelper.GetBidsByDateRangeFilter(
            searchModel.StartDate.Value,
            searchModel.EndDate.Value
        ));
    }

    // Combine all filters
    var combinedFilter = BidQueryHelper.CombineFiltersWithAnd(filters.ToArray());

    // Choose ordering
    var ordering = searchModel.SortByDeadline
        ? BidQueryHelper.GetBidOrderingByDeadline()
        : BidQueryHelper.GetDefaultBidOrdering();

    // Execute query
    return await _bidRepository.GetAsync(
        filter: combinedFilter,
        orderBy: ordering,
        pageNumber: searchModel.PageNumber,
        pageSize: searchModel.PageSize
    );
}
```

### Pattern 2: Association Dashboard Statistics
```csharp
public async Task<AssociationStatisticsDto> GetAssociationStatistics(long associationId)
{
    var associationFilter = BidQueryHelper.GetBidsByAssociationFilter(associationId);

    // Total bids
    var allBids = await _bidRepository.GetAsync(filter: associationFilter);

    // Draft bids
    var draftFilter = BidQueryHelper.CombineFiltersWithAnd(
        associationFilter,
        BidQueryHelper.GetDraftBidsFilter()
    );
    var drafts = await _bidRepository.GetAsync(filter: draftFilter);

    // Published bids
    var publishedFilter = BidQueryHelper.CombineFiltersWithAnd(
        associationFilter,
        BidQueryHelper.GetPublishedBidsFilter()
    );
    var published = await _bidRepository.GetAsync(filter: publishedFilter);

    // Active bids
    var activeFilter = BidQueryHelper.CombineFiltersWithAnd(
        associationFilter,
        BidQueryHelper.GetActiveBidsFilter(DateTime.UtcNow)
    );
    var active = await _bidRepository.GetAsync(filter: activeFilter);

    // Bids with purchases
    var paidFilter = BidQueryHelper.CombineFiltersWithAnd(
        associationFilter,
        BidQueryHelper.GetBidsWithTermsBookBoughtFilter()
    );
    var paid = await _bidRepository.GetAsync(filter: paidFilter);

    // Calculate revenue
    double totalRevenue = paid.Sum(b =>
        b.ProviderBids.Count(pb => pb.IsPaymentConfirmed) * b.Bid_Documents_Price
    );

    return new AssociationStatisticsDto
    {
        TotalBids = allBids.Count,
        DraftBids = drafts.Count,
        PublishedBids = published.Count,
        ActiveBids = active.Count,
        BidsWithPurchases = paid.Count,
        TotalRevenue = totalRevenue
    };
}
```

### Pattern 3: Supplier Recommendation Engine
```csharp
public async Task<List<Bid>> GetRecommendedBids(long supplierId)
{
    // Get supplier's industries
    var supplier = await _supplierRepository.GetByIdAsync(supplierId);
    var supplierIndustries = supplier.SupplierIndustries.Select(si => si.IndustryId).ToList();

    // Get supplier's regions
    var supplierRegions = supplier.SupplierRegions.Select(sr => sr.RegionId).ToList();

    // Build recommended bids query
    var recommendedBids = new List<Bid>();

    foreach (var industryId in supplierIndustries)
    {
        foreach (var regionId in supplierRegions)
        {
            var filter = BidQueryHelper.CombineFiltersWithAnd(
                BidQueryHelper.GetPublishedBidsFilter(),
                BidQueryHelper.GetActiveBidsFilter(DateTime.UtcNow),
                BidQueryHelper.GetPublicBidsFilter(),
                BidQueryHelper.GetBidsByIndustryFilter(industryId),
                BidQueryHelper.GetBidsByRegionFilter(regionId)
            );

            var bids = await _bidRepository.GetAsync(filter: filter);
            recommendedBids.AddRange(bids);
        }
    }

    // Remove duplicates and order by deadline
    return recommendedBids
        .Distinct()
        .OrderBy(b => b.LastDateInOffersSubmission)
        .Take(20)
        .ToList();
}
```

### Pattern 4: Automated Cleanup Job
```csharp
public async Task AutoCloseExpiredBids()
{
    // Find expired published bids
    var expiredFilter = BidQueryHelper.CombineFiltersWithAnd(
        BidQueryHelper.GetExpiredBidsFilter(DateTime.UtcNow),
        BidQueryHelper.GetPublishedBidsFilter()
    );

    var bidsToClose = await _bidRepository.GetAsync(filter: expiredFilter);

    foreach (var bid in bidsToClose)
    {
        // Change status to closed
        bid.TenderStatusId = (int)TenderStatus.Closed;
        bid.ModifiedDate = DateTime.UtcNow;

        await _bidRepository.UpdateAsync(bid);

        // Notify association
        await _notificationService.SendBidClosedNotificationAsync(bid.AssociationId, bid.Id);
    }

    await _unitOfWork.CommitAsync();

    Console.WriteLine($"Auto-closed {bidsToClose.Count} expired bids");
}
```

---

## 💡 Best Practices

### ✅ DO
- Use query helpers for all common filters
- Combine filters with CombineFiltersWithAnd for complex queries
- Build filters dynamically based on user input
- Use appropriate ordering (default or deadline)
- Reuse filter expressions across methods
- Include appropriate filters for authorization (association, visibility)

### ❌ DON'T
- Don't write inline filters for common cases
- Don't forget to filter by status when needed
- Don't mix business logic into query building
- Don't execute queries without appropriate filters
- Don't forget pagination for large result sets
- Don't hardcode status IDs in queries

---

## 🎯 Lines Saved in BidServiceCore

By using BidQueryHelper instead of inline expressions:
- **~30 lines** of code removed from BidServiceCore
- **14 methods** extracted to helper
- **Improved consistency** - same filters used everywhere
- **Better maintainability** - change filter logic in one place
- **Enhanced testability** - each filter independently testable

---

## 📝 Summary

BidQueryHelper provides 14 essential query builder methods for:
- **Status filters:** Published, Draft, Active, Expired
- **Visibility filters:** Public, Private
- **Entity filters:** Association, Region, Industry
- **Date filters:** Date range, Active/Expired
- **Special filters:** Terms book bought
- **Composition:** CombineFiltersWithAnd for complex queries
- **Ordering:** Default (newest) and by deadline (soonest)

All methods return expressions or ordering functions, integrate seamlessly with repositories, and are fully testable!
