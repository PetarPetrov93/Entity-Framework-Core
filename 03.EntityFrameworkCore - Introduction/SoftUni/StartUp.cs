using SoftUni.Data;
using SoftUni.Models;
using System.Net;
using System.Net.WebSockets;
using System.Text;

namespace SoftUni
{
    public class StartUp
    {
        static void Main(string[] args)
        {
            SoftUniContext context = new SoftUniContext();

            //UNCOMMENT THE METHOD YOU NEED TO USE:

            //Task 3
            //string resultTask3 = GetEmployeesFullInformation(context);
            //Console.WriteLine(resultTask3);

            //Task 4
            //string resultTask4 = GetEmployeesWithSalaryOver50000(context);
            //Console.WriteLine(resultTask4);

            //Task 5
            //string resultTask5 = GetEmployeesFromResearchAndDevelopment(context);
            //Console.WriteLine(resultTask5);

            //Task 6
            //string resultTask6 = AddNewAddressToEmployee(context);
            //Console.WriteLine(resultTask6);

            //Task 7
            //string resultTask7 = GetEmployeesInPeriod(context);
            //Console.WriteLine(resultTask7);

            //Problem 8
            //string resultTask8 = GetAddressesByTown(context);
            //Console.WriteLine(resultTask8);

            //Problem 9
            //string resultTask9 = GetEmployee147(context);
            //Console.WriteLine(resultTask9);

            //Problem 10
            //string resultTask10 = GetDepartmentsWithMoreThan5Employees(context);
            //Console.WriteLine(resultTask10);

            //Problem 11
            //string resultTask11 = GetLatestProjects(context);
            //Console.WriteLine(resultTask11);

            //Problem 12
            //string resultTask12 = IncreaseSalaries(context);
            //Console.WriteLine(resultTask12);

            //Problem 13
            //string resultTask13 = GetEmployeesByFirstNameStartingWithSa(context);
            //Console.WriteLine(resultTask13);

            //Problem 14
            //string resultTask14 = DeleteProjectById(context);
            //Console.WriteLine(resultTask14);

            //Problem 15
            //string resultTask15 = RemoveTown(context);
            //Console.WriteLine(resultTask15);
        }


        //Method for task 3
        public static string GetEmployeesFullInformation(SoftUniContext context)
        {
            StringBuilder sb = new StringBuilder();
            var employees = context.Employees.OrderBy(e => e.EmployeeId).Select(e => new
            {
                e.FirstName,
                e.LastName,
                e.MiddleName,
                e.JobTitle,
                e.Salary
            })
            .ToList();

            foreach ( var e in employees )
            {
                sb.AppendLine($"{e.FirstName} {e.LastName} {e.MiddleName} {e.JobTitle} {e.Salary:f2}");
            }
            return sb.ToString().TrimEnd();
        }

        //Method for task 4
        public static string GetEmployeesWithSalaryOver50000(SoftUniContext context)
        {
            StringBuilder sb = new StringBuilder();

            var employees = context.Employees
                .Where(e => e.Salary > 50000)
                .Select(e => new
                {
                    e.FirstName,
                    e.Salary
                })
                .OrderBy(e => e.FirstName)
                .ToList();

            foreach ( var e in employees )
            {
                sb.AppendLine($"{e.FirstName} - {e.Salary:f2}");
            }

            return sb.ToString().TrimEnd();
        }

        //Method for task 5
        public static string GetEmployeesFromResearchAndDevelopment(SoftUniContext context)
        {
            StringBuilder sb = new StringBuilder();

            var employees = context.Employees
                .Where(e => e.Department.Name == "Research and Development")
                .OrderBy(e => e.Salary)
                .ThenByDescending(e =>  e.FirstName)
                .Select(e => new
                {
                    e.FirstName,
                    e.LastName,
                    Department = e.Department.Name,
                    e.Salary
                })
                .ToList();

            foreach ( var e in employees)
            {
                sb.AppendLine($"{e.FirstName} {e.LastName} from {e.Department} - ${e.Salary:f2}");
            }
            return sb.ToString().TrimEnd();
        }

