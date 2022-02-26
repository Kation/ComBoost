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

            var viewModel = JsonSerializer.Deserialize<ClientViewModel<UserDto>>(await client.GetStringAsync("/api/user/list"), serializerOptions);
            Assert.Empty(viewModel.Items);

            var newUser = new UserDto
            {
                Id = Guid.NewGuid(),
                Email = "test@test.com",
                UserName = "test",
                DisplayName = "test",
                Password = "test"
            };

            var postContent = new StringContent(JsonSerializer.Serialize(newUser, serializerOptions), Encoding.UTF8, "application/json");
            var response = await client.PostAsync("/api/user/create", postContent);
            var updateModel = JsonSerializer.Deserialize<ClientUpdateModel<UserDto>>(await response.Content.ReadAsStringAsync(), serializerOptions);
            Assert.True(updateModel.IsSuccess);
            Assert.Empty(updateModel.ErrorMessage);
            Assert.Equal(newUser.Id, updateModel.Item.Id);

            viewModel = JsonSerializer.Deserialize<ClientViewModel<UserDto>>(await client.GetStringAsync("/api/user/list"), serializerOptions);
            Assert.Single(viewModel.Items);
            Assert.Equal(newUser.DisplayName, viewModel.Items[0].DisplayName);

            newUser.DisplayName = "newUsername";
            var putContent = new StringContent(JsonSerializer.Serialize(newUser, serializerOptions), Encoding.UTF8, "application/json");
            response = await client.PutAsync("/api/user/edit", putContent);
            updateModel = JsonSerializer.Deserialize<ClientUpdateModel<UserDto>>(await response.Content.ReadAsStringAsync(), serializerOptions);
            Assert.True(updateModel.IsSuccess);
            Assert.Empty(updateModel.ErrorMessage);
            Assert.Equal(newUser.Id, updateModel.Item.Id);

            viewModel = JsonSerializer.Deserialize<ClientViewModel<UserDto>>(await client.GetStringAsync("/api/user/list"), serializerOptions);
            Assert.Single(viewModel.Items);
            Assert.Equal(newUser.DisplayName, viewModel.Items[0].DisplayName);

            response = await client.DeleteAsync("/api/user/remove?id=" + newUser.Id);
            Assert.Equal(System.Net.HttpStatusCode.NoContent, response.StatusCode);

            viewModel = JsonSerializer.Deserialize<ClientViewModel<UserDto>>(await client.GetStringAsync("/api/user/list"), serializerOptions);
            Assert.Empty(viewModel.Items);
        }
    }
}
