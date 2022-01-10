using EF_Activity001;
using InventoryHelpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Linq;
using EF_Activity001.DTOs;

namespace ConsoleApp1
{
    class Program
    {
        static IConfigurationRoot _configuration;
        static DbContextOptionsBuilder<AdventureWorksContext>
        _optionsBuilder;
        static void BuildOptions()
        {
            _configuration = ConfigurationBuilderSingleton.ConfigurationRoot;
            _optionsBuilder = new DbContextOptionsBuilder<AdventureWorksContext>();
            _optionsBuilder.UseSqlServer(_configuration.GetConnectionString
            ("AdventureWorks"));
        }
        static void Main(string[] args)
        {
            BuildOptions();

            var input = string.Empty;
            Console.WriteLine("Would you like to view the sales report?");
            input = Console.ReadLine();
            if (input.StartsWith("y", StringComparison.OrdinalIgnoreCase))
            {
                //GenerateSalesReport();
                GenerateSalesReportToDTO();
            }
        }

        private static void GenerateSalesReportToDTO()
        {
            decimal filter = GetFilterFromUser();

            using (var db = new AdventureWorksContext(_optionsBuilder.Options))
            {
                var salesReportDetails = db.SalesPeople.Select(sp => new SalesReportListingDto
                {
                    BusinessEntityId = sp.BusinessEntityId,
                    FirstName = sp.BusinessEntity.BusinessEntity.FirstName,
                    LastName = sp.BusinessEntity.BusinessEntity.LastName,
                    SalesYtd = sp.SalesYtd,
                    Territories = sp.SalesTerritoryHistories
                        .Select(y => y.Territory.Name),
                    TotalOrders = sp.SalesOrderHeaders.Count(),
                    TotalProductsSold = sp.SalesOrderHeaders
                        .SelectMany(y => y.SalesOrderDetails)
                        .Sum(z => z.OrderQty)
                }).Where(srds => srds.SalesYtd > filter)
                .OrderBy(srds => srds.LastName)
                .ThenBy(srds => srds.FirstName)
                .ThenByDescending(srds => srds.SalesYtd);

                foreach (var srd in salesReportDetails)
                {
                    Console.WriteLine(srd);
                }
            }
        }

        private static void ShowAllSalesPeople()
        {
            using (var db = new AdventureWorksContext(_optionsBuilder.
            Options))
            {
                var salesPeople = db.SalesPeople
                    .Include(x => x.BusinessEntity)
                    .ThenInclude(y => y.BusinessEntity)
                    .Take(20).ToList();

                foreach (var sp in salesPeople)
                {

                    Console.WriteLine($"{sp} | {sp.BusinessEntity.BusinessEntity.LastName}" + $", {sp.BusinessEntity.BusinessEntity.FirstName}");

                }
            }
        }

        private static void ShowAllSalesPeopleUsingProjection()
        {
            using (var db = new AdventureWorksContext(_optionsBuilder.Options))
            {
                var salesPeople = db.SalesPeople
                    .Select(x => new
                    {
                        x.BusinessEntityId,
                        x.BusinessEntity.BusinessEntity.FirstName,
                        x.BusinessEntity.BusinessEntity.LastName,
                        x.SalesQuota,
                        x.SalesYtd,
                        x.SalesLastYear
                    }).ToList();

                foreach (var sp in salesPeople)
                {
                    Console.WriteLine($"BID: {sp.BusinessEntityId} | Name: {sp.LastName}" + $", {sp.FirstName} | Quota: {sp.SalesQuota} | " +
                    $"YTD Sales: {sp.SalesYtd} | SalesLastYear {sp.SalesLastYear}");
                }
            }
        }

        static void GenerateSalesReport()
        {
            decimal filter = GetFilterFromUser();

            using (var db = new AdventureWorksContext(_optionsBuilder.Options))
            {
                var salesReportDetails = db.SalesPeople.Select(sp => new
                {
                    beid = sp.BusinessEntityId,
                    sp.BusinessEntity.BusinessEntity.FirstName,
                    sp.BusinessEntity.BusinessEntity.LastName,
                    sp.SalesYtd,
                    Territories = sp.SalesTerritoryHistories
                        .Select(y => y.Territory.Name),
                    OrderCount = sp.SalesOrderHeaders.Count(),
                    TotalProductsSold = sp.SalesOrderHeaders
                        .SelectMany(y => y.SalesOrderDetails)
                        .Sum(z => z.OrderQty)
                }).Where(srds => srds.SalesYtd > filter)
                .OrderBy(srds => srds.LastName)
                .ThenBy(srds => srds.FirstName)
                .ThenByDescending(srds => srds.SalesYtd)
                .Take(20)
                .ToList();

                foreach (var srd in salesReportDetails)
                {
                    Console.WriteLine($"{srd.beid}| {srd.LastName}, {srd.FirstName}" + $"| YTD Sales: {srd.SalesYtd}" +
                    $"| {string.Join(',', srd.Territories)}" + $"| Order Count: {srd.OrderCount}" +
                    $"| Products Sold: {srd.TotalProductsSold}");
                }
            }
        }

        private static decimal GetFilterFromUser()
        {
            Console.WriteLine("What is the minimum amount of sales?");
            var input = Console.ReadLine();
            decimal filter = 0.0m;
            if (!decimal.TryParse(input, out filter))
            {
                Console.WriteLine("Bad input");
                return 0.0m;
            }
            return filter;
        }
    }
}
