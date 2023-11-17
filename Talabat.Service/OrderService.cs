using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core;
using Talabat.Core.Entities;
using Talabat.Core.Entities.order_Aggregate;
using Talabat.Core.Repositories;
using Talabat.Core.Services;
using Talabat.Core.Specifications.Order_Spec;

namespace Talabat.Service
{
    public class OrderService : IOrderService
    {
        private readonly IBasketRepository _basketRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPaymentService _paymentService;

        public OrderService(IBasketRepository basketRepository,
            IUnitOfWork unitOfWork,
            IPaymentService paymentService)
        {
            _basketRepository = basketRepository;
            _unitOfWork = unitOfWork;
            this._paymentService = paymentService;
        }
        public async Task<Order?> CreateOrderAsync(string buyerEmail, string basketId, int DeliveryMethodId, Address shippingAddress)
        {
            // 1. Get Basket from basket repo
            var Basket = await _basketRepository.GetBasketAsync(basketId);

            // 2. get selected items at basket from product repo
            var OrderItems = new List<OrderItem>();
            if (Basket?.Items.Count > 0)
            {
                foreach (var item in Basket.Items)
                {
                    var Product = await _unitOfWork.Repository<Product>().GetByIdAsync(item.Id);
                    var ProductItemOrderd = new ProductItemOrdered(Product.Id,Product.Name,Product.PictureUrl);
                    var OrderItem = new OrderItem(ProductItemOrderd,item.Quantity,Product.Price);
                    OrderItems.Add(OrderItem);

                }
            }


            // 3. calculate subtotal // price of product * quantity
            var SubTotal = OrderItems.Sum(item=> item.Price * item.Quantity);

            // 4. get delivery method from deliverymethod repo
            var DeliveryMethod = await _unitOfWork.Repository<DeliveryMethod>().GetByIdAsync(DeliveryMethodId);

            // 5. create order
            var Spec = new orderWithPaymentIntentSpec(Basket.PaymentIntentId);
            var ExOrder = await _unitOfWork.Repository<Order>().GetEntityWithSpecAsync(Spec);
            if(ExOrder is not null)
            {
                _unitOfWork.Repository<Order>().Delete(ExOrder);
                await _paymentService.CreateOrUpdatePaymentIntent(basketId);
            }


            var Order = new Order(buyerEmail, shippingAddress, DeliveryMethod, OrderItems, SubTotal, Basket.PaymentIntentId);

            // 6. add order locally
            await _unitOfWork.Repository<Order>().Add(Order);

            // 7. save order to database 
            var  Result  = await _unitOfWork.CompleteAsync();
            if (Result <= 0) return null;

            return Order;
        }

        public async Task<IReadOnlyList<DeliveryMethod>> GetDeliveryMethodsAsync()
        {
            var DeliveryMethods = await _unitOfWork.Repository<DeliveryMethod>().GetAllAsync();
            return DeliveryMethods;
        }

        public async Task<Order> GetOrderByIdForSpecificUserAsync(string buyerEmail, int orderId)
        {
            var Spec = new OrderSpecifications(buyerEmail,orderId);
            var Order = await _unitOfWork.Repository<Order>().GetEntityWithSpecAsync(Spec);
            return Order;
        }

        public Task<IReadOnlyList<Order>> GetOrdersForSpecificUserAsync(string buyerEmail)
        {
            var Spec = new OrderSpecifications(buyerEmail);
            var Orders = _unitOfWork.Repository<Order>().GetAllWithSpecAsync(Spec);
            return Orders;
        }



    }
}
