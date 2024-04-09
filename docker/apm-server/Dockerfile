FROM docker.elastic.co/apm/apm-server:7.17.19

# copy apm-server.yml configuration
COPY apm-server.yml /usr/share/apm-server/apm-server.yml

# perms
USER root
RUN chown root:apm-server /usr/share/apm-server/apm-server.yml
USER apm-server