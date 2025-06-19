using Dapper;

using SPORTLIGHTS_SERVER.Areas.Admin.DTOs.Orders;
using SPORTLIGHTS_SERVER.Areas.Admin.Repository.Orders.Abstractions;
using SPORTLIGHTS_SERVER.Entities;
using SPORTLIGHTS_SERVER.Modules;
using System.Data;

namespace SPORTLIGHTS_SERVER.Areas.Admin.Repository.OrderRepository
{
	public class OrderRepository : IOrderRepository
	{

		public List<Order> GetOrders(OrderFilterDto filter)
		{
			if (!string.IsNullOrEmpty(filter.SearchValue))
				filter.SearchValue = "%" + filter.SearchValue + "%";

			using (var connection = ConnectDB.LiteCommerceDB())
			{
				var sql = @"SELECT  *
                    FROM    (
                            SELECT  o.*,
                                    c.CustomerName,
                                    c.ContactName as CustomerContactName,
                                    c.Address as CustomerAddress,
                                    c.Email as CustomerEmail,
                                    e.FullName as EmployeeFullName,
                                    s.ShipperName,
                                    s.Phone as ShipperPhone,
                                    ROW_NUMBER() OVER(ORDER BY o.OrderID DESC) AS RowNumber
                            FROM    Orders as o
                                    LEFT JOIN Customers AS c ON o.CustomerID = c.CustomerID
                                    LEFT JOIN Employees AS e ON o.EmployeeID = e.EmployeeID
                                    LEFT JOIN Shippers AS s ON o.ShipperID = s.ShipperID
                            WHERE   (@Status = 0 OR o.Status = @Status)
                                AND (@SearchValue = N'' OR c.CustomerName LIKE @SearchValue OR s.ShipperName LIKE @SearchValue)
                            ) AS t
                    WHERE (@PageSize = 0) OR (t.RowNumber BETWEEN(@Page -1)*@PageSize + 1 AND @Page*@PageSize)
                    ORDER BY t.RowNumber";
				var parameters = new
				{
					page = filter.Page,
					pageSize = filter.PageSize,
					Status = filter.Status,
					searchValue = filter.SearchValue
				};
				return connection.Query<Order>(sql: sql, param: parameters, commandType: CommandType.Text).ToList();
			}
		}

		public Order? GetOrderById(int orderId)
		{
			using (var connection = ConnectDB.LiteCommerceDB())
			{
				var sql = @"SELECT  o.*,  
                            c.CustomerName,
                            c.ContactName as CustomerContactName,
                            c.Address as CustomerAddress,
                            c.Email as CustomerEmail,
                            e.FullName as EmployeeFullName,
                            s.ShipperName,
                            s.Phone as ShipperPhone
                    FROM    Orders as o
                            LEFT JOIN Customers AS c ON o.CustomerID = c.CustomerID
                            LEFT JOIN Employees AS e ON o.EmployeeID = e.EmployeeID
                            LEFT JOIN Shippers AS s ON o.ShipperID = s.ShipperID
                    WHERE   o.OrderID = @OrderID";
				return connection.QueryFirstOrDefault<Order>(sql: sql, param: new { OrderID = orderId }, commandType: CommandType.Text);
			}
		}

		public int CreateOrder(CreateOrderDto dto)
		{
			var data = new Order
			{
				CustomerID = dto.CustomerID,
				OrderTime = dto.OrderTime,
				EmployeeID = dto.EmployeeID,
				AcceptTime = dto.AcceptTime,
				ShipperID = dto.ShipperID,
				ShippedTime = dto.ShippedTime,
				FinishedTime = dto.FinishedTime,
				Status = dto.Status,
				DeliveryAddress = dto.DeliveryAddress,
				DeliveryProvince = dto.DeliveryProvince
			};

			var details = dto.Details.Select(d => new OrderDetail
			{
				ProductID = d.ProductID,
				Quantity = d.Quantity,
				SalePrice = d.SalePrice
			});

			using (var connection = ConnectDB.LiteCommerceDB())
			{
				var sqlAddOrder = @"IF EXISTS(SELECT * FROM Orders WHERE OrderID = @OrderID)
                            SELECT -1
                        ELSE
                        BEGIN
                            INSERT INTO Orders(CustomerID, OrderTime, EmployeeID, AcceptTime, ShipperID, ShippedTime, FinishedTime, Status, DeliveryAddress, DeliveryProvince)
                            VALUES(@CustomerID, @OrderTime, @EmployeeID, @AcceptTime, @ShipperID, @ShippedTime, @FinishedTime, @Status, @DeliveryAddress, @DeliveryProvince);
                            SELECT @@IDENTITY;
                        END";

				var orderID = connection.ExecuteScalar<int>(sqlAddOrder, data);

				var sqlAddOrderDetail = @"INSERT INTO OrderDetails(OrderID, ProductID, Quantity, SalePrice) VALUES(@OrderID, @ProductID, @Quantity, @SalePrice)";
				foreach (var item in details)
				{
					connection.Execute(sqlAddOrderDetail, new { OrderID = orderID, item.ProductID, item.Quantity, item.SalePrice });
				}

				return orderID;
			}
		}

		public bool UpdateOrder(EditOrderDto dto)
		{
			var order = new Order
			{
				OrderID = dto.OrderId,
				CustomerID = dto.CustomerId,
				OrderTime = dto.OrderTime,
				EmployeeID = dto.EmployeeId,
				AcceptTime = dto.AcceptTime,
				ShipperID = dto.ShipperId,
				ShippedTime = dto.ShippedTime,
				FinishedTime = dto.FinishedTime,
				Status = dto.Status
			};

			using (var connection = ConnectDB.LiteCommerceDB())
			{
				var sql = @"UPDATE Orders
                    SET CustomerID = @CustomerID,
                        OrderTime = @OrderTime,
                        EmployeeID = @EmployeeID,
                        AcceptTime = @AcceptTime,
                        ShipperID = @ShipperID,
                        ShippedTime = @ShippedTime,
                        FinishedTime = @FinishedTime,
                        Status = @Status
                    WHERE OrderID = @OrderID";
				return connection.Execute(sql, order) > 0;
			}
		}

		public bool DeleteOrder(int orderId)
		{
			using (var connection = ConnectDB.LiteCommerceDB())
			{
				var sql = @"DELETE FROM OrderDetails WHERE OrderID = @OrderID;
                    DELETE FROM Orders WHERE OrderID = @OrderID;";
				return connection.Execute(sql, new { OrderID = orderId }) > 0;
			}
		}

		public int Count(OrderFilterDto filter)
		{
			if (!string.IsNullOrEmpty(filter.SearchValue))
				filter.SearchValue = "%" + filter.SearchValue + "%";

			using (var connection = ConnectDB.LiteCommerceDB())
			{
				var sql = @"SELECT COUNT(*)
                    FROM Orders AS o
                        LEFT JOIN Customers AS c ON o.CustomerID = c.CustomerID
                        LEFT JOIN Shippers AS s ON o.ShipperID = s.ShipperID
                    WHERE (@Status = 0 OR o.Status = @Status)
                        AND (@SearchValue = N'' OR c.CustomerName LIKE @SearchValue OR s.ShipperName LIKE @SearchValue)";

				return connection.ExecuteScalar<int>(sql, new { filter.Status, filter.SearchValue });
			}
		}

	}
}