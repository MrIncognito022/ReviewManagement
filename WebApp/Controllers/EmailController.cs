using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Net.Mail;
using WebApp.Data;
using WebApp.Models;

public class EmailController : Controller
{
    private readonly ApplicationDbContext _Context;

    public EmailController(ApplicationDbContext dbContext)
    {
        _Context = dbContext;
    }

    public IActionResult SendEmail()
    {
        var model = _Context.Customers.ToList();
        return View(model);
    }
    // Action to send feedback emails
    [HttpGet]
    public IActionResult SendFeedbackEmails()
    {
        List<Customer> customers = _Context.Customers.Where(x=>x.IsEmailSent==false).ToList();

        foreach (Customer customer in customers)
        {

            // Generate a unique feedback URL for each customer
            string feedbackUrl = $"https://localhost:7129/feedback/{customer.Id}";

            // Send email with feedback link to the customer's email
            SendEmail(customer.Email, "Feedback Request", $"Please provide your feedback <a href='{feedbackUrl}'>here</a>.");
            var customerToUpdate = _Context.Customers.FirstOrDefault(c => c.Id == customer.Id);
            if (customerToUpdate != null)
            {
                customerToUpdate.IsEmailSent = true;
                _Context.SaveChanges();
            }
            // You can save the feedback URL in the database if needed
            // customer.FeedbackUrl = feedbackUrl;
        }
        TempData["success1"] = "Email send Successfully";
        return RedirectToAction ("Index","Home");
    }

    [HttpGet]
    public ActionResult Feedback(int id)
    {
        // Retrieve the customer by ID
        Customer customer = _Context.Customers.Find(id);

        if (customer != null)
        {
            // Pass the customer model to the feedback view
            return View(customer);
        }

        // Handle the case when customer is not found
        return NotFound();
    }

    //[HttpPost]
    //public ActionResult Feedback(int id, string feedback)
    //{
    //    // Retrieve the customer by ID
    //    Customer customer = _dbContext.Customers.Find(id);

    //    if (customer != null)
    //    {
    //        // Save the feedback to the customer's record in the database
    //        customer.Feedback = feedback;
    //        _dbContext.SaveChanges();

    //        // Redirect to a thank you page or perform any desired action
    //        return RedirectToAction("ThankYou");
    //    }

    //    // Handle the case when customer is not found
    //    return HttpNotFound();
    //}

    //public ActionResult ThankYou()
    //{
    //    return View();
    //}

    private void SendEmail(string recipient, string subject, string body)
    {
        string senderEmail = "ahmedqureshdummy@gmail.com"; // Replace with your sender email address
        string senderPassword = "wzintsgkzsbubcdd"; // Replace with your sender email password

        var mailMessage = new MailMessage(senderEmail, recipient, subject, body);
        mailMessage.IsBodyHtml = true;

        var smtpClient = new SmtpClient("smtp.gmail.com", 587); // Replace with your SMTP server address and port
        smtpClient.EnableSsl = true;
        smtpClient.UseDefaultCredentials = false;
        smtpClient.Credentials = new NetworkCredential(senderEmail, senderPassword);

        try
        {
            smtpClient.Send(mailMessage);
        }
        catch (SmtpException ex)
        {
            // Handle the exception or log the error
            // For simplicity, rethrowing the exception here
            throw ex;
        }
        finally
        {
            // Dispose of the SmtpClient and MailMessage objects
            smtpClient.Dispose();
            mailMessage.Dispose();
        }
    }
}