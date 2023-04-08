# Foxy (@vulns_search_bot)
Small high scalable Telegram bot for RHT (Red Hat Teams) for gathering and analyzing target sources information in your smartphone.

One of the ideas is to deploy collectors and parsers (CaP) to different regions for gathering data close to targets based on geolocation information, also CaP can be run on other devices (computers, IoT, smartphones) the primary communication protocol is AMQP. This approach allows for the creation of a mesh of devices that can process and analyze data independently. All communication between CaP is additionally encrypted with DH and AES over the TLS protocol.
### Local run

```bash
Set environment variables:

export API_KEY=<key>
export RMQ_USER=<user>
export RMQ_PASS=<pass>
export REDIS_PASSWORD=<pass>

cd foxy/
make docker-up
```

### Local cleanup

```bash
cd foxy/
make docker-cleanup
```

### Deployment to AWS ECS


```bash
cd foxy/infra
make deploy
```