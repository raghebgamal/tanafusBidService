# BidCalculationHelper - Complete Documentation

## 📋 Table of Contents
1. [Overview](#overview)
2. [Class Purpose](#class-purpose)
3. [When to Use This Helper](#when-to-use-this-helper)
4. [Methods Documentation](#methods-documentation)
5. [Quick Reference Table](#quick-reference-table)
6. [Calculation Formulas](#calculation-formulas)
7. [Common Usage Patterns](#common-usage-patterns)

---

## Overview

**File:** `Helpers/BidCalculationHelper.cs`
**Type:** Static Helper Class
**Namespace:** `Nafis.Services.Implementation.Helpers`
**Purpose:** Centralizes all financial calculations for bids including fees, taxes, and pricing.

---

## Class Purpose

The `BidCalculationHelper` class extracts and centralizes all financial calculation logic related to bids. This includes:
- ✅ Bid document price calculations
- ✅ Tanafos platform fee calculations
- ✅ VAT (Value Added Tax) calculations
- ✅ Total price computations
- ✅ Fee validations

**Why use this helper?**
- Consistent financial calculations across the system
- Single source of truth for pricing formulas
- Easy to update pricing rules
- Reduces calculation errors
- Simplifies testing of financial logic
- Separates financial logic from business logic

---

## When to Use This Helper

Use `BidCalculationHelper` when you need to:
- ✅ Calculate bid document prices
- ✅ Compute Tanafos platform fees
- ✅ Calculate VAT on amounts
- ✅ Get total prices including all fees and taxes
- ✅ Update bid entity with calculated prices

**Don't use this helper for:**
- ❌ Payment processing (use payment services)
- ❌ Database operations (use repositories)
- ❌ Validation logic (use BidValidationHelper)
- ❌ Invoice generation (use invoice services)

---

## Methods Documentation

### 1. CalculateAndUpdateBidPrices

#### **Purpose**
Main method that calculates all bid-related prices (association fees, Tanafos fees, VAT, total) and updates the bid entity.

#### **Method Signature**
```csharp
public static OperationResult<bool> CalculateAndUpdateBidPrices(
    double association_Fees,
    ReadOnlyAppGeneralSettings settings,
    Bid bid)
```

#### **Parameters**
| Parameter | Type | Description |
|-----------|------|-------------|
| `association_Fees` | `double` | Base fee set by the association |
| `settings` | `ReadOnlyAppGeneralSettings` | System settings (percentages, min/max values) |
| `bid` | `Bid` | Bid entity to update with calculated values |

#### **Return Value**
| Type | Value | Meaning |
|------|-------|---------|
| `OperationResult<bool>` | Success(true) | Calculations completed, bid updated |
| `OperationResult<bool>` | Fail(NotFound) | Bid or settings is null |
| `OperationResult<bool>` | Fail(Conflict) | Invalid association fees or price exceeds maximum |

#### **What It Does**
1. Calculates Tanafos fees (percentage of association fees, with minimum)
2. Calculates subtotal (association fees + Tanafos fees)
3. Calculates VAT on subtotal
4. Calculates total price (subtotal + VAT)
5. Validates total doesn't exceed maximum
6. Updates bid entity with all calculated values

#### **Properties Updated in Bid Entity**
- `bid.Association_Fees` - Association fees
- `bid.Tanafos_Fees` - Platform fees
- `bid.Bid_Documents_Price` - Total price with tax

#### **Business Rules**

**Tanafos Fees Calculation:**
```
TanafosFeesBase = AssociationFees × (TanfasPercentage / 100)
TanafosFeesWithMin = Max(TanafosFeesBase, MinTanfasOfBidDocumentPrice)
```

**VAT Calculation:**
```
Subtotal = AssociationFees + TanafosFeesWithMin
VAT = Subtotal × (VATPercentage / 100)
```

**Total Price:**
```
TotalPrice = Subtotal + VAT
```

**Validation:**
```
AssociationFees >= 0
TotalPrice <= MaxBidDocumentPrice
```

#### **When to Use**
Call this method when:
- Creating new bid with pricing
- Updating bid association fees
- Recalculating prices after settings change
- Before saving bid to database

#### **Where to Replace in BidServiceCore**
**Original Location:** Line 864-885 in BidServiceCore.cs

**Original Code:**
```csharp
private OperationResult<bool> CalculateAndUpdateBidPrices(double association_Fees, ReadOnlyAppGeneralSettings settings, Bid bid)
{
    if (bid is null || settings is null)
        return OperationResult<bool>.Fail(HttpErrorCode.NotFound, CommonErrorCodes.NOT_FOUND);

    double tanafosMoneyWithoutTax = Math.Round((association_Fees * ((double)settings.TanfasPercentage / 100)), 8);
    if (tanafosMoneyWithoutTax < settings.MinTanfasOfBidDocumentPrice)
        tanafosMoneyWithoutTax = settings.MinTanfasOfBidDocumentPrice;

    var bidDocumentPricesWithoutTax = Math.Round((association_Fees + tanafosMoneyWithoutTax), 8);
    var bidDocumentTax = Math.Round((bidDocumentPricesWithoutTax * ((double)settings.VATPercentage / 100)), 8);
    var bidDocumentPricesWithTax = Math.Round((bidDocumentPricesWithoutTax + bidDocumentTax), 8);

    if (association_Fees < 0 || bidDocumentPricesWithTax > settings.MaxBidDocumentPrice)
        return OperationResult<bool>.Fail(HttpErrorCode.Conflict, CommonErrorCodes.INVALID_INPUT);

    bid.Association_Fees = association_Fees;
    bid.Tanafos_Fees = tanafosMoneyWithoutTax;
    bid.Bid_Documents_Price = bidDocumentPricesWithTax;

    return OperationResult<bool>.Success(true);
}
```

**Replacement:**
```csharp
// ✅ Using BidCalculationHelper instead of private method
var calculationResult = BidCalculationHelper.CalculateAndUpdateBidPrices(
    model.Association_Fees,
    generalSettings,
    bid
);

if (!calculationResult.IsSucceeded)
{
    return OperationResult<AddBidResponse>.Fail(
        calculationResult.HttpErrorCode,
        calculationResult.Code,
        calculationResult.ErrorMessage
    );
}
```

#### **Usage Example**
```csharp
// In AddBidNew method
public async Task<OperationResult<AddBidResponse>> AddBidNew(AddBidModelNew model)
{
    // ... create bid entity ...
    var bid = _mapper.Map<Bid>(model);

    // Get system settings
    var settingsResult = await _appGeneralSettingService.GetAppGeneralSettings();
    var settings = settingsResult.Data;

    // Calculate all prices and update bid
    var calculationResult = BidCalculationHelper.CalculateAndUpdateBidPrices(
        model.Association_Fees,  // e.g., 1000.00 SAR
        settings,                // Contains percentages and limits
        bid                      // Bid entity to update
    );

    if (!calculationResult.IsSucceeded)
    {
        return OperationResult<AddBidResponse>.Fail(
            calculationResult.HttpErrorCode,
            calculationResult.Code
        );
    }

    // Now bid contains:
    // - bid.Association_Fees = 1000.00
    // - bid.Tanafos_Fees = 50.00 (5% of 1000, or minimum if higher)
    // - bid.Bid_Documents_Price = 1102.50 (1050 + 5% VAT)

    // Save bid to database
    await _bidRepository.Add(bid);
}
```

#### **Real-World Scenarios**

**Scenario 1: Standard Calculation**
```csharp
var settings = new ReadOnlyAppGeneralSettings
{
    TanfasPercentage = 5,              // 5% platform fee
    MinTanfasOfBidDocumentPrice = 10,  // Minimum 10 SAR
    VATPercentage = 5,                 // 5% VAT
    MaxBidDocumentPrice = 100000       // Maximum 100,000 SAR
};

var bid = new Bid();

var result = BidCalculationHelper.CalculateAndUpdateBidPrices(
    1000.00,    // Association fees: 1000 SAR
    settings,
    bid
);

// Result: Success
// Calculations:
// 1. Tanafos Fees = 1000 × 5% = 50.00 (> minimum 10, so use 50)
// 2. Subtotal = 1000 + 50 = 1050.00
// 3. VAT = 1050 × 5% = 52.50
// 4. Total = 1050 + 52.50 = 1102.50
//
// Bid Updated:
// bid.Association_Fees = 1000.00
// bid.Tanafos_Fees = 50.00
// bid.Bid_Documents_Price = 1102.50
```

**Scenario 2: Minimum Tanafos Fee Applied**
```csharp
var settings = new ReadOnlyAppGeneralSettings
{
    TanfasPercentage = 5,              // 5% platform fee
    MinTanfasOfBidDocumentPrice = 100, // Minimum 100 SAR (HIGH)
    VATPercentage = 15,                // 15% VAT
    MaxBidDocumentPrice = 100000
};

var bid = new Bid();

var result = BidCalculationHelper.CalculateAndUpdateBidPrices(
    500.00,     // Association fees: 500 SAR (small bid)
    settings,
    bid
);

// Result: Success
// Calculations:
// 1. Tanafos Fees Base = 500 × 5% = 25.00
//    BUT minimum is 100, so use 100.00 ✅
// 2. Subtotal = 500 + 100 = 600.00
// 3. VAT = 600 × 15% = 90.00
// 4. Total = 600 + 90 = 690.00
//
// Bid Updated:
// bid.Association_Fees = 500.00
// bid.Tanafos_Fees = 100.00  (minimum enforced)
// bid.Bid_Documents_Price = 690.00
```

**Scenario 3: Price Exceeds Maximum (ERROR)**
```csharp
var settings = new ReadOnlyAppGeneralSettings
{
    TanfasPercentage = 5,
    MinTanfasOfBidDocumentPrice = 10,
    VATPercentage = 15,
    MaxBidDocumentPrice = 1000         // Low maximum: 1000 SAR
};

var bid = new Bid();

var result = BidCalculationHelper.CalculateAndUpdateBidPrices(
    10000.00,   // Association fees: 10,000 SAR (too high)
    settings,
    bid
);

// Result: FAIL
// Calculations:
// 1. Tanafos Fees = 10000 × 5% = 500.00
// 2. Subtotal = 10000 + 500 = 10500.00
// 3. VAT = 10500 × 15% = 1575.00
// 4. Total = 10500 + 1575 = 12075.00 ❌
//    Total (12075) > Maximum (1000) - ERROR!
//
// Result: OperationResult.Fail(HttpErrorCode.Conflict, INVALID_INPUT)
```

**Scenario 4: Negative Association Fees (ERROR)**
```csharp
var result = BidCalculationHelper.CalculateAndUpdateBidPrices(
    -100.00,    // ❌ Negative fees
    settings,
    bid
);

// Result: FAIL
// OperationResult.Fail(HttpErrorCode.Conflict, INVALID_INPUT)
```

---

### 2. CalculateTanafos Fees

#### **Purpose**
Calculates Tanafos platform fees as a percentage of association fees, with a minimum threshold.

#### **Method Signature**
```csharp
public static double CalculateTanafos Fees(
    double associationFees,
    double tanfasPercentage,
    double minTanfasOfBidDocumentPrice)
```

#### **Parameters**
| Parameter | Type | Description |
|-----------|------|-------------|
| `associationFees` | `double` | Base association fees |
| `tanfasPercentage` | `double` | Platform fee percentage |
| `minTanfasOfBidDocumentPrice` | `double` | Minimum platform fee |

#### **Return Value**
| Type | Description |
|------|-------------|
| `double` | Calculated Tanafos fees (8 decimal precision) |

#### **Formula**
```
TanafosFeesBase = associationFees × (tanfasPercentage / 100)
Result = Max(TanafosFeesBase, minTanfasOfBidDocumentPrice)
```

#### **When to Use**
- When you need ONLY Tanafos fees calculation
- For display purposes before saving
- For quote/estimate generation
- When building price breakdowns

#### **Usage Example**
```csharp
// Calculate just Tanafos fees
double associationFees = 1000.00;
double platformPercentage = 5;     // 5%
double minimumFee = 25.00;

double tanafos Fees = BidCalculationHelper.CalculateTanafos Fees(
    associationFees,
    platformPercentage,
    minimumFee
);

// Result: 50.00 SAR (5% of 1000 = 50, which is > 25 minimum)

// For small bids
double smallBidFees = BidCalculationHelper.CalculateTanafos Fees(
    100.00,     // Small bid
    5,
    25.00
);

// Result: 25.00 SAR (5% of 100 = 5, but minimum is 25, so return 25)
```

#### **Real-World Scenario**
```csharp
// Display price breakdown to user before creating bid
public async Task<PriceBreakdownDto> GetPriceEstimate(double associationFees)
{
    var settings = await _appGeneralSettingService.GetAppGeneralSettings();

    var breakdown = new PriceBreakdownDto
    {
        AssociationFees = associationFees,
        TanafosWithoutTax = BidCalculationHelper.CalculateTanafos Fees(
            associationFees,
            settings.Data.TanfasPercentage,
            settings.Data.MinTanfasOfBidDocumentPrice
        )
    };

    // Calculate VAT
    double subtotal = breakdown.AssociationFees + breakdown.TanafosWithoutTax;
    breakdown.VAT = BidCalculationHelper.CalculateVAT(
        subtotal,
        settings.Data.VATPercentage
    );

    breakdown.Total = subtotal + breakdown.VAT;

    return breakdown;
}
```

---

### 3. CalculateVAT

#### **Purpose**
Calculates Value Added Tax (VAT) on a given amount.

#### **Method Signature**
```csharp
public static double CalculateVAT(double amountWithoutTax, double vatPercentage)
```

#### **Parameters**
| Parameter | Type | Description |
|-----------|------|-------------|
| `amountWithoutTax` | `double` | Base amount before tax |
| `vatPercentage` | `double` | VAT percentage |

#### **Return Value**
| Type | Description |
|------|-------------|
| `double` | VAT amount (8 decimal precision) |

#### **Formula**
```
VAT = amountWithoutTax × (vatPercentage / 100)
```

#### **When to Use**
- Calculating VAT on any amount
- Price breakdowns
- Invoice generation
- Tax reports

#### **Usage Example**
```csharp
// Calculate VAT on subtotal
double subtotal = 1050.00;
double vatPercentage = 15;  // 15% VAT

double vat = BidCalculationHelper.CalculateVAT(subtotal, vatPercentage);
// Result: 157.50 SAR

double total = subtotal + vat;
// Result: 1207.50 SAR

// For 5% VAT
double vat5 = BidCalculationHelper.CalculateVAT(1000.00, 5);
// Result: 50.00 SAR
```

#### **Real-World Scenario**
```csharp
// Calculate VAT for invoice
public async Task<InvoiceDto> GenerateInvoice(long bidId)
{
    var bid = await _bidRepository.FindOneAsync(x => x.Id == bidId);
    var settings = await _appGeneralSettingService.GetAppGeneralSettings();

    double subtotal = bid.Association_Fees + bid.Tanafos_Fees;

    var invoice = new InvoiceDto
    {
        BidId = bidId,
        AssociationFees = bid.Association_Fees,
        PlatformFees = bid.Tanafos_Fees,
        Subtotal = subtotal,
        VAT = BidCalculationHelper.CalculateVAT(
            subtotal,
            settings.Data.VATPercentage
        ),
        Total = bid.Bid_Documents_Price
    };

    return invoice;
}
```

---

### 4. CalculateTotalBidDocumentPrice

#### **Purpose**
Calculates the complete total price including association fees, Tanafos fees, and VAT. Does NOT update bid entity.

#### **Method Signature**
```csharp
public static double CalculateTotalBidDocumentPrice(
    double associationFees,
    ReadOnlyAppGeneralSettings settings)
```

#### **Parameters**
| Parameter | Type | Description |
|-----------|------|-------------|
| `associationFees` | `double` | Base association fees |
| `settings` | `ReadOnlyAppGeneralSettings` | System settings |

#### **Return Value**
| Type | Description |
|------|-------------|
| `double` | Total price with all fees and taxes (8 decimal precision) |

#### **Formula**
```
TanafosWithoutTax = CalculateTanafos Fees(associationFees, settings)
Subtotal = associationFees + TanafosWithoutTax
VAT = CalculateVAT(Subtotal, settings.VATPercentage)
Total = Subtotal + VAT
```

#### **Difference from `CalculateAndUpdateBidPrices`**
| Feature | CalculateTotalBidDocumentPrice | CalculateAndUpdateBidPrices |
|---------|--------------------------------|-----------------------------|
| Updates bid entity | ❌ No | ✅ Yes |
| Returns total | ✅ Yes | Only success/fail |
| Validates | ❌ No | ✅ Yes (max price, negative fees) |
| Use case | Quick calculations, estimates | Saving to database |

#### **When to Use**
- Quick price calculations
- Price estimates/quotes
- Displaying prices to users before saving
- Price comparisons
- When you don't have a bid entity yet

#### **Usage Example**
```csharp
// Get quick price estimate
var settings = await _appGeneralSettingService.GetAppGeneralSettings();

double totalPrice = BidCalculationHelper.CalculateTotalBidDocumentPrice(
    1000.00,        // Association fees
    settings.Data
);

// Result: Total price with all fees and taxes
// Can display to user immediately without creating bid entity

// Show to user
Console.WriteLine($"Total bid document price: {totalPrice:C} SAR");
```

#### **Real-World Scenario**
```csharp
// Price calculator API endpoint
[HttpGet("calculate-price")]
public async Task<ActionResult<PriceEstimateResponse>> CalculatePrice(double associationFees)
{
    var settings = await _appGeneralSettingService.GetAppGeneralSettings();

    double totalPrice = BidCalculationHelper.CalculateTotalBidDocumentPrice(
        associationFees,
        settings.Data
    );

    // Also calculate breakdown
    double tanafos Fees = BidCalculationHelper.CalculateTanafos Fees(
        associationFees,
        settings.Data.TanfasPercentage,
        settings.Data.MinTanfasOfBidDocumentPrice
    );

    double subtotal = associationFees + tanafos Fees;
    double vat = BidCalculationHelper.CalculateVAT(subtotal, settings.Data.VATPercentage);

    return new PriceEstimateResponse
    {
        AssociationFees = associationFees,
        PlatformFees = tanafos Fees,
        Subtotal = subtotal,
        VAT = vat,
        Total = totalPrice
    };
}
```

---

## Quick Reference Table

| Method | Purpose | Updates Bid | Returns | When to Use |
|--------|---------|-------------|---------|-------------|
| `CalculateAndUpdateBidPrices` | Calculate all prices + update bid | ✅ Yes | `OperationResult<bool>` | Before saving bid to DB |
| `CalculateTanafos Fees` | Calculate platform fees only | ❌ No | `double` | Price breakdowns, estimates |
| `CalculateVAT` | Calculate VAT only | ❌ No | `double` | Tax calculations, invoices |
| `CalculateTotalBidDocumentPrice` | Calculate total price | ❌ No | `double` | Quick estimates, quotes |

---

## Calculation Formulas

### Complete Price Calculation Flow

```
Input: AssociationFees = 1000 SAR

Step 1: Calculate Tanafos Fees
────────────────────────────────
TanafosFeesBase = 1000 × (5 / 100) = 50.00
TanafosFeesWithMin = Max(50.00, MinTanfasOfBidDocumentPrice)
                   = Max(50.00, 10.00)
                   = 50.00 SAR

Step 2: Calculate Subtotal
────────────────────────────────
Subtotal = AssociationFees + TanafosWithoutTax
         = 1000.00 + 50.00
         = 1050.00 SAR

Step 3: Calculate VAT
────────────────────────────────
VAT = Subtotal × (VATPercentage / 100)
    = 1050.00 × (15 / 100)
    = 157.50 SAR

Step 4: Calculate Total
────────────────────────────────
Total = Subtotal + VAT
      = 1050.00 + 157.50
      = 1207.50 SAR

Final Result:
────────────────────────────────
Association Fees: 1000.00 SAR
Platform Fees:      50.00 SAR
─────────────────────────────
Subtotal:         1050.00 SAR
VAT (15%):         157.50 SAR
─────────────────────────────
TOTAL:            1207.50 SAR
```

---

## Common Usage Patterns

### Pattern 1: Creating New Bid with Pricing
```csharp
public async Task<OperationResult<AddBidResponse>> AddBidNew(AddBidModelNew model)
{
    // Create bid entity
    var bid = _mapper.Map<Bid>(model);

    // Get settings
    var settingsResult = await _appGeneralSettingService.GetAppGeneralSettings();
    var settings = settingsResult.Data;

    // Calculate and update all prices
    var calculationResult = BidCalculationHelper.CalculateAndUpdateBidPrices(
        model.Association_Fees,
        settings,
        bid
    );

    if (!calculationResult.IsSucceeded)
    {
        return OperationResult<AddBidResponse>.Fail(
            calculationResult.HttpErrorCode,
            calculationResult.Code
        );
    }

    // Save bid with calculated prices
    await _bidRepository.Add(bid);

    return OperationResult<AddBidResponse>.Success(new AddBidResponse
    {
        Id = bid.Id,
        TotalPrice = bid.Bid_Documents_Price
    });
}
```

### Pattern 2: Price Estimate Before Creating Bid
```csharp
public async Task<PriceEstimateDto> GetPriceEstimate(double associationFees)
{
    var settings = await _appGeneralSettingService.GetAppGeneralSettings();

    // Calculate total quickly
    double totalPrice = BidCalculationHelper.CalculateTotalBidDocumentPrice(
        associationFees,
        settings.Data
    );

    // Get breakdown details
    double platformFees = BidCalculationHelper.CalculateTanafos Fees(
        associationFees,
        settings.Data.TanfasPercentage,
        settings.Data.MinTanfasOfBidDocumentPrice
    );

    double subtotal = associationFees + platformFees;

    double vat = BidCalculationHelper.CalculateVAT(
        subtotal,
        settings.Data.VATPercentage
    );

    return new PriceEstimateDto
    {
        AssociationFees = associationFees,
        PlatformFees = platformFees,
        Subtotal = subtotal,
        VATAmount = vat,
        VATPercentage = settings.Data.VATPercentage,
        Total = totalPrice
    };
}
```

### Pattern 3: Updating Bid Prices
```csharp
public async Task<OperationResult<bool>> UpdateBidPricing(long bidId, double newAssociationFees)
{
    var bid = await _bidRepository.FindOneAsync(x => x.Id == bidId);
    var settings = await _appGeneralSettingService.GetAppGeneralSettings();

    // Recalculate all prices
    var calculationResult = BidCalculationHelper.CalculateAndUpdateBidPrices(
        newAssociationFees,
        settings.Data,
        bid
    );

    if (!calculationResult.IsSucceeded)
    {
        return OperationResult<bool>.Fail(
            calculationResult.HttpErrorCode,
            calculationResult.Code
        );
    }

    // Update in database
    await _bidRepository.Update(bid);

    return OperationResult<bool>.Success(true);
}
```

---

## Error Handling

### Common Errors

| Error | Cause | Solution |
|-------|-------|----------|
| `NotFound` | Bid or settings is null | Check parameters before calling |
| `Conflict` with `INVALID_INPUT` | Negative association fees | Validate fees > 0 before calling |
| `Conflict` with `INVALID_INPUT` | Total exceeds maximum | Inform user, reduce association fees |

### Validation Before Calling

```csharp
// Always validate before calculating
if (model.Association_Fees < 0)
{
    return OperationResult<AddBidResponse>.Fail(
        HttpErrorCode.InvalidInput,
        "Association fees cannot be negative"
    );
}

var settings = await _appGeneralSettingService.GetAppGeneralSettings();
if (!settings.IsSucceeded)
{
    return OperationResult<AddBidResponse>.Fail(
        settings.HttpErrorCode,
        settings.Code
    );
}

// Safe to call now
var calculationResult = BidCalculationHelper.CalculateAndUpdateBidPrices(
    model.Association_Fees,
    settings.Data,
    bid
);
```

---

## Summary

**BidCalculationHelper** provides all financial calculation logic:
- ✅ **4 methods** for complete price calculations
- ✅ **Zero dependencies** - pure static helper
- ✅ **Consistent formulas** - single source of truth
- ✅ **8 decimal precision** - accurate calculations
- ✅ **Reduces BidServiceCore** by ~50 lines
- ✅ **Reusable** across all services
- ✅ **Easy to test** independently

**Use this helper for all bid pricing calculations!**
