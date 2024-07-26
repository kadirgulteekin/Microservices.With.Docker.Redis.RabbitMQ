﻿using StackExchange.Redis;

namespace Services.Basket.Services
{
    public class RedisService
    {
        private readonly string? _host;
        private readonly int? _port;
        private ConnectionMultiplexer? _connectionMultiplexer;

        public RedisService(string host, int port)
        {
            _host = host;
            _port = port;
        }

        public void Connect() => _connectionMultiplexer ??=  ConnectionMultiplexer.Connect($"{_host}:{_port}");

        public IDatabase GetDb(int db = 1) => (_connectionMultiplexer ?? throw new InvalidOperationException("ConnectionMultiplexer is not initialized. Call Connect() method first.")).GetDatabase(db);
    }
}
