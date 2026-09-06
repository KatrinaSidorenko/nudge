# nudge

## Local development

Copy `.env.example` to `.env` and adjust values as needed, then bring up the dev database:

```
cp .env.example .env
docker compose --env-file .env up -d
```

See `docker-compose.yml`.

## Credits:
    - https://github.com/meysamhadeli/booking-microservices
    - https://github.com/evolutionary-architecture/evolutionary-architecture-by-example