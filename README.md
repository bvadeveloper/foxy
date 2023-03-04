# Foxy (@vulns_search_bot)
Small high scalable Telegram bot for RHT (Red Hat Teams) for gathering and analyzing target sources information in your smartphone.

### Local run

```bash
Set environment variables:

export API_KEY=<key>
export RMQ_USER=<user>
export RMQ_PASS=<pass>

cd foxy/
make docker-up
```

### Local cleanup

```bash
cd foxy/
make docker-cleanup
```

### Deployment to AWS ECS

One of the ideas is to deploy scanners and collectors (SaC) to different regions for gathering data close to targets based on geolocation information, also SaC can be run on other devices (computers, IoT, smartphones) the primary communication protocol is AMQP. This approach allows for the creation of a mesh of devices that can process and analyze data independently.

```bash
cd foxy/infra
make deploy
```
~~~~