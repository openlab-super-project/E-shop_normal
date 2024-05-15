﻿using AspNetCoreAPI.Data;
using AspNetCoreAPI.DTO;
using AspNetCoreAPI.Models;
using Microsoft.AspNetCore.Mvc;

namespace AspNetCoreAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class OrdersController : Controller
    {
        private readonly ApplicationDbContext _context;

        public OrdersController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPut("create-order")]
        public ActionResult<OrdersDTO> CreateOrder([FromBody] OrdersDTO ordersDTO)
        {
            try
            {
                using (var context = _context)
                {
                    var newOrder = new OrdersModel
                    {
                        OrderId = ordersDTO.OrderId,
                        Name = ordersDTO.Name,
                        Surname = ordersDTO.Surname,
                        Email = ordersDTO.Email,
                        PhoneNumber = ordersDTO.PhoneNumber,
                        Address = ordersDTO.Address,
                        PSC = ordersDTO.PSC,
                        City = ordersDTO.City,
                        Country = ordersDTO.Country,
                        DeliveryOption = ordersDTO.DeliveryOption,
                        Payment = ordersDTO.Payment,
                        TotalPrice = ordersDTO.TotalPrice,
                        OrderVerificationKey = ordersDTO.OrderVerificationKey,
                    };

                    context.Orders.Add(newOrder);
                    context.SaveChanges();

                    var info = new OrdersDTO
                    {
                        OrderId = ordersDTO.OrderId,
                        Name = ordersDTO.Name,
                        Surname = ordersDTO.Surname,
                        Email = ordersDTO.Email,
                        PhoneNumber = ordersDTO.PhoneNumber,
                        Address = ordersDTO.Address,
                        PSC = ordersDTO.PSC,
                        City = ordersDTO.City,
                        Country = ordersDTO.Country,
                        DeliveryOption = ordersDTO.DeliveryOption,
                        Payment = ordersDTO.Payment,
                        TotalPrice = ordersDTO.TotalPrice,
                        OrderVerificationKey = ordersDTO.OrderVerificationKey
                    };

                    return Ok(info);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error");
                return StatusCode(500, new { message = "Mame problem." });
            }
        }
        [HttpGet("{orderVerificationKey}")]
        public ActionResult<int> GetOrderId(string orderVerificationKey)
        {
            var order = _context.Orders.FirstOrDefault(o => o.OrderVerificationKey == orderVerificationKey);

            if(order == null)
            {
                return NotFound();
            }

            return Ok(order.OrderId);
        }
        [HttpPut("add-productId")]
        public ActionResult<OrderProductsDTO> AddProductId([FromBody] OrderProductsDTO orderProductsDTO)
        {
            try
            {
                var newRow = new OrderProductsModel
                {
                    ProductId = orderProductsDTO.ProductId,
                    OrderId = orderProductsDTO.OrderId,
                    Quantity = orderProductsDTO.Quantity
                };

                _context.OrderProducts.Add(newRow);
                _context.SaveChanges();

                return Ok("Success!");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message); 
                return StatusCode(500, new { message = "Internal server error" });
            }
        }
        [HttpGet]
        public IEnumerable<OrdersDTO> GetOrders()
        {
            IEnumerable<OrdersModel> dbOrders = _context.Orders;

            return dbOrders.Select(dbOrders => new OrdersDTO
            {
                OrderId = dbOrders.OrderId,
                OrderVerificationKey = dbOrders.OrderVerificationKey,
                DeliveryOption = dbOrders.DeliveryOption,
                Address = dbOrders.Address,
                City = dbOrders.City,
                Country = dbOrders.Country,
                Email = dbOrders.Email,
                Name = dbOrders.Name,
                Payment = dbOrders.Payment,
                PhoneNumber = dbOrders.PhoneNumber,
                PSC = dbOrders.PSC,
                Surname = dbOrders.Surname,
                TotalPrice = dbOrders.TotalPrice,
            });
        }
        [HttpGet]
        [Route("getOrderInfo")]
        public ActionResult<OrdersDTO> getOrderInfo(int orderId)
        {
            OrdersModel order = _context.Orders.Where(o => o.OrderId == orderId).FirstOrDefault();

            if(order == null)
            {
                return NotFound();
            }

            var info = new OrdersDTO
            {
                OrderId = order.OrderId,
                OrderVerificationKey = order.OrderVerificationKey,
                DeliveryOption = order.DeliveryOption,
                Address = order.Address,
                City = order.City,
                Country = order.Country,
                Email = order.Email,
                Name = order.Name,
                Payment = order.Payment,
                PhoneNumber = order.PhoneNumber,
                PSC = order.PSC,
                Surname = order.Surname,
                TotalPrice = order.TotalPrice,
            };

            return Ok(info);
        }
        [HttpGet]
        [Route("getOrderProducts")]
        public ActionResult<List<ProductsDTO>> getOrderProducts([FromQuery] int orderId)
        {
            try
            {
                List<OrderProductsDTO> products = _context.OrderProducts
                .Where(p => p.OrderId == orderId)
                .Select(r => new OrderProductsDTO
                {
                    ProductId = r.ProductId,
                    Quantity = r.Quantity,
                })
                .ToList();


                if (products.Count == 0)
                {
                    return NotFound();
                }

                List<ProductsDTO> productsInfos = new List<ProductsDTO>();

                foreach (var orderProduct in products)
                {
                    ProductsModel product = _context.Products.FirstOrDefault(p => p.ProductId == orderProduct.ProductId);

                    if (product != null)
                    {
                        ProductsDTO info = new ProductsDTO
                        {
                            ProductName = product.ProductName,
                            Quantity = orderProduct.Quantity,
                        };

                        productsInfos.Add(info);
                    }
                }
                return Ok(productsInfos);
            }
            catch(Exception ex) 
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }
    }
}
