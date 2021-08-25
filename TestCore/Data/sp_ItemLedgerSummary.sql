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
	@StartDate date = NULL, 
	@EndDate date = NULL,
	@Summary bit = 0
AS
	-- Storing opening balances before start date, if given 
	DECLARE @tblOpeningBal TABLE (
		ProductId bigint, LocationId bigint, OpeningBalQty int)

	IF @StartDate IS NOT NULL 
	BEGIN
		;with cte_bal_in AS(
		select ProductId, ToLocationId, SUM(Qty) AS InQty
		FROM StockMovement 
			WHERE ToLocationId IN (SELECT LocationId from Location where IsWearhouse=1)
			AND StockMovementType IN (2,0)
			AND (@StartDate IS NULL OR Date <@StartDate)
			AND (@ProductId IS NULL OR ProductId = @ProductId)
			AND (@LocationId IS NULL OR ToLocationId = @LocationId)
			Group By ProductId, ToLocationId
		), 
		cte_bal_out AS(
		select ProductId, FromLocationId, SUM(Qty) As OutQty
		FROM StockMovement 
			WHERE FromLocationId IS NOT NULL AND FromLocationId IN (SELECT LocationId from Location where IsWearhouse=1)
			AND StockMovementType IN (1)
			AND (@StartDate IS NULL OR Date < @StartDate)
			AND (@ProductId IS NULL OR ProductId = @ProductId)
			AND (@LocationId IS NULL OR FromLocationId = @LocationId)
			Group BY ProductId, FromLocationId
			)
		INSERT INTO @tblOpeningBal(ProductId, LocationId, OpeningBalQty)
		SELECT CASE WHEN I.ProductId IS NULL THEN O.ProductId ELSE I.ProductId END AS ProductId, 
				CASE WHEN I.ToLocationId IS NULL THEN O.FromLocationId ELSE I.ToLocationId END As LocationId, 
			(ISNULL(I.InQty,0) - ISNULL(O.OutQty,0)) AS OpeningBalQty
		FROM cte_bal_in I FULL Join cte_bal_out O 
			ON I.ProductId = O.ProductId AND I.ToLocationId = O.FromLocationId
	END

--SELECT * from @tblOpeningBal

	-- to store In/Out stock
	DECLARE @tblInOutQty TABLE (
		ProductId bigint, LocationId bigint, InQty int, OutQty int)

	;with cte_in AS(
	select ProductId, ToLocationId, SUM(Qty) AS InQty
	FROM StockMovement 
		WHERE ToLocationId IN (SELECT LocationId from Location where IsWearhouse=1)
		AND StockMovementType IN (2,0)
		AND (@StartDate IS NULL OR Date >= @StartDate)
		AND (@EndDate IS NULL OR Date <= @EndDate)
		AND (@ProductId IS NULL OR ProductId = @ProductId)
		AND (@LocationId IS NULL OR ToLocationId = @LocationId)
		Group By ProductId, ToLocationId
	), 
	cte_out AS(
	select ProductId, FromLocationId, SUM(Qty) As OutQty
	FROM StockMovement 
		WHERE FromLocationId IS NOT NULL AND FromLocationId IN (SELECT LocationId from Location where IsWearhouse=1)
		AND StockMovementType IN (1)
		AND (@StartDate IS NULL OR Date >= @StartDate)
		AND (@EndDate IS NULL OR Date <= @EndDate)
		AND (@ProductId IS NULL OR ProductId = @ProductId)
		AND (@LocationId IS NULL OR FromLocationId = @LocationId)
		Group BY ProductId, FromLocationId
		)
	INSERT INTO @tblInOutQty(ProductId, LocationId, InQty, OutQty)
	SELECT CASE WHEN I.ProductId IS NULL THEN O.ProductId ELSE I.ProductId END AS ProductId, 
			CASE WHEN I.ToLocationId IS NULL THEN O.FromLocationId ELSE I.ToLocationId END As LocationId, 
		(ISNULL(I.InQty,0)) As InQty, (ISNULL(O.OutQty,0)) AS OutQty
	FROM cte_in I FULL Join cte_out O 
		ON I.ProductId = O.ProductId AND I.ToLocationId = O.FromLocationId

	IF @Summary = 0 OR @LocationId IS NOT NULL
		Select P.ProductName, L.LocationName,
				ISNULL(OB.OpeningBalQty,0) AS OpeningBal,
				ISNULL(Q.InQty,0) AS InQty, 
				ISNULL(Q.OutQty,0) AS OutQty,
				ISNULL(OB.OpeningBalQty,0)+ISNULL(Q.InQty,0)-ISNULL(Q.OutQty,0) AS BalanceQty
		from StockStatus SS 
				Left Join @tblOpeningBal OB 
					ON SS.ProductId = OB.ProductId AND SS.LocationId = OB.LocationId
				Left Join @tblInOutQty Q ON SS.ProductId = Q.ProductId AND SS.LocationId = Q.LocationId
				Inner Join Product P ON P.ProductId = SS.ProductId 
				Inner Join Location L ON L.LocationId = SS.LocationId AND L.IsWearhouse=1
		WHERE OpeningBalQty>0 OR InQty>0 OR OutQty>0
	ELSE
		Select P.ProductName, '' AS LocationName,
				SUM(ISNULL(OB.OpeningBalQty,0)) AS OpeningBal,
				SUM(ISNULL(Q.InQty,0)) AS InQty, 
				SUM(ISNULL(Q.OutQty,0)) AS OutQty,
				SUM(ISNULL(OB.OpeningBalQty,0))+SUM(ISNULL(Q.InQty,0))-SUM(ISNULL(Q.OutQty,0)) AS BalanceQty
		from StockStatus SS 
				Left Join @tblOpeningBal OB 
					ON SS.ProductId = OB.ProductId AND SS.LocationId = OB.LocationId
				Left Join @tblInOutQty Q ON SS.ProductId = Q.ProductId AND SS.LocationId = Q.LocationId
				Inner Join Product P ON P.ProductId = SS.ProductId 
				--Inner Join Location L ON L.LocationId = SS.LocationId AND L.IsWearhouse=1
		WHERE OpeningBalQty>0 OR InQty>0 OR OutQty>0
		GROUP BY P.ProductName
	
GO

-- =============================================
-- Example to execute the stored procedure
-- =============================================
--EXECUTE dbo.ItemLedgerSummary 1,1,'2021-01-01',NULL,1
--GO
