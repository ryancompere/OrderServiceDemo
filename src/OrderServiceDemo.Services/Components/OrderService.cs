﻿using System.Linq;
using System.Threading.Tasks;
using OrderServiceDemo.Models;
using OrderServiceDemo.Models.Exceptions;
using OrderServiceDemo.Services.Infrastructure;
using OrderServiceDemo.Services.Interfaces;

namespace OrderServiceDemo.Services.Components
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IOrderLineItemRepository _orderLineItemRepository;

        public OrderService(
            IOrderRepository orderRepository,
            IOrderLineItemRepository orderLineItemRepository)
        {
            _orderRepository = orderRepository;
            _orderLineItemRepository = orderLineItemRepository;
        }

        public async Task<Order> CreateOrder(Order order)
        {
            if (order.OrderLineItems?.Any() != true)
                throw new InvalidRequestException("To create an order you must supply at least 1 line item");

            var createdOrder = await _orderRepository.CreateOrder(order);

            foreach(var lineItem in order.OrderLineItems)
            {
                lineItem.OrderId = createdOrder.OrderId;
            }

            var lineItems = await Task.WhenAll(order.OrderLineItems.Select(x => _orderLineItemRepository.CreateOrderLineItem(x)));
            createdOrder.OrderLineItems = lineItems.ToList();
            return createdOrder;
        }

        public async Task<Order> GetOrder(int orderId)
        {
            var order = await _orderRepository.GetOrder(orderId);
            await BuildUpOrder(order);
            return order;
        }

        public Task<Order> CancelOrder(int orderId)
        {
            //TODO: Add service implementation. Throw exception if the indicated order does not exist or has already been cancelled.
            //TODO: Add Unit tests for this service method.
            throw new System.NotImplementedException();
        }

        public Task<Order> DeleteOrder(int orderId)
        {
            //TODO: Add service implementation. Throw exception if the indicated order does not exist.
            //TODO: Add Unit tests for this service method.
            throw new System.NotImplementedException();
        }

        private async Task<Order> BuildUpOrder(Order order)
        {
            if (order == null)
                return order;

            var lineItems = await _orderLineItemRepository.GetOrderLineItems(order.OrderId);
            order.OrderLineItems = lineItems.ToList();
            return order;
        }
    }
}
