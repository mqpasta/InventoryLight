Use [PastaPackages]
Go

BEGIN TRANSACTION
GO
ALTER TABLE dbo.PurchaseOrder ADD
	CostPrice decimal(18, 2) NOT NULL CONSTRAINT DF_PurchaseOrder_CostPrice DEFAULT 0
GO
ALTER TABLE dbo.PurchaseOrder SET (LOCK_ESCALATION = TABLE)
GO


/****** Object:  View [dbo].[ViewPurchaseOrders]    Script Date: 8/21/2021 3:49:18 PM ******/
DROP VIEW [dbo].[ViewPurchaseOrders]
GO

/****** Object:  View [dbo].[ViewPurchaseOrders]    Script Date: 8/21/2021 3:49:18 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE VIEW [dbo].[ViewPurchaseOrders]
AS
SELECT        PO.PurchaseOrderId, PO.PODate, PO.ProductId, PO.ConvRate, PO.RMBRate, PO.Quantity, PO.IsReceived, PO.CostPrice, ISNULL(SUM(SM.Qty), 0) AS ReceivedQuantity
FROM            dbo.PurchaseOrder AS PO LEFT OUTER JOIN
                         dbo.StockMovement AS SM ON SM.PurchaseOrderId IS NOT NULL AND PO.PurchaseOrderId = SM.PurchaseOrderId AND PO.ProductId = SM.ProductId
GROUP BY PO.PurchaseOrderId, PO.PODate, PO.ProductId, PO.ConvRate, PO.RMBRate, PO.Quantity, PO.IsReceived, PO.CostPrice
GO


COMMIT
