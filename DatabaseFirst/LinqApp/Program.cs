using Microsoft.EntityFrameworkCore;

namespace LinqApp
{
    internal class Program
    {
        static void Main(string[] args)
        {
            using (var context = new NorthwindContext())
            {
                Employee employee = new Employee()
                {
                    
                    FirstName = "Chowdhury",
                    LastName = "Ahmed"
                };
                var employeeQuery = from e in context.Employees
                               where e.FirstName == employee.FirstName &&
                               e.LastName == employee.LastName
                               select e;
            
                if (employeeQuery.FirstOrDefault()==null)
                {
                    context.Add(employee);
                    context.SaveChanges();
                }

                foreach(var e in context.Employees)
                {
                    Console.WriteLine($"Employee Name: {e.FirstName} {e.LastName}");
                }
                var FindEmployee =context.Employees.Find(1011);
                if (FindEmployee != null)
                {
                    FindEmployee.Country = "UK";
                    context.SaveChanges();
                }
                if (FindEmployee !=null)
                {

                    context.Employees.Remove(FindEmployee);
                    context.SaveChanges();
                }
             


            }
        }
    }
}