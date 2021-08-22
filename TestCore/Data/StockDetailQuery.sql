DECLARE @LocationId bigint, @ProductId bigint, @StockMovementType int
SET @LocationId = 2;
SET @ProductId = NULL;
SET @StockMovementType = 1;

with CTE_Stock AS (
	Select Date,FromLocationId, ToLocationId, ProductId, Qty as InQty, 0 AS OutQty, 'Opening' AS Detail, StockMovementType
	from StockMovement 
	WHERE StockMovementType = 2 AND (@LocationId IS NULL OR ToLocationId=@LocationId)
		AND (@ProductId IS NULL OR ProductId = @ProductId)
	UNION ALL
	Select Date,FromLocationId, ToLocationId,ProductId, Qty as InQty, 0 AS OutQty, 'In' AS Detail , StockMovementType
	from StockMovement 
	WHERE StockMovementType IN (0,1) AND (@LocationId IS NULL OR ToLocationId=@LocationId)
		AND (@ProductId IS NULL OR ProductId = @ProductId)
	UNION ALL
	Select Date, FromLocationId,ToLocationId, ProductId, 0 as InQty, Qty AS OutQty, 'Out' AS Detail , StockMovementType
	from StockMovement 
	 WHERE StockMovementType IN (0,1) AND (@LocationId IS NULL OR FromLocationId=@LocationId)
		AND (@ProductId IS NULL OR ProductId = @ProductId)
)
Select S.* , L1.LocationName as FromLocationName, L2.LocationName as ToLocationName, P.ProductName
from CTE_Stock S 
	Left Join Location L1 ON S.FromLocationId = L1.LocationId
	Left Join Location L2 ON S.ToLocationId = L2.LocationId
	Inner Join Product P ON S.ProductId = P.ProductId
WHERE (@StockMovementType IS NULL OR  S.StockMovementType IN (0,1))
