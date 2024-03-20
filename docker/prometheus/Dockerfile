ARG PROM_VERSION="latest"
FROM prom/prometheus:$PROM_VERSION

# copy prometheus config
COPY prometheus.yaml /etc/prometheus/prometheus.yaml

# copy rules
RUN mkdir -p /prometheus/
COPY rules.yaml /prometheus/ 