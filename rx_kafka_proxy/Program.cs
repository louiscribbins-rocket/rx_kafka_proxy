using Confluent.Kafka;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;

namespace rx_kafka_proxy
{
    internal class Program
    {
        const string HOSTS_FILENAME = "C:\\Windows\\System32\\drivers\\etc\\hosts";

        static void Main(string[] args)
        {
            if (!File.Exists("appsettings.json") || File.ReadAllText("appsettings.json").Length == 0)
            {
                Console.WriteLine("");
                Console.WriteLine("");
                Console.WriteLine("************ PROBLEM! ************");
                Console.WriteLine("We need a valid 'appsettings.json' file alongside our exe.");
                Console.WriteLine("Please pull down the version in myvault here:");
                Console.WriteLine($"https://myvault.rockfin.com/SecretServer/app/#/secrets/68795/general");
                Console.WriteLine("");
                Console.WriteLine("");
                return;
            }

            var config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .AddEnvironmentVariables()
                .Build();

            var kafkaConfig = config.GetRequiredSection("Kafka");
            var brokers = GetBrokers(kafkaConfig);

            if (BrokersMissingInHosts(brokers))
            {
                Console.WriteLine("");
                Console.WriteLine("");
                Console.WriteLine("");
                Console.WriteLine("************ PROBLEM! ************");
                Console.WriteLine("All of the current brokers are NOT in your hosts file!");
                Console.WriteLine($"  {HOSTS_FILENAME}");
                Console.WriteLine("");
                Console.WriteLine("Please update with the following in the Confluent Broker section.");
                Console.WriteLine("");
                Console.WriteLine("");
                Console.WriteLine("# Confluent Brokers");

                for (int i = 0; i<brokers.Count; i++) 
                {
                    Console.WriteLine($"127.0.0.1\t{brokers[i]} # Qtrade nonprod - broker{i}");
                }
            }
            else
            {
                Console.WriteLine("");
                Console.WriteLine("");
                Console.WriteLine("************ CONFIGURATION LOOKS GOOD! ************");
                Console.WriteLine("");
                Console.WriteLine("");

            }
        }

        static bool BrokersMissingInHosts(List<string> brokers)
        {
            var contents = File.ReadAllText(HOSTS_FILENAME);

            // are there ANY brokers that are NOT in our hosts file contents?
            var results = brokers.Any(x => !contents.Contains(x));
            return results;
        }

        static List<string> GetBrokers(IConfigurationSection config)
        {
            var dictionary = new Dictionary<string, string>();
            foreach (var kv in config.AsEnumerable())
            {
                if (kv.Key == config.Key)
                    continue;
                dictionary.Add(kv.Key.Replace($"{config.Key}:", ""), kv.Value);
            }

            var brokers = new List<string>();
            using (var adminClient = new AdminClientBuilder(new AdminClientConfig(dictionary)).Build())
            {
                var meta = adminClient.GetMetadata(TimeSpan.FromSeconds(2));
                Console.WriteLine("Bootstrap Server:");
                Console.WriteLine($"   Broker: {meta.OriginatingBrokerId} {meta.OriginatingBrokerName}");
                Console.WriteLine("");
                Console.WriteLine("Brokers:");
                meta.Brokers.ForEach(broker =>
                    {
                        Console.WriteLine($"Broker: {broker.BrokerId} {broker.Host}:{broker.Port}");
                        brokers.Add($"{broker.Host}:{broker.Port}");
                    });

            }
            return brokers;
        }
    }
}
