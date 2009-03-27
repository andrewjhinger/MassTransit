﻿using System;
using System.Threading;
using MassTransit.Saga;
using Starbucks.Messages;

namespace Starbucks.Barista
{
    using MassTransit;

    public class DrinkPreparationSaga :
        InitiatedBy<NewOrderMessage>,
        Orchestrates<PaymentCompleteMessage>,
        ISaga
    {
        private readonly Guid _correlationId;
        private bool DrinkReady { get; set; }
        private bool PaymentComplete { get; set; }
        public string Drink { get; set; }
        public string Name { get; set; }

        public DrinkPreparationSaga(Guid correlationId)
        {
            _correlationId = correlationId;
        }

        public Guid CorrelationId
        {
            get { return _correlationId; }
        }

        public IServiceBus Bus
        {
            get; set;
        }

        public void Consume(NewOrderMessage message)
        {

            string drink = string.Format("{0} {1}", message.Size, message.Item);

        	Drink = drink;
        	Name = message.Name;

            Console.WriteLine(string.Format("{0} for {1}, got it!", drink, message.Name));

            for (int i = 0; i < 10; i++)
            {
                Thread.Sleep(i * 200);
                Console.WriteLine("[wwhhrrrr....psssss...chrhrhrhrrr]");
            }

            DrinkReady = true;

            ServeDrinkIfStateComplete();
        }

        private void ServeDrinkIfStateComplete()
        {
            if(PaymentComplete && DrinkReady)
            {
                Console.WriteLine(string.Format("I've got a {0} ready for {1}!", Drink, Name));

            	var message = new DrinkReadyMessage
            		{
            			CorrelationId = CorrelationId,
            			Drink = Drink,
						Name = Name,
            		};

                Bus.Publish(message);
            }
        }

        public void Consume(PaymentCompleteMessage message)
        {
            PaymentComplete = true;
            Console.WriteLine("Payment Complete for '{0}' got it!", Name);
            ServeDrinkIfStateComplete();
        }
    }
}