        //Method for task 6
        public static string AddNewAddressToEmployee(SoftUniContext context)
        {
            Address newAddress = new Address()
            {
                AddressText = "Vitoshka 15",
                TownId = 4
            };

            //context.Addresses.Add(newAddress); -> this is how you add new Address, however it is not needed here since it will be automatically added by EFC
            //upon updating employeeNakov.Address

            var employeeNakov = context.Employees.FirstOrDefault(e => e.LastName == "Nakov");
            employeeNakov!.Address = newAddress; // the "!" after the employeeNakov assures that this will NOT be null

            context.SaveChanges();

            var employeesAddresses = context.Employees
                .OrderByDescending(e => e.AddressId)
                .Take(10)
                .Select(e => e.Address!.AddressText)
                .ToArray();

            return string.Join(Environment.NewLine, employeesAddresses);
        }

        //Method for task 7 THIS IS AN EXAMPLE OF HOW TO USE MAPPING TABLE TO ACCESS THE INFORMATION FROM THE 2ND TABLE FROM THE FIRST
        // THIS EXAMPLE IS QUITE COMPLEX!
        public static string GetEmployeesInPeriod(SoftUniContext context)
        {
            StringBuilder sb = new StringBuilder();

            var employeesWithProjects = context.Employees
                .Take(10)
                .Select(e => new
                {
                    e.FirstName,
                    e.LastName,
                    ManagerFirstName = e.Manager!.FirstName,
                    ManagerLastName = e.Manager!.LastName,
                    Projects = e.EmployeesProjects // this is how to get a collection of results from the 2nd mapped table
                                .Where(ep => ep.Project.StartDate.Year >= 2001 && // this row shows how we can access the 2nd mapped table properties
                                        ep.Project.StartDate.Year <= 2003)
                                .Select(ep => new 
                                {
                                    PojectName = ep.Project.Name,
                                    StartDate = ep.Project!.StartDate.ToString("M/d/yyyy h:mm:ss tt"),
                                    EndDate = ep.Project.EndDate.HasValue ? ep.Project.EndDate.Value.ToString("M/d/yyyy h:mm:ss tt") : "not finished"
                                })
                                .ToArray()
                })
                .ToArray();

            foreach ( var e in employeesWithProjects)
            {
                sb.AppendLine($"{e.FirstName} {e.LastName} - Manager: {e.ManagerFirstName} {e.ManagerLastName}");

                foreach (var p in e.Projects)
                {
                    sb.AppendLine($"--{p.PojectName} - {p.StartDate} - {p.EndDate}");
                }
            }

            return sb.ToString().TrimEnd();
        }

        //Method for task 8
        public static string GetAddressesByTown(SoftUniContext context)
        {
            StringBuilder sb = new StringBuilder();

            var addresses = context.Addresses
                            .OrderByDescending(a => a.Employees.Count)
                            .ThenBy(a => a.Town!.Name)
                            .ThenBy(a => a.AddressText)
                            .Take(10)
                            .Select(a => new
                            {
                                Text = a.AddressText,
                                Town = a.Town!.Name,
                                EmployeesCount = a.Employees.Count
                            })
                            .ToArray();

            foreach ( var a in addresses)
            {
                sb.AppendLine($"{a.Text}, {a.Town} - {a.EmployeesCount} employees");
            }

            return sb.ToString().TrimEnd();
        }

        //Method for task 9
        public static string GetEmployee147(SoftUniContext context)
        {
            StringBuilder sb = new StringBuilder();

            var employees = context.Employees
                            .Where(e => e.EmployeeId == 147)
                            .Select(e => new
                            {
                                e.FirstName,
                                e.LastName,
                                e.JobTitle,
                                ProjectNames = e.EmployeesProjects //Example on how to get the info from the 2nd table in a composite key
                                        .Select(ep => ep.Project.Name)
                                        .OrderBy(e => e)
                                        .ToArray()
                            })
                            .ToArray();

            foreach ( var e in employees)
            {
                sb.AppendLine($"{e.FirstName} {e.LastName} - {e.JobTitle}");

                foreach (var p in e.ProjectNames)
                {
                    sb.AppendLine(p);
                }
            }

            return sb.ToString().TrimEnd();
        }

        //Method for task 10
        public static string GetDepartmentsWithMoreThan5Employees(SoftUniContext context)
        {
            StringBuilder sb = new StringBuilder();

            var departments = context.Departments
                            .Where(d => d.Employees.Count > 5)
                            .OrderBy(d => d.Employees.Count)
                            .ThenBy(d => d.Name)
                            .Select(d => new
                            {
                                d.Name,
                                ManagerFirstName = d.Manager.FirstName,
                                ManagerLastName= d.Manager.LastName,
                                EmployeesNames = d.Employees
                                                 .Select(e => new
                                                 {
                                                     e.FirstName,
                                                     e.LastName,
                                                     e.JobTitle
                                                 })
                                                 .OrderBy(e => e.FirstName)
                                                 .ThenBy(e => e.LastName)
                                                 .ToArray()
                            }).ToArray();

            foreach ( var d in departments)
            {
                sb.AppendLine($"{d.Name} - {d.ManagerFirstName} {d.ManagerLastName}");

                foreach (var e in d.EmployeesNames)
                {
                    sb.AppendLine($"{e.FirstName} {e.LastName} - {e.JobTitle}");
                }
            }

            return sb.ToString().TrimEnd();
        }

