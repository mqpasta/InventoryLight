-- =============================================
-- Create basic stored procedure template
-- =============================================

-- Drop stored procedure if it already exists
IF EXISTS (
  SELECT * 
    FROM INFORMATION_SCHEMA.ROUTINES 
   WHERE SPECIFIC_SCHEMA = N'dbo'
     AND SPECIFIC_NAME = N'ItemLedgerSummary' 
)
   DROP PROCEDURE dbo.ItemLedgerSummary
GO

CREATE PROCEDURE dbo.ItemLedgerSummary
	@LocationId bigint = NULL, 
	@ProductId bigint = NULL, 
	@StockMovType bigint = NULL, 
	@StartDate date = NULL, 
	@EndDate date = NULL
AS
	;with CTE_Stock AS(
	Select Date, FromLocationId, ToLocationId, ProductId, Qty as InQty, 0 AS OutQty, 'Opening' AS Detail, 0 as Sort,
		StockMovementType, PurchaseOrderId 
	from StockMovement 
	WHERE StockMovementType = 2 
			AND (@LocationId IS NULL OR ToLocationId = @LocationId ) AND (@ProductId IS NULL OR ProductId = @ProductId)  
	
	UNION ALL  
	
	Select Date, FromLocationId, ToLocationId, ProductId, Qty as InQty, 0 AS OutQty, 'Purchase' AS Detail,  1 as Sort,
		StockMovementType, PurchaseOrderId 
	from StockMovement 
	WHERE StockMovementType IN (0) 
		AND (@LocationId  IS NULL OR ToLocationId = @LocationId )  
		AND (@ProductId IS NULL OR ProductId = @ProductId)  

	UNION ALL

		Select Date, FromLocationId, ToLocationId, ProductId, Qty as InQty, 0 AS OutQty, 'Transfer-In' AS Detail,  2 as Sort,
		StockMovementType, PurchaseOrderId 
	from StockMovement 
	WHERE StockMovementType IN (1) 
		AND (@LocationId  IS NULL OR ToLocationId = @LocationId )  
		AND (@ProductId IS NULL OR ProductId = @ProductId)  
	
	UNION ALL  
	
	Select Date, FromLocationId, ToLocationId, ProductId, 0 as InQty, Qty AS OutQty, 'Transfer-Out' AS Detail, 3 as Sort,
			StockMovementType  ,PurchaseOrderId 
	from StockMovement  
	WHERE StockMovementType IN (0,1) AND (@LocationId  IS NULL OR FromLocationId = @LocationId )  
                 AND (@ProductId IS NULL OR ProductId = @ProductId)
	)
	Select S.Detail , L1.LocationName as FromLocationName, L2.LocationName as ToLocationName, P.ProductName,
				SUM(S.InQty) as InQty, SUM(S.OutQty) as OutQty
	from CTE_Stock S  
                 Left Join Location L1 ON S.FromLocationId = L1.LocationId  
                 Left Join Location L2 ON S.ToLocationId = L2.LocationId  
                 Inner Join Product P ON S.ProductId = P.ProductId  
	WHERE(@StockMovType IS NULL OR S.StockMovementType IN (@StockMovType))  
                 AND (@StartDate IS NULL OR S.Date >= @StartDate )  
                 AND (@EndDate IS NULL OR S.Date <= @EndDate )
	Group by S.Sort, S.Detail , L1.LocationName , L2.LocationName , P.ProductName
	Order by S.Sort
	
GO

-- =============================================
-- Example to execute the stored procedure
-- =============================================
--EXECUTE dbo.ItemLedgerSummary 2,2,NULL,'2021-01-01',NULL
--GO
