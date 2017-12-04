using Braintree;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BakedRaspberryPi
{
    public class BakedPiPaymentServices
    {

        protected BraintreeGateway gateway;

        public BakedPiPaymentServices()
        {
            string merchantId = System.Configuration.ConfigurationManager.AppSettings["Braintree.MerchantId"];
            string environment = System.Configuration.ConfigurationManager.AppSettings["Braintree.Environment"];
            string publicKey = System.Configuration.ConfigurationManager.AppSettings["Braintree.PublicKey"];
            string privateKey = System.Configuration.ConfigurationManager.AppSettings["Braintree.PrivateKey"];

            gateway = new BraintreeGateway(environment, merchantId, publicKey, privateKey);
        }

        public Customer GetCustomer(string email)
        {
            var customerGateway = gateway.Customer;
            CustomerSearchRequest query = new CustomerSearchRequest();
            query.Email.Is(email);
            var matchedCustomers = customerGateway.Search(query);
            if (matchedCustomers.Ids.Count == 0)
            {
                CustomerRequest newCustomer = new CustomerRequest
                {
                    Email = email
                };

                var result = customerGateway.Create(newCustomer);
                return result.Target;
            }
            else
            {
                return matchedCustomers.FirstItem;
            }
        }

        internal Customer UpdateCustomer(string firstName, string lastName, string id)
        {
            CustomerRequest request = new CustomerRequest();
            request.FirstName = firstName;
            request.LastName = lastName;
            var result = gateway.Customer.Update(id, request);
            return result.Target;
        }

        internal void DeleteAddress(string email, string id)
        {
            Customer c = GetCustomer(email);
            gateway.Address.Delete(c.Id, id);
        }

        public void AddAddress(string email, string firstName, string lastName, string company, string streetAddress, string extendedAddress, string locality, string region, string postalCode, string countryName)
        {
            Customer c = GetCustomer(email);

            AddressRequest newAddress = new AddressRequest
            {
                FirstName = firstName,
                LastName = lastName,
                Company = company,
                CountryName = countryName,
                PostalCode = postalCode,
                ExtendedAddress = extendedAddress,
                Locality = locality,
                Region = region,
                StreetAddress = streetAddress
            };

            gateway.Address.Create(c.Id, newAddress);
        }

        public string AuthorizeCard(string email, decimal total, decimal tax, string trackingNumber, string addressId, string cardholderName, string cvv, string cardNumber, string expirationMonth, string expirationYear)
        {
            var customer = GetCustomer(email);
            TransactionRequest transaction = new TransactionRequest();
            transaction.Amount = total;
            transaction.TaxAmount = tax;
            transaction.OrderId = trackingNumber;
            transaction.CustomerId = customer.Id;
            transaction.ShippingAddressId = addressId;
            transaction.CreditCard = new Braintree.TransactionCreditCardRequest
            {
                CardholderName = cardholderName,
                CVV = cvv,
                Number = cardNumber,
                ExpirationYear = expirationMonth,
                ExpirationMonth = expirationYear
            };

            var result = gateway.Transaction.Sale(transaction);

            return result.Message;
        }

    }
}