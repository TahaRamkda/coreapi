using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using WhatsAppAPISolutionDL.Dto.Order.KFG;
using WhatsAppAPISolutionDL.Models;
using WhatsAppAPISolutionDL.UserModels;

namespace WhatsAppAPISolutionBL.Master.Services
{
    public class KFGOrderService
    {
        #region Fields    

        private readonly WhatsAppSolutionContext _dbContext;
        private readonly WhatsAppSolutionContext2 _dbContext2;
        private readonly ILogger<KFGOrderService> _logger;

        #endregion

        #region Ctor

        public KFGOrderService(
            WhatsAppSolutionContext dbContext,
            WhatsAppSolutionContext2 dbContext2,
            ILogger<KFGOrderService> logger)
        {
            _dbContext = dbContext;
            _dbContext2 = dbContext2;
            _logger = logger;
        }

        #endregion

        #region Methods

        public async Task<string> CreateOrder(int orderId)
        {
            var order = await _dbContext.Orders.FindAsync(orderId);
            if (order == null)
                throw new ArgumentNullException(nameof(order));

            var orderAddress = await _dbContext.OrderAddresses.FirstOrDefaultAsync(x => x.OrderId == order.OrderId);
            if (orderAddress == null)
                throw new ArgumentNullException(nameof(orderAddress));

            var orderItems = await _dbContext.OrderItems.Where(x => x.OrderId == order.OrderId).ToListAsync();
            if (orderItems == null || !orderItems.Any())
                throw new ArgumentNullException(nameof(orderItems));

            var senderName = await _dbContext.SenderNames.FindAsync(order.SenderId);
            if (senderName == null)
                throw new ArgumentNullException(nameof(senderName));

            var createdAt = DateTime.UtcNow;
            if (order.CreatedDate.HasValue)
                createdAt = order.CreatedDate.Value;

            var currency = "KWD";

            KFGOrder kFGOrder = new KFGOrder
            {
                id = order.OrderId.ToString(),
                asap = true,
                orderCreatedAt = createdAt.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
                brandId = senderName.PhoneNumber,
                prepareFrom = createdAt.AddMinutes(15).ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
                deliverAt = createdAt.AddMinutes(30).ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
                cutleryNotes = String.Empty,
                fulfillmentType = "Restaurant",
                status = "placed",
                orderNotes = String.Empty,
                subTotal = new KFGOrder.Amount
                {
                    amount = order.Subtotal ?? 0,
                    currencyCode = currency
                },
                deliveryFee = new KFGOrder.Amount
                {
                    amount = order.DeliveryCharges ?? 0,
                    currencyCode = currency
                },
                discounts = new List<KFGOrder.Discount>(),
                grandTotal = new KFGOrder.Amount
                {
                    amount = order.Total ?? 0,
                    currencyCode = currency
                },
                items = new List<KFGOrder.Item>()
            };

            kFGOrder.customer = new KFGOrder.Customer
            {
                firstName = order.Name,
                lastName = String.Empty,
                contactNumber = order.PhoneNumber,
                email = String.Empty
            };

            kFGOrder.delivery = new KFGOrder.Delivery
            {
                block = orderAddress.Block,
                street = orderAddress.Street,
                flat = orderAddress.FlatNo,
                floor = orderAddress.Floor,
                building = orderAddress.House,
                deliveryNotes = orderAddress.Direction
            };

            if (!String.IsNullOrWhiteSpace(orderAddress.Cordinates))
            {
                var cordinates = orderAddress.Cordinates.Split(',').Select(x => Convert.ToDouble(x)).ToList();

                kFGOrder.delivery.location = new KFGOrder.Delivery.Location
                {
                    latitude = cordinates[0],
                    longitude = cordinates[1]
                };
            }

            kFGOrder.payments.Add(new KFGOrder.Payment
            {
                amount = order.Total ?? 0,
                currencyCode = currency,
                name = "KNET",
                referenceNumber = order.OrderId.ToString()
            });

            foreach (var orderItem in orderItems.Where(x => x.Level == 0).OrderBy(x => x.GroupId))
            {
                var item = await _dbContext.Items.FindAsync(orderItem.ItemId);
                if (item == null)
                    continue;

                //Fetch item modifiers by group id and level and sort by sequence
                var modifiers = orderItems.Where(x => x.GroupId == orderItem.GroupId && x.Level > 0).OrderBy(x => x.Sequence).ToList();

                var modifierPriceSum = modifiers.Sum(x => x.Price) ?? 0;

                var kfgitem = new KFGOrder.Item
                {
                    posItemId = item.IntegrationId,
                    name = item.NameEn,
                    quantity = 1,
                    unitPrice = new KFGOrder.Amount
                    {
                        amount = orderItem.Price ?? 0,
                        currencyCode = currency
                    },
                    totalPrice = new KFGOrder.Amount
                    {
                        amount = ((orderItem.Price ?? 0) + modifierPriceSum),
                        currencyCode = currency
                    },
                    modifiers = new List<KFGOrder.Item.Modifier>()
                };

                foreach (var modifier in modifiers)
                {
                    var modifierItem = await _dbContext.Items.FindAsync(modifier.ItemId);
                    if (modifierItem == null)
                        continue;

                    var kfgModifierItem = new KFGOrder.Item.Modifier
                    {
                        posItemId = modifierItem.IntegrationId,
                        name = modifierItem.NameEn,
                        quantity = 1,
                        unitPrice = new KFGOrder.Amount
                        {
                            amount = modifierItem.Price ?? 0,
                            currencyCode = currency
                        },
                        totalPrice = new KFGOrder.Amount
                        {
                            amount = modifierItem.Price ?? 0,
                            currencyCode = currency
                        },
                        modifiers = new List<KFGOrder.Item.Modifier>()
                    };

                    kfgitem.modifiers.Add(kfgModifierItem);
                }

                kFGOrder.items.Add(kfgitem);
            }

            var json = JsonConvert.SerializeObject(kFGOrder);

            return json;
        }
         
        #endregion
    }
}