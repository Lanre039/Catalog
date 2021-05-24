using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Catalog.Api.Controllers;
using Catalog.Api.Dtos;
using Catalog.Api.Entities;
using Catalog.Api.Repositories;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Catalog.UnitTests
{
    public class ItemControllerTests
    {
        private readonly Mock<IItemRepository> respositoryStub = new();
        private readonly Mock<ILogger<ItemController>> loggerStub = new();
        
        private readonly Random rand = new();
        
        [Fact]
        // UnitOfWork_StateUnderTest_ExpectedBehaviour
        public async Task GetItemAsync_WithUnexistingItem_ReturnsNotFound()
        {
            // Arrange
            respositoryStub.Setup(repo => repo.GetItemAsync(It.IsAny<Guid>()))
                            .ReturnsAsync((Item) null);
            
            var controller = new ItemController(respositoryStub.Object, loggerStub.Object);
            
            // ACT
            var result = await controller.GetItemAsync(Guid.NewGuid());
            
            // Assert
            // Assert.IsType<NotFoundResult>(result.Result);
            result.Result.Should().BeOfType<NotFoundResult>();
        }
        
        [Fact]
        public async Task GetItemAsync_WithExistingItem_ReturnsExpectedItem()
        {
            // Arrange
            var item = CreateRandomItem();
            respositoryStub.Setup(repo => repo.GetItemAsync(It.IsAny<Guid>()))
                            .ReturnsAsync(item);
            var controller = new ItemController(respositoryStub.Object, loggerStub.Object);
            
            // Act
            var result = await controller.GetItemAsync(Guid.NewGuid());
            
            // Assert
            // Assert.IsType<ItemDto>(result.Value);
            // var dto = (result as ActionResult<ItemDto>).Value;
            // Assert.Equal(item.Id, dto.Id);
            
            // comapare result object with expectedItems object
            result.Value.Should().BeEquivalentTo(
                        item, 
                        options => options.ComparingByMembers<Item>());
        }
        
        [Fact]
        public async Task GetItemsAsync_WithExistingItems_ReturnAllItems()
        {
            // Arrange
            var expectedIitems = new[]{ CreateRandomItem(), CreateRandomItem(), CreateRandomItem() };
            respositoryStub.Setup(repo => repo.GetItemsAsync())
                            .ReturnsAsync(expectedIitems);
            var controller = new ItemController(respositoryStub.Object, loggerStub.Object);
            
            // Act
            var result = await controller.GetItemsAsync();
            
            // Assert
            // comapare result object with expectedItems object
            result.Should().BeEquivalentTo(
                    expectedIitems,
                    options => options.ComparingByMembers<Item>());
            
        }
        
        [Fact]
        public async Task CreateItemAsync_WithItemToCreate_ReturnCreatedItem()
        {
            //Arrange
            var itemToCreate = new CreateItemDto()
            {
                Name = Guid.NewGuid().ToString(),
                Price = rand.Next(1000)
            };
            
            var controller = new ItemController(respositoryStub.Object, loggerStub.Object);
            
            // Act
            var result = await controller.CreateItemAsync(itemToCreate);
            
            // Assert
            var expectedItem = (result.Result as CreatedAtActionResult).Value as ItemDto;
            // comapare itemToCreate object with expectedItem object and exclude the missing members
            itemToCreate.Should().BeEquivalentTo(
                expectedItem,
                options => options.ComparingByMembers<ItemDto>().ExcludingMissingMembers()
            );
            expectedItem.Id.Should().NotBeEmpty();
            expectedItem.CreatedDate.Should().BeCloseTo(DateTimeOffset.UtcNow, 1000);
            
        }
        
        [Fact]
        public async Task UpdateItemAsync_WithExistingItem_ReturnsNoContent()
        {
            Item existingItem = CreateRandomItem();
            respositoryStub.Setup(repo => repo.GetItemAsync(It.IsAny<Guid>()))
                            .ReturnsAsync(existingItem);
            var itemId = existingItem.Id;
            var itemToUpdate = new UpdateItemDto()
            {
                Name = Guid.NewGuid().ToString(),
                Price = existingItem.Price + 3
            };
            
            var controller = new ItemController(respositoryStub.Object, loggerStub.Object);
            
            var result = await controller.UpdateItemAsync(itemId, itemToUpdate);
            
            result.Should().BeOfType<NoContentResult>();
            
        }
        
        [Fact]
        public async Task DeleteItemAsync_WithExistingItem_ReturnsNoContent()
        {
            Item existingItem = CreateRandomItem();
            respositoryStub.Setup(repo => repo.GetItemAsync(It.IsAny<Guid>()))
                            .ReturnsAsync(existingItem);

            var controller = new ItemController(respositoryStub.Object, loggerStub.Object);
            
            var result = await controller.DeleteItemAsync(existingItem.Id);
            
            result.Should().BeOfType<NoContentResult>();
            
        }
        
        private Item CreateRandomItem()
        {
            return new(){
                Id= Guid.NewGuid(),
                Name = Guid.NewGuid().ToString(),
                Price = rand.Next(1000),
                CreatedDate = DateTimeOffset.UtcNow
            };
        }
    }
}
