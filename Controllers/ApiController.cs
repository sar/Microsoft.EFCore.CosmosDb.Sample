namespace Microsoft.EFCore.CosmosDb.Sample.Controllers
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EFCore.CosmosDb.Sample.Models;
    using Microsoft.EntityFrameworkCore;
    using Newtonsoft.Json;
    using Microsoft.Extensions.Configuration;

    [AspNetCore.Mvc.ApiController]
    [Produces("application/json")]
    [Route("[controller]")]
    public class ApiController : ApiControllerAttribute
    {
        private readonly ToDoItemsContext _context;
        private readonly IConfiguration _configuration;
        private static bool _instantiated = false;

        public ApiController(ToDoItemsContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        [HttpGet]
        [Route("")]
        public string GetDefault()
        {
            return $"Default endpoint for the {nameof(ApiController)}\\{nameof(this.GetDefault)} method.";
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<string> GetAsync()
        {
            List<ToDoItem> getOpenItems;

            var dbContextOpts = new DbContextOptions<ToDoItemsContext>();
            await using (ToDoItemsContext context = new ToDoItemsContext(dbContextOpts, _configuration))
            {
                await context.Database.EnsureCreatedAsync();
                getOpenItems = await context.ToDoItems
                    .Where(w => w.Completed)
                    .ToListAsync();
            }

            return DefaultSerializer(getOpenItems);
        }

        [HttpGet]
        [Route("[action]")]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public List<string> Error()
        {
            ErrorViewModel err = new ErrorViewModel { RequestId = Activity.Current?.Id ?? null };
            return new List<string> { err.RequestId, err.ShowRequestId.ToString() };
        }

        private static string DefaultSerializer(object any)
        {
            return JsonConvert.SerializeObject(any, Formatting.Indented);
        }
    }
}
