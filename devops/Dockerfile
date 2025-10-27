FROM mcr.microsoft.com/mssql/server:2022-latest

# Configuración regional (opcional)
RUN apt-get update && \
    apt-get install -y locales && \
    localedef -i es_ES -c -f UTF-8 -A /usr/share/locale/locale.alias es_ES.UTF-8

ENV LANG=es_ES.UTF-8
ENV LC_ALL=es_ES.UTF-8
