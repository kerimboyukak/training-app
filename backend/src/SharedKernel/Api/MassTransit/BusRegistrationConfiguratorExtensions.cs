using MassTransit;

namespace Api.MassTransit
{
    public static class BusRegistrationConfiguratorExtensions   //used to configure RabbitMQ for each microservice
    {
        public static void UseRabbitMq(this IBusRegistrationConfigurator x, RabbitMqSettings settings)
        {
            x.UsingRabbitMq((context, config) =>
            {
                config.Host(settings.Host, "/", host =>
                {
                    host.Username(settings.UserName);
                    host.Password(settings.Password);
                });

                //One receive endpoint (queue) for all consumers
                config.ReceiveEndpoint(settings.QueueName, e =>
                {
                    e.ConfigureConsumers(context);      // all the configured consumers are linked to the endpoint, every consumer in the AppLogic layer of a microservice will listen to the same queue
                });
            });
        }
    }
}