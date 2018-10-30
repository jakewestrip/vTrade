using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using IdentityServer4.Models;
using IdentityServer4.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace vTrade.IdentityServer
{
    public class ProfileService : IProfileService
    {
        private readonly IAmazonDynamoDB _amazonDynamoDB;

        public ProfileService(IAmazonDynamoDB amazonDynamoDB)
        {
            _amazonDynamoDB = amazonDynamoDB;
        }

        public Task GetProfileDataAsync(ProfileDataRequestContext context)
        {
            var userId = context.Subject.Claims.First(x => x.Type == "sub").Value;

            var userRoleRequest = new QueryRequest
            {
                TableName = "Users",
                ProjectionExpression = "#R",
                ExpressionAttributeNames = new Dictionary<string, string> { { "#R", "Role" } },
                KeyConditionExpression = "UserId = :v_UserId",
                ExpressionAttributeValues = new Dictionary<string, AttributeValue> {
                        {":v_UserId", new AttributeValue { N = userId }}
                    }
            };
            var userRoleQuery = _amazonDynamoDB.QueryAsync(userRoleRequest).Result;
            var userRole = userRoleQuery.Items.First().First().Value.N;

            context.IssuedClaims.Add(new Claim("role", userRole));

            return Task.CompletedTask;
        }

        public Task IsActiveAsync(IsActiveContext context)
        {
            context.IsActive = true;
            return Task.FromResult(true);
        }
    }
}
