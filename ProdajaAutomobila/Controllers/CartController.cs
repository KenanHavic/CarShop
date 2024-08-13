using ProdajaAutomobila.Models;
using ProdajaAutomobila.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ProdajaAutomobila.Models;
using ProdajaAutomobila.Services;

namespace ProdajaAutomobila.Controllers
{
    public class CartController : Controller
    {
        private readonly ApplicationDbContext context;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly decimal shippingFee;

        public CartController(ApplicationDbContext context, IConfiguration configuration
            , UserManager<ApplicationUser> userManager)
        {
            this.context = context;
            this.userManager = userManager;
            shippingFee = configuration.GetValue<decimal>("CartSettings:ShippingFee");
        }

        public IActionResult Index()
        {
            List<OrderItem> cartItems = CartHelper.GetCartItems(Request, Response, context);
            decimal subtotal = CartHelper.GetSubtotal(cartItems);


            ViewBag.CartItems = cartItems;
            ViewBag.ShippingFee = shippingFee;
            ViewBag.Subtotal = subtotal;
            ViewBag.Total = subtotal + shippingFee;

            return View();
        }
        [HttpPost]
        [Authorize]
        public IActionResult Index(CheckoutDto model)
        {
            List<OrderItem> cartItems = CartHelper.GetCartItems(Request, Response, context);
            decimal subtotal = CartHelper.GetSubtotal(cartItems);


            ViewBag.CartItems = cartItems;
            ViewBag.ShippingFee = shippingFee;
            ViewBag.Subtotal = subtotal;
            ViewBag.Total = subtotal + shippingFee;

            if (!ModelState.IsValid)
            {
                return View(model);
            }
            //Provjeravam da li je korpa prazna
            if (cartItems.Count == 0)
            {
                ViewBag.ErrorMessage = "Vaša korpa je prazna";
                return View(model);
            }
            TempData["DeliveryAddress"] = model.DeliveryAddress;
            TempData["PaymentMethod"] = model.PaymentMethod;

            return RedirectToAction("Confirm");

        }

        public IActionResult Confirm()
        {
            List<OrderItem> cartItems = CartHelper.GetCartItems(Request, Response, context);
            decimal total = CartHelper.GetSubtotal(cartItems) + shippingFee;
            int cartSize = 0;
            foreach(var item in cartItems)
            {
                cartSize += item.Quantity;
            }
            string deliveryAddress = TempData["DeliveryAddress"] as string ?? "";
            string paymentMethod = TempData["PaymentMethod"] as string ?? "";
            TempData.Keep();

            if(cartSize == 0 || deliveryAddress.Length == 0 || paymentMethod.Length == 0)
            {
                return RedirectToAction("Index", "Home");
            }

            ViewBag.DeliveryAddress = deliveryAddress;
            ViewBag.PaymentMethod = paymentMethod;
            ViewBag.Total = total;
            ViewBag.CartSize = cartSize;


            return View();
        }
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Confirm(int any)
        {
            var cartItems = CartHelper.GetCartItems(Request, Response, context);

            string deliveryAddress = TempData["DeliveryAddress"] as string ?? "";
            string paymentMethod = TempData["PaymentMethod"] as string ?? "";
            TempData.Keep();

            if (cartItems.Count == 0 || deliveryAddress.Length == 0 || paymentMethod.Length == 0)
            {
                return RedirectToAction("Index", "Home");
            }
            var appUser = await userManager.GetUserAsync(User);
            if (appUser == null)
            {
                return RedirectToAction("Index", "Home");
            }

            //snimanje narudzbe
            var order = new Order
            {
                ClientId = appUser.Id,
                Items = cartItems,
                ShippingFee = shippingFee,
                DeliveryAddress = deliveryAddress,
                PaymentMethod = paymentMethod,
                PaymentStatus = "pending",
                PaymentDetails = "",
                OrderStatus = "created",
                CreatedAt = DateTime.Now,

            };
            context.Orders.Add(order);
            context.SaveChanges();

            Response.Cookies.Delete("shopping_cart");
            ViewBag.SuccessMessage = "Narudžba je uspješno kreirana";
            return View();

        }


    }
}
