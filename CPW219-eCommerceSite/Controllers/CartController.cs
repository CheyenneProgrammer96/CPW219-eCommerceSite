﻿using CPW219_eCommerceSite.Data;
using CPW219_eCommerceSite.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace CPW219_eCommerceSite.Controllers
{
    public class CartController : Controller
    {
        private readonly VideoGameContext _context;
        private const string Cart = "ShoppingCart";

        public CartController(VideoGameContext context)
        {
            _context = context;
        }

        public IActionResult Add(int id)
        {
            Game? gameToAdd = _context.Games.Where(g => g.GameId == id).SingleOrDefault();

            if (gameToAdd == null)
            {
                // Game with specified id does not exist
                TempData["Message"] = "Sorry that game no longer exists";
            }

            CartGameViewModel cartGame = new()
            {
                GameId = gameToAdd.GameId,
                Title = gameToAdd.Title,
                Price = gameToAdd.Price
            };

            List<CartGameViewModel> cartGames = GetExistingCartData();
            cartGames.Add(cartGame);

            string cookieData = JsonConvert.SerializeObject(cartGames);
            

            HttpContext.Response.Cookies.Append(Cart, cookieData, new CookieOptions()
            {
                Expires = DateTimeOffset.Now.AddYears(1)
            });
            // Todo: Add item to a shopping cart cookie
            TempData["Message"] = "Item added to cart";
            return RedirectToAction("Index", "Games");
        }

        /// <summary>
        /// Return the current list of vieo games in the users shopping
        /// cart cookie. If there is no cookie, an empty list will be returned
        /// </summary>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        private List<CartGameViewModel> GetExistingCartData()
        {
            string? cookie = HttpContext.Request.Cookies[Cart];
            if (string.IsNullOrWhiteSpace(cookie))
            {
                return new List<CartGameViewModel>();
            }

            return JsonConvert.DeserializeObject<List<CartGameViewModel>>(cookie);
        }
    }
}