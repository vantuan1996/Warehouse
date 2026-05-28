using MediatR;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Warehouse.Core.Commands;
using Warehouse.Core.DTOs;
using Warehouse.Core.Interfaces;
using Warehouse.Core.Queries;
using Warehouse.Domain.Entities;

namespace Warehouse.Core.Handlers
{
    public class CreateCategoryCommandHandler
         : IRequestHandler<CreateCategoryCommand, string>
    {
        private readonly IUnitOfWork _unitOfWork;
        public CreateCategoryCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<string> Handle(
                   CreateCategoryCommand request,
                   CancellationToken cancellationToken)
        {
            var dto = request.Model;

            string imagePath = null;

            // upload ảnh
            if (dto.Image != null)
            {
                var folder = Path.Combine(
                    Directory.GetCurrentDirectory(),
                    "wwwroot",
                    "uploads"
                );

                if (!Directory.Exists(folder))
                    Directory.CreateDirectory(folder);

                var fileName = Guid.NewGuid() + Path.GetExtension(dto.Image.FileName);

                var path = Path.Combine(folder, fileName);

                using (var stream = new FileStream(path, FileMode.Create))
                {
                    await dto.Image.CopyToAsync(stream);
                }

                imagePath = "/uploads/" + fileName;
            }

            var category = new Category
            {
                Id = Guid.NewGuid().ToString(),
                Name = dto.Name,
                Description = dto.Description,
                ImageUrl = imagePath,
                CreatedAt = DateTime.Now
            };

            await _unitOfWork.CategoryRepository.AddAsync(category);

            await _unitOfWork.SaveAsync();

            return category.Id;
        }

    }

    public class UpdateCategoryHandler : IRequestHandler<UpdateCategoryCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;

        public UpdateCategoryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> Handle(UpdateCategoryCommand request, CancellationToken cancellationToken)
        {
            var dto = request.Dto;

            var category = await _unitOfWork.CategoryRepository.GetByIdAsync(request.Id);

            if (category == null)
                throw new Exception("Category not found");

            category.Name = dto.Name;
            category.Description = dto.Description;

            if (dto.Image != null)
            {
                var folder = Path.Combine(
                    Directory.GetCurrentDirectory(),
                    "wwwroot",
                    "uploads"
                );

                if (!Directory.Exists(folder))
                    Directory.CreateDirectory(folder);

                var fileName = Guid.NewGuid() + Path.GetExtension(dto.Image.FileName);

                var path = Path.Combine(folder, fileName);

                using (var stream = new FileStream(path, FileMode.Create))
                {
                    await dto.Image.CopyToAsync(stream);
                }

                category.ImageUrl = "/uploads/" + fileName;
            }

            _unitOfWork.CategoryRepository.Update(category);

            await _unitOfWork.SaveAsync();

            return true;
        }
    }

    public class DeleteCategoryHandler : IRequestHandler<DeleteCategoryCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;

        public DeleteCategoryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> Handle(DeleteCategoryCommand request, CancellationToken cancellationToken)
        {
            //var repo = _unitOfWork.CategoryRepository<Category>();

            var category = await _unitOfWork.CategoryRepository.GetByIdAsync(request.Id);

            if (category == null)
                throw new Exception("Category not found");

            _unitOfWork.CategoryRepository.Delete(category);
            await _unitOfWork.SaveAsync();
            return true;
        }
    }


    public class DeleteCategoryCommandHandler : IRequestHandler<DeleteCategoryIdsCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;

        public DeleteCategoryCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> Handle(DeleteCategoryIdsCommand request, CancellationToken cancellationToken)
        {
            var categories = await _unitOfWork.CategoryRepository
                .Query()
                .Where(x => request.Ids.Contains(x.Id))
                .ToListAsync(cancellationToken);

            if (!categories.Any())
                return false;

            _unitOfWork.CategoryRepository.RemoveRange(categories);

            await _unitOfWork.SaveAsync();

            return true;
        }
    }

}
