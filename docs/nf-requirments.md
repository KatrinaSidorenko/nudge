# Non-Functional Requirements (NFR) Document

**Project:** Spaced Repetition Flashcard Telegram System  
**Target Level:** Production-Grade / Middle+ Architectural Standards  

---

## 1. Observability Requirements

| Requirement ID | Category | Specification & Target Metric | Verification Method |
| :--- | :--- | :--- | :--- |
| **NFR-OBS-01** | **Metrics Collection** | Expose Prometheus metrics endpoint (`/metrics`) tracking:<br>• Webhook processing latency (p95 < 200ms)<br>• Active Telegram study sessions count<br>• Reminders worker queue depth & error rate | Automated Grafana alerts + load testing dashboards. |
| **NFR-OBS-02** | **Distributed Tracing** | Instrument all HTTP webhooks, gRPC/REST internal calls, DB transactions, and broker messages with OpenTelemetry context propagation (Jaeger/Zipkin backend). | Trace sampling checks in staging environment. |
| **NFR-OBS-03** | **Structured Logging** | Logs must be structured JSON format containing trace/span IDs, `user_id`, `session_id`, log level, and component name. Sensitive user data (flashcard text) must be sanitized. | Log parser integration tests in CI. |
| **NFR-OBS-04** | **Alerting Thresholds** | Fire high-priority alerts on:<br>• Telegram API error rate > 2% for 5 mins<br>• DB connection pool utilization > 85%<br>• Reminder queue lag > 10,000 messages | Chaos engineering / simulated failure testing. |

---

## 2. Testing Strategy & Quality Assurance

| Requirement ID | Category | Specification & Target Metric | Verification Method |
| :--- | :--- | :--- | :--- |
| **NFR-TST-01** | **Unit Testing** | Achieve minimum **80% line coverage** and **100% domain logic coverage** (SM-2/FSRS scheduling algorithm, `StudySession` state transitions). | CI pipeline gating (Go test / PyTest / JUnit). |
| **NFR-TST-02** | **Integration Testing** | Test DB repositories and Redis session persistence using **Testcontainers** with real PostgreSQL and Redis instances (no mocking of storage layers). | CI workflow pre-merge test suite execution. |
| **NFR-TST-03** | **End-to-End Testing** | Simulate complete user journeys: creating decks, adding cards, starting sessions, answering cards, and session expiration via mock Telegram Webhook server. | Automated nightly E2E testing job. |
| **NFR-TST-04** | **Contract & Outbox Testing** | Verify transactional outbox events payload serialization and schema validation prior to broker publishing (NATS/RabbitMQ). | Schema registry compatibility checks in CI. |

---

## 3. Performance & Scalability

| Requirement ID | Category | Specification & Target Metric | Verification Method |
| :--- | :--- | :--- | :--- |
| **NFR-PRF-01** | **Latency SLA** | **Webhook Response Time:** p95 < 150ms, p99 < 300ms.<br>**Card Answer Processing:** p95 < 100ms. | k6 / Locust load testing against API gateway. |
| **NFR-PRF-02** | **Throughput & Capacity** | System must support **1,000 concurrent active study sessions** and process up to **500 card answer attempts/sec** without degradation. | Stress testing under peak load simulation. |
| **NFR-PRF-03** | **Notification Rate Limiting** | Worker pools must dynamically limit outgoing Telegram notification requests to **30 messages/sec globally** (Token-bucket algorithm) to avoid HTTP 429 errors. | Mock Telegram rate-limit compliance tests. |
| **NFR-PRF-04** | **Autoscaling & Resilience** | HPA (Horizontal Pod Autoscaler) must scale worker pods based on queue length (trigger at > 1,000 pending tasks). System must recover from crashed pods within **30s**. | Kubernetes Chaos Mesh / Pod kill testing. |
| **NFR-PRF-05** | **Data Consistency** | Enforce idempotent processing for user actions. DB transactions must use optimistic concurrency control (`version` column) for state updates. | Concurrent execution stress testing scripts. |