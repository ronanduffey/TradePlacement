using TradePlacement.Domain.Exceptions;
using TradePlacement.Domain.StakeProviders.Opening;
using TradePlacement.Models;
using TradePlacement.Models.Api;
using System.Collections.Generic;
using System.Linq;

namespace TradePlacement.Domain.Manager
{
    public class OrderPriceFinder : IOrderPriceFinder
    {
        private readonly IOpeningStakeProviderFactory _openingStakeProviderFactory;

        public OrderPriceFinder(IOpeningStakeProviderFactory openingStakeProviderFactory)
        {
            _openingStakeProviderFactory = openingStakeProviderFactory;
        }

        public OrderTick GetPrice(Side side, ExchangePrices prices)
        {
            if (prices == null)
            {
                throw new System.Exception();
            }

            var stakeProvider = _openingStakeProviderFactory.GetStakeProvider("Fixed");

            if (side == Side.BACK)
            {
                var openingPrices = prices.AvailableToBack.OrderByDescending(x => x.Price).ToList();
                ValidateHasPrices(openingPrices);
                return GetOrderTick(stakeProvider, openingPrices);
            }
            else if (side == Side.LAY)
            {
                var openingPrices = prices.AvailableToLay.OrderBy(x => x.Price).ToList();
                ValidateHasPrices(openingPrices);
                return GetOrderTick(stakeProvider, openingPrices);
            }
            else
            {
                throw new System.Exception();
            }
        }

        private static OrderTick GetOrderTick(IOpeningStakeProvider stakeProvider, List<PriceSize> openingPrices)
        {
            var stake = stakeProvider.GetStake(openingPrices[0].Price, new List<double>());

            if (stake <= openingPrices[0].Size)
            {
                return new OrderTick(openingPrices[0].Price, stake);
            }

            if (openingPrices.Count() > 1 && stake <= openingPrices[1].Size)
            {
                return new OrderTick(openingPrices[1].Price, stake);
            }

            if (openingPrices.Count() > 2 && stake <= openingPrices[2].Size)
            {
                return new OrderTick(openingPrices[2].Price, stake);
            }

            throw new NoPriceWithRequiredStakeAvailableException();
        }

        private void ValidateHasPrices(List<PriceSize> prices)
        {
            if (prices == null || !prices.Any())
            {
                throw new NoPriceWithRequiredStakeAvailableException();
            }
        }
    }
}