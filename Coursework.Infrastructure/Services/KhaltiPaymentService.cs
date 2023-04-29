using Coursework.Application.Common.Interface;
using Coursework.Application.DTO;
using Coursework.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Coursework.Infrastructure.Services
{
    public class KhaltiPaymentService : IKhaltiPaymentService
    {
        private readonly string _apiKey;
        private readonly string return_url;
        private readonly string website_url;
        private readonly string _baseUrl;
        private readonly IApplicationDBContext _dbContext;
        private readonly UserManager<AppUser> _userManager;
        public KhaltiPaymentService(IConfiguration configuration, IApplicationDBContext dBContext, UserManager<AppUser> userManager)
        {
            _dbContext = dBContext;
            _userManager = userManager;
            _apiKey = configuration.GetSection("Khalti:Key").Value!;
            return_url = configuration.GetSection("Khalti:ReturnURL").Value!;
            website_url = configuration.GetSection("Khalti:WebsiteURL").Value!;
            _baseUrl = configuration.GetSection("Khalti:BaseURL").Value!;
        }
        public async Task<ResponseDataDTO<KhaltiResponseDTO>> InitializePayment(KhaltiPaymentDTO model, string email)
        {
            try
            {
                var client = new HttpClient();
                client.DefaultRequestHeaders.Add("Authorization", $"Key {_apiKey}");
                var customerBooking = await _dbContext.CustomerBooking.FindAsync(Guid.Parse(model.BookingId));
                var user = await _userManager.FindByEmailAsync(email);
                var car = await _dbContext.Car.FindAsync(Guid.Parse(customerBooking.CarId));
                var customer = await _dbContext.Customer.SingleOrDefaultAsync(c => c.UserId == customerBooking.customerId);

                var customerInfo = new
                {
                    name = customer.Name,
                    email = user.Email,
                    phone = "9815091234",
                };

                var rentalAmount = (int)Math.Round((customerBooking.RentEnddate - customerBooking.RentStartdate).Days * car.RatePerDay*100); //amount in paisa
                var VAT = (int)Math.Round(0.13 * rentalAmount); // 13% Vat
                var totalAmount = rentalAmount + VAT;

                var productDetails = new[]
                {
                   new
                   {
                       identity = customerBooking.CarId,
                       name = car.Name,
                       total_price = totalAmount,
                       quantity = 1,
                       unit_price = car.RatePerDay*100
                   }
                };

                var amountBreakdown = new[]
                {
                    new {label = "Rental Price", amount = rentalAmount },
                    new {label = "VAT", amount = VAT }
                };

                var data = new
                {
                    return_url,
                    website_url,
                    amount = totalAmount,
                    purchase_order_id = car.Id,
                    purchase_order_name = car.Name,
                    customer_info = customerInfo,
                    product_details = productDetails,
                    amount_breakdown = amountBreakdown,
                };
                var content = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");
                var response = await client.PostAsync($"{_baseUrl}/epayment/initiate/", content);
                var result = await response.Content.ReadAsStringAsync();
                dynamic jsonResponse = JsonConvert.DeserializeObject(result);
                JObject jsonObject = jsonResponse;

                if (jsonObject.ContainsKey("error_key"))
                {
                    var error = jsonResponse.detail.ToString();
                    var errorMessage = error.Substring(0, error.IndexOf('.'));
                    return new ResponseDataDTO<KhaltiResponseDTO>
                    {
                        Status = "Error",
                        Message = "Error",
                        Data = new KhaltiResponseDTO
                        {
                            error = errorMessage,
                        }
                    };
                }
                var pdx = jsonResponse.pidx;
                var url = jsonResponse.payment_url;
                return new ResponseDataDTO<KhaltiResponseDTO>
                {
                    Status = "Success",
                    Message = "Success",
                    Data = new KhaltiResponseDTO
                    {
                        pidx = pdx,
                        payment_url = url
                    }
                };
            }
            catch (Exception ex)
            {
                return new ResponseDataDTO<KhaltiResponseDTO>
                {
                    Status = "Error",
                    Message = "Error",
                    Data = new KhaltiResponseDTO
                    {
                        error = ex.ToString()
                    }
                };
            }

        }

        public async Task<ResponseDataDTO<KhaltiResponseDTO>> CheckPaymentSuccess(KhaltiPaymentCheckDTO model)
        {
            try
            {
                var client = new HttpClient();
                client.DefaultRequestHeaders.Add("Authorization", $"Key {_apiKey}");
                var data = new
                {
                    pidx = model.pidx,
                };
                var content = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");
                var response = await client.PostAsync($"{_baseUrl}/epayment/lookup/", content);
                var result = await response.Content.ReadAsStringAsync();
                dynamic jsonResponse = JsonConvert.DeserializeObject(result);
                return new ResponseDataDTO<KhaltiResponseDTO>
                {
                    Status = "Success",
                    Message = "Success",
                    Data = new KhaltiResponseDTO
                    {
                        pidx = jsonResponse.pidx,
                       status = jsonResponse.status,
                       total_amount = jsonResponse.total_amount / 100
                    }
                };
            }
            catch (Exception ex)
            {
                return new ResponseDataDTO<KhaltiResponseDTO>
                {
                    Status = "Error",
                    Message = "Error",
                    Data = new KhaltiResponseDTO
                    {
                        error = ex.ToString()
                    }
                };
            }
        }
    }
}
