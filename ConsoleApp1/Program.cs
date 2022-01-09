using EF_Activity001;
using InventoryHelpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Linq;

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
            Console.WriteLine("Would you like to view all salespeople? [y / n]");

            var input = Console.ReadLine();
            if (input.StartsWith("y", StringComparison.OrdinalIgnoreCase))
            {
                ShowAllSalesPeopleUsingProjection();
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
    }
}
