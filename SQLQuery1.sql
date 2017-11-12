SELECT DISTINCT StateProvince FROM Address 
INNER JOIN SalesOrderHeader ON Address.AddressID = SalesOrderHeader.BillToAddressID



-- variable(s) is(are) declared
DECLARE @StateProvince NVARCHAR(100), @HowMany INT
SET @StateProvince = 'California'
SET @HowMany = 6
-- choose which columns are to be retrieved is to be retrieved 
SELECT TOP (@HowMany) ProductID, SUM(OrderQty)
FROM
--join all the necessary tables
	SalesOrderDetail 
INNER JOIN SalesOrderHeader ON SalesOrderDetail.SalesOrderID = SalesOrderHeader.SalesOrderID 
INNER JOIN Address ON SalesOrderHeader.BillToAddressID = Address.AddressID
-- now that all the necessary tables are joined, start filtering (choose which rows are to be retrieved)
WHERE
	StateProvince = @StateProvince
-- now that they are filtered, set how they are displayed.
GROUP BY 
	ProductID
	ORDER BY SUM(OrderQty) DESC


	
-- variable(s) is(are) declared
DECLARE @StateProvince NVARCHAR(100), @HowMany INT
SET @StateProvince = 'California'
SET @HowMany = 6
-- choose which columns are to be retrieved is to be retrieved 
SELECT TOP (@HowMany) ProductID, SUM(LineTotal)
FROM
--join all the necessary tables
	SalesOrderDetail 
INNER JOIN SalesOrderHeader ON SalesOrderDetail.SalesOrderID = SalesOrderHeader.SalesOrderID 
INNER JOIN Address ON SalesOrderHeader.BillToAddressID = Address.AddressID
-- now that all the necessary tables are joined, start filtering (choose which rows are to be retrieved)
WHERE
	StateProvince = @StateProvince
-- now that they are filtered, set how they are displayed.
GROUP BY 
	ProductID
	ORDER BY SUM(LineTotal) DESC
