using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Wodsoft.ComBoost.Test;
using Wodsoft.ComBoost.Test.Models;
using Xunit;

namespace Wodsoft.ComBoost.Mvc.Data.Test
{
    public class EntityDTOControlerTest
    {
        [Fact]
        public async Task Base_Test()
        {
            using var host = await new HostBuilder()
                .ConfigureWebHost(webBuilder =>
                {
                    webBuilder
                        .UseEnvironment(Environments.Development)
                        .ConfigureLogging(builder => builder.AddDebug())
                        .UseTestServer()
                        .UseStartup<SingleMvcStartup>();
                })
                .StartAsync();
            var client = host.GetTestClient();

            //Wodsoft.ComBoost.Grpc.AspNetCore.DomainGrpcService.GetAssembly();
            //var generator = new Lokad.ILPack.AssemblyGenerator();
            //// for ad-hoc serialization
            //var bytes = generator.GenerateAssemblyBytes(Wodsoft.ComBoost.Grpc.AspNetCore.DomainGrpcService.GetAssembly());
            //System.IO.File.WriteAllBytes("dynamic.dll", bytes);

            var serializerOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };

            var viewModel = JsonSerializer.Deserialize<ClientViewModel<UserDto>>(await client.GetStringAsync("/api/user"), serializerOptions);
            Assert.Empty(viewModel.Items);

            var newUser = new UserDto
            {
                Id = Guid.NewGuid(),
                Email = "test@test.com",
                UserName = "test",
                Password = "test"
            };

            var postContent = new StringContent(JsonSerializer.Serialize(newUser, serializerOptions), Encoding.UTF8, "application/json");
            var response = await client.PostAsync("/api/user", postContent);
            var updateModel = JsonSerializer.Deserialize<ClientUpdateModel<UserDto>>(await response.Content.ReadAsStringAsync(), serializerOptions);
            Assert.True(updateModel.IsSuccess);
            Assert.Empty(updateModel.ErrorMessage);
            Assert.Equal(newUser.Id, updateModel.Result.Id);

            viewModel = JsonSerializer.Deserialize<ClientViewModel<UserDto>>(await client.GetStringAsync("/api/user"), serializerOptions);
            Assert.Single(viewModel.Items);
            Assert.Equal(newUser.UserName, viewModel.Items[0].UserName);

            newUser.UserName = "newUsername";
            var putContent = new StringContent(JsonSerializer.Serialize(newUser, serializerOptions), Encoding.UTF8, "application/json");
            response = await client.PutAsync("/api/user", putContent);
            updateModel = JsonSerializer.Deserialize<ClientUpdateModel<UserDto>>(await response.Content.ReadAsStringAsync(), serializerOptions);
            Assert.True(updateModel.IsSuccess);
            Assert.Empty(updateModel.ErrorMessage);
            Assert.Equal(newUser.Id, updateModel.Result.Id);

            viewModel = JsonSerializer.Deserialize<ClientViewModel<UserDto>>(await client.GetStringAsync("/api/user"), serializerOptions);
            Assert.Single(viewModel.Items);
            Assert.Equal(newUser.UserName, viewModel.Items[0].UserName);

            var deleteContent = new StringContent(JsonSerializer.Serialize(newUser, serializerOptions), Encoding.UTF8, "application/json");
            var message = new HttpRequestMessage(HttpMethod.Delete, "/api/user");
            message.Content = deleteContent;
            response = await client.SendAsync(message);
            updateModel = JsonSerializer.Deserialize<ClientUpdateModel<UserDto>>(await response.Content.ReadAsStringAsync(), serializerOptions);
            Assert.True(updateModel.IsSuccess);
            Assert.Empty(updateModel.ErrorMessage);
            Assert.Null(updateModel.Result);

            viewModel = JsonSerializer.Deserialize<ClientViewModel<UserDto>>(await client.GetStringAsync("/api/user"), serializerOptions);
            Assert.Empty(viewModel.Items);
        }
    }
}
