using Fawry_ECommerce_Project.Data;
using Fawry_ECommerce_Project.Logic;
using Fawry_ECommerce_Project.Models;
using Microsoft.EntityFrameworkCore;

namespace Fawry_ECommerce_Project
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            AppDbContext _context = new AppDbContext();
            
            string UserEmail = "ahmed@gmail.com";
            var User = await _context.users.FirstOrDefaultAsync(x => x.Email == UserEmail);
            Console.WriteLine($"Hello {User.Name}, Welcome Back");

            while (true)
            {
                Console.WriteLine("Please Select an option:");
                Console.WriteLine("1- Show Products");
                Console.WriteLine("2- Show Cart");
                Console.WriteLine("3- Add Product to Cart");
                Console.WriteLine("4- Remove Product From Cart");
                Console.WriteLine("5- Clear Cart");
                Console.WriteLine("6- CheckOut");
                Console.Write("Answer: ");
                int.TryParse(Console.ReadLine(), out int option);
                switch (option)
                {
                    case 1:
                        ClientLogic.ShowProducts();
                        break;
                    case 2:
                        ClientLogic.ShowCartItem(User.Id);
                        break;
                    case 3:
                        Console.WriteLine("Please Enter Product Id:");
                        int.TryParse(Console.ReadLine(), out int ProductId);
                        Console.WriteLine("Please Enter Product'Quantity:");
                        int.TryParse(Console.ReadLine(), out int Quantity);
                        var data = ClientLogic.AddProductToCart(ProductId, Quantity, User.Id);
                        Console.WriteLine(data.message);
                        Console.WriteLine("\n\n=========================================================\n\n");
                        break;

                    case 4:
                        Console.WriteLine("Please Enter Product Id:");
                        int.TryParse(Console.ReadLine(), out int ProdId);
                        Console.WriteLine("Please Enter Product'Quantity:");
                        int.TryParse(Console.ReadLine(), out int Qty);
                        var res = ClientLogic.RemoveProductFromCart(ProdId, Qty, User.Id);
                        Console.WriteLine(res.message);
                        Console.WriteLine("\n\n=========================================================\n\n");
                        break;

                    case 5:
                        ClientLogic.ClearCart(User.Id);
                        break;

                    case 6:
                        ClientLogic.CheckOut(User.Id,8000);
                        break;
                }
            }



        }
    }
}
