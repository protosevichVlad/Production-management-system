using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using ProductionManagementSystem.BLL.DTO;
using ProductionManagementSystem.BLL.Interfaces;
using ProductionManagementSystem.DAL.Entities;
using ProductionManagementSystem.DAL.Interfaces;

namespace ProductionManagementSystem.BLL.Services
{
    public class DesignService : IDesignService
    {
        private IUnitOfWork _database { get; }
        private IMapper _mapperToDTO;
        private IMapper _mapperFromDTO;
        
        public DesignService(IUnitOfWork uow)
        {
            _database = uow;
            _mapperToDTO = new MapperConfiguration(cfg => cfg.CreateMap<Design, DesignDTO>())
                .CreateMapper();
            _mapperFromDTO = new MapperConfiguration(cfg => cfg.CreateMap<DesignDTO, Design>())
                .CreateMapper();
        }
        
        public void CreateDesign(DesignDTO designDto)
        {
            var design = _mapperFromDTO.Map<DesignDTO, Design>(designDto);
            _database.Designs.Create(design);
            _database.Save();
        }

        public void UpdateDesign(DesignDTO designDto)
        {
            var design = _mapperFromDTO.Map<DesignDTO, Design>(designDto);
            _database.Designs.Update(design);
            _database.Save();
        }

        public IEnumerable<DesignDTO> GetDesigns()
        {
            return _mapperToDTO.Map<IEnumerable<Design>, IEnumerable<DesignDTO>>(_database.Designs.GetAll());
        }

        public DesignDTO GetDesign(int? id)
        {
            if (id == null)
            {
                throw new NotImplementedException();
            }

            return _mapperToDTO.Map<Design, DesignDTO>(_database.Designs.Get((int) id));
        }

        public void DeleteDesign(int? id)
        {
            if (id == null)
            {
                throw new NotImplementedException();
            }

            _database.Designs.Delete((int) id);
            _database.Save();
        }

        public IEnumerable<string> GetTypes()
        {
            var designs = _database.Designs.GetAll();
            IEnumerable<string> types = designs.OrderBy(d => d.Type).Select(d => d.Type).Distinct();
            return types;
        }

        public void AddDesign(int? id, int quantity)
        {
            if (id == null)
            {
                throw new NotImplementedException();
            }

            var design = _database.Designs.Get((int) id);
            design.Quantity += quantity;
            _database.Save();
        }

        public void Dispose()
        {
            _database.Dispose();
        }
        
        private bool CheckInDevices(Design design , out string errorMessage)
        {
            var designInDevice = _database.DeviceDesignTemplate.GetAll()
                .FirstOrDefault(d => design.Id == d.DesignId);
            if (designInDevice != null)
            {
                var device = _database.Devices.GetAll().FirstOrDefault(d => d.Id == designInDevice.DeviceId);
                errorMessage = "Невозможно удаление конструктива.<br />" +
                               $"<i class='bg-light'>{design}</i> используется в <i class='bg-light'>{device}</i>.<br />" +
                               $"Для удаления <i class='bg-light'>{design}</i>, удалите <i class='bg-light'>{device}</i>.<br />";
                return false;
            }

            errorMessage = "";
            return true;
        }
        
                
        private bool DesignExists(int id)
        {
            return _database.Designs.GetAll().Any(e => e.Id == id);
        }
    }
}