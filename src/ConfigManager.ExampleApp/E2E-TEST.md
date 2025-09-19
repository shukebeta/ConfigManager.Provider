# End-to-End Testing Guide

This guide demonstrates the complete ConfigManager system working together.

## Prerequisites

1. Redis running on localhost:6379
2. ConfigManager.Web API running on port 3001  
3. ConfigManager.ExampleApp running on port 5000

## Test Scenario

We'll demonstrate real-time configuration updates:

### Step 1: Start all services

```bash
# Terminal 1: Start Redis (if not already running)
redis-server

# Terminal 2: Start ConfigManager.Web API
cd /home/davidwei/Projects/ConfigManager.Web
npm start

# Terminal 3: Start ExampleApp
cd ConfigManager.ExampleApp
dotnet run
```

### Step 2: Initial state - Check current config

```bash
# Check what configuration ExampleApp currently sees
curl http://localhost:5000/config

# Check weather endpoint (uses default values)
curl http://localhost:5000/weatherforecast
```

Expected: No Redis config yet, using defaults (location="Unknown", temps=-20 to 55)

### Step 3: Set configuration via API

```bash
# Set weather location
curl -X POST http://localhost:3001/redis/exampleapp:config:weather:location \
  -H "Content-Type: application/json" \
  -d '{"value": "San Francisco"}'

# Set temperature range
curl -X POST http://localhost:3001/redis/exampleapp:config:weather:maxTemp \
  -H "Content-Type: application/json" \
  -d '{"value": "25"}'

curl -X POST http://localhost:3001/redis/exampleapp:config:weather:minTemp \
  -H "Content-Type: application/json" \
  -d '{"value": "10"}'
```

### Step 4: Verify real-time updates

```bash
# Check config immediately (should show new values)
curl http://localhost:5000/config

# Check weather forecast (should use new location and temp range)
curl http://localhost:5000/weatherforecast
```

Expected: location="San Francisco", temperatures between 10-25°C

### Step 5: Test configuration changes

```bash
# Change location dynamically
curl -X POST http://localhost:3001/redis/exampleapp:config:weather:location \
  -H "Content-Type: application/json" \
  -d '{"value": "Tokyo"}'

# Immediately check weather forecast again
curl http://localhost:5000/weatherforecast
```

Expected: location="Tokyo" without restarting the app!

## Success Criteria

✅ ExampleApp starts without Redis (optional=true works)  
✅ ExampleApp reads initial config from Redis  
✅ Configuration changes via API are immediately reflected  
✅ No application restart required for config updates  
✅ Multiple config keys work correctly  
✅ Type conversion works (string to int for temperatures)

## Troubleshooting

- **"Redis client not connected"**: Ensure Redis is running on localhost:6379
- **"Package not found"**: Run `dotnet restore` in ExampleApp directory
- **API not responding**: Check ConfigManager.Web is running with `npm test` passing
- **Config not updating**: Check Redis pub/sub with `redis-cli monitor`