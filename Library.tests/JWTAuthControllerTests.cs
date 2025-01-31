using Library.Api.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Moq;
using System.Collections.Generic;
using Xunit;

namespace Library.Tests
{
    public class AuthControllerTests
    {
        private readonly AuthController _controller;
        private readonly Mock<IConfiguration> _configMock;

        public AuthControllerTests()
        {
            _configMock = new Mock<IConfiguration>();

            var inMemorySettings = new Dictionary<string, string>
            {
                { "Jwt:Key", "ThisIsAReallySuperStrongKeyThatHasMoreThan256bits" },
                { "Jwt:Issuer", "WSEI" },
                { "Jwt:Audience", "WSEI" },
                { "Jwt:ExpiresInMinutes", "30" }
            };

            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(inMemorySettings)
                .Build();

            _controller = new AuthController(configuration);
        }

        [Fact]
        public void Login_ReturnsToken_WhenCredentialsAreValid()
        {
            var loginModel = new LoginModel { Username = "test", Password = "password" };

            var result = _controller.Login(loginModel);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var resultValue = okResult.Value;

            var usernameProp = resultValue.GetType().GetProperty("Username").GetValue(resultValue, null);
            var tokenProp = resultValue.GetType().GetProperty("Token").GetValue(resultValue, null);
            var expiryProp = resultValue.GetType().GetProperty("Expiry").GetValue(resultValue, null);

            Assert.Equal("test", usernameProp);
            Assert.NotNull(tokenProp);
            Assert.NotNull(expiryProp);
        }

        [Fact]
        public void Login_ReturnsUnauthorized_WhenCredentialsAreInvalid()
        {
            var loginModel = new LoginModel { Username = "invalid", Password = "invalid" };

            var result = _controller.Login(loginModel);

            Assert.IsType<UnauthorizedResult>(result);
        }

        
    }
}
