using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using Warehouse.Core.DTOs;
using Warehouse.Core.Interfaces;
using Warehouse.Core.Queries;
using Warehouse.Domain.Entities;

namespace Warehouse.Core.Handlers
{
    public class GetCustomerHandler
    : IRequestHandler<GetCustomerQuery, CustomerDto?>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetCustomerHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<CustomerDto?> Handle(GetCustomerQuery request, CancellationToken cancellationToken)
        {
            // 1. Lấy customer + tax trước
            var customer = await (
                from c in _unitOfWork.CustomersRepository.Query()
                where c.Id == request.Id

                join t in _unitOfWork.CustomerTaxInfosRepository.Query()
                    on c.Id equals t.CustomerId into taxGroup
                from tx in taxGroup.DefaultIfEmpty()

                select new CustomerDto
                {
                    Id = c.Id,
                    FirstName = c.FirstName,
                    LastName = c.LastName,
                    Email = c.Email,
                    Phone = c.Phone,
                    Gender = c.Gender.ToString(),
                    DateOfBirth = c.DateOfBirth.ToString(),
                    AcceptMarketing = c.AcceptMarketing,
                    Note = c.Note ?? "",
                    CustomerGroupId = c.CustomerGroupId,

                    // TAX
                    IsActiveTax = tx != null,
                    TaxCode = tx != null ? tx.TaxCode : null,
                    CompanyName = tx != null ? tx.CompanyName : null,
                    Address = tx != null ? tx.Address : null,
                    BuyerName = tx != null ? tx.BuyerName : null,
                    CardId = tx != null ? tx.CardId : null,
                    BudgetCode = tx != null ? tx.BudgetCode : null,
                    PhoneTax = tx != null ? tx.Phone : null,
                    EmailTax = tx != null ? tx.Email : null,

                    CreateBy = "OKAdmin"
                }
            ).FirstOrDefaultAsync(cancellationToken);

            if (customer == null) return null;

            // 2. Lấy list address riêng
            var addresses = await _unitOfWork.CustomerAddressesRepository.Query()
                .Where(x => x.CustomerId == request.Id)
                .Select(ad => new AddressDto
                {
                    Id = ad.Id,
                    Province = ad.Province,
                    District = ad.District,
                    Ward = ad.Ward,
                    FirstName = ad.FirstName,
                    LastName = ad.LastName,
                    Company = ad.Company,
                    Mobile = ad.Phone,
                    Country = ad.Country ?? "Vietnam",
                    PostalCode = ad.PostalCode,
                    AddressLine = ad.AddressLine,
                    IsDefault = ad.IsDefault
                })
                .ToListAsync(cancellationToken);

            customer.Addresses = addresses;

            return customer;
        }
    }


    public class GetAllCustomerQueryHandler
    : IRequestHandler<GetALLCustomerQuery, PagedResult<CustomerDto>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetAllCustomerQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<PagedResult<CustomerDto>> Handle(GetALLCustomerQuery request, CancellationToken cancellationToken)
        {
            var query =
                from c in _unitOfWork.CustomersRepository.Query()

                join t in _unitOfWork.CustomerTaxInfosRepository.Query()
                    on c.Id equals t.CustomerId into taxGroup
                from tx in taxGroup.DefaultIfEmpty()

                //    // JOIN orders
                //join o in _unitOfWork.Orde.Query()
                //    on c.Id equals o.CustomerId into orderGroup

                select new CustomerDto
                {
                    Id = c.Id,
                    FirstName = c.FirstName,
                    LastName = c.LastName,
                    Email = c.Email,
                    Phone = c.Phone,
                    Gender = c.Gender.ToString(),
                    DateOfBirth = c.DateOfBirth.ToString(),
                    AcceptMarketing = c.AcceptMarketing,
                    Note = c.Note ?? "",
                    CustomerGroupId = c.CustomerGroupId,

                    

                    //// ORDER INFO
                    //TotalOrders = orderGroup.Count(),
                    //TotalSpent = orderGroup.Sum(x => (decimal?)x.TotalAmount) ?? 0,
                    //LastOrderDate = orderGroup
                    //                    .OrderByDescending(x => x.CreatedDate)
                    //                    .Select(x => (DateTime?)x.CreatedDate)
                    //                    .FirstOrDefault(),

                    CreateBy = "OKAdmin"
                };

            // SEARCH
            if (!string.IsNullOrEmpty(request.Search))
            {
                query = query.Where(x =>
                    x.FirstName.Contains(request.Search) ||
                    x.LastName.Contains(request.Search) ||
                    x.Email.Contains(request.Search) ||
                    x.Phone.Contains(request.Search)
                );
            }

            // TOTAL COUNT
            var totalCount = await query.CountAsync(cancellationToken);

            // PAGING
            var items = await query
                .OrderByDescending(x => x.Id)
                .Skip((request.Page - 1) * request.Limit)
                .Take(request.Limit)
                .ToListAsync(cancellationToken);

            return new PagedResult<CustomerDto>
            {
                Items = items,
                Total = totalCount
            };
        }
    }
}
