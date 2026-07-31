using FluentAssertions;
using InsuranceApi.DTO;
using InsuranceApi.Models;
using Microsoft.VisualStudio.TestPlatform.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Tests.Helpers;
using Tests.Infrastructure;
using Xunit.Abstractions;

namespace Tests
{
    public  class PolicyTests:IClassFixture<CustomWebApplicationFactory>
    {
        private readonly HttpClient _client;
        private readonly CustomWebApplicationFactory _factory;
        private readonly ITestOutputHelper _output;
        public PolicyTests(CustomWebApplicationFactory factory, ITestOutputHelper output)
        {
            _factory = factory;
            _client = factory.CreateClient();
            _output = output;

        }

        [Fact]
        public async Task Api_Start_Get()
        {
            var response = await _client.GetAsync("/swagger/index.html");
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task Admin_Login_Should_Return_Token()
        {
            var token = await AuthHelpers.Login(_client, TestUsers.AdminEmail, TestUsers.AdminPassword);
            token.Should().NotBeNullOrWhiteSpace();
        }

        [Fact]
        public async Task Customer_Login_Should_Return_Token()
        {
            var token = await AuthHelpers.Login(_client, TestUsers.CustomerEmail, TestUsers.CustomerPassword);
            token.Should().NotBeNullOrWhiteSpace();
        }
        [Fact]
        public async Task Admin_And_Customer_Should_Not_Create_Duplicate_Policy()
        {
            var adminClient = _factory.CreateClient();
            var customerClient = _factory.CreateClient();

            var adminToken = await AuthHelpers.Login(adminClient, TestUsers.AdminEmail, TestUsers.AdminPassword);
            var customerToken = await AuthHelpers.Login(customerClient, TestUsers.CustomerEmail, TestUsers.CustomerPassword);

            //attach the jwt's 

            adminClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", adminToken);
            customerClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", customerToken);

            var purchaseRequest = new CustomerPolicyPurchaseRequestDTO
            {
                PlanId = 1003,
                StartDate = DateTime.Today.AddDays(1)
            };

            var issuePolicyRequest = new AgentOrAdminPolicyIssueRequestDTO
            {
                CustomerId = 2003,
                PlanId = 1003,
                StartDate = DateTime.Today.AddDays(1)
            };


            var adminTask = adminClient.PostAsJsonAsync("api/Policy/issue", issuePolicyRequest);
            var customerTask = customerClient.PostAsJsonAsync("api/policy/purchase", purchaseRequest);

            try
            {
                await Task.WhenAll(adminTask, customerTask);
            }
            catch
            {
                // Ignore for now so we can inspect both responses.
            }

            var adminResponse = await adminTask;
            var customerResponse = await customerTask;

            _output.WriteLine($"Admin Status: {adminResponse.StatusCode}");
            _output.WriteLine(await adminResponse.Content.ReadAsStringAsync());

            _output.WriteLine($"Customer Status: {customerResponse.StatusCode}");
            _output.WriteLine(await customerResponse.Content.ReadAsStringAsync());
        }


        [Fact]
        public async Task Admin_Customer_Policy_Payment_Update()
        {
            var adminClient = _factory.CreateClient();
            var customerClient = _factory.CreateClient();

            var adminToken = await AuthHelpers.Login(adminClient, TestUsers.AdminEmail, TestUsers.AdminPassword);
            var customerToken =await AuthHelpers.Login(customerClient, TestUsers.CustomerEmail, TestUsers.CustomerPassword);

            //default header request

            adminClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", adminToken);
            customerClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", customerToken);

            var adminRequest = new PaymentRequestDTO
            {
                PolicyId = 2013,
                PaymentMode = PaymentMode.UPI,
                Amount = 1000,
                TransactionReference = Guid.NewGuid().ToString()

            };

            var customerRequest = new PaymentRequestDTO
            {
                PolicyId = 2013,
                PaymentMode = PaymentMode.UPI,
                Amount = 1000,
                TransactionReference = Guid.NewGuid().ToString()
            };

            var adminTask = adminClient.PostAsJsonAsync("api/payment/officer-payment", adminRequest);
            var customerTask = customerClient.PostAsJsonAsync("api/payment/my-payment", customerRequest);
            try
            {
                await Task.WhenAll(adminTask, customerTask);
            }
            catch
            {
                // Ignore for now so we can inspect both responses.
            }

            var adminResponse = await adminTask;
            var customerResponse = await customerTask;

            _output.WriteLine($"Admin Status: {adminResponse.StatusCode}");
            _output.WriteLine(await adminResponse.Content.ReadAsStringAsync());

            _output.WriteLine($"Customer Status: {customerResponse.StatusCode}");
            _output.WriteLine(await customerResponse.Content.ReadAsStringAsync());

        }

    }
}
