using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Warehouse.Core.Commands;
using Warehouse.Core.Interfaces;
using Warehouse.Domain.Entities;
using Warehouse.Core.Common.Validators;
namespace Warehouse.Core.Handlers
{
    public class CreateCustomerCommandHandler : IRequestHandler<CreateCustomersCommand, string>
    {
        private readonly IUnitOfWork _unitOfWork;
        public CreateCustomerCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<string> Handle(CreateCustomersCommand request, CancellationToken cancellationToken)
        {

            var dto = request.Model;
            // transaction 
            using var transaction = await _unitOfWork.BeginTransactionAsync();

            try
            {
                var customerId = Guid.NewGuid().ToString();


                if (string.IsNullOrWhiteSpace(dto.LastName))
                {
                    throw new Exception("Last name is required");
                }   

                // Email
                if (!string.IsNullOrWhiteSpace(dto.Email))
                {
                    if (!ValidationHelper.IsValidEmail(dto.Email))
                        throw new Exception("Email không hợp lệ");
                }

                // Phone (VN basic)
                if (!string.IsNullOrWhiteSpace(dto.Phone))
                {
                    if (!ValidationHelper.IsValidPhone(dto.Phone))
                        throw new Exception("Số điện thoại không hợp lệ");
                }

                // Ngày sinh
                DateTime? dob = null;
                if (!string.IsNullOrWhiteSpace(dto.DateOfBirth))
                {
                    if (!DateTime.TryParse(dto.DateOfBirth, out var parsedDob))
                        throw new Exception("Ngày sinh không hợp lệ");

                    dob = parsedDob;
                }
                // ======================
                // 1. CREATE CUSTOMER
                // ======================
                var customer = new Customers
                {
                    Id = customerId,

                    FirstName = dto.FirstName,
                    LastName = dto.LastName,

                    Email = dto.Email,
                    Phone = dto.Phone,

                    Gender = Convert.ToInt32(dto.Gender),
                    DateOfBirth = DateTime.Parse(dto.DateOfBirth),

                    AcceptMarketing = (bool)dto.AcceptMarketing,
                    Note = dto.Note,
                    CustomerGroupId = dto.CustomerGroupId,

                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = "system"
                };

                await _unitOfWork.CustomersRepository.AddAsync(customer);

                // ======================
                // 2. CREATE ADDRESSES
                // ======================

                var addresses = new CustomerAddresses
                {
                    Id = Guid.NewGuid().ToString(),
                    CustomerId = customerId,

                    FirstName = dto.FirstNamePersonRecive,
                    LastName = dto.LastNamePersonRecive,
                    Company = dto.Company,
                    Phone = dto.Phone,

                    Country = dto.Country,
                    Province = dto.Province,
                    District = dto.District,
                    Ward = dto.Ward,

                    AddressLine = dto.AddressLine,
                    PostalCode = dto.PostalCode,

                    IsDefault = (bool)dto.IsDefault,

                    CreatedAt = DateTime.Now,
                    CreatedBy = "system"
                };

                await _unitOfWork.CustomerAddressesRepository.AddAsync(addresses);


                // ======================
                // 3. CREATE TAX INFO
                // ======================
                if ((bool)dto.IsActiveTax)
                {
                    var tax = new CustomerTaxInfos
                    {
                        Id = Guid.NewGuid().ToString(),
                        CustomerId = customerId,

                        CompanyName = dto.CompanyName,
                        TaxCode = dto.TaxCode,
                        Address = dto.Address,
                        Email = dto.EmailTax,
                        BudgetCode = dto.BudgetCode,
                        BuyerName = dto.BuyerName,
                        CardId = dto.CardId,
                        Phone = dto.PhoneTax,
                        IsActive = (bool)dto.IsActiveTax,
                        CreatedAt = DateTime.Now,
                        CreatedBy = "system"
                    };

                    await _unitOfWork.CustomerTaxInfosRepository.AddAsync(tax);
                }

                // ======================
                // SAVE
                // ======================
                await _unitOfWork.SaveAsync();
                await transaction.CommitAsync();

                return customerId;
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

    }
   
    public class UpdateCustomerHandler : IRequestHandler<UpdateCustomerCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;

        public UpdateCustomerHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> Handle(UpdateCustomerCommand request, CancellationToken cancellationToken)
        {
            var dto = request.Dto;

            var customer = await _unitOfWork.CustomersRepository.GetByIdAsync(request.Id);

            if (customer == null)
                throw new Exception("customer not found");



            customer.FirstName = dto.FirstName;
            customer.LastName = dto.LastName;
            customer.DateOfBirth = DateTime.Parse(dto.DateOfBirth);
            customer.Phone = dto.Phone;
            customer.Email = dto.Email;
            customer.Gender = Convert.ToInt32(dto.Gender);
            customer.AcceptMarketing = (bool)dto.AcceptMarketing;
            customer.UpdatedAt = DateTime.Now;
            customer.UpdatedBy = "Ok";
            _unitOfWork.CustomersRepository.Update(customer);
            await _unitOfWork.SaveAsync();

            return true;
        }

    }

