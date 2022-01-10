using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EF_Activity001.DTOs
{
    public class SalesReportListingDto
    {
        [Required]
        public int BusinessEntityId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public decimal? SalesYtd { get; set; }
        public IEnumerable<string> Territories { get; set; }
        public int TotalProductsSold { get; set; }
        public int TotalOrders { get; set; }
        public string DisplayName => $"{LastName}, {FirstName}";
        public string DisplayTerritories => string.Join(",", Territories);
        public override string ToString()
        {
            return $"BID: {BusinessEntityId} |{DisplayName,25}|{ DisplayTerritories,25}| " 
                + $"{SalesYtd} | Orders: {TotalOrders} |" 
                + $"Products Sold: {TotalProductsSold}";
        }
    }
}
