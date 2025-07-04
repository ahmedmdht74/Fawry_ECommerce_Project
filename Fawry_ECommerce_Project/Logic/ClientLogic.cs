using Fawry_ECommerce_Project.Data;
using Fawry_ECommerce_Project.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fawry_ECommerce_Project.Logic
{
    public class ClientLogic 
    {

        public static void ShowProducts()
        {
            Console.WriteLine("\n\n=========================================================\n\n");
            AppDbContext _context = new AppDbContext();
            Console.WriteLine("Products");
            var products = _context.products.ToList();
            for (int i = 0; i < products.Count; i++)
            {
                Console.WriteLine($"{i + 1}- Name =>{products[i].Name}, Price =>{products[i].Price} , Available Qty =>{products[i].Quantity}");
            }
            Console.WriteLine("\n\n=========================================================\n\n");
        }

        public static dynamic AddProductToCart(int ProductId, int Quantity,string UserId)
        {
            AppDbContext _context = new AppDbContext();
            Console.WriteLine("\n\n=========================================================\n\n");
            var UserCart = _context.carts.FirstOrDefault(x => x.User_Id == UserId);
            if (UserCart == null)
            {
                UserCart = new Models.Cart
                {
                    User_Id = UserId,
                };
                _context.carts.Add(UserCart);
                _context.SaveChanges();
            }

            var Product = _context.products.FirstOrDefault(x => x.Id == ProductId);
            if (Product == null)
            {
                return new { result = false, message = "Wrong Product Id" };
            }

            var CartDetail = _context.cartdetails.FirstOrDefault(x => x.Cart_Id == UserCart.Id && x.Product_Id == ProductId);
            if (CartDetail != null)
            {
                if (Product.Quantity < Quantity)
                {
                    return new { result = false, message = "Quantity Not Available" };
                }
                else
                {
                    CartDetail.Quantity = Quantity;
                    _context.SaveChanges();
                }
            }
            else
            {
                CartDetail = new Models.CartDetail
                {
                    Cart_Id = UserCart.Id,
                    Product_Id = ProductId,
                    Quantity = Product.Quantity < Quantity ? 0 : Quantity,
                    Price = Product.Price,
                };
                _context.cartdetails.Add(CartDetail);
                _context.SaveChanges();
            }
            return new { result = true, message = "Product Added Successfully" };
        }

        public static dynamic RemoveProductFromCart(int ProductId, int Quantity, string UserId)
        {
            AppDbContext _context = new AppDbContext();
            Console.WriteLine("\n\n=========================================================\n\n");
            var UserCart = _context.carts.FirstOrDefault(x => x.User_Id == UserId);


            var Product = _context.products.FirstOrDefault(x => x.Id == ProductId);
            if (Product == null)
            {
                return new { result = false, message = "Wrong Product Id" };
            }

            var CartDetail = _context.cartdetails.FirstOrDefault(x => x.Cart_Id == UserCart.Id && x.Product_Id == ProductId);
            if (CartDetail != null)
            {
                if (CartDetail.Quantity == Quantity)
                {
                    _context.cartdetails.Remove(CartDetail);
                    _context.SaveChanges();
                }
                else if (CartDetail.Quantity > Quantity)
                {
                    CartDetail.Quantity -= Quantity;
                    _context.SaveChanges();
                }
                else
                {
                    return new { result = false, message = "Quantity to remove exceeds available quantity in cart" };
                }

                return new { result = true, message = "Product Removed Successfully" };
            }
            return new { result = false, message = "Cart Is Empty" };
        }

        public static void ShowCartItem(string UserId)
        {
            AppDbContext _db = new AppDbContext();
            Console.WriteLine("\n\n=========================================================\n\n");
            Console.WriteLine("Items in Cart:");
            var UserCart = _db.carts.FirstOrDefault(x => x.User_Id == UserId);
            if (UserCart == null)
            {
                Console.WriteLine("Your Cart is Empty");
                return;
            }
            var CartDetails = _db.cartdetails.Include(x => x.Product).Where(x => x.Cart_Id == UserCart.Id).ToList();
            for (int i = 0; i < CartDetails.Count; i++)
            {
                Console.WriteLine($"Item {i+1}==> Name: {CartDetails[i].Product.Name}, Qty: {CartDetails[i].Quantity}");
            }
            Console.WriteLine("\n\n=========================================================\n\n");

        }

        public static void ClearCart(string UserId)
        {
            AppDbContext _context = new AppDbContext();
            Console.WriteLine("\n\n=========================================================\n\n");
            var UserCart = _context.carts.FirstOrDefault(x => x.User_Id == UserId);
            if (UserCart == null)
            {
                Console.WriteLine("Your Cart is Empty");
                return;
            }
            var CartDetails = _context.cartdetails.Where(x => x.Cart_Id == UserCart.Id).ToList();
            if (CartDetails.Count > 0)
            {
                _context.cartdetails.RemoveRange(CartDetails);
                _context.SaveChanges();
                Console.WriteLine("Cart Cleared Successfully");
            }
            else
            {
                Console.WriteLine("Your Cart is Already Empty");
            }
        }

        public static void CheckOut(string UserId,double ClientBalance)
        {
            AppDbContext _context = new AppDbContext();
            Console.WriteLine("\n\n=========================================================\n\n");
            var UserCart = _context.carts.FirstOrDefault(x => x.User_Id == UserId);
            if (UserCart == null)
            {
                Console.WriteLine("Your Cart is Empty\n");
                return;
            }

            var CartDetails = _context.cartdetails.Include(x => x.Product).Where(x => x.Cart_Id == UserCart.Id).ToList();
            if (CartDetails.Count == 0)
            {
                Console.WriteLine("Your Cart is Empty\n");
                return;
            }

            if (CartDetails.Sum(x => x.Price * x.Quantity) + 30 > ClientBalance)
            {
                Console.WriteLine("Your Balance is not enough to place this order\n");
                return;
            }



            var Order = new Models.Order
            {
                User_Id = UserId,
                CreatedDate = DateTime.Now,
                Status = "Approved"
            };
            _context.orders.Add(Order);
            _context.SaveChanges();
            foreach (var item in CartDetails)
            {
                var Product = _context.products.FirstOrDefault(x => x.Id == item.Product_Id);
                if(item.Quantity > Product.Quantity)
                {
                    Console.WriteLine($"Product =>{Product.Name} is out of stock..\n\n");
                    continue;
                }
                var orderDetail = new OrderDetail
                {
                    Order_Id = Order.Id,
                    Price = item.Price,
                    Quantity = item.Quantity,
                    Product_Id = item.Product_Id
                };
                _context.orderdetails.Add(orderDetail);
                _context.SaveChanges();
            }
            _context.cartdetails.RemoveRange(CartDetails);
            _context.SaveChanges();

            var OrderDetails = _context.orderdetails.Include(x => x.Product).Where(x => x.Order_Id == Order.Id).ToList();
            if(OrderDetails.Count == 0)
            {
                Console.WriteLine("Not Allowed....");
                return;
            }
            int totalWeight = 0;
            double SubTotal = 0;
            Console.WriteLine("Order Placed Successfully. Your Order ID is: " + Order.Id);
            Console.WriteLine("Order Details:");
            Console.WriteLine("** Shipment notice **");
            foreach (var item in OrderDetails)
            {
                Console.WriteLine($"{item.Quantity}x: {item.Product.Name}      {item.Product.Weight}");
                totalWeight += item.Product.Weight.Value;
                SubTotal += item.Price * item.Quantity;
            }
            Console.WriteLine("Total package weight "+ totalWeight+"\n");
            Console.WriteLine("** Checkout receipt **");
            foreach (var item in OrderDetails)
            {
                Console.WriteLine($"{item.Quantity}x: {item.Product.Name}      {item.Price * item.Quantity}");
            }
            Console.WriteLine("-----------------------------");
            Console.WriteLine($"subtotal        {SubTotal}");
            Console.WriteLine($"Shipping        30");
            Console.WriteLine($"Shipping        {SubTotal+30}\n");
            Console.WriteLine($"Your Current Balance        {ClientBalance-(SubTotal+30)}\n");
            Console.WriteLine("Order Placed Successfully");
            Console.WriteLine("\n\n=========================================================\n\n");

        }
    }
}
