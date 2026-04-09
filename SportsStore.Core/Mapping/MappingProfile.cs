using AutoMapper;
using SportsStore.Core.Domain.DTOs;
using SportsStore.Core.Domain.Entities;

namespace SportsStore.Core.Mapping;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // Order mappings
        CreateMap<Order, OrderResponseDto>()
            .ForMember(dest => dest.OrderId, opt => opt.MapFrom(src => src.OrderID))
            .ForMember(dest => dest.TotalAmount, opt => opt.MapFrom(src =>
                src.Items.Sum(i => i.Quantity * i.UnitPrice)))
            .ForMember(dest => dest.ItemCount, opt => opt.MapFrom(src =>
                src.Items.Count));

        CreateMap<OrderItem, OrderItemDto>()
            .ForMember(dest => dest.ProductId, opt => opt.MapFrom(src => src.ProductID))
            .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src =>
                src.Product != null ? src.Product.Name : "Unknown"));

        // Product mappings
        CreateMap<Product, ProductDto>()
            .ForMember(dest => dest.ProductID, opt => opt.MapFrom(src => src.ProductID));

        // Inventory mappings
        CreateMap<InventoryRecord, InventoryRecordDto>()
            .ForMember(dest => dest.OrderId, opt => opt.MapFrom(src => src.OrderId))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status))
            .ForMember(dest => dest.StockAvailability, opt => opt.MapFrom(src => src.StockAvailability));

        CreateMap<InventoryRecordDto, InventoryRecord>()
            .ForMember(dest => dest.OrderId, opt => opt.MapFrom(src => src.OrderId))
            .ForMember(dest => dest.StockAvailability, opt => opt.MapFrom(src => src.StockAvailability));

        // Payment mappings
        CreateMap<PaymentRecord, PaymentRecordDto>()
            .ForMember(dest => dest.OrderId, opt => opt.MapFrom(src => src.OrderId))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status));

        CreateMap<PaymentRecordDto, PaymentRecord>()
            .ForMember(dest => dest.OrderId, opt => opt.MapFrom(src => src.OrderId));

        // Shipment mappings
        CreateMap<ShipmentRecord, ShipmentRecordDto>()
            .ForMember(dest => dest.OrderId, opt => opt.MapFrom(src => src.OrderId));

        CreateMap<ShipmentRecordDto, ShipmentRecord>()
            .ForMember(dest => dest.OrderId, opt => opt.MapFrom(src => src.OrderId));
    }
}
