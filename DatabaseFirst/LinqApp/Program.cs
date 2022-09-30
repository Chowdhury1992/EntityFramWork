using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace LinqApp
{
    internal class Program
    {
        static void Main(string[] args)
        {
            using (var context = new NorthwindContext())
            {
                #region CRUD operations
                //Employee employee = new Employee()
                //{

                //    FirstName = "Chowdhury",
                //    LastName = "Ahmed"
                //};
                //var employeeQuery = from e in context.Employees
                //               where e.FirstName == employee.FirstName &&
                //               e.LastName == employee.LastName
                //               select e;
                ////Create
                //if (employeeQuery.FirstOrDefault()==null)
                //{
                //    context.Add(employee);
                //    context.SaveChanges();
                //}
                ////Read
                //foreach(var e in context.Employees)
                //{
                //    Console.WriteLine($"Employee Name: {e.FirstName} {e.LastName}");
                //}

                ////Update
                //var FindEmployee =context.Employees.Find(1011);
                //if (FindEmployee != null)
                //{
                //    FindEmployee.Country = "UK";
                //    context.SaveChanges();
                //}

                ////Delete
                //if (FindEmployee !=null)
                //{

                //    context.Employees.Remove(FindEmployee);
                //    context.SaveChanges();
                //}

                #endregion
                #region Query Syntax
                //Customers who live in Paris or London
                var customerDetails = from c in context.Customers
                                      where c.City=="London" || c.City =="Paris"
                                      select new {c.CustomerId, c.CompanyName, c.Address, c.City, c.PostalCode, c.Country};
                foreach(var c in customerDetails)
                {
                    Console.WriteLine(c);
                }

                //Products that stored in bottles
                var productsStoredInBottles = from p in context.Products
                                              where p.QuantityPerUnit.Contains("bottles")
                                              select new {p.ProductName, p.QuantityPerUnit};
                foreach(var p in productsStoredInBottles)
                {
                    Console.WriteLine($"ProductName: {p.ProductName} QuantityPerUnit: {p.QuantityPerUnit}");
                }
                //Product details with their supplier details
                var productDetailsWithSuppliersNameAndCountry = from p in context.Products
                                                                join s in context.Suppliers
                                                                on p.SupplierId equals s.SupplierId
                                                                where p.QuantityPerUnit.Contains("bottles")
                                                                select new {p.ProductName, p.QuantityPerUnit ,s.CompanyName, s.Country };
                foreach(var p in productDetailsWithSuppliersNameAndCountry)
                {
                    Console.WriteLine($"Product Name: {p.ProductName} Quantity per unit: {p.QuantityPerUnit} Company name: {p.CompanyName} Country: {p.Country}");
                }

                //Number of products in each category
                var productsByCategory =
                    from products in context.Products
                    join category in context.Categories on products.CategoryId equals category.CategoryId
                    group products by products.CategoryId into productGroup
                    select new
                    {
                        CategoryID = productGroup.Key,
                        NumberOfProducts = productGroup.Count()
                    };



                foreach (var productgroup in productsByCategory.ToList())
                {

                    Console.WriteLine($"Category {context.Categories.Find(productgroup.CategoryID).CategoryName} , No Of Products {productgroup.NumberOfProducts}");
                }
                //Employee who lives in London
                var employeeNameAndCity = from emp in context.Employees
                                          where emp.City == "London"
                                          select new {emp.TitleOfCourtesy, emp.FirstName, emp.LastName, emp.City };
                foreach (var emp in employeeNameAndCity)
                {
                    Console.WriteLine($"Employee Name: {emp.TitleOfCourtesy} {emp.FirstName} {emp.LastName}, City: {emp.City}");
                }
                //Number of orders more than 100 and shiped country USA ok UK
                var numberOfOrder = from o in context.Orders
                                     where (o.ShipCountry == "USA" || o.ShipCountry == "UK") && o.Freight > 100
                                     select o.OrderId;
                
                 Console.WriteLine($" No of Orders> 100 from USA or UK: {numberOfOrder.Count()} ");
                //Maximum amount of discount given to a orderId
                var maximumNumberOfdiscountGivenToAnOrderId =
                     from od in context.OrderDetails
                     let maximumDiscount = (from orderDetail in context.OrderDetails
                                            select orderDetail.UnitPrice * orderDetail.Quantity * Convert.ToDecimal(orderDetail.Discount)).Max()
                     where od.UnitPrice * od.Quantity * Convert.ToDecimal(od.Discount) == maximumDiscount
                     select new { od.OrderId, maximumDiscount };
                foreach (var od in maximumNumberOfdiscountGivenToAnOrderId)
                {
                    Console.WriteLine($"OrderId:{od.OrderId} Maximum amount of discount given to a orderId: {string.Format("{0:c}" ,od.maximumDiscount.ToString("c", new CultureInfo("en-US")))}");
                }
                //Employee Name and Reports to
                var ListOfEmployees = from emp in context.Employees
                                      join empReportTo in context.Employees
                                      on emp.ReportsTo equals empReportTo.EmployeeId

                                      select new { ReportingEmployeeFirstName=emp.FirstName, ReportingEmployeeLastName=emp.LastName, REmployeeFirstName=empReportTo.FirstName, REmployeeLastName=empReportTo.LastName };
                foreach (var emp in ListOfEmployees)
                {
                    Console.WriteLine($"Employee Name: {emp.ReportingEmployeeFirstName} {emp.ReportingEmployeeLastName} , ReportsTo: {emp.REmployeeFirstName} {emp.REmployeeLastName}");
                }

                #endregion
                #region Method Syntax
                //Customers who live in Paris or London
                var CustomerFromParisOrLondon = context.Customers.Where(c => c.City == "London" || c.City == "Paris").Select(c => new { c.CustomerId, c.CompanyName, c.Address, c.City, c.PostalCode, c.Country });
                foreach (var c in CustomerFromParisOrLondon)
                {
                    Console.WriteLine(c);
                }
                //Employee who lives in London
                var EmployeeLivesInLondon = context.Employees.Where(e => e.City == "London").Select(e => new { e.TitleOfCourtesy, e.FirstName, e.LastName, e.City });
                foreach (var emp in EmployeeLivesInLondon)
                {
                    Console.WriteLine($"Employee Name: {emp.TitleOfCourtesy} {emp.FirstName} {emp.LastName}, City: {emp.City}");
                }
                //Products that stored in bottles
                var productsInBottles = context.Products.Where(p => p.QuantityPerUnit.Contains("bottles")).Select(p => new { p.ProductName, p.QuantityPerUnit });
                foreach (var p in productsInBottles)
                {
                    Console.WriteLine($"ProductName: {p.ProductName} QuantityPerUnit: {p.QuantityPerUnit}");
                }
                //Product details with their supplier details
                var productsAndSupplierDetails = context.Products.Join(context.Suppliers, p=>p.SupplierId, s=> s.SupplierId,(p,s)=> new {p.ProductName, p.QuantityPerUnit,s.CompanyName, s.Country}).Where(pro=> pro.QuantityPerUnit.Contains("bottles")).Select (p=>new { p.ProductName, p.QuantityPerUnit, p.CompanyName, p.Country });

                foreach (var p in productsAndSupplierDetails)
                {
                    Console.WriteLine($"Product Name: {p.ProductName} Quantity per unit: {p.QuantityPerUnit} Company name: {p.CompanyName} Country: {p.Country}");
                }
                //Number of products in each category
                var numOfProductsInEachCategory = context.Products.Join(context.Categories, p => p.CategoryId, cat => cat.CategoryId, (pro, cat) => new { pro.CategoryId, cat.Products}).GroupBy(pro => new { pro.CategoryId, p=pro.Products.Count() }).Select(pro=> new { pro.Key});

                foreach (var pro in numOfProductsInEachCategory)
                {
                    
                        Console.WriteLine($"Category: {context.Categories.Find(pro.Key.CategoryId).CategoryName} , No Of Products: {pro.Key.p} ");
                    
                }
                //Number of orders more than 100 and shiped country USA ok UK
                var orderDetails = context.Orders.Where(o => (o.ShipCountry == "USA" || o.ShipCountry == "UK") && o.Freight > 100).Select(o => new { o.OrderId }).Count();

                Console.WriteLine($"Number of Orders: {orderDetails}");

                //Maximum amount of discount given to a orderId
                var maxDiscount = context.OrderDetails.Max(o => o.UnitPrice * o.Quantity * Convert.ToDecimal(o.Discount));
                var maxDiscountOfAnOrderId = context.OrderDetails.Where(o => o.UnitPrice * o.Quantity * Convert.ToDecimal(o.Discount) == maxDiscount).Select(or => new { or.OrderId, maxDiscount});
                foreach (var od in maxDiscountOfAnOrderId)
                {
                    Console.WriteLine($"OrderId:{od.OrderId} Maximum amount of discount given to a orderId: {string.Format("{0:c}", od.maxDiscount.ToString("C", new CultureInfo("en-US")))}");
                }

                //Employee Name and Reports to
                var employeeDetails = context.Employees.Join(context.Employees, e1 => e1.ReportsTo, e2 => e2.EmployeeId, (em1, em2) => new { em1.TitleOfCourtesy, em1.FirstName, em1.LastName, reEmployeeFName=em2.FirstName, reEmployeeLName=em2.LastName }).Select(emp => new { emp.TitleOfCourtesy, emp.FirstName, emp.LastName, emp.reEmployeeFName, emp.reEmployeeLName });
                foreach (var emp in employeeDetails)
                {
                    Console.WriteLine($"Employee Name:  {emp.TitleOfCourtesy} {emp.FirstName} {emp.LastName} , ReportsTo: {emp.reEmployeeFName} {emp.reEmployeeLName}");
                }
                #endregion

            }
        }
    }
}