namespace Microsoft.EFCore.CosmosDb.Sample.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Text.Json;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EFCore.CosmosDb.Sample.Models;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.ChangeTracking;
    using Microsoft.Extensions.Configuration;

    [AspNetCore.Mvc.ApiController]
    [Produces("application/json")]
    [Route("[controller]")]
    public class ApiController : ApiControllerAttribute
    {
        private readonly ToDoItemsContext _context;
        private readonly DbContextOptions<ToDoItemsContext> _options;
        private readonly IConfiguration _configuration;
        private static bool _instantiated = false;

        private string NoContentResult { get; } =
            $"No content found in database for defined query {typeof(List<ToDoItem>)}";

        public ApiController(
            ToDoItemsContext context,
            IConfiguration configuration,
            DbContextOptions<ToDoItemsContext> options)
        {
            _context = context;
            _configuration = configuration;
            _options = options;
        }

        private static string DefaultSerializer(object any)
        {
            return System.Text.Json.JsonSerializer.Serialize(any, new JsonSerializerOptions()
            {
                WriteIndented = true,
                IgnoreNullValues = true,
                DictionaryKeyPolicy = JsonNamingPolicy.CamelCase,
                //Encoder = JavaScriptEncoder.Default,
            });
        }

        [HttpGet]
        [Route("")]
        public string GetDefault()
        {
            return $"Default endpoint for the {nameof(ApiController)}\\{nameof(this.GetDefault)} method.";
        }

        [HttpGet]
        [Route("[action]")]
        [ProducesResponseType(typeof(List<ToDoItem>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status204NoContent)]
        public async Task<string> GetAllAsync()
        {
            await using (ToDoItemsContext context = new ToDoItemsContext(_options, _configuration))
            {
                await context.Database.EnsureCreatedAsync();
                List<ToDoItem> getOpenItems = await context.ToDoItems
                    .Where(w => !w.Completed)
                    .ToListAsync();

                return DefaultSerializer(getOpenItems) ?? NoContentResult;
            }
        }

        [HttpPost]
        [Route("[action]")]
        [ProducesResponseType(typeof(string), StatusCodes.Status201Created)]
        public async Task<string> PostSeedAsync()
        {
            List<ToDoItem> sampleToDoList = new List<ToDoItem>
            {
                new ToDoItem
                {
                    Id = Guid.NewGuid(),
                    Name = "Virtual Golfing with Remote Team",
                    Description =
                        "Engage the remote team in a virtual golfing tournament at the local multi-player arena.",
                    Completed = false,
                },
                new ToDoItem
                {
                    Id = Guid.NewGuid(),
                    Name = "Book International Flight to Sea-Tac",
                    Description = "Departure from Toronto (CYYZ) to destination Seattle (KSEA) sometime next month.",
                    Completed = false,
                },
                new ToDoItem
                {
                    Id = Guid.NewGuid(),
                    Name = "Tour Seattle Space Needle",
                    Description =
                        "The Seattle Space Needle is a landmark observation tower and a must visit location for any tourist.",
                    Completed = false,
                },
            };

            try
            {
                foreach (ToDoItem item in sampleToDoList)
                {
                    await using (ToDoItemsContext context = new ToDoItemsContext(_options, _configuration))
                    {
                        await context.ToDoItems.AddAsync(item);
                        await context.SaveChangesAsync();
                    }
                }

                return $"Iterated {nameof(ToDoItem)} in list {nameof(sampleToDoList)} for {nameof(EntityEntry)} type.";
            }
            catch (CosmosException ce)
            {
                Console.WriteLine(ce);
            }

            return null;
        }

        [HttpPost]
        [Route("[action]")]
        [ProducesResponseType(typeof(string), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(CosmosException), StatusCodes.Status424FailedDependency)]
        [ProducesErrorResponseType(typeof(CosmosException))]
        public async Task<string> PostAsync([FromBody] ToDoItem toDoItem)
        {
            try
            {
                if (toDoItem == null)
                {
                    return Error();
                }

                await using (ToDoItemsContext context = new ToDoItemsContext(_options, _configuration))
                {
                    await context.ToDoItems.AddAsync(toDoItem);
                    await context.SaveChangesAsync();
                    ToDoItem insertedItem = await context.ToDoItems.LastOrDefaultAsync();
                    return $"Created item in database with GUID: {insertedItem.Id} from serialized body input.";
                }
            }
            catch (CosmosException ce)
            {
                Console.WriteLine(ce);
                return $"Caught cosmos exception while performing operation: {ce}";
            }
        }

        [HttpGet]
        [Route("[action]")]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        private string Error()
        {
            ErrorViewModel err = new ErrorViewModel { RequestId = Activity.Current?.Id ?? null };
            return DefaultSerializer(new List<string> { err.RequestId, err.ShowRequestId.ToString() });
        }
    }

    }
}