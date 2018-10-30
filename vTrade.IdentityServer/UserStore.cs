using Amazon.DynamoDBv2;
using IdentityServer4.Test;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace vTrade.IdentityServer
{
    public interface IUserStore
    {
        List<TestUser> GetUsers();
    }

    public class UserStore : IUserStore
    {
        private readonly IAmazonDynamoDB _amazonDynamoDB;

        public UserStore(IAmazonDynamoDB amazonDynamoDB)
        {
            _amazonDynamoDB = amazonDynamoDB;
        }

        public List<TestUser> GetUsers()
        {
            var scan = _amazonDynamoDB.ScanAsync("Users", new List<string>() { "UserId", "Name", "Password", "Role", "Username" }).Result;
            var users = scan.Items.Select(x => new TestUser()
            {
                SubjectId = x["UserId"].N,
                Username = x["Username"].S,
                Password = x["Password"].S,
                Claims = new List<Claim>
                {
                    new Claim("name", x["Name"].S),
                    new Claim("role", x["Role"].N)
                }
            }).ToList();

            return users;
        }
    }
}
