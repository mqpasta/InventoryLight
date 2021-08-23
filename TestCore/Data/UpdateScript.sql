Use [PastaPackages]
Go

--begin tran
--update StockMovement set Date = '2021-01-01' Where StockMovementType = 2
--select top 10 * from StockMovement WHERE StockMovementType=2
--commit

select * from StockStatus 