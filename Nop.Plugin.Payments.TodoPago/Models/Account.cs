using Nop.Plugin.Payments.TodoPago.DTO;
using System;
using TodoPagoConnector;
using TodoPagoConnector.Model;

namespace Nop.Plugin.Payments.TodoPago.Models
{
    internal class Account
    {
        internal CredentialsResponse GetCredentials(User user, string ambiente)
        {
            User resultUser = new User();
            CredentialsResponse response = new CredentialsResponse();
            TPConnector connector = InitializeConnector(ambiente);

            try
            {
                resultUser = connector.getCredentials(user);
                string[] securityD = resultUser.getApiKey().Split(' ');
                response.security = securityD[1];
                response.success = true;
                response.merchandid = resultUser.getMerchant();
                response.apikey = resultUser.getApiKey();
            }
            catch (Exception ex)
            {
                response.success = false;
                response.message = ex.Message;
            }

            return response;
        }

        private TPConnector InitializeConnector(string ambiente)
        {
            TPConnector connector;

            if (ambiente.Equals("prod"))
                connector = new TPConnector(TPConnector.productionEndpoint);
            else
                connector = new TPConnector(TPConnector.developerEndpoint);

            return connector;
        }
    }
}
