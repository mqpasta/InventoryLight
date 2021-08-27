--Use [PastaPackages]
--Go

--begin tran
--update StockMovement set Date = '2021-01-01' Where StockMovementType = 2
--select top 10 * from StockMovement WHERE StockMovementType=2
--commit


--- CHANGES BRANCH: CostPrice
--- Removing COST PRICE FROM PURCHASEORDER TABLE

USE [PastaPackages]
GO

/****** Object:  View [dbo].[ViewPurchaseOrders]    Script Date: 27/08/2021 11:16:50 pm ******/
DROP VIEW [dbo].[ViewPurchaseOrders]
GO


/****** Object:  View [dbo].[ViewPurchaseOrders]    Script Date: 27/08/2021 11:17:02 pm ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE VIEW [dbo].[ViewPurchaseOrders]
AS
SELECT        PO.PurchaseOrderId, PO.PODate, PO.ProductId, PO.ConvRate, PO.RMBRate, PO.Quantity, PO.IsReceived, ISNULL(SUM(SM.Qty), 0) AS ReceivedQuantity
FROM            dbo.PurchaseOrder AS PO LEFT OUTER JOIN
                         dbo.StockMovement AS SM ON SM.PurchaseOrderId IS NOT NULL AND PO.PurchaseOrderId = SM.PurchaseOrderId AND PO.ProductId = SM.ProductId
GROUP BY PO.PurchaseOrderId, PO.PODate, PO.ProductId, PO.ConvRate, PO.RMBRate, PO.Quantity, PO.IsReceived
GO

IF EXISTS(SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS
	WHERE COLUMN_NAME='CostPrice' AND TABLE_NAME='PurchaseOrder')
BEGIN
		begin tran
		ALTER TABLE PurchaseOrder
			DROP  CONSTRAINT DF_PurchaseOrder_CostPrice
		ALTER TABLE PurchaseOrder
			Drop Column CostPrice
		commit
END