        //Method for task 11
        public static string GetLatestProjects(SoftUniContext context)
        {
            StringBuilder sb = new StringBuilder();

            var projects = context.Projects
                           .OrderByDescending(p => p.StartDate)
                           .Take(10)
                           .Select(p => new
                           {
                               p.Name,
                               p.Description,
                               StartDate = p.StartDate.ToString("M/d/yyyy h:mm:ss tt")
                           })
                           .OrderBy(p=>p.Name)
                           .ToArray();

            foreach (var p in projects)
            {
                sb.AppendLine(p.Name);
                sb.AppendLine(p.Description);
                sb.AppendLine(p.StartDate);
            }

            return sb.ToString().TrimEnd();
        }

        //Method for task 12
        public static string IncreaseSalaries(SoftUniContext context)
        {
            StringBuilder sb = new StringBuilder();

            var employees = context.Employees
                            .Where(e => e.Department.Name == "Engineering" ||
                                        e.Department.Name == "Tool Design" ||
                                        e.Department.Name == "Marketing" ||
                                        e.Department.Name == "Information Services")
                            .Select(e => new
                            {
                                e.FirstName,
                                e.LastName,
                                Salary = e.Salary + (e.Salary * 0.12m)
                            })
                            .OrderBy(e => e.FirstName)
                            .ThenBy(e => e.LastName);

            foreach (var e in employees)
            {
                sb.AppendLine($"{e.FirstName} {e.LastName} (${e.Salary:f2})");
            }

            return sb.ToString().TrimEnd();
        }

        //Task 13
        public static string GetEmployeesByFirstNameStartingWithSa(SoftUniContext context)
        {
            StringBuilder sb = new StringBuilder();

            var employees = context.Employees
                            .Where(e => e.FirstName.Substring(0,2).ToLower() == "sa")
                            .Select(e => new
                            {
                                e.FirstName,
                                e.LastName,
                                e.JobTitle,
                                e.Salary
                            })
                            .OrderBy(e => e.FirstName)
                            .ThenBy(e => e.LastName)
                            .ToArray();

            foreach (var e in employees)
            {
                sb.AppendLine($"{e.FirstName} {e.LastName} - {e.JobTitle} - (${e.Salary:f2})");
            }

            return sb.ToString().TrimEnd();
        }

        //Task 14 Delete example. First we need to delete all rows from EmplopyeeProject mapping table in order to remove the relations
        public static string DeleteProjectById(SoftUniContext context)
        {
            var epToDelete = context.EmployeesProjects
                             .Where(ep => ep.ProjectId == 2);

            context.EmployeesProjects.RemoveRange(epToDelete);

            Project? projectToDelete = context.Projects.Find(2); // or we can also use FirstOrDefault()
            context.Projects.Remove(projectToDelete!);

            //context.SaveChanges(); <- uncomment for Judge

            var tenProjects = context.Projects
                              .Take(10)
                              .Select(p => p.Name)
                              .ToArray();
                    

            return string.Join(Environment.NewLine, tenProjects);
        }

        //Task 15
        public static string RemoveTown(SoftUniContext context)
        {
            var townToRemove = context.Towns.FirstOrDefault(t => t.Name == "Seattle");

            var employeesToSetAddressToNull = context.Employees
                                              .Where(e => e.Address!.Town!.Name == townToRemove!.Name) // instead of townToRemove.Name you can just type "Seattle"
                                              .Select(e => e.Address == null);

            var addressesToDelete = context.Addresses
                                    .Where(a => a.Town!.Name == townToRemove!.Name);

            int addressesCount = addressesToDelete.Count();

            context.Addresses.RemoveRange(addressesToDelete);

            context.Towns.Remove(townToRemove!);

            //context.SaveChanges(); -- UNCOMMENT FOR JUDGE

            return $"{addressesCount} addresses in {townToRemove!.Name} were deleted";
        }

    }
}
