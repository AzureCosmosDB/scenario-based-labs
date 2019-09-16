using System;
using System.Collections.Generic;
using System.Text;

namespace ContosoFunctionApp
{
    public class OrderDetailViewModel
    {
        public int Quantity { get; set; }

        public double UnitPrice { get; set; }
        public string ProductName { get; set; }

        public double Cost
        {
            get
            {
                return Math.Round(UnitPrice * Quantity, 2);
            }
        }

    }
}
