using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Contoso.Apps.Movies.Data.Models;

namespace Contoso.Apps.Movies.Data.Logic
{
    public class OrderActions : IDisposable
    {
        List<Order> orders;

        public OrderActions(List<Order> orders)
        {
            this.orders = orders;
        }

        /// <summary>
        /// Retrieves all completed orders.
        /// </summary>
        /// <returns></returns>
        public List<Order> GetCompletedOrders()
        {
            return orders.Where(o => !(o.PaymentTransactionId == null || o.PaymentTransactionId.Trim() == string.Empty)).ToList();
        }

        public Order GetOrder(int orderId)
        {
            var order = new Order();

            // Retrieve the order record.
            if (orders.Any(o => o.OrderId == orderId))
            {
                order = orders.Single(o => o.OrderId == orderId);
            }
           
            // Success.
            return order;
        }

        /// <summary>
        /// Updates the order record with the Uri of the generated Pdf receipt.
        /// </summary>
        /// <param name="updateCartID"></param>
        /// <param name="updateProductID"></param>
        /// <param name="quantity"></param>
        public void UpdateReceiptUri(int orderId, string uri)
        {
            try
            {
                var order = orders.Where(o => o.OrderId == orderId).FirstOrDefault();
                if (order != null)
                {
                    order.ReceiptUrl = uri;

                    //_db.SaveChanges();
                }
            }
            catch (Exception exp)
            {
                throw new Exception("ERROR: Unable to Update Order - " + exp.Message.ToString(), exp);
            }
        }

        public void Dispose()
        {
            
        }
    }
}
