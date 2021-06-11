using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Metadata;
using Moq;
using NUnit.Framework;
using ProductionManagementSystem.BLL.DTO;
using ProductionManagementSystem.BLL.Interfaces;
using ProductionManagementSystem.BLL.Services;
using ProductionManagementSystem.DAL.Entities;
using ProductionManagementSystem.DAL.Interfaces;
using Task = System.Threading.Tasks.Task;

namespace BLLTests
{
    public class ComponentsServiceTests
    {
        private IComponentService _componentService;

        [SetUp]
        public void Setup()
        {
            var mockUnitOfWork = new Mock<IUnitOfWork>();
            var mockRepositoryComponent = new Mock<IRepository<Component>>();
            mockRepositoryComponent.Setup(r => r.GetAllAsync()).ReturnsAsync(ComponentsServiceTestsSource.Components);
            mockRepositoryComponent.Setup((r) => r.GetAsync(It.IsAny<int>())).ReturnsAsync(ComponentsServiceTestsSource.Components.First());
            mockUnitOfWork.Setup(a => a.Components).Returns(mockRepositoryComponent.Object);
            _componentService = new ComponentService(mockUnitOfWork.Object);
        }

        [Test]
        public async Task GetAllComponentsTest()
        {
            var result = await _componentService.GetComponentsAsync() as List<ComponentDTO>;
            Assert.AreEqual(result, ComponentsServiceTestsSource.ComponentsDto);
        }
        
        [Test]
        public async Task GetComponentTest()
        {
            var result = await _componentService.GetComponentAsync(123);
            Assert.AreEqual(result, ComponentsServiceTestsSource.ComponentsDto.First());
        }
    }
}