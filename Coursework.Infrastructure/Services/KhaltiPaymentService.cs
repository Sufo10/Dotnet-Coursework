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
        private readonly IEmailService _emailService;

        public KhaltiPaymentService(IConfiguration configuration, IApplicationDBContext dBContext, UserManager<AppUser> userManager, IEmailService emailService)
        {
            _dbContext = dBContext;
            _userManager = userManager;
            _apiKey = configuration.GetSection("Khalti:Key").Value!;
            return_url = configuration.GetSection("Khalti:ReturnURL").Value!;
            website_url = configuration.GetSection("Khalti:WebsiteURL").Value!;
            _baseUrl = configuration.GetSection("Khalti:BaseURL").Value!;
            _emailService = emailService;
        }

        // Creating payment charge for Khalti
        public async Task<ResponseDataDTO<KhaltiResponseDTO>> InitializePayment(KhaltiPaymentDTO model, string email)
        {
            try
            {
                var client = new HttpClient();
                client.DefaultRequestHeaders.Add("Authorization", $"Key {_apiKey}");  //adding Authorization in header
                var customerBooking = await _dbContext.CustomerBooking.FindAsync(Guid.Parse(model.BookingId));
                var user = await _userManager.FindByEmailAsync(email);

                var role = await _userManager.GetRolesAsync(user);

                string name;

                // checking the role
                if (role.FirstOrDefault() == "Customer")
                {
                    var customer = await _dbContext.Customer.SingleOrDefaultAsync(c => c.UserId == customerBooking.customerId);
                    name = customer.Name;
                }
                else
                {
                    var employee = await _dbContext.Employee.SingleOrDefaultAsync(c => c.UserId == customerBooking.customerId);
                    name = employee.Name;
                }

                var car = await _dbContext.Car.FindAsync(Guid.Parse(customerBooking.CarId));

                var customerInfo = new
                {
                    name,
                    email = user.Email,
                    //phone = "9815091234",
                };

                var totalAmount = customerBooking.TotalAmount * 100;
                var rentalAmount = (int)Math.Round((customerBooking.TotalAmount / 1.13) * 100);
                var VAT = totalAmount - rentalAmount;

                var productDetails = new[]
                {
            new
            {
                identity = customerBooking.CarId,
                name = car.Name,
                total_price = totalAmount,
                quantity = 1,
                unit_price = car.RatePerDay * 100 //amount in paisa
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
                    purchase_order_id = model.BookingId,
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

        // Method to check khalti payment status 
        public async Task<ResponseDataDTO<KhaltiResponseDTO>> CheckPaymentSuccess(string pidx, string bookingId, int amount)
        {
            try
            {
                var client = new HttpClient();
                client.DefaultRequestHeaders.Add("Authorization", $"Key {_apiKey}");

                // Convert the payload to JSON and create a StringContent object
                var content = new StringContent(JsonConvert.SerializeObject(new { pidx }), Encoding.UTF8, "application/json");
                
                // Make the API call
                var response = await client.PostAsync($"{_baseUrl}/epayment/lookup/", content);
                
                // Read the response as a string
                var result = await response.Content.ReadAsStringAsync();
                
                // Parse the response as JSON
                dynamic jsonResponse = JsonConvert.DeserializeObject(result);

                // If the status of the payment is "Completed", update the booking in the database and send an invoice to the customer
                if (jsonResponse.status == "Completed")
                {
                    var customerBooking = await _dbContext.CustomerBooking.FindAsync(Guid.Parse(bookingId));
                    customerBooking.payment = true;
                    _dbContext.CustomerBooking.Update(customerBooking);
                    await _dbContext.SaveChangesAsync(default(CancellationToken));
                    var user = await _userManager.FindByIdAsync(customerBooking.customerId);
                    var role = await _userManager.GetRolesAsync(user);
                    var car = await _dbContext.Car.FindAsync(Guid.Parse(customerBooking.CarId));
                    string name;

                    // checking the role
                    if (role.FirstOrDefault() == "Customer")
                    {
                        var customer = await _dbContext.Customer.SingleOrDefaultAsync(c => c.UserId == customerBooking.customerId);
                        name = customer.Name;
                    }
                    else
                    {
                        var employee = await _dbContext.Employee.SingleOrDefaultAsync(c => c.UserId == customerBooking.customerId);
                        name = employee.Name;
                    }

                    double realAmount = amount / 113;
                    var rentalAmount = (int)Math.Round(realAmount);
                    var VATAmount = (int)Math.Round(amount / 100 - realAmount);
                    var invoice = new GenerateInvoiceDTO()
                    {
                        CustomerName = name,
                        CustomerEmail = user.Email,
                        CarName = car.Name,
                        RatePerDay = car.RatePerDay,
                        RentStartDate = customerBooking.RentStartdate,
                        RentEndDate = customerBooking.RentEnddate,
                        RentalAmount = rentalAmount,
                        VATAmount = VATAmount,
                        Message = "Thank you for paying your car rental fee."
                    };
                    await _emailService.SendPaymentInvoiceAsync(invoice);

                    // Return a success response object
                    return new ResponseDataDTO<KhaltiResponseDTO>
                    {
                        Status = "Success",
                        Message = "Success",
                    };
                }
                else // If the status of the payment is anything other than "Completed", return an error response object
                {
                    return new ResponseDataDTO<KhaltiResponseDTO>
                    {
                        Status = "Error",
                        Message = "Error",
                    };
                }
            }
            catch (Exception ex)
            {
                // If an exception is thrown, return an error response object with the exception details
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

        // Method to handle offline payments for a car rental booking using the Khalti payment gateway API.
        public async Task<ResponseDataDTO<KhaltiResponseDTO>> OfflinePayment(KhaltiPaymentDTO model)
        {
            try
            {
                // Retrieve the customer booking information from the database.
                var customerBooking = await _dbContext.CustomerBooking.FindAsync(Guid.Parse(model.BookingId));

                // Update the customer booking to reflect that it has been paid.
                customerBooking.payment = true;
                _dbContext.CustomerBooking.Update(customerBooking);
                await _dbContext.SaveChangesAsync(default(CancellationToken));

                // Retrieve user, car, and customer information associated with the booking.
                var user = await _userManager.FindByIdAsync(customerBooking.customerId);
                var role = await _userManager.GetRolesAsync(user);

                // checking the role
                if (role.FirstOrDefault() == "Customer")
                {
                    var car = await _dbContext.Car.FindAsync(Guid.Parse(customerBooking.CarId));
                    var customer = await _dbContext.Customer.SingleOrDefaultAsync(c => c.UserId == customerBooking.customerId);
                    // Calculate rental amount and VAT based on total amount.
                    var rentalAmount = (int)Math.Round(customerBooking.TotalAmount / 1.13); // amount in paisa
                    var VAT = customerBooking.TotalAmount - rentalAmount; // 13% VAT

                    // Generate an invoice and send it to the customer's email address.
                    var invoice = new GenerateInvoiceDTO()
                    {
                        CustomerName = customer.Name,
                        CustomerEmail = user.Email,
                        CarName = car.Name,
                        RatePerDay = car.RatePerDay,
                        RentStartDate = customerBooking.RentStartdate,
                        RentEndDate = customerBooking.RentEnddate,
                        RentalAmount = rentalAmount,
                        VATAmount = VAT,
                        Message = "Thank you for paying your car rental fee."
                    };
                    await _emailService.SendPaymentInvoiceAsync(invoice);

                    return new ResponseDataDTO<KhaltiResponseDTO>
                    {
                        Status = "Success",
                        Message = "Success",
                    };
                }
                else
                {
                    var car = await _dbContext.Car.FindAsync(Guid.Parse(customerBooking.CarId));
                    var employee = await _dbContext.Employee.SingleOrDefaultAsync(c => c.UserId == customerBooking.customerId);
                    // Calculate rental amount and VAT based on total amount.
                    var rentalAmount = (int)Math.Round(customerBooking.TotalAmount / 1.13); // amount in paisa
                    var VAT = customerBooking.TotalAmount - rentalAmount; // 13% VAT

                    // Generate an invoice and send it to the customer's email address.
                    var invoice = new GenerateInvoiceDTO()
                    {
                        CustomerName = employee.Name,
                        CustomerEmail = user.Email,
                        CarName = car.Name,
                        RatePerDay = car.RatePerDay,
                        RentStartDate = customerBooking.RentStartdate,
                        RentEndDate = customerBooking.RentEnddate,
                        RentalAmount = rentalAmount,
                        VATAmount = VAT,
                        Message = "Thank you for paying your car rental fee."
                    };
                    await _emailService.SendPaymentInvoiceAsync(invoice);

                    return new ResponseDataDTO<KhaltiResponseDTO>
                    {
                        Status = "Success",
                        Message = "Success",
                    };
                }
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


        // Method to handle offline payments for additional charges associated with a car rental booking
        // using the Khalti payment gateway API.
        public async Task<ResponseDataDTO<KhaltiResponseDTO>> OfflinePaymentForAdditionalCharges(AdditonalChargePaymentDTO model)
        {
            try
            {
                // Retrieve the additional charge and customer booking information from the database.
                var additonalCharge = await _dbContext.AdditionalCharges.FindAsync(Guid.Parse(model.AddtionalChargeId));
                var customerBooking = await _dbContext.CustomerBooking.FindAsync(Guid.Parse(additonalCharge.BookingId));

                // Update the additional charge to reflect that it has been paid.
                additonalCharge.IsPaid = true;
                _dbContext.CustomerBooking.Update(customerBooking);
                await _dbContext.SaveChangesAsync(default(CancellationToken));

                return new ResponseDataDTO<KhaltiResponseDTO>
                {
                    Status = "Success",
                    Message = "Success",
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
