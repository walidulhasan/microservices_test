using AutoMapper;
using Discount.Grpc.Models;
using Discount.Grpc.Protos;
using Discount.Grpc.Repository;
using Grpc.Core;

namespace Discount.Grpc.Services
{
    public class DiscountService : DiscountProtoService.DiscountProtoServiceBase
    {
        private readonly ICouponRepository _couponRepository;
        private readonly ILogger<DiscountService> _logger;
        private readonly IMapper _mapper;

        public DiscountService(ICouponRepository couponRepository, ILogger<DiscountService> logger,IMapper mapper)
        {
            _couponRepository = couponRepository;
            _logger = logger;
            _mapper = mapper;
        }

        public override async Task<CouponRequest> GetDiscount(GetDiscountRequest request, ServerCallContext context)
        {
            var coupon = await _couponRepository.GetDisount(request.ProductId);
            if (coupon is null)
            {
                throw new RpcException(new Status(StatusCode.NotFound, "Siscount not found"));
            }
            _logger.LogInformation("Discount is retrived for ProductName:{productName},Amount:{amount}", coupon.ProductName, coupon.Amount);
            //return new CouponRequest { ProductId = request.ProductId, ProductName = coupon.ProductName, Description = coupon.Description, Amount = coupon.Amount };

            return _mapper.Map<CouponRequest>(coupon);
            
        }

        public override async Task<CouponRequest> CreateDiscount(CouponRequest request, ServerCallContext context)
        {
            var coupon=_mapper.Map<Coupon>(request);
           bool isSaved=await _couponRepository.CreateDiscount(coupon);
            if (isSaved)
            {
                _logger.LogInformation("Discount is successfully created. ProductName:{ProductName}", coupon.ProductName);
            }
            else
            {
                _logger.LogInformation("Discount Created failed.");
            }
            

            return _mapper.Map<CouponRequest>(coupon);
        }

        public override async Task<CouponRequest> UpdateDiscount(CouponRequest request, ServerCallContext context)
        {
            var coupon = _mapper.Map<Coupon>(request);
            bool IsModified=await _couponRepository.UpdateDiscount(coupon);
            if (IsModified)
            {
                _logger.LogInformation("Discount is successfully updated. ProductName:{ProductName}",coupon.ProductName);
            }
            else
            {
                _logger.LogInformation("Discount update failed.");
            }
            

            return _mapper.Map<CouponRequest>(coupon);
        }

        public override async Task<DeleteDiscountResponse> DeleteDiscount(DeleteDiscountRequest request, ServerCallContext context)
        {
            bool isDeleted = await _couponRepository.DeleteDiscount(request.ProductId);
            if (isDeleted)
            {
                _logger.LogInformation("Discount has been deleted .ProductName:{ProductId}",request.ProductId);
            }
            else
            {
                _logger.LogInformation("Discount Deleted failed.");
            }
            return new DeleteDiscountResponse() { Success=isDeleted};


        }
    }
}
