using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Stripe;
using Stripe.Checkout;
using Web_App.Data;
using Web_App.Models;

namespace Web_App.Pages
{
    public class CheckoutModel : PageModel
    {

        private readonly Web_AppContext _db;
        private readonly UserManager<IdentityUser> _userManager;
        public IList<CheckoutItem> Items { get; set; }
        public OrderHistory Order = new OrderHistory();

        public decimal Total;
        public long AmountPayable;

        // For stripe payment
        private readonly StripeSettings _stripeSettings;
        public string sessionId { get; set; }
        

        public CheckoutModel(Web_AppContext db, UserManager<IdentityUser> UserManager, IOptions<StripeSettings> stripeSettings)
        {
            _db = db;
            _userManager = UserManager;
            _stripeSettings = stripeSettings.Value;
        }   
        

        public async Task OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            CheckoutCustomer customer = await _db.CheckoutCustomers.FindAsync(user.Email);

            // Renamed 'FoodName' column to 'Item_Name' for mapping to the CheckoutItems class
            Items = _db.CheckoutItems.FromSqlRaw(
                "SELECT FoodItem.FoodID AS ID, FoodItem.Price, " +
                "FoodItem.FoodName AS Item_Name, " +
                "BasketItems.BasketID, BasketItems.Quantity " +
                "FROM FoodItem INNER JOIN BasketItems " +
                "ON FoodItem.FoodID = BasketItems.StockID " +
                "WHERE BasketID = {0}", customer.BasketID)
                .ToList();

            Total = 0;

            foreach (var item in Items)
            {
                Total += (item.Quantity * item.Price);
            }
            AmountPayable = (long)Total;
        }


        // Method for processing the 'Buy' button click action
        public async Task<IActionResult> OnPostBuyAsync()
        {
            var currentOrder = _db.OrderHistories.FromSqlRaw("SELECT * FROM OrderHistories")
                .OrderByDescending(b => b.OrderNo)
                .FirstOrDefault();

            if (currentOrder == null)
            {
                Order.OrderNo = 1;
            }
            else
            {
                Order.OrderNo = currentOrder.OrderNo + 1;
            }
        
            var user = await _userManager.GetUserAsync(User);
            Order.Email = user.Email;
            _db.OrderHistories.Add(Order);

            CheckoutCustomer customer = await _db
                .CheckoutCustomers
                .FindAsync(user.Email);

            var basketItems = _db.BasketItems
                .FromSqlRaw("SELECT * FROM BasketItems WHERE BasketID = {0}", customer.BasketID)
                .ToList();

            foreach (var item in basketItems)
            {
                OrderItem oi = new OrderItem
                {
                    OrderNo = Order.OrderNo,
                    StockID = item.StockID,
                    Quantity = item.Quantity
                };
                _db.OrderItems.Add(oi);
                _db.BasketItems.Remove(item);
            }

            await _db.SaveChangesAsync();
            return RedirectToPage("/CheckoutSuccess");
        }


        // Reducing the quantity of item in basket on checkout screen 
        public async Task<IActionResult> OnPostRemoveAsync(int checkoutItemID)
        {
            var user = await _userManager.GetUserAsync(User);
            CheckoutCustomer customer = await _db.CheckoutCustomers.FindAsync(user.Email);

            var item = _db.BasketItems
                .FromSqlRaw("SELECT * FROM BasketItems WHERE StockID = {0}" + " AND BasketID = {1}", checkoutItemID, customer.BasketID)
                .ToList()
                .FirstOrDefault();

            if (item.Quantity > 0)
            {
                item.Quantity--;
                _db.Attach(item).State = EntityState.Modified;
            }
            if (item.Quantity == 0)
            {
                _db.BasketItems.Remove(item);
            }

            await _db.SaveChangesAsync();

            return RedirectToPage();
        }


        // Increasing the quantity of item in basket on checkout screen 
        public async Task<IActionResult> OnPostAddAsync(int checkoutItemID)
        {
            var user = await _userManager.GetUserAsync(User);
            CheckoutCustomer customer = await _db.CheckoutCustomers.FindAsync(user.Email);

            var item = _db.BasketItems
                .FromSqlRaw("SELECT * FROM BasketItems WHERE StockID = {0}" + " AND BasketID = {1}", checkoutItemID, customer.BasketID)
                .ToList()
                .FirstOrDefault();

            item.Quantity++;
            _db.Attach(item).State = EntityState.Modified;
            await _db.SaveChangesAsync();

            return RedirectToPage();
        }



        public IActionResult CreateCheckoutSession(string amountPayable)
        {

            StripeConfiguration.ApiKey = _stripeSettings.SecretKey;

            var options = new SessionCreateOptions
            {
                PaymentMethodTypes = new List<string>
                {
                    "card",
                },
                LineItems = new List<SessionLineItemOptions>
                {
                    new SessionLineItemOptions
                    {
                        PriceData = new SessionLineItemPriceDataOptions
                        {
                            UnitAmount = Convert.ToInt32(amountPayable) * 100, // value in pence e.g. 1000 = £10
                            Currency = "gbp",
                            ProductData = new SessionLineItemPriceDataProductDataOptions
                            {
                                Name = "Prod name ***",
                                Description = "Prod desc ***"
                            },
                        },
                        Quantity = 1,
                    },
                },
                Mode = "payment",
                SuccessUrl = "https://localhost:7288/Contact/", // ***
                CancelUrl = "https://localhost:7288/Menu",
            };

            var service = new SessionService();
            var session = service.Create(options);
            sessionId = session.Id;

            return Redirect(session.Url);
        }


    }
}
