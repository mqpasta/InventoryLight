CREATE DATABASE [PastaPackages2]
GO

USE [PastaPackages2]
GO
/****** Object:  Table [dbo].[Location]    Script Date: 8/7/2021 8:56:29 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Location](
	[LocationId] [bigint] IDENTITY(1,1) NOT NULL,
	[LocationCode] [int] NOT NULL,
	[LocationName] [nvarchar](50) NOT NULL,
	[LastUpdated] [datetime] NOT NULL,
 CONSTRAINT [PK_Location] PRIMARY KEY CLUSTERED 
(
	[LocationId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Product]    Script Date: 8/7/2021 8:56:29 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Product](
	[ProductId] [bigint] IDENTITY(1,1) NOT NULL,
	[ProductCode] [int] NOT NULL,
	[ProductName] [nvarchar](50) NOT NULL,
	[PurchasePrice] [decimal](18, 0) NOT NULL,
	[SalePrice] [decimal](18, 0) NOT NULL,
	[LastUpdate] [datetime] NOT NULL,
 CONSTRAINT [PK_Product] PRIMARY KEY CLUSTERED 
(
	[ProductId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[StockMovement]    Script Date: 8/7/2021 8:56:29 PM ******/
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
	[PurchasePrice] [decimal](18, 0) NOT NULL,
	[SalePrice] [decimal](18, 0) NOT NULL,
	[LastUpdate] [datetime] NOT NULL,
	[StockMovementType] [int] NOT NULL,
 CONSTRAINT [PK_StockMovement] PRIMARY KEY CLUSTERED 
(
	[StockMovementId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[StockStatus]    Script Date: 8/7/2021 8:56:29 PM ******/
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
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  View [dbo].[StockStatusView]    Script Date: 8/7/2021 8:56:29 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[StockStatusView]
AS
SELECT        P.ProductName, L.LocationName, SS.PurchaseQty AS InQuantity, SS.SaleQty AS OutQuantity, SS.PurchaseQty - SS.SaleQty AS BalanceQuantity, SS.ProductId, SS.LocationId
FROM            dbo.StockStatus AS SS INNER JOIN
                         dbo.Product AS P ON SS.ProductId = P.ProductId INNER JOIN
                         dbo.Location AS L ON L.LocationId = SS.LocationId
GO
ALTER TABLE [dbo].[Location] ADD  CONSTRAINT [DF_Location_LastUpdated]  DEFAULT (getdate()) FOR [LastUpdated]
GO
ALTER TABLE [dbo].[Product] ADD  CONSTRAINT [DF_Product_PurchasePrice]  DEFAULT ((0)) FOR [PurchasePrice]
GO
ALTER TABLE [dbo].[Product] ADD  CONSTRAINT [DF_Product_SalePrice]  DEFAULT ((0)) FOR [SalePrice]
GO
ALTER TABLE [dbo].[Product] ADD  CONSTRAINT [DF_Product_LastUpdate]  DEFAULT (getdate()) FOR [LastUpdate]
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
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane1', @value=N'[0E232FF0-B466-11cf-A24F-00AA00A3EFFF, 1.00]
Begin DesignProperties = 
   Begin PaneConfigurations = 
      Begin PaneConfiguration = 0
         NumPanes = 4
         Configuration = "(H (1[40] 4[20] 2[20] 3) )"
      End
      Begin PaneConfiguration = 1
         NumPanes = 3
         Configuration = "(H (1 [50] 4 [25] 3))"
      End
      Begin PaneConfiguration = 2
         NumPanes = 3
         Configuration = "(H (1 [50] 2 [25] 3))"
      End
      Begin PaneConfiguration = 3
         NumPanes = 3
         Configuration = "(H (4 [30] 2 [40] 3))"
      End
      Begin PaneConfiguration = 4
         NumPanes = 2
         Configuration = "(H (1 [56] 3))"
      End
      Begin PaneConfiguration = 5
         NumPanes = 2
         Configuration = "(H (2 [66] 3))"
      End
      Begin PaneConfiguration = 6
         NumPanes = 2
         Configuration = "(H (4 [50] 3))"
      End
      Begin PaneConfiguration = 7
         NumPanes = 1
         Configuration = "(V (3))"
      End
      Begin PaneConfiguration = 8
         NumPanes = 3
         Configuration = "(H (1[56] 4[18] 2) )"
      End
      Begin PaneConfiguration = 9
         NumPanes = 2
         Configuration = "(H (1 [75] 4))"
      End
      Begin PaneConfiguration = 10
         NumPanes = 2
         Configuration = "(H (1[66] 2) )"
      End
      Begin PaneConfiguration = 11
         NumPanes = 2
         Configuration = "(H (4 [60] 2))"
      End
      Begin PaneConfiguration = 12
         NumPanes = 1
         Configuration = "(H (1) )"
      End
      Begin PaneConfiguration = 13
         NumPanes = 1
         Configuration = "(V (4))"
      End
      Begin PaneConfiguration = 14
         NumPanes = 1
         Configuration = "(V (2))"
      End
      ActivePaneConfig = 0
   End
   Begin DiagramPane = 
      Begin Origin = 
         Top = 0
         Left = 0
      End
      Begin Tables = 
         Begin Table = "SS"
            Begin Extent = 
               Top = 6
               Left = 38
               Bottom = 136
               Right = 208
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "P"
            Begin Extent = 
               Top = 6
               Left = 246
               Bottom = 136
               Right = 416
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "L"
            Begin Extent = 
               Top = 138
               Left = 38
               Bottom = 268
               Right = 208
            End
            DisplayFlags = 280
            TopColumn = 0
         End
      End
   End
   Begin SQLPane = 
   End
   Begin DataPane = 
      Begin ParameterDefaults = ""
      End
   End
   Begin CriteriaPane = 
      Begin ColumnWidths = 11
         Column = 1440
         Alias = 900
         Table = 1170
         Output = 720
         Append = 1400
         NewValue = 1170
         SortType = 1350
         SortOrder = 1410
         GroupBy = 1350
         Filter = 1350
         Or = 1350
         Or = 1350
         Or = 1350
      End
   End
End
' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'StockStatusView'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPaneCount', @value=1 , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'StockStatusView'
GO
