CREATE DATABASE [PastaPackages]
GO

USE [PastaPackages]
GO
/****** Object:  Table [dbo].[Location]    Script Date: 8/15/2021 5:12:57 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Location](
	[LocationId] [bigint] IDENTITY(1,1) NOT NULL,
	[LocationCode] [int] NOT NULL,
	[LocationName] [nvarchar](50) NOT NULL,
	[LastUpdated] [datetime] NOT NULL,
	[IsWearhouse] [bit] NOT NULL,
 CONSTRAINT [PK_Location] PRIMARY KEY CLUSTERED 
(
	[LocationId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Product]    Script Date: 8/15/2021 5:12:57 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Product](
	[ProductId] [bigint] IDENTITY(1,1) NOT NULL,
	[ProductCode] [int] NOT NULL,
	[ProductName] [nvarchar](50) NOT NULL,
	[PurchasePrice] [decimal](18, 4) NOT NULL,
	[SalePrice] [decimal](18, 4) NOT NULL,
	[LastUpdate] [datetime] NOT NULL,
	[AvgPrice] [decimal](18, 4) NOT NULL,
 CONSTRAINT [PK_Product] PRIMARY KEY CLUSTERED 
(
	[ProductId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[StockStatus]    Script Date: 8/15/2021 5:12:57 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[StockStatus](
	[StockStatusId] [bigint] IDENTITY(1,1) NOT NULL,
	[ProductId] [bigint] NOT NULL,
	[LocationId] [bigint] NOT NULL,
	[PurchaseQty] [int] NOT NULL,
	[SaleQty] [int] NOT NULL,
	[LastUpdate] [datetime] NOT NULL,
 CONSTRAINT [PK_StockStatus] PRIMARY KEY CLUSTERED 
(
	[StockStatusId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  View [dbo].[StockStatusView]    Script Date: 8/15/2021 5:12:57 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[StockStatusView]
AS
SELECT        P.ProductName, L.LocationName, SS.PurchaseQty AS InQuantity, SS.SaleQty AS OutQuantity, SS.PurchaseQty - SS.SaleQty AS BalanceQuantity, SS.ProductId, SS.LocationId, P.AvgPrice
FROM            dbo.StockStatus AS SS INNER JOIN
                         dbo.Product AS P ON SS.ProductId = P.ProductId INNER JOIN
                         dbo.Location AS L ON L.LocationId = SS.LocationId
GO
/****** Object:  Table [dbo].[PurchaseOrder]    Script Date: 8/15/2021 5:12:57 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[PurchaseOrder](
	[PurchaseOrderId] [bigint] IDENTITY(1,1) NOT NULL,
	[PODate] [date] NOT NULL,
	[ProductId] [bigint] NOT NULL,
	[ConvRate] [decimal](18, 2) NOT NULL,
	[RMBRate] [decimal](18, 4) NOT NULL,
	[Quantity] [int] NOT NULL,
	[ReceivedQuantity] [int] NOT NULL,
	[IsReceived] [bit] NOT NULL,
 CONSTRAINT [PK_PurchaseOrder] PRIMARY KEY CLUSTERED 
(
	[PurchaseOrderId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[StockMovement]    Script Date: 8/15/2021 5:12:57 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[StockMovement](
	[StockMovementId] [bigint] IDENTITY(1,1) NOT NULL,
	[Date] [date] NOT NULL,
	[ProductId] [bigint] NOT NULL,
	[ToLocationId] [bigint] NOT NULL,
	[FromLocationId] [bigint] NULL,
	[Qty] [int] NOT NULL,
	[PurchasePrice] [decimal](18, 4) NOT NULL,
	[SalePrice] [decimal](18, 4) NOT NULL,
	[LastUpdate] [datetime] NOT NULL,
	[StockMovementType] [int] NOT NULL,
	[PurchaseOrderId] [bigint] NULL,
 CONSTRAINT [PK_StockMovement] PRIMARY KEY CLUSTERED 
(
	[StockMovementId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  View [dbo].[ViewPurchaseOrders]    Script Date: 8/15/2021 5:12:57 AM ******/
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
/****** Object:  View [dbo].[ViewWearhouseStock]    Script Date: 8/15/2021 5:12:57 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[ViewWearhouseStock]
AS
SELECT        dbo.Product.ProductName, SUM(dbo.StockStatus.PurchaseQty - dbo.StockStatus.SaleQty) AS BalanceQuantity, dbo.Product.AvgPrice
FROM            dbo.Location INNER JOIN
                         dbo.StockStatus ON dbo.Location.LocationId = dbo.StockStatus.LocationId INNER JOIN
                         dbo.Product ON dbo.StockStatus.ProductId = dbo.Product.ProductId
WHERE        (dbo.Location.IsWearhouse = 1)
GROUP BY dbo.Product.ProductName, dbo.Product.AvgPrice
GO
ALTER TABLE [dbo].[Location] ADD  CONSTRAINT [DF_Location_LastUpdated]  DEFAULT (getdate()) FOR [LastUpdated]
GO
ALTER TABLE [dbo].[Location] ADD  CONSTRAINT [DF_Location_IsWearhouse]  DEFAULT ((0)) FOR [IsWearhouse]
GO
ALTER TABLE [dbo].[Product] ADD  CONSTRAINT [DF_Product_PurchasePrice]  DEFAULT ((0)) FOR [PurchasePrice]
GO
ALTER TABLE [dbo].[Product] ADD  CONSTRAINT [DF_Product_SalePrice]  DEFAULT ((0)) FOR [SalePrice]
GO
ALTER TABLE [dbo].[Product] ADD  CONSTRAINT [DF_Product_LastUpdate]  DEFAULT (getdate()) FOR [LastUpdate]
GO
ALTER TABLE [dbo].[Product] ADD  CONSTRAINT [DF_Product_AvgPrice]  DEFAULT ((0.0)) FOR [AvgPrice]
GO
ALTER TABLE [dbo].[PurchaseOrder] ADD  CONSTRAINT [DF_PurchaseOrder_PODate]  DEFAULT (getdate()) FOR [PODate]
GO
ALTER TABLE [dbo].[PurchaseOrder] ADD  CONSTRAINT [DF_PurchaseOrder_ConvRate]  DEFAULT ((0.0)) FOR [ConvRate]
GO
ALTER TABLE [dbo].[PurchaseOrder] ADD  CONSTRAINT [DF_PurchaseOrder_RMBRate]  DEFAULT ((0.0)) FOR [RMBRate]
GO
ALTER TABLE [dbo].[PurchaseOrder] ADD  CONSTRAINT [DF_PurchaseOrder_Quantity]  DEFAULT ((0)) FOR [Quantity]
GO
ALTER TABLE [dbo].[PurchaseOrder] ADD  CONSTRAINT [DF_PurchaseOrder_ReceivedQuantity]  DEFAULT ((0)) FOR [ReceivedQuantity]
GO
ALTER TABLE [dbo].[PurchaseOrder] ADD  CONSTRAINT [DF_PurchaseOrder_IsReceived]  DEFAULT ((0)) FOR [IsReceived]
GO
ALTER TABLE [dbo].[StockMovement] ADD  CONSTRAINT [DF_StockMovement_Date]  DEFAULT (getdate()) FOR [Date]
GO
ALTER TABLE [dbo].[StockMovement] ADD  CONSTRAINT [DF_StockMovement_Qty]  DEFAULT ((0)) FOR [Qty]
GO
ALTER TABLE [dbo].[StockMovement] ADD  CONSTRAINT [DF_StockMovement_PurchasePrice]  DEFAULT ((0)) FOR [PurchasePrice]
GO
ALTER TABLE [dbo].[StockMovement] ADD  CONSTRAINT [DF_StockMovement_SalePrice]  DEFAULT ((0)) FOR [SalePrice]
GO
ALTER TABLE [dbo].[StockMovement] ADD  CONSTRAINT [DF_StockMovement_LastUpdate]  DEFAULT (getdate()) FOR [LastUpdate]
GO
ALTER TABLE [dbo].[StockMovement] ADD  CONSTRAINT [DF_StockMovement_StockMovementType]  DEFAULT ((0)) FOR [StockMovementType]
GO
ALTER TABLE [dbo].[StockStatus] ADD  CONSTRAINT [DF_StockStatus_PurchaseQty]  DEFAULT ((0)) FOR [PurchaseQty]
GO
ALTER TABLE [dbo].[StockStatus] ADD  CONSTRAINT [DF_StockStatus_SaleQty]  DEFAULT ((0)) FOR [SaleQty]
GO
ALTER TABLE [dbo].[StockStatus] ADD  CONSTRAINT [DF_StockStatus_LastUpdate]  DEFAULT (getdate()) FOR [LastUpdate]
GO
ALTER TABLE [dbo].[PurchaseOrder]  WITH CHECK ADD  CONSTRAINT [FK_PurchaseOrder_Product] FOREIGN KEY([ProductId])
REFERENCES [dbo].[Product] ([ProductId])
GO
ALTER TABLE [dbo].[PurchaseOrder] CHECK CONSTRAINT [FK_PurchaseOrder_Product]
GO
ALTER TABLE [dbo].[StockMovement]  WITH CHECK ADD  CONSTRAINT [FK_StockMovement_Location] FOREIGN KEY([ToLocationId])
REFERENCES [dbo].[Location] ([LocationId])
GO
ALTER TABLE [dbo].[StockMovement] CHECK CONSTRAINT [FK_StockMovement_Location]
GO
ALTER TABLE [dbo].[StockMovement]  WITH CHECK ADD  CONSTRAINT [FK_StockMovement_Location1] FOREIGN KEY([FromLocationId])
REFERENCES [dbo].[Location] ([LocationId])
GO
ALTER TABLE [dbo].[StockMovement] CHECK CONSTRAINT [FK_StockMovement_Location1]
GO
ALTER TABLE [dbo].[StockMovement]  WITH CHECK ADD  CONSTRAINT [FK_StockMovement_Product] FOREIGN KEY([ProductId])
REFERENCES [dbo].[Product] ([ProductId])
GO
ALTER TABLE [dbo].[StockMovement] CHECK CONSTRAINT [FK_StockMovement_Product]
GO
ALTER TABLE [dbo].[StockMovement]  WITH CHECK ADD  CONSTRAINT [FK_StockMovement_PurchaseOrder] FOREIGN KEY([PurchaseOrderId])
REFERENCES [dbo].[PurchaseOrder] ([PurchaseOrderId])
GO
ALTER TABLE [dbo].[StockMovement] CHECK CONSTRAINT [FK_StockMovement_PurchaseOrder]
GO
ALTER TABLE [dbo].[StockStatus]  WITH CHECK ADD  CONSTRAINT [FK_StockStatus_Location] FOREIGN KEY([LocationId])
REFERENCES [dbo].[Location] ([LocationId])
GO
ALTER TABLE [dbo].[StockStatus] CHECK CONSTRAINT [FK_StockStatus_Location]
GO
ALTER TABLE [dbo].[StockStatus]  WITH CHECK ADD  CONSTRAINT [FK_StockStatus_Product] FOREIGN KEY([ProductId])
REFERENCES [dbo].[Product] ([ProductId])
GO
ALTER TABLE [dbo].[StockStatus] CHECK CONSTRAINT [FK_StockStatus_Product]
GO
