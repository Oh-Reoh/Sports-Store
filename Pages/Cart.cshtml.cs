using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SportsStore.Infrastructure;
using SportsStore.Models;

namespace SportsStore.Pages
{
    public class CartModel : PageModel
    {
        private IStoreRepository repository;

        public CartModel(IStoreRepository repo, Cart cartService)
        {
            repository = repo;
            Cart = cartService;
        }

        public Cart Cart { get; set; }
        public string ReturnUrl { get; set; } = "/";

        public void OnGet(string? returnUrl)
        {
            ReturnUrl = returnUrl ?? "/";
        }

        public IActionResult OnPost(long productId, string returnUrl)
        {
            Product? product = repository.Products
                .FirstOrDefault(p => p.ProductID == productId);

            if (product != null)
            {
                Cart.AddItem(product, 1);
            }
            return RedirectToPage(new { returnUrl = returnUrl });
        }

        // Fix: Refactored to handle product removal based on your classmate's code
        public IActionResult OnGetRemove(long productId, string returnUrl)
        {
            var line = Cart.Lines.FirstOrDefault(cl => cl.Product.ProductID == productId);
            if (line != null)
            {
                // Decrement quantity if > 1, else remove the product entirely
                if (line.Quantity > 1)
                {
                    line.Quantity--;
                }
                else
                {
                    Cart.RemoveLine(line.Product); // Remove completely if quantity is 1
                }

                // Update session cart if session is being used
                if (Cart is SessionCart sessionCart)
                {
                    sessionCart.Session?.SetJson("Cart", sessionCart); // Save cart to session
                }
            }

            return RedirectToPage(new { returnUrl });
        }
    }
}
