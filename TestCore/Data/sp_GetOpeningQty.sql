-- =============================================
-- Create basic stored procedure template
-- =============================================

-- Drop stored procedure if it already exists
IF EXISTS (
  SELECT * 
	FROM INFORMATION_SCHEMA.ROUTINES 
   WHERE SPECIFIC_SCHEMA = N'dbo'
	 AND SPECIFIC_NAME = N'GetOpeningQty' 
)
   DROP PROCEDURE dbo.GetOpeningQty
GO

CREATE PROCEDURE dbo.GetOpeningQty
	@LocationId bigint = NULL,
	@ProductId bigint = NULL,
	@StartDate DATE = NULL
AS
	
	;with cte_in AS (
		select ProductId, Sum(Qty) as InQty
			from StockMovement --Inner Join Location L ON L.LocationId = StockMovement.ToLocationId and L.IsWearhouse =1
			where 1=1 
				AND (@ProductId IS NULL OR ProductId = @ProductId)
				AND (@LocationId IS NULL OR ToLocationId = @LocationId)
				AND (@StartDate IS NULL OR Date < @StartDate)
				group by ProductId --, ToLocationId
			),
			cte_out AS (
				select ProductId, Sum(Qty) AS OutQty
				from StockMovement --Inner Join Location L ON L.LocationId = StockMovement.FromLocationId and L.IsWearhouse =1
				where 1=1 
					AND (@ProductId IS NULL OR ProductId = @ProductId)
					and FromLocationId IS NOT NULL 
					AND (@LocationId IS NULL OR FromLocationId = @LocationId)
					AND (@StartDate IS NULL OR Date < @StartDate)
				group by ProductId --, FromLocationId
) Select (ISNULL(InQty,0) - ISNULL(OutQty,0)) AS Bal
	from cte_in I FULL outer Join cte_out O ON I.ProductId=O.ProductId 	

GO

-- exec GetOpeningQty 1, 30, NULL --'2021-08-20'


