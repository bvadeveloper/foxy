# Foxy (@vulns_search_bot)
Small high scalable Telegram bot for RHT (Red Hat Teams) for gathering and analyzing target sources information in your smartphone.

### Local run

```bash
Set environment variables:

API_KEY=<key>
RMQ_USER=<user>>
RMQ_PASS=<pass>

cd foxy/
make up
```

### Local cleanup

```bash
cd foxy/
make cleanup_all
```

### Deployment to AWS ECS

One of the ideas is to deploy scanners and collectors (SaC) to different regions for gathering data close to targets based on geolocation information, also SaC can be run on other devices (computers, IoT, smartphones) the primary communication protocol is AMQP. This approach allows for the creation of a mesh of devices that can process and analyze data independently.

```bash
cd foxy/infra
make deploy
```
