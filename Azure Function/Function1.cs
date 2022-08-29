using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Web.Http;
using Azure_Function.Model;
using System.Linq;
using System.Net.Http;
using Microsoft.EntityFrameworkCore;

namespace Azure_Function
{
    public class Function1
    {

        private readonly BookDatabaseContext _dbContext;

        public Function1(BookDatabaseContext bookDatabaseContext)
        {
            _dbContext = bookDatabaseContext ?? throw new ArgumentNullException(nameof(bookDatabaseContext));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sending the payment details"></param>
        /// <param name="Inserting the payment details "></param>
        /// <returns></returns>

        [FunctionName("Function1")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            string responseMessage = "";

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic paymentdetails = JsonConvert.DeserializeObject(requestBody);
            try
            {               

                if (requestBody != null)
                {
                    var payment = new Payment();
                    payment.BuyerName = paymentdetails.BuyerName;
                    payment.BuyerEmail = paymentdetails.BuyerEmail;
                    payment.BookId= paymentdetails.BookId;
                    payment.PaymentId = paymentdetails.PaymentId;                   
                    _dbContext.Payments.Add(payment);
                    _dbContext.SaveChanges();

                    responseMessage = "Payment inserted successfully";
                    return new OkObjectResult(responseMessage);
                }
                responseMessage = "Error occurred";

                return new OkObjectResult(responseMessage);
            }
            catch (Exception ex)
            {
                return new BadRequestObjectResult(ex.Message);
            }
        }
    }
}
