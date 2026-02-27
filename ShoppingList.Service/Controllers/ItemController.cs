using Microsoft.AspNetCore.Mvc;
using ShoppingList.Core.Domain.Entities;
using ShoppingList.Core.Domain.RepositoryContracts;
using ShoppingList.Core.Dtos;
using ShoppingList.Core.Mappings;
using Microsoft.Extensions.Caching.Hybrid;
using System.Threading.Channels;
using System.Collections.Concurrent;

namespace ShoppingList.Service.Controllers
{
    [ApiController]
    [Route("items")]
    [Consumes("application/json", "application/xml")]
    [Produces("application/json", "application/xml")]
    public class ItemController : ControllerBase
    {
        private readonly IRepository<Item> _repository;
        private readonly ILogger<ItemController> _logger;
        private readonly HybridCache _cache;

        public ItemController(IRepository<Item> repository, ILogger<ItemController> logger, HybridCache cache)
        {
            _repository = repository;
            _logger = logger;
            _cache = cache;   
        }

        [HttpGet("all")]
        public async Task<ActionResult<List<ItemDto>>> GetAllAsync()
        {
            var items = await _repository.GetAllAsync();
            var itemDtos = items.Select(item => item.ToDto()).ToList();

            return Ok(itemDtos);
        }

        [HttpGet()]
        public async Task<ActionResult<ItemsPageDto>> GetByParametersAsync([FromQuery] GetItemsDto request)
        {
            if (request.PageSize <= 0) request.PageSize = 10;
            if (request.PageNumber <= 0) request.PageNumber = 1;

            var filteredItems = await _repository.GetByParametersAsync(request.Name, request.PageNumber, request.PageSize);

            var itemsOnPage = filteredItems.Select(item => new ItemDto
            {
                Id = item.Id,
                Name = item.Name,
                Description = item.Description,
                Price = item.Price,
                Status = item.Status.ToString(),
                UpdatedAt = item.UpdatedAt
            }).ToList();

            var totalItems = await _repository.CountAsync(request.Name);
            var totalPages = (int)Math.Ceiling(totalItems / (double)request.PageSize);

            return Ok(new ItemsPageDto
            {
                TotalPages = totalPages,
                Data = itemsOnPage
            });
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ItemDto>> GetAsync(Guid id)
        {
            _logger.LogInformation("Attempting to cache CreateOrDelete <Item Id={Id}>", id);
            var cachedItem = await _cache.GetOrCreateAsync($"items_{id}", async entry =>
            {
                _logger.LogInformation("Attempting to GetById <Item Id={Id}>", id);
                Item? item = await _repository.GetByIdAsync(id);
                return item;
            }, tags: ["items"]);

            if (cachedItem == null)
            {
                _logger.LogWarning("Attempted to get non-existing item with Id={Id}", id);
                return NotFound();
            }

            return Ok(cachedItem.ToDto());
        }

        [HttpGet("{id}/status", Name = nameof(GetStatus))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult GetStatus(Guid id,
        [FromServices] ConcurrentDictionary<string, GenerationStatus> statusDictionary)
        {
            if (!statusDictionary.TryGetValue(id.ToString(), out GenerationStatus status))
            {
                return NotFound();
            }

            var response = new { id, status = status.ToString() };
            return Ok(response);
        }

        [HttpPost("{id}/generate-job")]
        [ProducesResponseType(StatusCodes.Status202Accepted)]
        public async Task<IActionResult> GenerateAsync(
            [FromServices] Channel<GenerationJob> channel,
            [FromServices] ConcurrentDictionary<string, GenerationStatus> statusDictionary,
            Guid id
        )
        {
            string jobId = id.ToString();
            var job = new GenerationJob(jobId);
            await channel.Writer.WriteAsync(job);

            statusDictionary[jobId] = GenerationStatus.Queued;

            var statusUrl = Url.Link(nameof(GetStatus), new { id });
            return Accepted(statusUrl, new {id = id, status = GenerationStatus.Queued.ToString() });
        }


        [HttpPost]
        [ProducesResponseType(typeof(ItemDto), StatusCodes.Status201Created)]
        public async Task<IActionResult> AddAsync(CreateItemDto itemDto)
        {
            var item = new Item
            {
                Id = Guid.NewGuid(),
                Name = itemDto.Name,
                Description = itemDto.Description,
                Price = itemDto.Price,
                Status = ItemStatus.NotPurchased,
                UpdatedAt = DateTime.UtcNow,

            };

            await _repository.InsertAsync(item);
            _logger.LogInformation("Created <Item Id={Id} Name={Name} Status={Status}>", item.Id, item.Name, item.Status);

            await _cache.SetAsync($"items_{item.Id}", item, tags: ["items"]);
            _logger.LogInformation("Cached <Item Id={Id}>", item.Id);

            return CreatedAtAction(nameof(GetAsync), new { id = item.Id }, item.ToDto());
        }

        [HttpPatch("{id}")]
        public async Task<IActionResult> PatchAsync(Guid id)
        {
            Item? existingItem = await _repository.GetByIdAsync(id);
            if (existingItem == null)
            {
                _logger.LogWarning("Attempted to change status of non-existing item with Id={Id}", id);
                return NotFound();
            }

            existingItem.Status = existingItem.Status == ItemStatus.NotPurchased ?
                ItemStatus.Purchased : ItemStatus.NotPurchased;
            existingItem.UpdatedAt = DateTime.UtcNow;

            await _repository.UpdateAsync(existingItem);
            _logger.LogInformation("Status changed <Item Id={Id} Name={Name} Status={Status}>",
                existingItem.Id, existingItem.Name, existingItem.Status);

            await _cache.SetAsync($"items_{existingItem.Id}", existingItem, tags: ["items"]);
            _logger.LogInformation("Cached <Item Id={Id}>", existingItem.Id);

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync(Guid id)
        {
            Item? existingItem = await _repository.GetByIdAsync(id);
            if (existingItem == null)
            {
                _logger.LogWarning("Attempted to delete non-existing item with Id={Id}", id);
                return NotFound();
            }

            _logger.LogInformation("Attempting to Delete <Item Id={Id}>", existingItem.Id);
            await _repository.RemoveAsync(id);
            _logger.LogInformation("Deleted <Item Id={Id} Name={Name} Status={Status}>",
                 existingItem.Id, existingItem.Name, existingItem.Status);

            return NoContent();
        }

        [HttpDelete("{id}/invalidate-cache")]
        public async Task<IActionResult> InvalidateCacheAsync(Guid id)
        {
            await _cache.RemoveAsync($"items_{id}");
            // await _cache.RemoveByTagAsync("items");
            return NoContent();
        }

        [HttpPost("initialize")]
        public async Task<IActionResult> InitializeAsync([FromBody] List<CreateItemDto> itemsDto)
        {
            var items = itemsDto.Select(dto => new Item
            {
                Id = Guid.NewGuid(),
                Name = dto.Name,
                Description = dto.Description,
                Price = dto.Price,
                Status = ItemStatus.NotPurchased,
                UpdatedAt = DateTime.UtcNow,
            }).ToList();

            await _repository.InitializeAsync(items);
            _logger.LogInformation("Created {Count} items", items.Count);

            return NoContent();
        }

        [HttpDelete("clear")]
        public async Task<IActionResult> ClearAsync()
        {
            await _repository.ClearAsync();
            _logger.LogInformation("Deleted all items from db");

            _logger.LogInformation("Deleted all items from cache");
            await _cache.RemoveByTagAsync("items");

            return NoContent();
        }
    }
}