    public class CreateAddressCommandHandler : IRequestHandler<CreateAddressCommand, string>
    {
        private readonly IUnitOfWork _unitOfWork;
        public CreateAddressCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<string> Handle(CreateAddressCommand request, CancellationToken cancellationToken)
        {

            var dto = request.Model;
            // transaction 
            using var transaction = await _unitOfWork.BeginTransactionAsync();

            try
            {
                
                // Phone (VN basic)
                if (!string.IsNullOrWhiteSpace(dto.Phone))
                {
                    if (!ValidationHelper.IsValidPhone(dto.Phone))
                        throw new Exception("Số điện thoại không hợp lệ");
                }

                
              

                // ======================
                // 2. CREATE ADDRESSES
                // ======================

                var addresses = new CustomerAddresses
                {
                    Id = Guid.NewGuid().ToString(),
                    CustomerId = dto.Id,

                    FirstName = dto.FirstNamePersonRecive,
                    LastName = dto.LastNamePersonRecive,
                    Company = dto.Company,
                    Phone = dto.Mobile,

                    Country = dto.Country,
                    Province = dto.Province,
                    District = dto.District,
                    Ward = dto.Ward,

                    AddressLine = dto.AddressLine,
                    PostalCode = dto.PostalCode,

                    IsDefault = (bool)dto.IsDefault,

                    CreatedAt = DateTime.Now,
                    CreatedBy = "system"
                };

                await _unitOfWork.CustomerAddressesRepository.AddAsync(addresses);


                //// ======================
                //// 3. CREATE TAX INFO
                //// ======================
                //if ((bool)dto.IsActiveTax)
                //{
                //    var tax = new CustomerTaxInfos
                //    {
                //        Id = Guid.NewGuid().ToString(),
                //        CustomerId = customerId,

                //        CompanyName = dto.CompanyName,
                //        TaxCode = dto.TaxCode,
                //        Address = dto.Address,
                //        Email = dto.EmailTax,
                //        BudgetCode = dto.BudgetCode,
                //        BuyerName = dto.BuyerName,
                //        CardId = dto.CardId,
                //        Phone = dto.PhoneTax,
                //        IsActive = (bool)dto.IsActiveTax,
                //        CreatedAt = DateTime.Now,
                //        CreatedBy = "system"
                //    };

                //    await _unitOfWork.CustomerTaxInfosRepository.AddAsync(tax);
                //}

                // ======================
                // SAVE
                // ======================
                await _unitOfWork.SaveAsync();
                await transaction.CommitAsync();

                return "1";
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

    }

    public class UpdateProductHandler : IRequestHandler<UpdateAddressCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;

        public UpdateProductHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> Handle(UpdateAddressCommand request, CancellationToken ct)
        {
            var dto = request.Dto;

            var address = await _unitOfWork.CustomerAddressesRepository.GetByIdAsync(request.Id);

            if (address == null)
                throw new Exception("địa chỉ not found");
            string? finalCategoryId = null;

            address.IsDefault = (bool) dto.IsDefault;
            address.Ward = dto.Ward;
            address. AddressLine= dto.AddressLine;
            address.Phone = dto.Mobile;
            address.District = dto.District;
            address.District = dto.District;
            address.Company = dto.Company;
            address.Country = dto.Country;
            address.FirstName = dto.FirstName;
            address.LastName = dto.LastName;
            address.PostalCode = dto.PostalCode;
            address.UpdatedAt = DateTime.Now;
            address.UpdatedBy = "ok";
          
            _unitOfWork.CustomerAddressesRepository.Update(address);

           
            await _unitOfWork.SaveAsync();

            return true;
        }
    }


}
